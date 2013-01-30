using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OmssaLib;

namespace OmssaNavigator
{
    public partial class MainForm : Form
    {
        #region Variables
        public enum SetType { ArgLine, Gui }

        [Flags]
        public enum InValidType
        {
            None = 0x0,
            Argument = 0x01,
            Database = 0x02,
            DtaFiles = 0x04
        }

        public List<DtaFile> DTAFiles;
        private int DtaIndex = 0;
        private DtaFile CurrentDTA;

        public string OutputFilePath = string.Empty;
        public string SuggestedOutputFilePath = string.Empty;

        public DataBaseFile DataBase;
        public UserModFile UserMod;

        private bool ValidArgumentLine = true;

        private InValidType ValidState;

        public Omssa Omssa;

        public Dictionary<string, int> EnzymeDictionary;
        public Dictionary<string, int> IonDictionary;
        public Dictionary<string, int> ModDictionary;
        public Dictionary<string, int> TaxDictionary;

        public ArgumentLine ArgumentLine = new ArgumentLine();

        public bool UserAction = false;

        #endregion

        #region Omssacl.exe

        public void ReadUserMods()
        {
            if(UserMod == null)
                return;
            Dictionary<string, int> mods = Omssa.ReadModFile(UserMod.FilePath);
            foreach(KeyValuePair<string, int> kvp in mods)
            {
                if(!ModDictionary.ContainsKey(kvp.Key))
                {
                    ModDictionary.Add(kvp.Key, kvp.Value);
                }
            }
            UpdateModLists();
        }

        private void ReadMods()
        {
            ModDictionary = Omssa.ReadModFile(Path.Combine(Application.StartupPath, "mods.xml"));
            UpdateModLists();
        }

        private void UpdateModLists()
        {
            fixmodLB.Items.Clear();
            varmodLB.Items.Clear();
            foreach(KeyValuePair<string, int> kvp in ModDictionary)
            {
                fixmodLB.Items.Add(kvp.Key);
                varmodLB.Items.Add(kvp.Key);
            }
        }

        private void ReadInEnzymes()
        {
            EnzymeDictionary = Omssa.ReadInDictionary("-el");
            foreach(KeyValuePair<string, int> kvp in EnzymeDictionary)
            {
                enzymeCB.Items.Add(kvp.Key);
            }
            enzymeCB.SelectedItem = "Trypsin";
        }

        private void ReadInIonSeries()
        {
            IonDictionary = Omssa.ReadInDictionary("-il");
            int index = 0;
            foreach(KeyValuePair<string, int> kvp in IonDictionary)
            {
                ionseriesLB.Items.Add(kvp.Key);
                if(kvp.Key == "b" || kvp.Key == "y")
                    ionseriesLB.SetItemChecked(index, true);
                index++;
            }
        }

        private string ReadVersion()
        {
            return Omssa.RunCommand("-version").Trim();
        }

        private void BeginSearch()
        {
            pictureBox2.Visible = true;
            pictureBox1.Visible = false;
            statusLabel.Text = "Searching...";
            DisableAll(false);
            DtaIndex = 0;
            StartNextSearch();
        }

        private delegate void StartNextSearchCallBack();
        private void StartNextSearch()
        {
            if(InvokeRequired)
            {
                Invoke(new StartNextSearchCallBack(StartNextSearch));
            }
            else
            {
                if(DtaIndex == 0)
                {
                    pictureBox1.Image = imageList1.Images["rotating"];
                    pictureBox1.Show();
                    ArgumentLine.SetDataBase("\"\\\"" + DataBase.FullPathWithoutExtension + "\\\"\"");
                    if(usermodsCB.Checked && UserMod != null)
                    {
                        ArgumentLine.SetUserModFile("\"" + UserMod.FilePath + "\"");
                    }
                }
                else
                {
                    foreach(ResultFile result in CurrentDTA.ResultFiles)
                    {
                        UpdateStatus(result.NameWithExt + " was created");
                    }
                }
                if(DtaIndex >= DTAFiles.Count)
                {
                    UpdateStatus("Searching Completed");
                    pictureBox1.Image = imageList1.Images["success"];
                    pictureBox1.Visible = true;
                    statusLabel.Text = "Searching Completed";
                    dtafileLB.ClearSelected();
                    DisableAll(true);
                    pictureBox2.Visible = false;
                    return;
                }
                CurrentDTA = DTAFiles[DtaIndex];
                dtafileLB.ClearSelected();
                dtafileLB.SelectedIndex = DtaIndex;
                UpdateStatus("Searching " + CurrentDTA.Name + "...");
                ArgumentLine.SetInputFile(CurrentDTA.FilePath);

                // Output Files
                if(ocCB.Checked)
                {
                    string outputfile = ArgumentLine.SetOutputFile(Path.Combine(OutputFilePath, CurrentDTA.Name), OutputFileType.Csv);
                    CurrentDTA.AddResult(new ResultFile(outputfile, OutputFilePath));
                }
                if(oxCB.Checked)
                {
                    string outputfile = ArgumentLine.SetOutputFile(Path.Combine(OutputFilePath, CurrentDTA.Name), OutputFileType.Xml);
                    CurrentDTA.AddResult(new ResultFile(outputfile, OutputFilePath));
                }
                if(opCB.Checked)
                {
                    string outputfile = ArgumentLine.SetOutputFile(Path.Combine(OutputFilePath, CurrentDTA.Name), OutputFileType.PepXml);
                    CurrentDTA.AddResult(new ResultFile(outputfile, OutputFilePath));
                }
                CurrentDTA.Arguments = ArgumentLine;
                UpdateStatus("Searching with the Following Parameters:");
                UpdateStatus(ArgumentLine.ToString());
                Omssa.StartSearch(ArgumentLine);
                DtaIndex++;
            }
        }

