using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Data;

namespace Balance
{
    public class SQLiteContext : IDisposable
    {
        private string _enc_db_name;
        private string _plainDatabase;
        private Aes _aes;
        private SQLiteConnection _qLiteConnection;
        protected SQLiteDataAdapter _UserAdapter, _RightsAdapter, _CategoryAdapter;
        private SQLiteCommandBuilder _UserCmdBuilder, _RightsCmdBuilder, _CategoryCmdBuilder;

        public byte[] Key { get => _aes.Key; }
        public event EventHandler OnLoad;
        protected DataSet DataSet { get; set; }

        private void initDataSet()
        {
            DataSet = new DataSet();

            DataTable tbl = new DataTable("users");
            tbl.Columns.Add("id", typeof(int));
            tbl.Columns.Add("login", typeof(string));
            tbl.PrimaryKey = new DataColumn[] { tbl.Columns["id"] };

            DataTable tbl2 = new DataTable("categoryes");
            tbl2.Columns.Add("id", typeof(int));
            tbl2.Columns.Add("category", typeof(string));
            tbl2.PrimaryKey = new DataColumn[] { tbl2.Columns["id"] };

            DataTable tbl3 = new DataTable("rights");
            tbl3.Columns.Add("id", typeof(int));
            tbl3.Columns.Add("user_id", typeof(int));
            tbl3.Columns.Add("type", typeof(string));
            tbl3.Columns.Add("table", typeof(string));
            tbl3.Columns.Add("cat_id", typeof(int));
            tbl3.Columns.Add("grandRead", typeof(string));
            tbl3.Columns.Add("grandWrite", typeof(string));
            tbl3.Columns.Add("grandCreate", typeof(string));
            tbl3.Columns.Add("grandDelete", typeof(string));
            tbl3.PrimaryKey = new DataColumn[] { tbl3.Columns["id"] };

            DataSet.Tables.Add(tbl);
            DataSet.Tables.Add(tbl2);
            DataSet.Tables.Add(tbl3);

            ForeignKeyConstraint userRightsFK = new ForeignKeyConstraint("userRightsFK", DataSet.Tables["users"].Columns["id"], DataSet.Tables["rights"].Columns["user_id"]);
            ForeignKeyConstraint categoryRightsFK = new ForeignKeyConstraint("categoryRightsFK", DataSet.Tables["categoryes"].Columns["id"], DataSet.Tables["rights"].Columns["cat_id"]);
            userRightsFK.UpdateRule = Rule.Cascade;
            categoryRightsFK.UpdateRule = Rule.Cascade;
            userRightsFK.DeleteRule = Rule.None;
            categoryRightsFK.DeleteRule = Rule.None;

            DataSet.Tables["rights"].Constraints.Add(userRightsFK);
            DataSet.Tables["rights"].Constraints.Add(categoryRightsFK);

        }
        protected SQLiteContext(string db_name) {
            _plainDatabase = Path.Combine(Path.GetTempPath(), $"{Path.GetFileName(db_name)}");
            _enc_db_name = db_name;
            if (File.Exists(_plainDatabase))
                File.Delete(_plainDatabase);
            _aes = Aes.Create();
            _aes.Mode = CipherMode.CFB;
            _aes.GenerateKey();
            initDataSet();
        }
        public SQLiteContext(string db_name, byte[] key)
        {
            _enc_db_name = db_name;
            _plainDatabase = Path.Combine(Path.GetTempPath(), $"{Path.GetFileName(_enc_db_name)}");
            if (File.Exists(_plainDatabase))
            {
#if DEBUG
                Console.WriteLine("SQLiteContext Можливе порушення безпеки, або ранiше програма була закрита не правильно.");
#endif
                File.Delete(_plainDatabase);
            }
            _aes = Aes.Create();
            _aes.Mode = CipherMode.CFB;
            _aes.Key = key;
            initDataSet();

        }
        private DataTable GetDataTable(string TableName)
        {
            DataTable dt = new DataTable();
            using (SQLiteCommand cmd = _qLiteConnection.CreateCommand())
            {
                cmd.CommandText = string.Format("SELECT * FROM {0}", TableName);
                var adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(dt);
                dt.TableName = TableName;
            }
            return dt;
        }
        private void LoadTables()
        {
#if DEBUG
            Console.WriteLine("SQLiteContext.LoadTables() Завантажую таблицi.");
#endif
            _UserAdapter = new SQLiteDataAdapter("SELECT * FROM users", _qLiteConnection);
            _UserCmdBuilder = new SQLiteCommandBuilder(_UserAdapter);
            _UserAdapter.InsertCommand = _UserCmdBuilder.GetInsertCommand();
            _UserAdapter.UpdateCommand = _UserCmdBuilder.GetUpdateCommand();
            _UserAdapter.DeleteCommand = _UserCmdBuilder.GetDeleteCommand();
            _UserAdapter.Fill(DataSet, "users");

            _CategoryAdapter = new SQLiteDataAdapter("SELECT * FROM categoryes", _qLiteConnection);
            _CategoryCmdBuilder = new SQLiteCommandBuilder(_CategoryAdapter);
            _CategoryAdapter.InsertCommand = _CategoryCmdBuilder.GetInsertCommand();
            _CategoryAdapter.UpdateCommand = _CategoryCmdBuilder.GetUpdateCommand();
            _CategoryAdapter.DeleteCommand = _CategoryCmdBuilder.GetDeleteCommand();
            _CategoryAdapter.Fill(DataSet, "categoryes");

            _RightsAdapter = new SQLiteDataAdapter("SELECT * FROM rights", _qLiteConnection);
            _RightsCmdBuilder = new SQLiteCommandBuilder(_RightsAdapter);
            _RightsAdapter.InsertCommand = _RightsCmdBuilder.GetInsertCommand();
            _RightsAdapter.UpdateCommand = _RightsCmdBuilder.GetUpdateCommand();
            _RightsAdapter.DeleteCommand = _RightsCmdBuilder.GetDeleteCommand();
            _RightsAdapter.Fill(DataSet, "rights");
#if DEBUG
            Console.WriteLine($"SQLiteContext.LoadTables() Завантажено {DataSet.Tables.Count} таблиць.");
#endif
        }
        public void Save()
        {
            _UserAdapter.Update(DataSet, "users");
            _CategoryAdapter.Update(DataSet, "categoryes");
            _RightsAdapter.Update(DataSet, "rights");
        }
        public virtual async Task Load()
        {
#if DEBUG  
            Console.WriteLine("SQLiteContext.Load() Розшифровую базу данних!");
#endif
            using (FileStream destanation = File.Create(_plainDatabase))
            {
                using (FileStream source = File.OpenRead(_enc_db_name))
                {
                    byte[] iv = new byte[_aes.IV.Length];
                    source.Read(iv, 0, iv.Length);
                    _aes.IV = iv;
                    ICryptoTransform transform = _aes.CreateDecryptor(_aes.Key, iv);    
                    using (CryptoStream stream = new CryptoStream(source, transform, CryptoStreamMode.Read))
                    {    
                        await stream.CopyToAsync(destanation);    
                    }
                }
            }
#if DEBUG
            Console.WriteLine($"SQLiteContext.Load() Створюю пiдключення до бази данних. {_plainDatabase}, Файл iснує? - {File.Exists(_plainDatabase)}");
#endif
            _qLiteConnection = new SQLiteConnection($"Data Source={_plainDatabase}");
            await _qLiteConnection.OpenAsync();
            LoadTables();
#if DEBUG
            Console.WriteLine($"Знайдено та завантажено {DataSet.Tables.Count} таблиць");
#endif
            OnLoad?.Invoke(this, new EventArgs());

        }
        public virtual async Task Unload()
        {
#if DEBUG
            Console.WriteLine("SQLiteContext.Unload() Закриваю з'єднання з базо данних, якщо воно вiдкрите");
#endif
            _qLiteConnection.Close();
#if DEBUG
            Console.WriteLine($"SQLiteContext.Unload() Шифрую базу данних! {_plainDatabase}");
#endif
            if (File.Exists(_enc_db_name))
                File.Move(_enc_db_name, $"{_enc_db_name}.old");
            using (FileStream destanation = File.Create(_enc_db_name))
            {
                
                    using (FileStream source = File.OpenRead(_plainDatabase))
                    {
                        _aes.Mode = CipherMode.CFB;
                        _aes.GenerateIV();
                        destanation.Write(_aes.IV, 0, _aes.IV.Length);
                        ICryptoTransform transform = _aes.CreateEncryptor(_aes.Key, _aes.IV);
                        using (CryptoStream stream = new CryptoStream(destanation, transform, CryptoStreamMode.Write))
                        {
                            await source.CopyToAsync(stream);
                        }
                    }
                //}
                //catch(Exception exc)
                //{
                //    Console.WriteLine(exc.StackTrace);
                //}
            }
            File.Delete($"{_enc_db_name}.old");
#if DEBUG
            Console.WriteLine("SQLiteContext.Unload() Очищаю кеш бази даних!");
#endif
            File.Delete(_plainDatabase);
        }
        public void Dispose()
        {
            _aes.Dispose();
            _qLiteConnection.Dispose();
            DataSet.Dispose();
        }
        public static SQLiteContext FirstRun(string db_name)
        {
            SQLiteContext result = new SQLiteContext(db_name);
            FirstRun(result, db_name);
            return result;
        }
        protected static void FirstRun(SQLiteContext ctx, string db_name)
        {
            SQLiteConnection.CreateFile(ctx._plainDatabase);
#if DEBUG
            Console.WriteLine($"Створено: {ctx._plainDatabase}");
#endif
            ctx._qLiteConnection = new SQLiteConnection($"Data Source={ctx._plainDatabase}");
            ctx._qLiteConnection.Open();
#if DEBUG
            Console.WriteLine($"З'єднання вiдкрито з: \"Data Source={ctx._plainDatabase}\"");
#endif
            using (var cmd = ctx._qLiteConnection.CreateCommand())
            {
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS categoryes (
    id INTEGER PRIMARY KEY AUTOINCREMENT ,
    category TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS accounting (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    cat_id INTEGER NOT NULL,
    title TEXT NOT NULL,
    dateshop TEXT,
    price REAL,
    status TEXT NOT NULL DEFAULT 'Витрата',
    FOREIGN KEY(cat_id) REFERENCES categoryes(id)
);
CREATE TABLE IF NOT EXISTS users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    login TEXT NOT NULL
);
CREATE TABLE rights(
	id INTEGER, 
	user_id INT, 
	type TEXT NOT NULL DEFAULT 'table', 
	`table` TEXT, 
	cat_id INT,
	grandRead TEXT NOT NULL DEFAULT 'y',
	grandModify TEXT NOT NULL DEFAULT 'n',
	grandCreate TEXT NOT NULL DEFAULT 'n',
	grandDelete TEXT NOT NULL DEFAULT 'n',
	FOREIGN KEY(cat_id) REFERENCES categoryes(id),
	FOREIGN KEY(user_id) REFERENCES users(id), 
	PRIMARY KEY(id AUTOINCREMENT) 
);
";
                int res = cmd.ExecuteNonQuery();
#if DEBUG
                Console.WriteLine($"Створення таблиць завершено: {res}");
#endif
            }
            ctx.LoadTables();
            
            DataRow row = ctx.DataSet.Tables["users"].NewRow();
            row["id"] = 1;
            row["login"] = "default";
            ctx.DataSet.Tables["users"].Rows.Add(row);

            row = ctx.DataSet.Tables["categoryes"].NewRow();
            row["id"] = 1;
            row["category"] = " ";
            ctx.DataSet.Tables["categoryes"].Rows.Add(row);

            for (int i = 0; i < ctx.DataSet.Tables.Count; i++)
            {
                row = ctx.DataSet.Tables["rights"].NewRow();
                row["id"] = i + 1;
                row["user_id"] = ctx.DataSet.Tables["users"].Select("login='default'")[0]["id"];
                row["type"] = "table";
                row["table"] = ctx.DataSet.Tables[i].TableName;
                row["cat_id"] = ctx.DataSet.Tables["categoryes"].Select("category=' '")[0]["id"];
                row["grandRead"] = "y";
                row["grandModify"] = "y";
                row["grandCreate"] = "y";
                row["grandDelete"] = "y";
                ctx.DataSet.Tables["rights"].Rows.Add(row);
            }
            ctx._UserAdapter.Update(ctx.DataSet, "users");
            ctx._CategoryAdapter.Update(ctx.DataSet, "categoryes");
            ctx._RightsAdapter.Update(ctx.DataSet, "rights");
            ctx.OnLoad?.Invoke(ctx, new EventArgs());
        }
    }
}
