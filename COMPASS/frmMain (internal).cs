using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using Coon.Compass.DatabaseMaker;
using Coon.Compass.Lotor;
using Coon.Compass.ProteinHoarder;
using Coon.Compass.TagQuant;
using Phosphinator;

namespace Coon.Compass
{
    public partial class frmMain : Form
    {
        private Version GetRunningVersion()
        {
            try
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }  

        public frmMain()
        {
            InitializeComponent();

            Text += ' ' + GetRunningVersion().ToString();
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

        private void aboutCompassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }

        private void tsbDatabaseMaker_Click(object sender, EventArgs e)
        {
            DatabaseMakerForm form = new DatabaseMakerForm {MdiParent = this};
            form.Show();
        }

        private void tsbDtaGenerator_Click(object sender, EventArgs e)
        {
            DtaGenerator.frmMain dta_generator = new DtaGenerator.frmMain();
            dta_generator.MdiParent = this;
            dta_generator.Show();
        }

        private void tsbCoondornator_Click(object sender, EventArgs e)
        {
            Process.Start(@"\\coongrp\Groups\Condor\Coondornator\Coondornator.application");
        }

        private void tsmiFdrOptimizer_Click(object sender, EventArgs e)
        {
            FdrOptimizer.frmMain fdr_optimizer = new FdrOptimizer.frmMain();
            fdr_optimizer.MdiParent = this;
            fdr_optimizer.Show();
        }

        private void tsmiBatchFdrOptimizer_Click(object sender, EventArgs e)
        {
            BatchFdrOptimizer.frmMain batch_fdr_optimizer = new BatchFdrOptimizer.frmMain();
            batch_fdr_optimizer.MdiParent = this;
            batch_fdr_optimizer.Show();
        }

        private void tsmiLowResFdrOptimizer_Click(object sender, EventArgs e)
        {
            LowResFdrOptimizer.frmMain low_res_fdr_optimizer = new LowResFdrOptimizer.frmMain();
            low_res_fdr_optimizer.MdiParent = this;
            low_res_fdr_optimizer.Show();
        }

        private void tsmiBatchLowResFdrOptimizer_Click(object sender, EventArgs e)
        {
            BatchLowResFdrOptimizer.frmMain batch_low_res_fdr_optimizer = new BatchLowResFdrOptimizer.frmMain();
            batch_low_res_fdr_optimizer.MdiParent = this;
            batch_low_res_fdr_optimizer.Show();
        }

        private void tsbTagQuant_Click(object sender, EventArgs e)
        {
            TagQuantForm tag_quant = new TagQuantForm();
            tag_quant.MdiParent = this;
            tag_quant.Show();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ProteinHoarderForm proteinHoarder = new ProteinHoarderForm {MdiParent = this};
            proteinHoarder.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var lotorForm = new lotorForm {MdiParent = this};
            lotorForm.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            var phosphintaorForm = new phosphinatorForm {MdiParent = this};
            phosphintaorForm.Show();
        }
    }
}
