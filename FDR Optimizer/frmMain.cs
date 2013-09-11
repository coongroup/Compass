using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CSMSL.IO.OMSSA;
using CSMSL.Proteomics;

namespace Coon.Compass.FdrOptimizer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            UpdateModsListboxes();
            comboBox1.DataSource = Enum.GetValues(typeof (UniquePeptideType));
            comboBox1.SelectedItem = UniquePeptideType.SequenceAndModifactions;
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            
        }

        private void UpdateModsListboxes()
        {
            lstAllModifications.Items.Clear();
            lstSelectedFixedModifications.Items.Clear();

            lstAllModifications.DisplayMember = "Text";
            lstSelectedFixedModifications.DisplayMember = "Text";
            foreach (OmssaModification modification in OmssaModification.GetAllModifications())
            {
                ListViewItem list_view_item = new ListViewItem(modification.ToString());
                list_view_item.Tag = modification;
                if(modification.Name == "carbamidomethyl C")
                {
                    lstSelectedFixedModifications.Items.Add(list_view_item);
                }
                else
                {
                    lstAllModifications.Items.Add(list_view_item);
                }
            }
        }

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach(string filepath in filepaths)
                {
                    if(Path.GetExtension(filepath).Equals(".csv", StringComparison.InvariantCultureIgnoreCase) &&
                        !lstOmssaCsvFiles.Items.Contains(filepath))
                    {
                        e.Effect = DragDropEffects.Link;
                        break;
                    }
                    else if(Path.GetExtension(filepath).Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                    {
                        e.Effect = DragDropEffects.Link;
                        break;
                    }
                }
            }
        }

        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach(string filepath in filepaths)
            {
                if(Path.GetExtension(filepath).Equals(".csv", StringComparison.InvariantCultureIgnoreCase) &&
                    !lstOmssaCsvFiles.Items.Contains(filepath))
                {
                    lstOmssaCsvFiles.Items.Add(filepath);
                    UpdateOutputFolder(filepath);
                    UpdateRawFolder(filepath);
                }
                else if(Path.GetExtension(filepath).Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    OmssaModification.LoadOmssaModifications(filepath,true);
                    UpdateModsListboxes();
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(ofdOmssaCsvFiles.ShowDialog() == DialogResult.OK)
            {
                lstOmssaCsvFiles.Items.AddRange(ofdOmssaCsvFiles.FileNames);
                UpdateOutputFolder(ofdOmssaCsvFiles.FileName);
                UpdateRawFolder(ofdOmssaCsvFiles.FileName);
            }
        }

        private void UpdateRawFolder(string filepath)
        {
            if (txtRawFolder.Text == string.Empty)
            {
                string rawFile = Path.GetFileNameWithoutExtension(filepath);
                string[] files = null;
                while ((files = Directory.GetFiles(Path.GetDirectoryName(filepath), "*.raw", SearchOption.AllDirectories)) != null)
                {
                    foreach (string file in files)
                    {
                        txtRawFolder.Text = Path.GetDirectoryName(file);
                        return;
                    }
                    filepath = Directory.GetParent(filepath).FullName;
                }              
            }
        }

        private void UpdateOutputFolder(string filepath)
        {
            if(txtOutputFolder.Text == string.Empty)
            {
                txtOutputFolder.Text = Path.GetDirectoryName(filepath);             
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            while(lstOmssaCsvFiles.SelectedIndices.Count > 0)
            {
                lstOmssaCsvFiles.Items.RemoveAt(lstOmssaCsvFiles.SelectedIndices[0]);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstOmssaCsvFiles.Items.Clear();
            prgProgress.Value = prgProgress.Minimum;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if(fbdRawFolder.ShowDialog() == DialogResult.OK)
            {
                txtRawFolder.Text = fbdRawFolder.SelectedPath;
            }
        }

        private void btnMoveRight_Click(object sender, EventArgs e)
        {
            while(lstAllModifications.SelectedItems.Count > 0)
            {
                lstSelectedFixedModifications.Items.Add(lstAllModifications.SelectedItem);
                lstAllModifications.Items.Remove(lstAllModifications.SelectedItem);
            }
        }

        private void btnMoveLeft_Click(object sender, EventArgs e)
        {
            while(lstSelectedFixedModifications.SelectedItems.Count > 0)
            {
                lstAllModifications.Items.Add(lstSelectedFixedModifications.SelectedItem);
                lstSelectedFixedModifications.Items.Remove(lstSelectedFixedModifications.SelectedItem);
            }
        }

        private void btnBrowseMods_Click(object sender, EventArgs e)
        {
            if(ofdModsXml.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OmssaModification.LoadOmssaModifications(ofdModsXml.FileName,true);
                UpdateModsListboxes();
            }
        }

        private void frmMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!txtRawFolder.Focused && !txtOutputFolder.Focused)
            {
                int index1 = -1;
                int index2 = -1;
                switch(e.KeyChar)
                {
                    case 'i':
                        index1 = lstAllModifications.FindString("iTRAQ 4-plex on n-term peptide");
                        index2 = lstAllModifications.FindString("iTRAQ 4-plex on K");
                        e.Handled = true;
                        break;
                    case 'I':
                        index1 = lstAllModifications.FindString("iTRAQ 8-plex on n-term peptide");
                        index2 = lstAllModifications.FindString("iTRAQ 8-plex on K");
                        e.Handled = true;
                        break;
                    case 't':
                        index1 = lstAllModifications.FindString("TMT duplex on n-term peptide");
                        index2 = lstAllModifications.FindString("TMT duplex on K");
                        e.Handled = true;
                        break;
                    case 'T':
                        index1 = lstAllModifications.FindString("TMT 6-plex on n-term peptide");
                        index2 = lstAllModifications.FindString("TMT 6-plex on K");
                        e.Handled = true;
                        break;
                }
                if(index1 >= 0)
                {
                    lstSelectedFixedModifications.Items.Add(lstAllModifications.Items[index1]);
                    lstAllModifications.Items.RemoveAt(index1);
                }
                if(index2 >= 0)
                {
                    lstSelectedFixedModifications.Items.Add(lstAllModifications.Items[index2]);
                    lstAllModifications.Items.RemoveAt(index2);
                }
            }
        }

        private void btnBrowseOverallOutputFolder_Click(object sender, EventArgs e)
        {
            if(fbdOutputFolder.ShowDialog() == DialogResult.OK)
            {
                txtOutputFolder.Text = fbdOutputFolder.SelectedPath;
            }
        }

        private void Run()
        {
            richTextBox1.Clear();
            string rawFolder = txtRawFolder.Text;

            List<string> csvFilepaths = new List<string>(lstOmssaCsvFiles.Items.Cast<string>());
            List<Modification> fixedModifications = new List<Modification>(from ListViewItem checkedItem in lstSelectedFixedModifications.Items select (Modification)checkedItem.Tag);

            double maxPrecursorMassError = (double)numMaximumPrecursorMassError.Value;
            double precursorMassErrorIncrement = (double)numPrecursorMassErrorIncrement.Value;
            bool higherScoresAreBetter = chkHigherScoresAreBetter.Checked;
            double maxFalseDiscoveryRate = (double)numMaximumFalseDiscoveryRate.Value;
            bool overallOutputs = chkOverallOutputs.Checked;
            bool phosphopeptideOutputs = chkPhosphopeptideOutputs.Checked;
            bool isBatched = checkBox1.Checked;
            bool is2DFDR = radioButton2.Checked;
            bool includeFixedMods = checkBox2.Checked;
            string outputFolder = txtOutputFolder.Text;
            UniquePeptideType uniquePeptideType = (UniquePeptideType)comboBox1.SelectedValue;

            if (precursorMassErrorIncrement > maxPrecursorMassError)
            {
                MessageBox.Show("Precursor mass error increment (" + precursorMassErrorIncrement.ToString() + " ppm) must not be greater than maximum precursor mass error (" + maxPrecursorMassError.ToString() + " ppm)");
                return;
            }

            if (string.IsNullOrEmpty(outputFolder))
            {
                MessageBox.Show("Output folder must be specified");
                return;
            }
            Directory.CreateDirectory(outputFolder);

            FdrOptimizer fdrOptimizer = new FdrOptimizer(csvFilepaths, rawFolder,
                fixedModifications,
                maxPrecursorMassError, precursorMassErrorIncrement,
                higherScoresAreBetter,
                maxFalseDiscoveryRate, uniquePeptideType,
                overallOutputs, phosphopeptideOutputs, outputFolder, isBatched, is2DFDR, includeFixedMods);

            fdrOptimizer.Starting += handleStarting;
            fdrOptimizer.StartingFile += handleStartingFile;
            fdrOptimizer.UpdateProgress += handleUpdateProgress;
            fdrOptimizer.ThrowException += handleThrowException;
            fdrOptimizer.FinishedFile += handleFinishedFile;
            fdrOptimizer.Finished += handleFinished;
            fdrOptimizer.UpdateLog += fdrOptimizer_UpdateLog;

            lstOmssaCsvFiles.SelectedItem = null;
            prgProgress.Value = prgProgress.Minimum;


            Thread thread = new Thread(fdrOptimizer.Optimize) {IsBackground = true};
            thread.Start();
        }

        void fdrOptimizer_UpdateLog(object sender, StatusEventArgs e)
        {
            UpdateLog(e.Message);
        }
   
        private delegate void UpdateLogDelegate(string msg);

        public void UpdateLog(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateLogDelegate(UpdateLog), new object[] { msg });
            }
            else
            {
                richTextBox1.AppendText(string.Format("[{0}]\t{1}\n", DateTime.Now.ToLongTimeString(), msg));
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
        }
 
        private delegate void changeMainPanelEnabledCallback(bool enabled);

        private void changeMainPanelEnabled(bool enabled)
        {
            //if(pnlMain.InvokeRequired)
            //{
            //    pnlMain.Invoke(new changeMainPanelEnabledCallback(changeMainPanelEnabled),
            //        new object[] { enabled });
            //}
            //else
            //{
            //    pnlMain.Enabled = enabled;
            //}
        }

        private void handleStarting(object sender, EventArgs e)
        {
            changeMainPanelEnabled(false);
        }

        private delegate void changeCurrentFileCallback(string filepath);

        private void changeCurrentFile(string filepath)
        {
            if(lstOmssaCsvFiles.InvokeRequired)
            {
                lstOmssaCsvFiles.Invoke(new changeCurrentFileCallback(changeCurrentFile),
                    new object[] { filepath });
            }
            else
            {
                lstOmssaCsvFiles.SelectedItem = filepath;
            }
        }

        private void handleStartingFile(object sender, FilepathEventArgs e)
        {
            changeCurrentFile(e.Filepath);
        }

        private delegate void changeProgressBarValueCallback(int progressValue);

        private void changeProgressBarValue(int progressValue)
        {
            if(prgProgress.InvokeRequired)
            {
                prgProgress.Invoke(new changeProgressBarValueCallback(changeProgressBarValue),
                    new object[] { progressValue });
            }
            else
            {
                prgProgress.Value = progressValue;
            }
        }

        private void handleUpdateProgress(object sender, ProgressEventArgs e)
        {
            changeProgressBarValue(e.Progress);
        }

        private void handleThrowException(object sender, ExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
            changeCurrentFile(null);
            changeMainPanelEnabled(true);
        }

        private void handleFinishedFile(object sender, FilepathEventArgs e)
        {
            changeCurrentFile(null);
        }

        private void handleFinished(object sender, EventArgs e)
        {
            changeCurrentFile(null);
            changeMainPanelEnabled(true);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            numMaximumPrecursorMassError.Enabled = true;
            numPrecursorMassErrorIncrement.Enabled = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            numMaximumPrecursorMassError.Enabled = false;
            numPrecursorMassErrorIncrement.Enabled = false;
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            Run();
        }

   
    }
}