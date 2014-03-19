using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OmssaNavigator;

namespace Coon.Compass
{
    public partial class CompassForm : Form
    {
        protected override void OnLoad(EventArgs e)
        {
            Text = "COMPASS " + GetRunningVersion().ToString();
            tsbCoondornator.Text = "OMSSA Navigator";
            base.OnLoad(e);
        }
     
        private void tsbCoondornator_Click(object sender, EventArgs e)
        {
            var form = new MainForm { MdiParent = this };
            form.Show();
        }
    }
}
