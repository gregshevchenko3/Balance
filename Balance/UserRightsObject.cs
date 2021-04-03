using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance
{
    public class UserRightsObject
    {
        /**------------------- Таблиця "users" ---------------*/
        /** Дозволити перегляд списку користувачів */
        public bool AllowReadUserList { get; set; }
        /** Дозволити змінювати користувачів */
        public bool AllowChangeUser { get; set; }
        /** Дозволити створення нових користувачів */
        public bool AllowUserAdd { get; set; }
        /** Дозволити видалення користувачів */
        public bool AllowUserDelete { get; set; }

        /**------------------ Таблиця "rights" ---------------*/
        /** Дозволити перегляд прав користувачів */
        public bool AllowReadUserRights { get; set; }
        /** Дозволити змінювати права користувачів */
        public bool AllowChangeUserRights { get; set; }
        /** Дозволити видаляти права користувачів */
        public bool AllowDeleteUserRights { get; set; }
        /** Дозволити cтворювати нові права для користувачів */
        public bool AllowCreateUserRights { get; set; }

        private SecureSQLiteContext _ctx;
        private int _user_id;

        public UserRightsObject(string userlogin, SecureSQLiteContext ctx)
        {
            _ctx = ctx;
            _user_id = _ctx.Select("users", $"login='{userlogin}'")[0].Field<int>("id");

            DataRow[] rows = _ctx.Select("rights", $"user_id={_user_id}");
            foreach(DataRow row in rows)
            {
                if(row.Field<string>("table") == "users")
                {
                    AllowReadUserList = (row.Field<string>("grandRead") == "y");
                    AllowChangeUser = (row.Field<string>("grandModify") == "y");
                    AllowUserAdd = (row.Field<string>("grandCreate") == "y");
                    AllowUserDelete = (row.Field<string>("grandDelete") == "y");
                }
                if(row.Field<string>("table") == "rights")
                {
                    AllowReadUserRights = (row.Field<string>("grandRead") == "y");
                    AllowChangeUserRights = (row.Field<string>("grandModify") == "y");
                    AllowDeleteUserRights = (row.Field<string>("grandDelete") == "y");
                    AllowCreateUserRights = (row.Field<string>("grandCreate") == "y");
                }
            }
        }
        public void SaveChanges()
        {
            DataRow row_for_table_users = _ctx.Select("rights", $"user_id={_user_id} AND table = 'users'")[0];
            DataRow row_for_table_rights = _ctx.Select("rights", $"user_id={_user_id} AND table = 'rights'")[0];

            row_for_table_users["grandRead"] = AllowReadUserList?"y":"n";
            row_for_table_users["grandModify"] = AllowChangeUser ? "y" : "n";
            row_for_table_users["grandCreate"] = AllowUserAdd ? "y" : "n";
            row_for_table_users["grandDelete"] = AllowUserDelete ? "y" : "n";
            _ctx.GetTable("users").AcceptChanges();
            row_for_table_rights["grandRead"] = AllowReadUserRights ? "y" : "n";
            row_for_table_rights["grandModify"] = AllowChangeUserRights ? "y" : "n";
            row_for_table_rights["grandCreate"] = AllowCreateUserRights ? "y" : "n";
            row_for_table_rights["grandDelete"] = AllowDeleteUserRights ? "y" : "n";
            _ctx.Save();
        }
    }
}