        private void DisableAll(bool value)
        {
            groupBox1.Enabled = value;
            groupBox2.Enabled = value;
            groupBox3.Enabled = value;
            groupBox4.Enabled = value;
            groupBox5.Enabled = value;
            startsearchB.Enabled = value;
        }

        #endregion

        #region Constructor

        public MainForm()
        {
            ValidState = InValidType.Database | InValidType.DtaFiles;
            InitializeComponent();
            Omssa = new Omssa();
            Omssa.Exited += new EventHandler(Omssa_Exited);
            Omssa.UpdateStatus += new DataReceivedEventHandler(Omssa_UpdateStatus);

            // Update Form Header
            Text = "OMSSA Navigator (" + ReadVersion() + ")";

            ReadInEnzymes();
            ReadInIonSeries();
            ReadMods();
            LoadTaxIds();

            SetDefaults();
            ReadInArgumentLine();
            UpdateEnables();
        }

        #endregion

        #region GUI

        private bool CheckStart()
        {
            bool toReturn = false;
            if(ValidState == InValidType.None)
            {
                toReturn = true;
                statusLabel.Text = "Ready to Search";
                pictureBox1.Image = imageList1.Images["success"];
            }
            else
            {
                statusLabel.Text = String.Format("Invalid {0}", ValidState);
                pictureBox1.Image = imageList1.Images["fail"];
            }
            startsearchB.Enabled = toReturn;
            return toReturn;
        }

        private void SetDefaults()
        {
            UserAction = false;
            argTB.Text = string.Empty;
            ntUD.Maximum = Environment.ProcessorCount;
            ntUD.Value = ntUD.Maximum;
            ArgumentLine.RemoveDefaults = false;

            UserAction = true;
            ReadInArgumentLine();
        }

        void Omssa_UpdateStatus(object sender, DataReceivedEventArgs e)
        {
            if(String.IsNullOrEmpty(e.Data))
            {
                Omssa.Quit();
                StartNextSearch();
            }
            else
            {
                UpdateStatus(e.Data);
            }
        }

        void Omssa_Exited(object sender, EventArgs e)
        {
            UpdateStatus("Done");
        }

        public void UpdateEnables()
        {
            usermodButton.Enabled = usermodsCB.Checked;
            usermodTB.Enabled = usermodsCB.Checked;
            usermodTB.Text = (usermodsCB.Checked && UserMod != null) ? UserMod.Name : "";
            tiUD.Enabled = ArgumentLine.PrecursorIonSearch == PrecursorIonSearchType.MultiIsotope;
            zhUD.Enabled = zlUD.Enabled = ArgumentLine.PrecursorChargeType == PrecursorChargeType.UseRange;
            noEnzymeBox.Enabled = ArgumentLine.Enzyme == EnzymeType.NoEnzyme;

            scorpUD.Enabled = ArgumentLine.CorrelationCorrection;
            outputpathTB.Text = OutputFilePath;
            CheckStart();
        }

        public delegate void UpdateStatusDelegate(string msg);
        public void UpdateStatus(string msg)
        {
            if(InvokeRequired)
            {
                Invoke(new UpdateStatusDelegate(UpdateStatus), new object[] { msg });
            }
            else
            {
                statusTB.AppendText("[" + DateTime.Now.ToLongTimeString() + "]\t" + msg + '\n');
            }
        }

        #endregion

        #region ArgumentLine

        private void IsValid(bool value, string msg)
        {
            IsValid(value);
            UpdateStatus(msg);
        }

