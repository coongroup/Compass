using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Coon.Compass.DtaGenerator
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            checkedListBox2.Items.Add(new KeyValuePair<string,double>("CO2",43.98983));
            checkedListBox2.Items.Add(new KeyValuePair<string, double>("H2O", 18.010565));
            checkedListBox2.Items.Add(new KeyValuePair<string, double>("NH3", 17.02655));
        }

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach(string filepath in filepaths)
                {
                    if(Path.GetExtension(filepath).Equals(".raw", StringComparison.InvariantCultureIgnoreCase) &&
                        !lstRawFiles.Items.Contains(filepath))
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
                if(Path.GetExtension(filepath).Equals(".raw", StringComparison.InvariantCultureIgnoreCase) &&
                    !lstRawFiles.Items.Contains(filepath))
                {
                    lstRawFiles.Items.Add(filepath);
                    UpdateOutputFolder(filepath);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(ofdRawFiles.ShowDialog() == DialogResult.OK)
            {
                foreach(string filename in ofdRawFiles.FileNames)
                {
                    if(!lstRawFiles.Items.Contains(filename))
                    {
                        lstRawFiles.Items.Add(filename);
                        UpdateOutputFolder(filename);
                    }
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
            while(lstRawFiles.SelectedItems.Count > 0)
            {
                lstRawFiles.Items.RemoveAt(lstRawFiles.SelectedIndex);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstRawFiles.Items.Clear();
            prgProgress.Value = prgProgress.Minimum;
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            Process.Start("http://dx.doi.org/10.1016/j.jasms.2009.03.006");
        }

        private void btnBrowse_Click(object sender, EventArgs e)
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
            List<string> raw_filepaths = new List<string>(lstRawFiles.Items.Count);
            foreach(string filename in lstRawFiles.Items)
            {
                raw_filepaths.Add(filename);
            }
            int minimum_assumed_precursor_charge_state = (int)numMinimumAssumedPrecursorChargeState.Value;
            int maximum_assumed_precursor_charge_state = (int)numMaximumAssumedPrecursorChargeState.Value;
            if(minimum_assumed_precursor_charge_state > maximum_assumed_precursor_charge_state)
            {
                MessageBox.Show("Minimum assumed precursor charge state (" + minimum_assumed_precursor_charge_state.ToString() + ") cannot be larger than maximum asssumed precursor charge state (" + maximum_assumed_precursor_charge_state.ToString() + ')');
                return;
            }
            bool clean_precursor = chkCleanPrecursor.Checked;
            bool enable_etd_preprocessing = chkEnableEtdPreProcessing.Checked;
            bool clean_tmt_duplex = chkCleanTmtDuplex.Checked;
            bool clean_itraq_4plex = chkCleanItraq4Plex.Checked;
            bool clean_tmt_6plex = chkCleanTmt6Plex.Checked;
            bool clean_itraq_8plex = chkCleanItraq8Plex.Checked;
            bool group_by_activation_energy_time = chkGroupByActivationEnergyTime.Checked;
            bool sequest_dta_output = chkSequestDtaOutput.Checked;
            bool omssa_text_output = chkOmssaTxtOutput.Checked;
            bool mascot_mgf_output = chkMascotMgfOutput.Checked;
            bool includeLog = includeLogCB.Checked;
            string output_folder = txtOutputFolder.Text;


            if(output_folder == string.Empty)
            {
                MessageBox.Show("Output folder must be specified");
                return;
            }
            List<double> nlmass = new List<double>();
            foreach(KeyValuePair<string, double> kvp in checkedListBox2.CheckedItems) {
                nlmass.Add(kvp.Value);
            }

            DtaGenerator dta_generator = new DtaGenerator(raw_filepaths,
                minimum_assumed_precursor_charge_state, maximum_assumed_precursor_charge_state,
                clean_precursor, enable_etd_preprocessing,
                clean_tmt_duplex, clean_itraq_4plex, clean_tmt_6plex, clean_itraq_8plex,
                group_by_activation_energy_time,
                sequest_dta_output, omssa_text_output, mascot_mgf_output,
                output_folder,
                nlmass,
                includeLog);

            dta_generator.Starting += handleStarting;
            dta_generator.StartingFile += handleStartingFile;
            dta_generator.UpdateProgress += handleUpdateProgress;
            dta_generator.ThrowException += handleThrowException;
            dta_generator.FinishedFile += handleFinishedFile;
            dta_generator.Finished += handleFinished;

            lstRawFiles.SelectedItem = null;
            prgProgress.Value = prgProgress.Minimum;

            Thread generate_dtas = new Thread(new ThreadStart(dta_generator.GenerateDtas));
            generate_dtas.IsBackground = true;
            generate_dtas.Start();
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
            if(lstRawFiles.InvokeRequired)
            {
                lstRawFiles.Invoke(new changeCurrentFileCallback(changeCurrentFile),
                    new object[] { filepath });
            }
            else
            {
                lstRawFiles.SelectedItem = filepath;
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

        private void chkCleanItraq4Plex_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}