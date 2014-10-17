using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using Coon.Compass.DatabaseMaker;
using Coon.Compass.Lotor;
using Coon.Compass.PhosphoRS;
using Coon.Compass.ProteinHoarder;
using Coon.Compass.TagQuant;
using Coon.Compass.Procyon;
using Phosphinator;

namespace Coon.Compass
{
    public partial class CompassForm : Form
    {
        public static Version GetRunningVersion()
        {        
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            return Assembly.GetExecutingAssembly().GetName().Version;
        }  

        public CompassForm()
        {
            InitializeComponent();
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void crgMainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.chem.wisc.edu/~coon/");
        }

        private void crgSoftwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.chem.wisc.edu/~coon/software.php");
        }

        private void uwmMainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.wisc.edu/");
        }

        private void uwmDepartmentOfChemistryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://chem.wisc.edu/");
        }

        private void sourceCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/dbaileychess/Compass");
        }

        private void aboutCompassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new frmAbout();
            about.ShowDialog();
        }

        private void tsbDatabaseMaker_Click(object sender, EventArgs e)
        {
            var form = new DatabaseMakerForm {MdiParent = this};
            form.Show();
        }

        private void tsbDtaGenerator_Click(object sender, EventArgs e)
        {
            var dtaGenerator = new DtaGenerator.frmMain {MdiParent = this};
            dtaGenerator.Show();
        }

        private void tsbTagQuant_Click(object sender, EventArgs e)
        {
            var tagQuant = new TagQuantForm {MdiParent = this};
            tagQuant.Show();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var proteinHoarder = new ProteinHoarderForm {MdiParent = this};
            proteinHoarder.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var phosphoForm = new PhosphoRsForm{MdiParent = this};
            phosphoForm.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            var phosphintaorForm = new phosphinatorForm {MdiParent = this};
            phosphintaorForm.Show();
        }        

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            var fdrOptimizer = new FdrOptimizer.FdrOptimizerForm { MdiParent = this };
            fdrOptimizer.Show();
        }

        private void lowResToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lowResFDROptimizer = new LowResFdrOptimizer.frmMain { MdiParent = this };
            lowResFDROptimizer.Show();
        }

        private void lowResToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var batchLowResFDROptimizer = new BatchLowResFdrOptimizer.frmMain { MdiParent = this };
            batchLowResFDROptimizer.Show();
        }

        private void highResToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var batchFDROptimizer = new BatchFdrOptimizer.frmMain { MdiParent = this };
            batchFDROptimizer.Show();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            var procyonForm = new ProcyonForm { MdiParent = this };
            procyonForm.Show();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            var lotor = new lotorForm { MdiParent = this };
            lotor.Show();
        }

        private void getLocalDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath);
        }

       

    }
}