        private void IsValid(bool value)
        {
            ValidArgumentLine = value;
            if(value)
            {
                ValidState &= ~InValidType.Argument;
            }
            else
            {
                ValidState |= InValidType.Argument;
            }

            if((ValidState & InValidType.Argument) != InValidType.Argument)
            {
                argTB.BackColor = Color.White;
            }
            else
            {
                argTB.BackColor = Color.Red;
            }
            CheckStart();
        }

        public void I(IEnumerable<int> series, SetType type)
        {
            StringBuilder sb = new StringBuilder();

            foreach(int ion in series)
            {
                if(type == SetType.ArgLine)
                {
                    UserAction = true;

                    UserAction = false;
                }
                sb.Append(ion);
                sb.Append(',');
            }
            if(sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            ArgumentLine.IonSeries = sb.ToString();
        }

        private void SetGui(bool isUserAction)
        {
            UserAction = isUserAction;

            teTB.Text = ArgumentLine.PrecursorMZTolerance.ToString();

            toTB.Text = ArgumentLine.ProductMZTolerance.ToString();

            foreach(KeyValuePair<string, int> kvp in EnzymeDictionary)
            {
                if((int)ArgumentLine.Enzyme == kvp.Value)
                {
                    enzymeCB.SelectedItem = kvp.Key;
                    break;
                }
            }

            if(ArgumentLine.NumberOfSearchThreads == 0)
            {
                uaseAllSearchThreadsCB.Checked = true;
                ntUD.Enabled = false;
            }
            else
            {
                uaseAllSearchThreadsCB.Checked = false;
                ntUD.Enabled = true;
                ntUD.Value = ArgumentLine.NumberOfSearchThreads;
            }

            zccCB.SelectedIndex = (int)ArgumentLine.PrecursorChargeType - 1;

            zhUD.Value = (decimal)ArgumentLine.MaximumPrecursorCharge;

            zlUD.Value = (decimal)ArgumentLine.MinimumPrecursorCharge;

            zohUD.Value = (decimal)ArgumentLine.MaximumProductCharge;

            temCB.SelectedIndex = (int)ArgumentLine.PrecursorIonSearch;

            tomCB.SelectedIndex = (int)ArgumentLine.ProductIonSearch;

            tiUD.Value = (decimal)ArgumentLine.NumberOfIsotopicPeaksToSearch;

            teppmCB.SelectedIndex = ArgumentLine.PrecursorMZToleranceType == PrecursorMZToleranceType.DA ? 0 : 1;

            tezCB.SelectedIndex = ArgumentLine.ChargeDependencyOfPrecursorMassTolerance == ChargeDependencyType.None ? 0 : 1;

            cpCB.Checked = ArgumentLine.RemoveChargeReducedPrecursor;

            sbCB.Checked = ArgumentLine.SearchFirstForwardProductIon;

            sctCB.Checked = ArgumentLine.SearchCTerminusIons;

            mnmCB.Checked = ArgumentLine.NTermMethionineShouldNotBeCleaved;

            ztUD.Value = (int)ArgumentLine.MinimumPrecursorChargeToConsiderMultiplyChargedProducts;

            vUD.Value = (int)ArgumentLine.NumberOfMissedCleavages;

            ummCB.Checked = ArgumentLine.UseMemoryMappedSequences;

            wCB.Checked = ArgumentLine.IncludeSearchParamtersInResults;

            texUD.Value = (decimal)ArgumentLine.NeutronThresholdMZ;

            ClearAllCheckState(ionseriesLB);
            foreach(string number in ArgumentLine.IonSeries.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int value = 0;
                if(int.TryParse(number, out value))
                {
                    foreach(KeyValuePair<string, int> kvp in IonDictionary)
                    {
                        if(kvp.Value == value)
                        {
                            SetCheckState(ionseriesLB, kvp.Key, CheckState.Checked);
                        }
                    }
                }
            }

            ClearAllCheckState(fixmodLB);
            foreach(string number in ArgumentLine.FixedMods.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int value = 0;
                if(int.TryParse(number, out value))
                {
                    foreach(KeyValuePair<string, int> kvp in ModDictionary)
                    {
                        if(kvp.Value == value)
                        {
                            SetCheckState(fixmodLB, kvp.Key, CheckState.Checked);
                        }
                    }
                }
            }

            ClearAllCheckState(taxidCLB);
            foreach(string number in ArgumentLine.TaxIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int value = 0;
                if(int.TryParse(number, out value))
                {
                    foreach(KeyValuePair<string, int> kvp in TaxDictionary)
                    {
                        if(kvp.Value == value)
                        {
                            SetCheckState(taxidCLB, kvp.Key, CheckState.Checked);
                        }
                    }
                }
            }

            ClearAllCheckState(varmodLB);
            foreach(string number in ArgumentLine.VariableMods.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int value = 0;
                if(int.TryParse(number, out value))
                {
                    foreach(KeyValuePair<string, int> kvp in ModDictionary)
                    {
                        if(kvp.Value == value)
                        {
                            SetCheckState(varmodLB, kvp.Key, CheckState.Checked);
                        }
                    }
                }
            }

            clUD.Value = (decimal)ArgumentLine.LowIntensityCutoff;

            chUD.Value = (decimal)ArgumentLine.HighIntensityCutoff;

            ciUD.Value = (decimal)ArgumentLine.IntensityCutoffIncrement;

            w1UD.Value = (decimal)ArgumentLine.SingleChargeWindow;

            w2UD.Value = (decimal)ArgumentLine.DoubleChargeWindow;

            h1UD.Value = (decimal)ArgumentLine.NumberOfPeaksAllowedInSingleChargeWindow;

            h2UD.Value = (decimal)ArgumentLine.NumberOfPeaksAllowedInDoubleChargeWindow;

            hlUD.Value = (decimal)ArgumentLine.MaximumHitsRetainedPerChargeStatePerSpectrum;

            hcUD.Value = (decimal)ArgumentLine.MaximumHitsReportedPerSpectrum;

            heUD.Value = (decimal)ArgumentLine.MaximumEValue;

            hsUD.Value = (decimal)ArgumentLine.MinimumNumberOfPeaksInSpectrumToBeSearched;

            hmUD.Value = (decimal)ArgumentLine.MinimumNumberOfHitsFromLibraryToHavePeptideHitRecorded;

            htUD.Value = (decimal)ArgumentLine.NumberOfMostIntensePeaksNeededToMatchWithTheoreiticalPeptide;

            mmUD.Value = (decimal)ArgumentLine.MaximumMassLadderPerDatabasePeptide;

            z1UD.Value = (decimal)ArgumentLine.FractionOfPeaksToAssignPlus1;

            zcCB.Checked = ArgumentLine.AlgorithmicallyDetermineChargePlusOne;

            znCB.SelectedIndex = (ArgumentLine.Polarity == Polarity.Positive) ? 0 : 1;

            pcUD.Value = (decimal)ArgumentLine.MinimumPrecursorsToMatchSpectrum;

            spUD.Value = (decimal)ArgumentLine.MaximumIonsPerSeries;

            noUD.Value = (decimal)ArgumentLine.MinimumPeptideSizeNoEnzyme;

            noxUD.Value = (decimal)ArgumentLine.MaximumPeptideSizeNoEnzyme;

            scorrCB.Checked = ArgumentLine.CorrelationCorrection;

            scorpUD.Value = (decimal)ArgumentLine.ProbabilityOfConsecutiveIon;

            UpdateEnables();
            UserAction = !isUserAction;
        }

        private void ReadInArgumentLine()
        {
            IsValid(true);
            try
            {
                IsValid(ArgumentLine.SetArguments(argTB.Text));
            }
            catch(Exception e)
            {
                if(e is ArgumentNullException)
                {
                    IsValid(false);
                }
                else if(e is ArgumentOutOfRangeException)
                {
                    IsValid(false, e.Message);
                }
                else if(e is ArgumentException)
                {
                    IsValid(false);
                }
            }

            SetGui(false);
        }

        private void UpdateArgumentLine()
        {
            UpdateArgumentLine(!UserAction);
        }

        private void UpdateArgumentLine(bool force)
        {
            int pos = argTB.SelectionStart;
            argTB.Text = ArgumentLine.ToString(true);
            argTB.SelectionStart = Math.Min(pos, argTB.Text.Length);
            IsValid(true);
            UpdateEnables();
        }

        #endregion

        #region LoadFiles Methods

        private void LoadDtaFiles(IEnumerable<string> filePaths)
        {
            List<DtaFile> newDtas = new List<DtaFile>();
            foreach(string path in filePaths)
            {
                newDtas.Add(new DtaFile(path));
                SuggestedOutputFilePath = Path.GetDirectoryName(path);
            }
            if(DTAFiles == null)
            {
                DTAFiles = newDtas;
            }
            else
            {
                DTAFiles.AddRange(newDtas);
            }
            UpdateDtaList();
        }

        private void LoadUserModFile(string modPath)
        {
            UserMod = new UserModFile(modPath);
            usermodTB.Text = UserMod.Name;
            ReadUserMods();
        }

        private void LoadDatabase(string dbpath)
        {
            DataBase = new DataBaseFile(dbpath);
            databaseTB.Text = DataBase.FilePath;
            if((ValidState & InValidType.Database) == InValidType.Database)
            {
                ValidState ^= InValidType.Database;
            }
            UpdateEnables();
        }

        private void LoadTaxIds()
        {
            taxidCLB.Items.Clear();
            TaxDictionary = new Dictionary<string, int>();
            using(StreamReader reader = new StreamReader("taxids.txt"))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    if(!String.IsNullOrEmpty(line))
                    {
                        string[] data = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if(data != null || data.Length == 2)
                        {
                            string name = data[0];
                            int id = int.Parse(data[1]);
                            TaxDictionary.Add(name, id);
                            taxidCLB.Items.Add(name);
                        }
                    }
                }
            }
        }

