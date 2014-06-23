using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSMSL.Proteomics;
using LumenWorks.Framework.IO.Csv;

namespace Coon.Compass.ProteinHoarder
{
    public partial class ProteinHoarderForm : Form
    {
        private Version GetRunningVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        public static string LOG_FILE = "Protein_Hoarder_Log.txt";

        public ProteinHoarder Hoarder = null;
        public Thread MainThread = null;

        public BindingList<CsvFile> CsvFiles;

        public static bool FORTESTING = false;

        public static string ExperimentsTypes = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public BindingList<char> ExperimentIDs;

        private HashSet<string> UsedModifications;

        public ProteinHoarderForm()
        {
            InitializeComponent();
            AdditionalSetup();
        }

        private void AdditionalSetup()
        {
            // Include the file version in the title of the program for easy identification
            Text = string.Format("Protein Hoarder ({0})", GetRunningVersion());

            comboBox1.DataSource = Enum.GetValues(typeof (AnnotationType));

            UsedModifications = new HashSet<string>();
          
   
            progressBar.MarqueeAnimationSpeed = 50;

            ExperimentIDs = new BindingList<char>();
            for (int i = 0; i < 26; i++)
            {
                ExperimentIDs.Add(ExperimentsTypes[i]);
            }

            proteinscoringCB.DataSource = Enum.GetValues(typeof(PScoreCalculateionMethod));      

            csvDGV.AutoGenerateColumns = false;
            csvDGV.EditMode = DataGridViewEditMode.EditOnEnter;
            csvDGV.CellClick += new DataGridViewCellEventHandler(DGV_CellClick);
            CsvFiles = new BindingList<CsvFile>();
            csvDGV.DataSource = CsvFiles;

            DataGridViewTextBoxColumn nameCol = new DataGridViewTextBoxColumn();
            nameCol.ReadOnly = true;
            nameCol.HeaderText = "File Name";
            nameCol.DataPropertyName = "Name";
            nameCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            csvDGV.Columns.Add(nameCol);

            DataGridViewTextBoxColumn expCol = new DataGridViewTextBoxColumn();
            expCol.HeaderText = "Experiment Name";
            expCol.DataPropertyName = "ExperimentName";
            expCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            csvDGV.Columns.Add(expCol);

            //DataGridViewComboBoxColumn expCol = new DataGridViewComboBoxColumn();
            //expCol.HeaderText = "Experiment ID";
            //expCol.DataSource = ExperimentIDs;
            //expCol.DataPropertyName = "ExperimentGroup";
            //expCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //expCol.AutoComplete = true;
            //csvDGV.Columns.Add(expCol);

            List<Protease> proteases = CSMSL.Proteomics.Protease.GetAllProteases().ToList();

            DataGridViewComboBoxColumn proteaseCol = new DataGridViewComboBoxColumn();
            proteaseCol.HeaderText = "Protease";
            proteaseCol.DataSource = proteases.Select(p => p.Name).ToList();        
            proteaseCol.DataPropertyName = "Protease";
            proteaseCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            proteaseCol.AutoComplete = true;
            csvDGV.Columns.Add(proteaseCol);

          
        }

