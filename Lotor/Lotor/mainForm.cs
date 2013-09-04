using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.IO;
using Coon;
using CoonThermo.IO;
using LumenWorks.Framework.IO.Csv;
using Coon.Quant;

namespace Lotor
{
    public partial class mainForm : Form
    {
        public static Version GetRunningVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }  

        public Lotor Lotor = null;
        public Thread MainThread = null;

        public BindingList<PTM> activePTMs;
        public HashSet<PTM> activePTMsHashSet;

        public mainForm()
        {
            InitializeComponent();
            AdditionalSetup();           
        }

        private void AdditionalSetup()
        {
            Text = string.Format("Lotor ({0})", GetRunningVersion());

            comboBox1.DataSource = Enum.GetValues(typeof(Coon.ToleranceType));
            comboBox1.SelectedItem = Coon.ToleranceType.PPM;

            foreach (Modification mod in ModificationDictionary.Instance.Values)
            {
                comboBox2.Items.Add(mod);
            }
            activePTMsHashSet = new HashSet<PTM>();
            activePTMs = new BindingList<PTM>();
            activePTMs.RaiseListChangedEvents = true;
            activePTMs.ListChanged += new ListChangedEventHandler(activePTMs_ListChanged);
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = activePTMs;

            DataGridViewTextBoxColumn nameCol = new DataGridViewTextBoxColumn();
            nameCol.HeaderText = "Name";
            nameCol.DataPropertyName = "Name";
            nameCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns.Add(nameCol);

            DataGridViewTextBoxColumn massCol = new DataGridViewTextBoxColumn();
            massCol.HeaderText = "Mass";
            massCol.DataPropertyName = "Mass";
            massCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns.Add(massCol);

            DataGridViewCheckBoxColumn isfixedCol = new DataGridViewCheckBoxColumn();
            isfixedCol.HeaderText = "Fixed Mod";
            isfixedCol.DataPropertyName = "IsFixed";
            isfixedCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns.Add(isfixedCol);

            DataGridViewCheckBoxColumn isQuantCol = new DataGridViewCheckBoxColumn();
            isQuantCol.HeaderText = "Use for Quantification";
            isQuantCol.DataPropertyName = "Quantify";
            isQuantCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns.Add(isQuantCol);

            DataGridViewTextBoxColumn modCol = new DataGridViewTextBoxColumn();
            modCol.HeaderText = "Modified Sites";
            modCol.DataPropertyName = "ModifiableSites";
            modCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns.Add(modCol);

            DataGridViewCheckBoxColumn isprotnCol = new DataGridViewCheckBoxColumn();
            isprotnCol.HeaderText = "Protein N-Term";
            isprotnCol.DataPropertyName = "IsProteinNTerm";
            isprotnCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns.Add(isprotnCol);

            DataGridViewCheckBoxColumn isprotcCol = new DataGridViewCheckBoxColumn();
            isprotcCol.HeaderText = "Protein C-Term";
            isprotcCol.DataPropertyName = "IsProteinCTerm";
            isprotcCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns.Add(isprotcCol);

            dataGridView1.CellClick += new DataGridViewCellEventHandler(DGV_CellClick);

            AddMod(new PTM(ModificationDictionary.Instance["carbamidomethyl C"]));
          
            // TEMP

            //textBox2.Text = @"C:\Users\Derek\Desktop\Lotor";
            //LoadPeptides(@"C:\Users\Derek\Desktop\Lotor\B6_parsimony_peptides_Acetyl_wQuant.csv");
            //LoadUserMods(@"C:\Users\Derek\Desktop\Lotor\usermods_TMT_6-plex_AC.xml");
            //PTM ptm = new PTM(ModificationDictionary.Instance["TMT 6-plex K"], true);
            //ptm.ToggleModificationSite(ModificationSite.Npep, true);
            //AddMod(ptm);

            //LoadRawFiles(Directory.EnumerateFiles(@"C:\Users\Derek\Desktop\Lotor\rawFiles_07feb2012", "*.raw"));

            //listBox1.Items.AddRange(Directory.EnumerateFiles(@"C:\Users\Derek\Desktop\Lotor\rawfiles_02feb2012", "*.raw").ToArray());
            // TEMP
        }

