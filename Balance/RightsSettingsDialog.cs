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
    public partial class RightsSettingsDialog : Form
    {
        public RightsSettingsDialog(string userlogin, UserRightsObject obj)
        {
            InitializeComponent();
            loginValue.Text = userlogin;
            propertyGrid1.SelectedObject = obj;
        }
        private void RightsSettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((UserRightsObject)propertyGrid1.SelectedObject).SaveChanges();
        }
    }
}
