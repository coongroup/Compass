using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Phosphinator
{
    public partial class phosphinatorForm : Form
    {
        private frmAdvanced advanced = new frmAdvanced();

        public phosphinatorForm()
        {
            InitializeComponent();
            this.Text = "Phosphinator v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private static ModificationDictionary modifications;

        private void frmMain_Load(object sender, EventArgs e)
        {
            modifications = new ModificationDictionary(Application.StartupPath);

            lstAllModifications.DisplayMember = "Text";
            lstSelectedFixedModifications.DisplayMember = "Text";
            foreach(Modification modification in modifications.Values)
            {
                ListViewItem list_view_item = new ListViewItem(modification.Name);
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

            Peptide.SetModifications(modifications);

            cboFragmentIntensityThresholdType.DataSource = Enum.GetValues(typeof(IntensityThresholdType));
            cboFragmentIntensityThresholdType.SelectedItem = IntensityThresholdType.Absolute;
        }

        private void LoadUserMods(string filepath)
        {
            Peptide.MODIFICATIONS.ReadModificationsFromXmlFile(filepath);
            lstSelectedFixedModifications.Items.Clear();
            lstSelectedFixedModifications.Items.Clear();
            lstAllModifications.DisplayMember = "Text";
            lstSelectedFixedModifications.DisplayMember = "Text";
            foreach (Modification modification in Peptide.MODIFICATIONS.Values)
            {
                ListViewItem list_view_item = new ListViewItem(modification.Name);
                list_view_item.Tag = modification;
                if (modification.Name == "carbamidomethyl C")
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
            e.Effect = DragDropEffects.Link;
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
                if (Path.GetExtension(filepath).Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    LoadUserMods(filepath);
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

        private void UpdateRawFolder(string filepath)
        {
            if (txtRawFolder.Text == string.Empty)
            {
                txtRawFolder.Text = Path.GetDirectoryName(filepath);
            }
        }

        private void UpdateOutputFolder(string filepath)
        {
            if(txtOutputFolder.Text == string.Empty)
            {
                txtOutputFolder.Text = Path.GetDirectoryName(filepath);
            }
            UpdateRawFolder(filepath);
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

        private void frmMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!txtRawFolder.Focused && !txtOutputFolder.Focused)
            {
                int index1 = -1;
                int index2 = -1;
                switch(e.KeyChar)
                {
                    case 't':
                        index1 = lstAllModifications.FindStringExact("TMT duplex on n-term peptide");
                        index2 = lstAllModifications.FindStringExact("TMT duplex on K");
                        e.Handled = true;
                        break;
                    case 'T':
                        index1 = lstAllModifications.FindStringExact("TMT 6-plex on n-term peptide");
                        index2 = lstAllModifications.FindStringExact("TMT 6-plex on K");
                        e.Handled = true;
                        break;
                    case 'i':
                        index1 = lstAllModifications.FindStringExact("iTRAQ 4-plex on peptide N-terminus");
                        index2 = lstAllModifications.FindStringExact("iTRAQ 4-plex on K");
                        e.Handled = true;
                        break;
                    case 'I':
                        index1 = lstAllModifications.FindStringExact("iTRAQ 8-plex on peptide N-terminus");
                        index2 = lstAllModifications.FindStringExact("iTRAQ 8-plex on K");
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

        private void cboIntensityThresholdType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateFragmentIntensityThreshold();
        }

        private void txtFragmentIntensityThreshold_TextChanged(object sender, EventArgs e)
        {
            ValidateFragmentIntensityThreshold();
        }

        private void ValidateFragmentIntensityThreshold()
        {
            if((IntensityThresholdType)cboFragmentIntensityThresholdType.SelectedItem == IntensityThresholdType.Relative)
            {
                double intensity_threshold;
                if(double.TryParse(txtFragmentIntensityThreshold.Text, out intensity_threshold) && intensity_threshold > 100.0)
                {
                    txtFragmentIntensityThreshold.Text = "100.0";
                }
            }
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            advanced.ShowDialog();
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            if(Directory.Exists(txtOutputFolder.Text))
            {
                fbdOutput.SelectedPath = txtOutputFolder.Text;
            }
            if(fbdOutput.ShowDialog() == DialogResult.OK)
            {
                txtOutputFolder.Text = fbdOutput.SelectedPath;
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
            double intensity_threshold = double.Parse(txtFragmentIntensityThreshold.Text);
            IntensityThresholdType intensity_threshold_type = (IntensityThresholdType)cboFragmentIntensityThresholdType.SelectedItem;
            double mz_tolerance = (double)numFragmentMZTolerance.Value;
            double ambiguity_score_threshold = (double)numAmbiguityScoreThreshold.Value;
            bool eliminate_precursor_interference = advanced.EliminatePrecursorInterference;
            double precursor_interference_threshold = advanced.PrecursorInterferenceThreshold / 100;
            bool motif_x_output = advanced.MotifXOutput;
            string motif_x_fasta_protein_database_filepath = advanced.MotifXFastaProteinDatabaseFilepath;
            int motif_x_window_size = advanced.MotifXWindowSize;
            string output_folder = txtOutputFolder.Text;

            if(output_folder == string.Empty)
            {
                MessageBox.Show("Output folder must be specified");
                return;
            }

            Phosphinator phosphinator = new Phosphinator(csv_filepaths, 
                raw_folder,
                fixed_modifications, 
                intensity_threshold, intensity_threshold_type,
                mz_tolerance, 
                ambiguity_score_threshold,
                eliminate_precursor_interference, precursor_interference_threshold, 
                motif_x_output, motif_x_fasta_protein_database_filepath, motif_x_window_size, 
                output_folder);

            phosphinator.Starting += handleStarting;
            phosphinator.StartingFile += handleStartingFile;
            phosphinator.UpdateProgress += handleUpdateProgress;
            phosphinator.ThrowException += handleThrowException;
            phosphinator.Finished += handleFinished;

            lstOmssaCsvFiles.SelectedItem = null;
            prgProgress.Value = prgProgress.Minimum;

            Thread thread = new Thread(new ThreadStart(phosphinator.Phosphinate));
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