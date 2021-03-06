using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance
{
    
    public class SecureSQLiteContext : SQLiteContext
    {
        private string _login;

        SortedList<string, // 'table' or ...
            Dictionary<string,      // name_of_object
                    Dictionary<string, // grandRead, grandWrite, grandCreate, or grandDelete
                            bool    // Yes/No
                        >>> _current_user_rights;
        protected SecureSQLiteContext(string db_name) : base(db_name)
        {
            _login = "default";
            OnLoad += SecureSQLiteContext_OnLoad;
        }   
        public SecureSQLiteContext(string db_name, string login, byte[] key) : base(db_name, key)
        {
            _login = login;
            OnLoad += SecureSQLiteContext_OnLoad;
        }
        public void load_user()
        {
#if DEBUG
            Console.WriteLine("load_user()");
#endif
            string login = _login.Trim().ToLower();
            DataRow[] rows  = DataSet.Tables["rights"].Select(string.Format("user_id='{0}'",
                DataSet.Tables["users"].Select($"login='{login}'")[0]["id"]));
            _current_user_rights = new SortedList<string, Dictionary<string, Dictionary<string, bool>>>();
            foreach (DataRow row in rows)
            {
                Dictionary<string, bool> rights = new Dictionary<string, bool>();
                rights.Add("grandRead", row.Field<string>("grandRead") == "y");
                rights.Add("grandModify", row.Field<string>("grandModify") == "y");
                rights.Add("grandCreate", row.Field<string>("grandCreate") == "y");
                rights.Add("grandDelete", row.Field<string>("grandDelete") == "y");

                Dictionary<string, Dictionary<string, bool>> for_obj_name = new Dictionary<string, Dictionary<string, bool>>();
                for_obj_name.Add(row.Field<string>("table"), rights);

                if (!_current_user_rights.ContainsKey(row.Field<string>("type")))
                    _current_user_rights[row.Field<string>("type")] = for_obj_name;
                else
                    _current_user_rights[row.Field<string>("type")][row.Field<string>("table")] = rights;

                string root = row.Field<string>("type"), subroot = row.Field<string>("table");
                Console.WriteLine($"{subroot}    {_current_user_rights[root][subroot]["grandRead"]} " +
                   $"{_current_user_rights[root][subroot]["grandModify"]} {_current_user_rights[root][subroot]["grandCreate"]} " +
                   $"{_current_user_rights[root][subroot]["grandDelete"]}");
            }
        }
        private void SecureSQLiteContext_OnLoad(object sender, EventArgs e)
        {
            load_user();
            foreach (DataTable table in DataSet.Tables)
            {
                table.RowChanging += Table_RowChanging;
                table.RowDeleting += Table_RowDeleting;
                table.TableClearing += Table_TableClearing;
                table.TableNewRow += Table_TableNewRow;
            }
        }
        public bool get_table_rights(string name, string right)
        {
            return _current_user_rights["table"][name][right];
        }
        public override async Task Load()
        {
#if DEBUG
            Console.WriteLine("Load()");
#endif
            await base.Load();
            load_user();
        }
        protected DataTable GetTable(string name)
        {
            if(!get_table_rights(name, "grandRead"))
            {
#if DEBUG
                Console.WriteLine($"SecureSQLiteContext::GetTable(\"{name}\"): Для користувача \"{_login}\" доступ для таблицi \"{name}\" - заборонено!");
#endif
                throw new Exception($"SecureSQLiteContext::GetTable(\"{name}\"): Для користувача \"{_login}\" доступ для таблицi \"{name}\" - заборонено!");
            }
            return DataSet.Tables[name];
        }
        public virtual DataRow[] Select(string tbl_name, string filter)
        {
            return GetTable(tbl_name).Select(filter);
        }
        protected void Table_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            if (get_table_rights(e.Row.Table.TableName, "grandCreate"))
            {
#if DEBUG
                Console.WriteLine($"SecureSQLiteContext::TableNewRow(...): Користувачевi \"{_login}\" заборонено створювати записи в таблицi \"{e.Row.Table.TableName}\"!");
#endif
                throw new Exception($"SecureSQLiteContext::TableNewRow(...): Користувачевi  \"{_login}\" заборонено створювати записи в таблицi \"{e.Row.Table.TableName}\"!");
            }
        }
        protected void Table_TableClearing(object sender, DataTableClearEventArgs e)
        {
            if (get_table_rights(e.TableName, "grandDelete"))
            {
#if DEBUG
                Console.WriteLine($"SecureSQLiteContext::TableClearing(...): Користувачевi \"{_login}\"  заборонено видаляти записи з таблицi \"{e.TableName}\"!");
#endif
                throw new Exception($"SecureSQLiteContext::TableClearing(...): Користувачевi \"{_login}\" заборонено видаляти записи з таблицi \"{e.TableName}\"!");
            }
        }
        protected void Table_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (get_table_rights(e.Row.Table.TableName, "grandDelete"))
            {
#if DEBUG
                Console.WriteLine($"SecureSQLiteContext::RowDeleting(...): Користувачевi \"{_login}\" заборонено видаляти записи з таблицi \"{e.Row.Table.TableName}\"!");
#endif
                throw new Exception($"SecureSQLiteContext::RowDeleting(...): Користувачевi \"{_login}\" заборонено видаляти записи з таблицi \"{e.Row.Table.TableName}\"!");
            }
        }
        protected void Table_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            if (get_table_rights(e.Row.Table.TableName, "grandModify"))
            {
#if DEBUG
                Console.WriteLine($"SecureSQLiteContext::RowChanging(...): Користувачевi \"{_login}\" заборонено змiнювати записи з таблицi \"{e.Row.Table.TableName}\"!");
#endif
                throw new Exception($"SecureSQLiteContext::RowChanging(...): Користувачевi \"{_login}\" заборонено змiнювати записи з таблицi \"{e.Row.Table.TableName}\"!");
            }
        }
        public new static async Task<SecureSQLiteContext> FirstRun(string db_name)
        {
            SecureSQLiteContext ctx = new SecureSQLiteContext(db_name);
            await FirstRun(ctx, db_name);
            ctx.load_user();
            return ctx;
        }
        public bool SetDefaultUserRights(string login)
        {
            login = login.Trim().ToLower();

            DataTable usersTable = GetTable("users");
            DataTable rightsTable = GetTable("rights");

            DataRow[] rows = usersTable.Select($"login='{login}'");
            if (rows != null && rows.Length != 0) return false;
            
            DataRow newUser = usersTable.NewRow();
            newUser["id"] = usersTable.Rows.Count + 1;
            newUser["login"] = login;
            usersTable.Rows.Add(newUser);
#if DEBUG
            rows = usersTable.Select($"login='{login}'");
            if(rows != null)
            {
                Console.WriteLine($"Додано {rows.Length} користувачiв"); 
            }
#endif 
            rows = rightsTable.Select(string.Format("user_id={0}", DataSet.Tables["users"].Select("login='default'")[0]["Id"]));
            
            int id = rightsTable.Rows.Count + 1;
            for (int i = 0; i < rows.Length; i++)
            {
                DataRow newRights = rightsTable.NewRow();
                newRights["id"] = i + id;
                newRights["user_id"] = newUser["id"];
                rightsTable.Rows.Add(newRights);
            }
            return true;
        }
    }
}