        public void Run()
        {
            string fastaFile = databaseTB.Text;
            if (!System.IO.File.Exists(fastaFile))
            {
                UpdateLog("Cannot find protein database " + fastaFile + ", aborting!");
                return;
            }

            string outputDirectory = outputTB.Text;
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

            int minPeptidesperGroup = (int)minpepspergroupUD.Value;
            int maxMissedCleavage = (int)maxMissedCleavagedUD.Value;
            double maxFDR = (double)fdrUD.Value;
            bool useConservative = conservativeCB.Checked;
            bool useQuant = enableQuantCB.Checked;
            //bool filterinterference = quantigorneinterferenceCB.Checked;
            bool includeUnfiltereedResults = includeUnfliterCB.Checked;
            bool ignorePeptidesWithMissingData = ignorePepMissingCB.Checked;
            bool semiDigestion = semiCB.Checked;
            bool proteinPerMin = proteinsPerMinCB.Checked;
            bool duplexQuant = duplexCB.Enabled && duplexCB.Checked;
            bool useMedian = medianvalueCB.Enabled && medianvalueCB.Checked;
            bool useNoiseBandCap = useNBCCB.Enabled && useNBCCB.Checked;
            bool sequenceCoverageMap = sequenceMapCB.Enabled && sequenceMapCB.Checked;
            AnnotationType annotationType = (AnnotationType)comboBox1.SelectedItem;
            //double interferencecutoff = (double)quantintferenceUD.Value;
            
            HashSet<Modification> modstoignore = new HashSet<Modification>();
            if (useQuant)
            {
                foreach (string modName in ignoreModsCLB.CheckedItems.Cast<string>())
                {
                    Modification mod = new Modification(modName, true);
                   // mod.IgnoreMod = true;
                    modstoignore.Add(mod);                    
                }
            }           
            logTB.Clear();
            Hoarder = new ProteinHoarder(CsvFiles, fastaFile, outputDirectory, minPeptidesperGroup, maxMissedCleavage, maxFDR, annotationType, useConservative, useQuant, useMedian, duplexQuant, useNoiseBandCap, modstoignore, false, 0.0, includeUnfiltereedResults, ignorePeptidesWithMissingData, semiDigestion, proteinPerMin, sequenceCoverageMap);
            Hoarder.UpdateLog += new EventHandler<StatusEventArgs>(hoarder_UpdateLog);
            Hoarder.UpdateProgress += new EventHandler<ProgressEventArgs>(hoarder_UpdateProgress);
            MainThread = new Thread(Hoarder.Herd);
            MainThread.IsBackground = true;
            MainThread.Start();
            hoardB.Enabled = false;
            outputGB.Enabled = false;
            inputGB.Enabled = false;
            optionsGB.Enabled = false;
            quantGB.Enabled = false;
        }

