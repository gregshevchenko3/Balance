using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Balance
{
    public partial class UserControl1 : UserControl
    {
        public DataTable ListDataTable { get; set; }
        public DataTable AccessDataTable { get; set; }
        public UserControl1()
        {
            InitializeComponent();
            
        }
        public void UpdateData()
        {
            listView1.Items.Clear();
            foreach (DataRow row in ListDataTable.Rows)
            {
                listView1.Items.Add(row.Field<string>("login"), 0);
            }
        }
        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            string usrname = listView1.SelectedItems[0].Text;
            UserRightsObject properties = new UserRightsObject(usrname, ((MainWindow)ParentForm).SecureSQLiteContext);
            RightsSettingsDialog dlg = new RightsSettingsDialog(usrname, properties);
            dlg.Show();
        }
    }
}
