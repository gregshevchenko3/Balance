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
    public partial class LoginForm : Form
    {
        public SecureSQLiteContext Context { get; private set; } 
        public LoginForm(SecureSQLiteContext ctx = null)
        {
            InitializeComponent();
            Context = ctx;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if(Context == null)
            {
                byte[] key = null;
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    AuthStorage authStorage = AuthStorage.deserialize(Program.passwd_path);
                    key = authStorage.getDBKey(LoginBox.Text.Trim().ToLower(), PasswordBox.Text.Trim().ToLower());
                } 
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    Console.WriteLine(exc.StackTrace);
                }
                if (key == null)
                {
#if DEBUG
                    Console.WriteLine("Неправильний логiн або пароль");
#endif
                    status.ForeColor = Color.Red;
                    status.Text = "Wrong password or login.";
                }
                Context = new SecureSQLiteContext(Program.db_path, LoginBox.Text.Trim().ToLower(), key);
                Context.Load().Wait();
                Cursor.Current = Cursors.Default;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        private void textChanged(object sender, EventArgs e)
        {
            OkButton.Enabled = !string.IsNullOrEmpty(LoginBox.Text) && !string.IsNullOrEmpty(PasswordBox.Text);
        }
    }
}