        private void AddFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                switch (Path.GetExtension(file))
                {
                    case ".csv":
                        AddCsv(file);
                        break;
                    case ".fasta":
                    case ".fa":
                        SetDatabase(file);
                        break;
                    default:
                        break;
                }
            }
        }

        public async void AddCsv(string filename)
        {
            CsvFile file = new CsvFile(filename);
            if (!CsvFiles.Contains(file))
            {
                CsvFiles.Add(file);
                csvD.InitialDirectory = Path.GetDirectoryName(filename);
                databaseD.InitialDirectory = csvD.InitialDirectory;
                if (string.IsNullOrWhiteSpace(outputTB.Text))
                {
                    outputTB.Text = csvD.InitialDirectory;
                }
                var mods = await CheckMods(file);

                UsedModifications.UnionWith(mods);

                ignoreModsCLB.Items.Clear();
                ignoreModsCLB.Items.AddRange(UsedModifications.ToArray());
            }
        }

        private Task<ISet<string>> CheckMods(CsvFile file)
        {
            Task<ISet<string>> t = new Task<ISet<string>>(() =>
            {
                HashSet<string> localMods = new HashSet<string>();
                using (CsvReader reader = new CsvReader(new StreamReader(file.FilePath), true))
                {
                    while (reader.ReadNextRecord())
                    {
                        string modLine = reader["Mods"];
                        if (string.IsNullOrEmpty(modLine)) continue;
                        string[] mods = modLine.Split(',');
                        foreach (string modName in mods.Select(mod => mod.Split(':')[0]))
                        {
                            localMods.Add(modName);
                        }
                    }
                }
                return localMods;
            });
            t.Start();
            return t;
        }

        public void SetDatabase(string filename)
        {
            databaseTB.Text = filename;

            using (StreamReader reader = new StreamReader(filename))
            {
                string line = reader.ReadLine();
                if (Protein.UniProtRegex.IsMatch(line))
                {
                    comboBox1.SelectedItem = AnnotationType.UniProt;
                } else if (Protein.SGDRegex.IsMatch(line))
                {
                    comboBox1.SelectedItem = AnnotationType.SGD;
                }              
                else
                {
                    comboBox1.SelectedItem = AnnotationType.None;
                }
                comboBox1.Enabled = true;
            }

            if (string.IsNullOrWhiteSpace(outputTB.Text))
            {
                outputTB.Text = Path.GetDirectoryName(filename);
            }
        }

        private void hoarder_UpdateProgress(object sender, ProgressEventArgs e)
        {
            UpdateProgress(e.Percent);
        }

        private void hoarder_UpdateLog(object sender, StatusEventArgs e)
        {
            UpdateLog(e.Message);
        }

        private delegate void UpdateLogDelegate(string msg);

        public void UpdateLog(string msg)
        {
            if (InvokeRequired)
            {
                if (Hoarder != null)
                {
                    Invoke(new UpdateLogDelegate(UpdateLog), new object[] { msg });
                }
            }
            else
            {
                logTB.AppendText(string.Format("[{0}]\t{1}\n", DateTime.Now.ToLongTimeString(), msg));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void outputdirectoryB_Click(object sender, EventArgs e)
        {
            if (outputD.ShowDialog() == DialogResult.OK)
            {
                outputTB.Text = outputD.SelectedPath;
            }
        }

        private void databaseB_Click(object sender, EventArgs e)
        {
            if (databaseD.ShowDialog() == DialogResult.OK)
            {
                SetDatabase(databaseD.FileName);
            }
        }

        private delegate void UpdateProgressDelegate(double percent);

        public void UpdateProgress(double percent)
        {
            if (InvokeRequired)
            {
                if (Hoarder != null)
                {
                    Invoke(new UpdateProgressDelegate(UpdateProgress), new object[] { percent });
                }
            }
            else
            {
                // If the precent is less than zero, the program is finished
                if (percent < 0)
                {
                    hoardB.Enabled = true;
                    outputGB.Enabled = true;
                    inputGB.Enabled = true;
                    optionsGB.Enabled = true;
                    quantGB.Enabled = true;
                    progressBar.Style = ProgressBarStyle.Continuous;
                    progressBar.Value = 0;
                    WriteLog();
                    Hoarder.UpdateLog -= hoarder_UpdateLog;
                    Hoarder.UpdateProgress -= hoarder_UpdateProgress;
                    Hoarder = null;
                }
                else if (percent == 0.0)
                {
                    progressBar.Style = ProgressBarStyle.Marquee;
                    progressBar.Value = 0;
                }
                else
                {
                    progressBar.Style = ProgressBarStyle.Blocks;
                    progressBar.Value = (int)(10000 * percent);
                }
            }
        }

        private void WriteLog()
        {            
            string filename = Path.Combine(Hoarder.OutputDirectory, string.Format("Protein Hoarder Log {0:yyyyMMddhhmmss}.txt", DateTime.Now));
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine("===Protein Hoarder v{0}===", GetRunningVersion()); 
                foreach (string line in logTB.Lines)
                {
                    writer.WriteLine(line);
                }              
                writer.WriteLine("===Analysis Performed on: {0}===", DateTime.Now.ToLongDateString());
            }
            UpdateLog("Writing log file to " + filename);
        }

        private void UpdateProgressEvent(object sender, ProgressEventArgs e)
        {
            UpdateProgress(e.Percent);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MainThread != null && MainThread.IsAlive)
            {
                MainThread.Abort();
            }
            if (Hoarder != null)
            {
                //Hoarder.UpdateLog -= new EventHandler<StatusEventArgs>(hoarder_UpdateLog);
                //Hoarder.UpdateProgress -= new EventHandler<ProgressEventArgs>(hoarder_UpdateProgress);
                Hoarder = null;
            }
            Properties.Settings.Default.Save();
            e.Cancel = false;
        }

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

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                AddFiles(files);
            }
        }

        private void csvclearB_Click(object sender, EventArgs e)
        {
            CsvFiles.Clear();
        }

        private void csvsB_Click(object sender, EventArgs e)
        {
            if (csvD.ShowDialog() == DialogResult.OK)
            {
                AddFiles(csvD.FileNames);
            }
        }

        private void enableQuantCB_CheckedChanged(object sender, EventArgs e)
        {
            ignoreModsCLB.Enabled = enableQuantCB.Checked;
            duplexCB.Enabled = enableQuantCB.Checked;
           // quantigorneinterferenceCB.Enabled = enableQuantCB.Checked;
            //quantintferenceUD.Enabled = enableQuantCB.Checked;
            ignorePepMissingCB.Enabled = enableQuantCB.Checked;

            useNBCCB.Enabled = medianvalueCB.Enabled = enableQuantCB.Checked && duplexCB.Checked;
        }

        private void duplexCB_CheckedChanged(object sender, EventArgs e)
        {
            useNBCCB.Enabled = medianvalueCB.Enabled = duplexCB.Checked;
        }
    }
}