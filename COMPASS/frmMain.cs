using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace COMPASS
{
    public partial class frmMain : Form
    {
        private string AssemblyVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public frmMain()
        {
            InitializeComponent();

            Text += ' ' + AssemblyVersion;
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
            DatabaseMaker.frmMain database_maker = new DatabaseMaker.frmMain();
            database_maker.MdiParent = this;
            database_maker.Show();
        }

        private void tsbDtaGenerator_Click(object sender, EventArgs e)
        {
            DtaGenerator.frmMain dta_generator = new DtaGenerator.frmMain();
            dta_generator.MdiParent = this;
            dta_generator.Show();
        }

        private void tsbOmssaNavigator_Click(object sender, EventArgs e)
        {
            OmssaNavigator.MainForm omssa_navigator = new OmssaNavigator.MainForm();
            omssa_navigator.MdiParent = this;
            omssa_navigator.Show();
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
         
            TagQuant.TagQuantForm tag_quant = new TagQuant.TagQuantForm();
            tag_quant.MdiParent = this;
            tag_quant.Show();
        }

        private void tsbProteinHerder_Click(object sender, EventArgs e)
        {
            ProteinHerder.frmMain protein_herder = new ProteinHerder.frmMain();
            protein_herder.MdiParent = this;
            protein_herder.Show();
        }

        private void tsbProteinTagQuant_Click(object sender, EventArgs e)
        {
            ProteinTagQuant.PTQ_Form protein_tag_quant = new ProteinTagQuant.PTQ_Form();
            protein_tag_quant.MdiParent = this;
            protein_tag_quant.Show();
        }
    }
}
