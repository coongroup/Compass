using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace BatchFdrOptimizer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private static ModificationDictionary modifications;

        private void frmMain_Load(object sender, EventArgs e)
        {
            modifications = new ModificationDictionary(Path.Combine(Application.StartupPath, "mods.xml"));
            Peptide.SetModifications(modifications);
            UpdateModsListboxes();
        }

        private void UpdateModsListboxes()
        {
            lstAllModifications.Items.Clear();
            lstSelectedFixedModifications.Items.Clear();

            lstAllModifications.DisplayMember = "Text";
            lstSelectedFixedModifications.DisplayMember = "Text";
            foreach(Modification modification in modifications.Values)
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
                }
                else if(Path.GetExtension(filepath).Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    modifications.ReadModificationsFromXmlFile(filepath, true);
                    Peptide.SetModifications(modifications);
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
                modifications.ReadModificationsFromXmlFile(ofdModsXml.FileName, true);
                Peptide.SetModifications(modifications);
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            List<string> csv_filepaths = new List<string>(lstOmssaCsvFiles.Items.Count);
            foreach(string csv_filepath in lstOmssaCsvFiles.Items)
            {
                csv_filepaths.Add(csv_filepath);
            }
            string raw_folder = txtRawFolder.Text;
            List<Modification> fixed_modifications = new List<Modification>(lstSelectedFixedModifications.Items.Count);
            foreach(ListViewItem checked_item in lstSelectedFixedModifications.Items)
            {
                fixed_modifications.Add((Modification)checked_item.Tag);
            }
            double max_precursor_mass_error = (double)numMaximumPrecursorMassError.Value;
            double precursor_mass_error_increment = (double)numPrecursorMassErrorIncrement.Value;
            bool higher_scores_are_better = chkHigherScoresAreBetter.Checked;
            double max_false_discovery_rate = (double)numMaximumFalseDiscoveryRate.Value;
            bool unique = chkUnique.Checked;
            bool overall_outputs = chkOverallOutputs.Checked;
            bool phosphopeptide_outputs = chkPhosphopeptideOutputs.Checked;
            string output_folder = txtOutputFolder.Text;

            if(precursor_mass_error_increment > max_precursor_mass_error)
            {
                MessageBox.Show("Precursor mass error increment (" + precursor_mass_error_increment.ToString() + " ppm) must not be greater than maximum precursor mass error (" + max_precursor_mass_error.ToString() + " ppm)");
                return;
            }

            if(output_folder == null || output_folder == string.Empty)
            {
                MessageBox.Show("Output folder must be specified");
                return;
            }

            BatchFdrOptimizer batch_fdr_optimizer = new BatchFdrOptimizer(csv_filepaths, raw_folder,
                fixed_modifications,
                max_precursor_mass_error, precursor_mass_error_increment,
                higher_scores_are_better,
                max_false_discovery_rate, unique,
                overall_outputs, phosphopeptide_outputs, output_folder);

            batch_fdr_optimizer.Starting += handleStarting;
            batch_fdr_optimizer.StartingFile += handleStartingFile;
            batch_fdr_optimizer.UpdateProgress += handleUpdateProgress;
            batch_fdr_optimizer.ThrowException += handleThrowException;
            batch_fdr_optimizer.FinishedFile += handleFinishedFile;
            batch_fdr_optimizer.Finished += handleFinished;

            lstOmssaCsvFiles.SelectedItem = null;
            prgProgress.Value = prgProgress.Minimum;

            Thread thread = new Thread(new ThreadStart(batch_fdr_optimizer.Optimize));
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
                lstOmssaCsvFiles.SelectedItems.Remove(filepath);
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