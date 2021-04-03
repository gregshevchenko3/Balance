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
    public partial class MainWindow : Form
    {
        public SecureSQLiteContext SecureSQLiteContext { get; set; }
        private Control _current;
        public MainWindow(SecureSQLiteContext ctx)
        {
            InitializeComponent();
            SecureSQLiteContext = ctx;
        }
        private void UserManagement_Click(object sender, EventArgs e)
        {
            if(_current != null) _current.Hide();
            userControl11.ListDataTable = SecureSQLiteContext.GetTable("users");
            userControl11.UpdateData();
            _current = userControl11;
            _current.Show();
        }

        private void SaveUserData(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            userControl11.Hide();
        }
        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            SecureSQLiteContext.Unload().Wait();
        }
    }
}
