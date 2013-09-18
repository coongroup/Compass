using System;
using System.IO;
using System.Windows.Forms;

namespace Phosphinator
{
    public partial class frmAdvanced : Form
    {
        public bool EliminatePrecursorInterference
        {
            get { return chkEliminatePrecursorInterference.Checked; }
        }

        public double PrecursorInterferenceThreshold
        {
            get { return (double)numPrecursorInterferenceThreshold.Value; }
        }

        public bool MotifXOutput
        {
            get { return chkMotifXOutputs.Checked; }
        }

        public string MotifXFastaProteinDatabaseFilepath
        {
            get { return txtFasta.Text; }
        }

        public int MotifXWindowSize
        {
            get { return (int)numWindowSize.Value; }
        }

        public frmAdvanced()
        {
            InitializeComponent();
        }

        private void frmAdvanced_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
        }

        private void frmAdvanced_DragDrop(object sender, DragEventArgs e)
        {
            string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            txtFasta.Text = filepaths[0];
            chkMotifXOutputs.Checked = true;
        }

        private void chkEliminatePrecursorInterference_CheckedChanged(object sender, EventArgs e)
        {
            if(chkEliminatePrecursorInterference.Checked)
            {
                label2.Enabled = true;
                numPrecursorInterferenceThreshold.Enabled = true;
                label5.Enabled = true;
            }
            else
            {
                label2.Enabled = false;
                numPrecursorInterferenceThreshold.Enabled = false;
                label5.Enabled = false;
            }
        }

        private void chkMotifXOutputs_CheckedChanged(object sender, EventArgs e)
        {
            if(chkMotifXOutputs.Checked)
            {
                label4.Enabled = true;
                txtFasta.Enabled = true;
                label3.Enabled = true;
                numWindowSize.Enabled = true;
                btnBrowseFasta.Enabled = true;
            }
            else
            {
                label4.Enabled = false;
                txtFasta.Enabled = false;
                label3.Enabled = false;
                numWindowSize.Enabled = false;
                btnBrowseFasta.Enabled = false;
            }
        }

        private void btnBrowseFasta_Click(object sender, EventArgs e)
        {
            if(ofdFasta.ShowDialog() == DialogResult.OK)
            {
                txtFasta.Text = ofdFasta.FileName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmAdvanced_FormClosing(object sender, FormClosingEventArgs e)
        {
            if((int)numWindowSize.Value % 2 == 0)
            {
                MessageBox.Show("Motif-X window size must be an odd number");
                numWindowSize.Focus();
                e.Cancel = true;
            }
        }
    }
}