        void activePTMs_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                activePTMsHashSet.Clear();
                foreach (PTM ptm in activePTMs)
                {
                    activePTMsHashSet.Add(ptm);
                }
            }
        }

        public void Run()
        {
            logTB.Clear();
            logTB.BackColor = Color.White;
            Dictionary<string, ThermoRawFile> rawFiles = GetRawFiles();
            string inputcsvfile = textBox1.Text;
            if (!System.IO.File.Exists(inputcsvfile))
            {
                UpdateLog("Cannot open input csvfile: " + inputcsvfile + ", aborting!");
                return;
            }
            string outputDirectory = textBox2.Text;
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                UpdateLog("Must provide a valid output folder!");
                return;
            }
            if (!System.IO.Directory.Exists(outputDirectory))
            {
                UpdateLog("Output directory " + outputDirectory + " doesn't exist, creating it...");
                System.IO.Directory.CreateDirectory(outputDirectory);
            }
            Dictionary<string, PTM> ptms = new Dictionary<string, PTM>();
            foreach (PTM ptm in activePTMs)
            {
                ptms.Add(ptm.Name, ptm);
            }
            Tolerance prodTolerance = GetProductTolerance();
            Lotor = new Lotor(rawFiles, inputcsvfile, outputDirectory, ptms, prodTolerance, 13, FragmentType.b | FragmentType.y);
            Lotor.UpdateLog += new EventHandler<StatusEventArgs>(lotor_UpdateLog);
            Lotor.UpdateProgress += new EventHandler<ProgressEventArgs>(lotor_UpdateProgress);
            MainThread = new Thread(Lotor.Localize);
            MainThread.IsBackground = true;
            MainThread.Start();
            localizeB.Enabled = false;
        }

        public Tolerance GetProductTolerance()
        {
            double value = (double)numericUpDown1.Value;
            ToleranceType type = (ToleranceType)comboBox1.SelectedItem;
            return new Tolerance(value, type);
        }

        private Dictionary<string, ThermoRawFile> GetRawFiles()
        {
            Dictionary<string, ThermoRawFile> dict = new Dictionary<string, ThermoRawFile>();
            foreach (string rawFileName in listBox1.Items)
            {
                ThermoRawFile rawFile = new ThermoRawFile(rawFileName, true);
                dict.Add(rawFile.Name, rawFile);
            }
            return dict;
        }

        public void LoadUserMods(string userModFile)
        {
            ModificationDictionary.Instance.LoadOmssaModifications(userModFile);
            comboBox2.Items.Clear();
            foreach (Modification mod in ModificationDictionary.Instance.Values)
            {
                comboBox2.Items.Add(mod);
            }
            ReadMods(textBox1.Text);
        }

        public void LoadRawFiles(IEnumerable<string> rawFiles)
        {
            foreach (string rawfile in rawFiles)
            {
                listBox1.Items.Add(rawfile);               
            }
        }

        public void LoadPeptides(string filename)
        {
            textBox1.Text = filename;
            ReadMods(filename);
        }

        public void ReadMods(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return;
            HashSet<PTM> mods = new HashSet<PTM>();
            using (CsvReader reader = new CsvReader(new StreamReader(filename), true))
            {
                while (reader.ReadNextRecord())
                {
                    try
                    {
                        string modstring = reader["Mods"];
                        foreach (Modification mod in Modification.ParseModificationLine(modstring))
                        {
                            mods.Add(new PTM(mod, false));                           
                        }
                    }
                    catch (Exception e)
                    {
                        UpdateLog( e.Message);
                        return;
                    }

                }
            }
            foreach (PTM ptm in mods)
            {
                AddMod(ptm);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Run();
        }

        public void AddMod(PTM mod) 
        {
            if (activePTMsHashSet.Contains(mod))
            {
                return;
            }
            else
            {
                activePTMs.Add(mod);
                activePTMsHashSet.Add(mod);
            }
        }


        #region CallBacks       

        private void lotor_UpdateProgress(object sender, ProgressEventArgs e)
        {
            UpdateProgress(e.Percent);
        }

        private void lotor_UpdateLog(object sender, StatusEventArgs e)
        {
            UpdateLog(e.Message, e.IsError);
        }

        private delegate void UpdateLogDelegate(string msg, bool isError);
        public void UpdateLog(string msg, bool isError = false)
        {
            if (InvokeRequired)
            {
                if (Lotor != null)
                {
                    Invoke(new UpdateLogDelegate(UpdateLog), new object[] { msg, isError });
                }
            }
            else
            {
                logTB.AppendText(string.Format("[{0}]\t{1}\n", DateTime.Now.ToLongTimeString(), msg));
                if (isError)
                {
                    logTB.BackColor = Color.MediumVioletRed;
                }
            }
        }

        private delegate void UpdateProgressDelegate(double percent);
        public void UpdateProgress(double percent)
        {
            if (InvokeRequired)
            {
                if (Lotor != null)
                {
                    Invoke(new UpdateProgressDelegate(UpdateProgress), new object[] { percent });
                }
            }
            else
            {
                if (percent < 0)
                {   
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 0;                 
                    Lotor = null;
                    localizeB.Enabled = true;
                }
                else if (percent == 0.0)
                {
                    progressBar1.Style = ProgressBarStyle.Marquee;
                    progressBar1.Value = 0;
                }
                else
                {
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    progressBar1.Value = (int)(100*percent);
                }
            }
        }

        #endregion

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = ((DataGridView)sender);
            if (e.ColumnIndex < 0)
            {
                dgv.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                dgv.Focus();
                dgv.EndEdit();
            }
            else
            {
                dgv.EditMode = DataGridViewEditMode.EditOnEnter;
                if (dgv.CurrentCell != null)
                {
                    dgv.BeginEdit(false);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Modification mod = (Modification)comboBox2.SelectedItem;
            AddMod(new PTM(mod, true));
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (usermodD.ShowDialog() == DialogResult.OK)
            {
                LoadUserMods(usermodD.FileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (inputfD.ShowDialog() == DialogResult.OK)
            {
                LoadPeptides(inputfD.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (rawD.ShowDialog() == DialogResult.OK)
            {
                LoadRawFiles(rawD.FileNames);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (outputD.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = outputD.SelectedPath;
            }
        }
     
    }
}
