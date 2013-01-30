using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace LowResFdrOptimizer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
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
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(ofdOmssaCsvFiles.ShowDialog() == DialogResult.OK)
            {
                lstOmssaCsvFiles.Items.AddRange(ofdOmssaCsvFiles.FileNames);
                UpdateOutputFolder(ofdOmssaCsvFiles.FileName);
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
        }

        private void btnBrowseOverallOutputFolder_Click(object sender, EventArgs e)
        {
            if(fbdOutputFolder.ShowDialog() == DialogResult.OK)
            {
                txtOutputFolder.Text = fbdOutputFolder.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            List<string> csv_filepaths = new List<string>(lstOmssaCsvFiles.Items.Count);
            foreach(string csv_filepath in lstOmssaCsvFiles.Items)
            {
                csv_filepaths.Add(csv_filepath);
            }
            bool higher_scores_are_better = chkHigherScoresAreBetter.Checked;
            double max_false_discovery_rate = (double)numMaximumFalseDiscoveryRate.Value;
            bool unique = chkUnique.Checked;
            bool overall_outputs = chkOverallOutputs.Checked;
            bool phosphopeptide_outputs = chkPhosphopeptideOutputs.Checked;
            string output_folder = txtOutputFolder.Text;

            if(output_folder == null || output_folder == string.Empty)
            {
                MessageBox.Show("Output folder must be specified");
                return;
            }

            LowResFdrOptimizer low_res_fdr_optimizer = new LowResFdrOptimizer(csv_filepaths,
                higher_scores_are_better,
                max_false_discovery_rate,
                unique,
                overall_outputs, phosphopeptide_outputs, output_folder);

            low_res_fdr_optimizer.Starting += handleStarting;
            low_res_fdr_optimizer.StartingFile += handleStartingFile;
            low_res_fdr_optimizer.UpdateProgress += handleUpdateProgress;
            low_res_fdr_optimizer.ThrowException += handleThrowException;
            low_res_fdr_optimizer.FinishedFile += handleFinishedFile;
            low_res_fdr_optimizer.Finished += handleFinished;

            lstOmssaCsvFiles.SelectedItem = null;

            Thread thread = new Thread(new ThreadStart(low_res_fdr_optimizer.Optimize));
            thread.IsBackground = true;
            thread.Start();
        }

        private delegate void changeMainPanelEnabledCallback(bool enabled);

        private void changeMainPanelEnabled(bool enabled)
        {
            if(pnlMain.InvokeRequired)
            {
                pnlMain.Invoke(new changeMainPanelEnabledCallback(changeMainPanelEnabled),
                    new object[] { enabled });
            }
            else
            {
                pnlMain.Enabled = enabled;
            }
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

        private delegate void unselectFileCallback(string filepath);

        private void unselectFile(string filepath)
        {
            if(lstOmssaCsvFiles.InvokeRequired)
            {
                lstOmssaCsvFiles.Invoke(new unselectFileCallback(unselectFile),
                    new object[] { filepath });
            }
            else
            {
                int i = 0;
                while(i < lstOmssaCsvFiles.SelectedItems.Count)
                {
                    string test_filepath = (string)lstOmssaCsvFiles.SelectedItems[i];

                    if(test_filepath == filepath)
                    {
                        lstOmssaCsvFiles.SelectedItems.Remove(test_filepath);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        private void handleFinishedFile(object sender, FilepathEventArgs e)
        {
            unselectFile(e.Filepath);
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
    }
}