using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance.Models
{
    class Model
    {
        public string Table { get; set; }
        public SQLiteDataAdapter Adapter { get; private set; }
        public SQLiteCommandBuilder CmdBuilder{ get; private set; }
        public Model(SQLiteConnection connection){}
        public void Load(DataSet dataSet)
        {
            Adapter.Fill(dataSet, Table);
        }
        public void Save(DataSet dataSet)
        {
            Adapter.Update(dataSet);
        }
    }  
}
