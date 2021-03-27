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
    public partial class CreateNewUserForm : Form
    {
        private object _ctx;
        private AuthStorage _authData;
        public CreateNewUserForm(object ctx, AuthStorage auth)
        {
            InitializeComponent();
            _authData = auth;
            _ctx = ctx;

        }
        private T WaitIfTask<T>(object maybeTask)
        {
            T result;
            if(typeof(Task<T>) == maybeTask.GetType())
            {
                ((Task<T>)maybeTask).Wait();
                result = ((Task<T>)maybeTask).Result;
            } 
            else
            {
                result = (T)maybeTask;
            }
            return result;
        }
        private void RegisterBtn_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            SecureSQLiteContext ctx = WaitIfTask<SecureSQLiteContext>(_ctx);
            this.Cursor = Cursors.Default;
            if(PasswordBox1.Text.Trim() != PasswordBox2.Text.Trim())
            {
                status.ForeColor = Color.Red;
                status.Text = $"Введенi паролi не спiвпадаютью";
            }
            else if(!_authData.check_quality(PasswordBox1.Text.Trim()))
            {
                status.ForeColor = Color.Red;
                status.Text = "Пароль недостатньо надiйний";
            }
            else if (ctx.SetDefaultUserRights(LoginBox.Text)) {
                status.ForeColor = Color.Green;
                status.Text = "Користувача добавлено, права - надано";
                var sb = new StringBuilder(ctx.Key.Length * 2);
                foreach (byte b in ctx.Key)
                    sb.Append(b.ToString("x2"));
                _authData.appendUser(LoginBox.Text.Trim().ToLower(), PasswordBox1.Text.Trim().ToLower(), sb.ToString());
                
                DialogResult = DialogResult.OK;
                Close();
            } else
            {
                status.ForeColor = Color.Red;
                status.Text = $"Користувач '{LoginBox.Text}' iснує.";
            }
        }
        private void textChanged(object sender, EventArgs e)
        {
            RegisterBtn.Enabled = !string.IsNullOrEmpty(LoginBox.Text) && !string.IsNullOrEmpty(PasswordBox1.Text) &&
                !string.IsNullOrEmpty(PasswordBox2.Text);
        }
    }
}