        private void UpdateDtaList()
        {
            dtafileLB.Items.Clear();
            if(DTAFiles != null)
            {
                ValidState |= InValidType.DtaFiles;
                if(OutputFilePath == String.Empty)
                {
                    OutputFilePath = SuggestedOutputFilePath;
                }
                foreach(DtaFile dtaFile in DTAFiles)
                {
                    dtafileLB.Items.Add(dtaFile.Name);
                    ValidState &= ~InValidType.DtaFiles;
                }
            }
            else
            {
                ValidState |= InValidType.DtaFiles;
            }
            UpdateEnables();
        }

        private void RemoveFiles(IEnumerable<string> objectstoRemove)
        {
            if(DTAFiles != null)
            {
                DtaFile toRemove = null;
                foreach(string obj in objectstoRemove)
                {
                    bool remove = false;
                    foreach(DtaFile dtaFile in DTAFiles)
                    {
                        if(remove = (dtaFile.Name == obj))
                        {
                            toRemove = dtaFile;
                            break;
                        }
                    }
                    if(remove)
                    {
                        DTAFiles.Remove(toRemove);
                    }
                }
                UpdateDtaList();
            }
        }

        #endregion

        #region Event Handlers

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult res = openDTAFile.ShowDialog();
            if(res == DialogResult.OK)
            {
                LoadDtaFiles(openDTAFile.FileNames);
            }
            UpdateEnables();
        }

        private void clearrawB_Click(object sender, EventArgs e)
        {
            List<string> files = new List<string>(dtafileLB.Items.Count);
            foreach(string file in dtafileLB.Items)
            {
                files.Add(file);
            }
            RemoveFiles(files);
            OutputFilePath = string.Empty;
            UpdateEnables();
        }

        private void rawfileLB_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                List<string> files = new List<string>(dtafileLB.SelectedItems.Count);
                foreach(string file in dtafileLB.SelectedItems)
                {
                    files.Add(file);
                }
                RemoveFiles(files);
            }
        }

        private void usermodsCB_CheckedChanged(object sender, EventArgs e)
        {
            UpdateArgumentLine();
            UpdateEnables();
        }

        private void ocCB_CheckedChanged(object sender, EventArgs e)
        {
            UpdateArgumentLine();
        }

        private void oxCB_CheckedChanged(object sender, EventArgs e)
        {
            UpdateArgumentLine();
        }

        private void opCB_CheckedChanged(object sender, EventArgs e)
        {
            UpdateArgumentLine();
        }

        private void outputpathB_Click(object sender, EventArgs e)
        {
            DialogResult res = outputdirectoryDialog.ShowDialog();
            if(res == DialogResult.OK)
            {
                OutputFilePath = outputdirectoryDialog.SelectedPath;
                UpdateEnables();
            }
        }

        private void outputpathTB_TextChanged(object sender, EventArgs e)
        {
            OutputFilePath = outputpathTB.Text;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            List<string> dtaFiles = new List<string>();
            string xmlFile = string.Empty;
            string dbFile = string.Empty;
            foreach(string file in files)
            {
                switch(Path.GetExtension(file))
                {
                    case ".txt":
                        dtaFiles.Add(file);
                        break;
                    case ".xml":
                        xmlFile = file;
                        break;
                    case ".psq":
                    case ".pin":
                    case ".phr":
                    case ".pal":
                        dbFile = file;
                        break;
                    default:
                        break;
                }
            }
            LoadDtaFiles(dtaFiles);
            if(xmlFile != string.Empty)
            {
                LoadUserModFile(xmlFile);
                usermodsCB.Checked = true;
            }
            if(dbFile != string.Empty)
            {
                LoadDatabase(dbFile);
            }
        }

        private void usermodButton_Click(object sender, EventArgs e)
        {
            DialogResult res = openusermodfileDialog.ShowDialog();
            if(res == DialogResult.OK)
            {
                LoadUserModFile(openusermodfileDialog.FileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult res = opendbDialog.ShowDialog();
            if(res == DialogResult.OK)
            {
                LoadDatabase(opendbDialog.FileName);
            }
        }

        private void argTB_KeyUp(object sender, KeyEventArgs e)
        {
            ReadInArgumentLine();
        }

        private void enzymeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.TrySetArgument("-e", EnzymeDictionary[enzymeCB.SelectedItem.ToString()]);
                UpdateArgumentLine();
            }
        }

        private void ionseriesLB_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if(UserAction)
            {
                StringBuilder sb = new StringBuilder();
                List<int> indices = new List<int>();
                int itemindex = 0;
                foreach(object item in ionseriesLB.CheckedItems)
                {
                    if(IonDictionary.TryGetValue(item.ToString(), out itemindex))
                    {
                        indices.Add(itemindex);
                    }
                }
                if(e.NewValue == CheckState.Checked)
                {
                    if(IonDictionary.TryGetValue(ionseriesLB.Items[e.Index].ToString(), out itemindex))
                    {
                        indices.Add(itemindex);
                    }
                }
                else if(e.NewValue == CheckState.Unchecked)
                {
                    if(IonDictionary.TryGetValue(ionseriesLB.Items[e.Index].ToString(), out itemindex))
                    {
                        if(indices.Contains(itemindex))
                        {
                            indices.Remove(itemindex);
                        }
                    }
                }
                indices.Sort();
                foreach(int index in indices)
                {
                    sb.Append(index);
                    sb.Append(',');
                }
                if(sb.Length > 1) sb.Remove(sb.Length - 1, 1);
                ArgumentLine.IonSeries = sb.ToString();
                UpdateArgumentLine();
            }
        }

        private void zccCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            zlUD.Enabled = zhUD.Enabled = (zccCB.SelectedIndex == 1);
            if(UserAction)
            {
                ArgumentLine.PrecursorChargeType = (PrecursorChargeType)(zccCB.SelectedIndex + 1);
                UpdateArgumentLine();
            }
        }

        private void zlUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MinimumPrecursorCharge = (int)zlUD.Value;
                UpdateArgumentLine();
            }
        }

        private void zhUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MaximumPrecursorCharge = (int)zhUD.Value;
                UpdateArgumentLine();
            }
        }

        private void teppmCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.PrecursorMZToleranceType = (teppmCB.SelectedItem.ToString() == "PPM") ?
                    PrecursorMZToleranceType.PPM : PrecursorMZToleranceType.DA;
                UpdateArgumentLine();
            }
        }

        private void cpCB_CheckedChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.RemoveChargeReducedPrecursor = cpCB.Checked;
                UpdateArgumentLine();
            }
        }

        private void tezCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.ChargeDependencyOfPrecursorMassTolerance = tezCB.SelectedItem.ToString() == "Linear" ?
                    ChargeDependencyType.Linear : ChargeDependencyType.None;
                UpdateArgumentLine();
            }
        }

        private void temCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.PrecursorIonSearch = (PrecursorIonSearchType)temCB.SelectedIndex;
                UpdateArgumentLine();
            }
        }

        private void teTB_TextChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                try
                {
                    teTB.BackColor = Color.White;
                    ArgumentLine.TrySetArgument("-te", teTB.Text);
                }
                catch
                {
                    teTB.BackColor = Color.Red;
                    IsValid(false);
                }
                finally
                {
                    UpdateArgumentLine();
                }
            }
        }

        private void tiUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.NumberOfIsotopicPeaksToSearch = (int)tiUD.Value;
                UpdateArgumentLine();
            }
        }

        private void znCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.Polarity = (znCB.SelectedIndex == 0) ? Polarity.Positive : Polarity.Negative;
                UpdateArgumentLine();
            }
        }

        private void fixmodLB_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if(UserAction)
            {
                StringBuilder sb = new StringBuilder();
                List<int> indices = new List<int>();
                int itemindex = 0;
                foreach(object item in fixmodLB.CheckedItems)
                {
                    if(ModDictionary.TryGetValue(item.ToString(), out itemindex))
                    {
                        indices.Add(itemindex);
                    }
                }
                if(e.NewValue == CheckState.Checked)
                {
                    if(ModDictionary.TryGetValue(fixmodLB.Items[e.Index].ToString(), out itemindex))
                    {
                        indices.Add(itemindex);
                    }
                }
                else if(e.NewValue == CheckState.Unchecked)
                {
                    if(ModDictionary.TryGetValue(fixmodLB.Items[e.Index].ToString(), out itemindex))
                    {
                        if(indices.Contains(itemindex))
                        {
                            indices.Remove(itemindex);
                        }
                    }
                }
                indices.Sort();
                foreach(int index in indices)
                {
                    sb.Append(index);
                    sb.Append(',');
                }
                if(sb.Length > 1) sb.Remove(sb.Length - 1, 1);
                ArgumentLine.FixedMods = sb.ToString();
                UpdateArgumentLine();
            }
        }

        private void varmodLB_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if(UserAction)
            {
                StringBuilder sb = new StringBuilder();
                List<int> indices = new List<int>();
                int itemindex = 0;
                foreach(object item in varmodLB.CheckedItems)
                {
                    if(ModDictionary.TryGetValue(item.ToString(), out itemindex))
                    {
                        indices.Add(itemindex);
                    }
                }
                if(e.NewValue == CheckState.Checked)
                {
                    if(ModDictionary.TryGetValue(varmodLB.Items[e.Index].ToString(), out itemindex))
                    {
                        indices.Add(itemindex);
                    }
                }
                else if(e.NewValue == CheckState.Unchecked)
                {
                    if(ModDictionary.TryGetValue(varmodLB.Items[e.Index].ToString(), out itemindex))
                    {
                        if(indices.Contains(itemindex))
                        {
                            indices.Remove(itemindex);
                        }
                    }
                }
                indices.Sort();
                foreach(int index in indices)
                {
                    sb.Append(index);
                    sb.Append(',');
                }
                if(sb.Length > 1) sb.Remove(sb.Length - 1, 1);
                ArgumentLine.VariableMods = sb.ToString();
                UpdateArgumentLine();
            }
        }

        private void ntUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.NumberOfSearchThreads = (int)ntUD.Value;
                UpdateArgumentLine();
            }
        }

        private void startsearchB_Click(object sender, EventArgs e)
        {
            LoadDatabase(databaseTB.Text);
            BeginSearch();
        }

        private void uaseAllSearchThreadsCB_CheckedChanged(object sender, EventArgs e)
        {
            ntUD.Enabled = !uaseAllSearchThreadsCB.Checked;
            if(UserAction)
            {
                ArgumentLine.NumberOfSearchThreads = ntUD.Enabled ? (int)ntUD.Value : 0;
                UpdateArgumentLine();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Omssa.IsSearching)
            {
                if(DialogResult.No == MessageBox.Show("A current search is under way, cancel it?", "Stop Searching?", MessageBoxButtons.YesNo))
                {
                    e.Cancel = true;
                    return;
                }
            }
            Omssa.Quit();
        }

        #endregion

        #region Statics

        private static void ClearAllCheckState(CheckedListBox clb)
        {
            for(int i = 0; i < clb.Items.Count; i++)
            {
                clb.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        private static void SetCheckState(CheckedListBox clb, string value, CheckState state)
        {
            int index = 0;
            foreach(object item in clb.Items)
            {
                if(item.ToString() == value)
                {
                    clb.SetItemCheckState(index, state);
                    return;
                }
                index++;
            }
        }

        #endregion

        private void tomCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.ProductIonSearch = (ProductIonSearchType)tomCB.SelectedIndex;
                UpdateArgumentLine();
            }
        }

        private void toTB_TextChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                try
                {
                    toTB.BackColor = Color.White;
                    ArgumentLine.TrySetArgument("-to", toTB.Text);
                }
                catch(Exception ex)
                {
                    if(ex is ArgumentException)
                    {
                        toTB.BackColor = Color.Red;
                        IsValid(false);
                    }
                }
                finally
                {
                    UpdateArgumentLine();
                }
            }
        }

        private void zohUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MaximumProductCharge = (int)zohUD.Value;
                UpdateArgumentLine();
            }
        }

        private void sbCB_CheckedChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.SearchFirstForwardProductIon = !sbCB.Checked;
                UpdateArgumentLine();
            }
        }

        private void cbtCB_CheckedChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.SearchCTerminusIons = sctCB.Checked;
                UpdateArgumentLine();
            }
        }

        private void mnmCB_CheckedChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.NTermMethionineShouldNotBeCleaved = mnmCB.Checked;
                UpdateArgumentLine();
            }
        }

        private void ztUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MinimumPrecursorChargeToConsiderMultiplyChargedProducts = (int)ztUD.Value;
                UpdateArgumentLine();
            }
        }

        private void vUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.NumberOfMissedCleavages = (int)vUD.Value;
                UpdateArgumentLine();
            }
        }

        private void ummCB_CheckedChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.UseMemoryMappedSequences = ummCB.Checked;
                UpdateArgumentLine();
            }
        }

        private void wCB_CheckedChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.IncludeSearchParamtersInResults = wCB.Checked;
                UpdateArgumentLine();
            }
        }

        private void texUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.NeutronThresholdMZ = (double)texUD.Value;
                UpdateArgumentLine();
            }
        }

        private void clUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.LowIntensityCutoff = (double)clUD.Value;
                UpdateArgumentLine();
            }
        }

        private void chUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.HighIntensityCutoff = (double)chUD.Value;
                UpdateArgumentLine();
            }
        }

        private void ciUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.IntensityCutoffIncrement = (double)ciUD.Value;
                UpdateArgumentLine();
            }
        }

        private void w1UD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.SingleChargeWindow = (int)w1UD.Value;
                UpdateArgumentLine();
            }
        }

        private void w2UD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.DoubleChargeWindow = (int)w2UD.Value;
                UpdateArgumentLine();
            }
        }

        private void h1UD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.NumberOfPeaksAllowedInSingleChargeWindow = (int)h1UD.Value;
                UpdateArgumentLine();
            }
        }

        private void h2UD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.NumberOfPeaksAllowedInDoubleChargeWindow = (int)h2UD.Value;
                UpdateArgumentLine();
            }
        }

        private void hlUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MaximumHitsRetainedPerChargeStatePerSpectrum = (int)hlUD.Value;
                UpdateArgumentLine();
            }
        }

        private void hcUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MaximumHitsReportedPerSpectrum = (int)hcUD.Value;
                UpdateArgumentLine();
            }
        }

        private void htUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.NumberOfMostIntensePeaksNeededToMatchWithTheoreiticalPeptide = (int)htUD.Value;
                UpdateArgumentLine();
            }
        }

        private void hmUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MinimumNumberOfHitsFromLibraryToHavePeptideHitRecorded = (int)hmUD.Value;
                UpdateArgumentLine();
            }
        }

        private void hsUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MinimumNumberOfPeaksInSpectrumToBeSearched = (int)hsUD.Value;
                UpdateArgumentLine();
            }
        }

        private void heUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MaximumEValue = (double)heUD.Value;
                UpdateArgumentLine();
            }
        }

        private void mmUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MaximumMassLadderPerDatabasePeptide = (int)mmUD.Value;
                UpdateArgumentLine();
            }
        }

        private void z1UD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.FractionOfPeaksToAssignPlus1 = (double)z1UD.Value;
                UpdateArgumentLine();
            }
        }

        private void zcCB_CheckedChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.AlgorithmicallyDetermineChargePlusOne = zcCB.Checked;
                UpdateArgumentLine();
            }
        }

        private void pcUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MinimumPrecursorsToMatchSpectrum = (int)pcUD.Value;
                UpdateArgumentLine();
            }
        }

        private void spUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MaximumIonsPerSeries = (int)spUD.Value;
                UpdateArgumentLine();
            }
        }

        private void noUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MinimumPeptideSizeNoEnzyme = (int)noUD.Value;
                UpdateArgumentLine();
            }
        }

        private void noxUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.MaximumPeptideSizeNoEnzyme = (int)noxUD.Value;
                UpdateArgumentLine();
            }
        }

        private void scorrCB_CheckedChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.CorrelationCorrection = scorrCB.Checked;
                UpdateArgumentLine();
            }
        }

        private void scorpUD_ValueChanged(object sender, EventArgs e)
        {
            if(UserAction)
            {
                ArgumentLine.ProbabilityOfConsecutiveIon = (double)scorpUD.Value;
                UpdateArgumentLine();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process p = Process.Start("taxids.txt");
            p.WaitForExit();
            LoadTaxIds();
        }

        private void taxidCLB_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if(UserAction)
            {
                StringBuilder sb = new StringBuilder();
                List<int> indices = new List<int>();
                int itemindex = 0;
                foreach(object item in taxidCLB.CheckedItems)
                {
                    if(TaxDictionary.TryGetValue(item.ToString(), out itemindex))
                    {
                        indices.Add(itemindex);
                    }
                }
                if(e.NewValue == CheckState.Checked)
                {
                    if(TaxDictionary.TryGetValue(taxidCLB.Items[e.Index].ToString(), out itemindex))
                    {
                        indices.Add(itemindex);
                    }
                }
                else if(e.NewValue == CheckState.Unchecked)
                {
                    if(TaxDictionary.TryGetValue(taxidCLB.Items[e.Index].ToString(), out itemindex))
                    {
                        if(indices.Contains(itemindex))
                        {
                            indices.Remove(itemindex);
                        }
                    }
                }
                indices.Sort();
                foreach(int index in indices)
                {
                    sb.Append(index);
                    sb.Append(',');
                }
                if(sb.Length > 1) sb.Remove(sb.Length - 1, 1);
                ArgumentLine.TaxIds = sb.ToString();
                UpdateArgumentLine();
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process p = Process.Start("usermods.xml");
            p.WaitForExit();
            LoadUserModFile("usermods.xml");
        }
    }
}