using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CSMSL;

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
            int minimum_assumed_precursor_charge_state = 2; //(int)numMinimumAssumedPrecursorChargeState.Value;
            int maximum_assumed_precursor_charge_state = 2; // (int)numMaximumAssumedPrecursorChargeState.Value;
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

            double clnPrecursorLowMZ = (double)numericUpDown1.Value;
            double clnPrecursorHighMZ = (double)numericUpDown2.Value;

            double etdLowDa = (double)numericUpDown3.Value;
            double etdHighDa = (double)numericUpDown4.Value;

            if(output_folder == string.Empty)
            {
                MessageBox.Show("Output folder must be specified");
                return;
            }
            List<double> nlmass = new List<double>();
            foreach(KeyValuePair<string, double> kvp in checkedListBox2.CheckedItems) {
                nlmass.Add(kvp.Value);
            }

            List<MzRange> rangesToRemove = listBox1.Items.Cast<MzRange>().ToList();

            DtaGenerator dta_generator = new DtaGenerator(raw_filepaths,
                minimum_assumed_precursor_charge_state, maximum_assumed_precursor_charge_state,
                clean_precursor, enable_etd_preprocessing,
                clean_tmt_duplex, clean_itraq_4plex, clean_tmt_6plex, clean_itraq_8plex,
                group_by_activation_energy_time,
                sequest_dta_output, omssa_text_output, mascot_mgf_output,
                output_folder,
                nlmass,
                rangesToRemove,
                includeLog,
                clnPrecursorLowMZ, clnPrecursorHighMZ,
                etdLowDa, etdHighDa);
                      
            dta_generator.StartingFile += handleStartingFile;
            dta_generator.UpdateProgress += handleUpdateProgress;
            dta_generator.ThrowException += handleThrowException;
            dta_generator.FinishedFile += handleFinishedFile;
            dta_generator.Finished += handleFinished;

            lstRawFiles.SelectedItem = null;
            prgProgress.Value = prgProgress.Minimum;

            btnOK.Enabled = false;
            Thread generate_dtas = new Thread(dta_generator.GenerateDtas);         
            generate_dtas.IsBackground = true;
            generate_dtas.Start();
        }
           
        private void handleStartingFile(object sender, FilepathEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, FilepathEventArgs>(handleStartingFile), sender, e);
                return;
            }
            lstRawFiles.SelectedItems.Add(e.Filepath);        
        }

        private void handleUpdateProgress(object sender, ProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ProgressEventArgs>(handleUpdateProgress), sender, e);
                return;
            }
            prgProgress.Value = (int)(prgProgress.Maximum * e.Progress);         
        }

        private void handleThrowException(object sender, ExceptionEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ExceptionEventArgs>(handleThrowException), sender, e);
                return;
            }
            MessageBox.Show(e.Exception.ToString());           
        }

        private void handleFinishedFile(object sender, FilepathEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, FilepathEventArgs>(handleFinishedFile), sender, e);
                return;
            }
            lstRawFiles.SelectedItems.Remove(e.Filepath);         
        }

        private void handleFinished(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, EventArgs>(handleFinished), sender, e);
                return;
            }
            lstRawFiles.SelectedItems.Clear();
            prgProgress.Value = 0;
            btnOK.Enabled = true;          
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
          
        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstRawFiles.Items.Clear();
        }

        private void clearSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while (lstRawFiles.SelectedItems.Count > 0)
            {
                lstRawFiles.Items.RemoveAt(lstRawFiles.SelectedIndex);
            }
        }

        public static Version GetRunningVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }  

        protected override void OnLoad(EventArgs e)
        {
            Text = string.Format("Dta Generator {0}-bit (v{1}) running {2} cores", IntPtr.Size * 8, Assembly.GetExecutingAssembly().GetName().Version, Environment.ProcessorCount);
            base.OnLoad(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new DtaViewerForm().Show();
        }

        private void chkCleanPrecursor_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = numericUpDown2.Enabled = chkCleanPrecursor.Checked;
        }

        private void chkEnableEtdPreProcessing_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown3.Enabled = numericUpDown4.Enabled = chkEnableEtdPreProcessing.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double lowMZ = (double)numericUpDown5.Value;
            double highMZ = (double)numericUpDown6.Value;
            MzRange range = new MzRange(lowMZ, highMZ);
            listBox1.Items.Add(range);
        }

        private readonly MzRange tmtDuplex = new MzRange(125.5, 127.5);

        private void chkCleanTmtDuplex_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCleanTmtDuplex.Checked)
            {
                listBox1.Items.Add(tmtDuplex);
            }
            else
            {
                listBox1.Items.Remove(tmtDuplex);
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete)
                return;

            object range = listBox1.SelectedItem;
            if (range == null)
                return;
           
            listBox1.Items.Remove(range);
        }
    }
}