using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CSMSL.IO.OMSSA;
using CSMSL.Proteomics;

namespace Coon.Compass.FdrOptimizer
{
    public partial class FdrOptimizerForm : Form
    {
        public FdrOptimizerForm()
        {
            InitializeComponent();
        }
    
        private void UpdateModsListboxes()
        {
            lstAllModifications.Items.Clear();
            lstSelectedFixedModifications.Items.Clear();

            lstAllModifications.DisplayMember = "Text";
            lstSelectedFixedModifications.DisplayMember = "Text";
            List<OmssaModification> allMods = OmssaModification.GetAllModifications().ToList();
            foreach (OmssaModification modification in allMods.OrderByDescending(mod => mod.Name.Contains("*")).ThenBy(mod => mod.Name))
            {
                string text = (modification.UserMod) ? "*" :"";
                text += modification.ToString();
                ListViewItem list_view_item = new ListViewItem(text) { Tag = modification };
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

            double maxFalseDiscoveryRate = (double)numMaximumFalseDiscoveryRate.Value;
            double maxPPM = (double)maximumPPMUD.Value;
            bool isBatched = checkBox1.Checked;
            bool is2DFDR = twoDCB.Checked;
            bool includeFixedMods = checkBox2.Checked;
            UniquePeptideType uniquePeptideType = (UniquePeptideType)comboBox1.SelectedValue;
            string outputFolder = txtOutputFolder.Text;
           
            if (string.IsNullOrEmpty(outputFolder))
            {
                MessageBox.Show("Output folder must be specified");
                return;
            }
            
            Directory.CreateDirectory(outputFolder);

            FdrOptimizer fdrOptimizer = new FdrOptimizer(csvFilepaths, rawFolder,
                fixedModifications,
                maxFalseDiscoveryRate,maxPPM, uniquePeptideType,
                  outputFolder, isBatched, is2DFDR, includeFixedMods);
          
            fdrOptimizer.UpdateProgress += HandleUpdateProgress;
            fdrOptimizer.Finished += fdrOptimizer_Finished;
            fdrOptimizer.UpdateLog += fdrOptimizer_UpdateLog;

            prgProgress.Value = prgProgress.Minimum;

            btnOK.Enabled = false;
            Thread thread = new Thread(fdrOptimizer.Optimize) {IsBackground = true};
            thread.Start();

        }

        void fdrOptimizer_Finished(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, EventArgs>(fdrOptimizer_Finished), sender, e);
                return;
            }

            btnOK.Enabled = true;
        }

        void fdrOptimizer_UpdateLog(object sender, StatusEventArgs e)
        {
            UpdateLog(e.Message);
        }
   
        public void UpdateLog(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateLog), msg);
                return;
            }
            
            richTextBox1.AppendText(string.Format("[{0}]\t{1}\n", DateTime.Now.ToLongTimeString(), msg));
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    
        private void ChangeProgressBarValue(int progressValue)
        {
            if(InvokeRequired)
            {
                prgProgress.Invoke(new Action<int>(ChangeProgressBarValue), progressValue);
                return;
            }
            
            prgProgress.Value = progressValue;
        }

        private void HandleUpdateProgress(object sender, ProgressEventArgs e)
        {
            ChangeProgressBarValue(e.Progress);
        }
        
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
        
        private void btnOK_Click_1(object sender, EventArgs e)
        {
            Run();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ofdModsXml.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OmssaModification.LoadOmssaModifications(ofdModsXml.FileName, true);
                UpdateModsListboxes();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Text = string.Format("FDR Optimizer {0}-bit (v{1})", IntPtr.Size*8, Assembly.GetExecutingAssembly().GetName().Version);
           
            UpdateModsListboxes();
            comboBox1.DataSource = Enum.GetValues(typeof(UniquePeptideType));
            comboBox1.SelectedItem = UniquePeptideType.SequenceAndModifications;
            
            new ToolTip().SetToolTip(checkBox1, "Process all OMSSA .csv files with the same e-value and ppm thresholds");

            base.OnLoad(e);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("changelog.txt");
        }

        private void twoDCB_CheckedChanged(object sender, EventArgs e)
        {
            maximumPPMUD.Enabled = twoDCB.Checked;
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("ReducePsms.txt");
        }
   
    }
}