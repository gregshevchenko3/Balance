using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace Balance
{
    public class SQLiteContext : IDisposable
    {
        private string _enc_db_name;
        private string _plainDatabase;
        private Aes _aes;
        private SQLiteConnection _qLiteConnection;
        public byte[] Key { get => _aes.Key; }
        public event EventHandler OnLoad;
        protected DataSet DataSet { get; set; }

        protected SQLiteContext(string db_name) {
            _plainDatabase = Path.Combine(Path.GetTempPath(), $"{Path.GetFileName(db_name)}");
            _enc_db_name = db_name;
            if (File.Exists(_plainDatabase))
                File.Delete(_plainDatabase);
            _aes = Aes.Create();
            _aes.Mode = CipherMode.CFB;
            _aes.GenerateKey();
            DataSet = new DataSet();
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
            DataSet = new DataSet();

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
        protected 
        private async Task LoadTables()
        {
#if DEBUG
            Console.WriteLine("SQLiteContext.LoadTables() Завантажую таблицi.");
#endif
            using (SQLiteCommand cmd = _qLiteConnection.CreateCommand())
            {
                cmd.CommandText = @"SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1;";
                using (DbDataReader rdr = await cmd.ExecuteReaderAsync())
                {          
                    while (await rdr.ReadAsync())
                    {
                        DataSet.Tables.Add(GetDataTable(rdr.GetString(0)));
                    }
                }
            }
#if DEBUG
            Console.WriteLine($"SQLiteContext.LoadTables() Завантажено {DataSet.Tables.Count} таблиць.");
#endif
        }
        private async Task Save()
        {

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
            Console.WriteLine("SQLiteContext._qLiteConnection");
            await _qLiteConnection.OpenAsync();
            await LoadTables();
            OnLoad?.Invoke(this, new EventArgs());

        }
        public virtual async Task Unload()
        {
#if DEBUG
            Console.WriteLine("SQLiteContext.Unload() Закриваю з'єднання з базо данних, якщо воно вiдкрите");
#endif
            _qLiteConnection.Close();
#if DEBUG
            Console.WriteLine("SQLiteContext.Unload() Шифрую базу данних!");
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
        public static async Task<SQLiteContext> FirstRun(string db_name)
        {
            SQLiteContext result = new SQLiteContext(db_name);
            await FirstRun(result, db_name);
            return result;
        }
        protected static async Task FirstRun(SQLiteContext ctx, string db_name)
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
INSERT INTO users(login) VALUES ('default');
INSERT INTO categoryes(category) VALUES ('default');
INSERT INTO rights(`user_id`, `type`, `table`, `cat_id`, `grandRead`, `grandModify`, `grandCreate`, `grandDelete`)
VALUES 
    (1, 'table', 'users', 1, 'y', 'y', 'y','y'),
    (1, 'table', 'categoryes', 1, 'y', 'y', 'y', 'y'),
    (1, 'table', 'rights', 1, 'y', 'y', 'y', 'y');";
                int res = cmd.ExecuteNonQuery();
#if DEBUG
                Console.WriteLine($"Створення таблиць завершено: {res}");
#endif
            }
            await ctx.LoadTables();
        }
        
    }
}
