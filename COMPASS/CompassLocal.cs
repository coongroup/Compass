using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Compass.Coondornator;

namespace Coon.Compass
{
    public partial class CompassForm : Form
    {
        protected override void OnLoad(EventArgs e)
        {
            Text = string.Format("COMPASS {0}-bit (v{1} Internal)", IntPtr.Size*8, GetRunningVersion());
            tsbCoondornator.Text = "Coondornator";
            base.OnLoad(e);
        }

        private void tsbCoondornator_Click(object sender, EventArgs e)
        {
            var coondornatorForm = new CoondornatorForm { MdiParent = this };
            coondornatorForm.Show();            
        }
    }
}
