using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using System.Windows.Forms;
using CSMSL;
using CSMSL.Chemistry;
using CSMSL.IO;
using CSMSL.IO.OMSSA;
using CSMSL.IO.Thermo;
using CSMSL.Proteomics;
using LumenWorks.Framework.IO.Csv;

namespace Coon.Compass.Lotor
{
    public partial class lotorForm : Form
    {
        public static Version GetRunningVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }  

        public Lotor Lotor = null;
        public Thread MainThread = null;

        public lotorForm()
        {
            InitializeComponent();
            AdditionalSetup();           
        }

        private void AdditionalSetup()
        {
            Text = string.Format("Lotor ({0})", GetRunningVersion());

            comboBox1.DataSource = Enum.GetValues(typeof(MassToleranceType));
            comboBox1.SelectedItem = MassToleranceType.DA;

            foreach (OmssaModification mod in OmssaModification.GetAllModifications())
            {
                if (mod.Name == "carbamidomethyl C")
                    listBox2.Items.Add(mod);
                else
                    listBox1.Items.Add(mod);
            }
        }

        public void Run()
        {
            logTB.Clear();
            logTB.BackColor = Color.White;

            string rawFileDirectory = textBox3.Text;
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

            List<Modification> fixedModifications = listBox2.Items.OfType<Modification>().ToList();

            List<string> modNames = checkedListBox1.CheckedItems.OfType<string>().ToList();
            List<Modification> quantifiedModifications = new List<Modification>();
            foreach (string modName in modNames)
            {
                OmssaModification mod;
                if (!OmssaModification.TryGetModification(modName, out mod))
                {
                    UpdateLog("Unable to load mod "+modName+". Did you load the correct modification file?");
                    return;
                }
                quantifiedModifications.Add(mod);
            }
           
            double ascoreThreshold = (double)numericUpDown2.Value;
            double prodThreshold = (double)numericUpDown3.Value / 100.0;
            bool ignoreCTerminal = checkBox2.Checked;


            MassTolerance prodTolerance = GetProductTolerance();
            Lotor = new Lotor(rawFileDirectory, inputcsvfile, outputDirectory, fixedModifications,
                quantifiedModifications, prodTolerance, ascoreThreshold, prodThreshold, ignoreCTerminal,
                FragmentTypes.b | FragmentTypes.y);
            Lotor.UpdateLog += lotor_UpdateLog;
            Lotor.UpdateProgress += lotor_UpdateProgress;
            localizeB.Enabled = false;
            MainThread = new Thread(Lotor.Localize) {IsBackground = true};
            MainThread.Start();
        }

        public MassTolerance GetProductTolerance()
        {
            double value = (double)numericUpDown1.Value;
            MassToleranceType type = (MassToleranceType)comboBox1.SelectedItem;
            return new MassTolerance(type, value);
        }
        
        public void LoadUserMods(string userModFile)
        {
            OmssaModification.LoadOmssaModifications(userModFile, true);
           
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (OmssaModification mod in OmssaModification.GetAllModifications())
            {
                if (mod.Name == "carbamidomethyl C")
                    listBox2.Items.Add(mod);
                else
                    listBox1.Items.Add(mod);
            }
        }

        private HashSet<string> _variableModifications = new HashSet<string>();

        private void UpdateVariableMods()
        {
            if (string.IsNullOrEmpty(textBox1.Text))
                return;
            _variableModifications.Clear();
            using (CsvReader reader = new CsvReader(new StreamReader(textBox1.Text), true))
            {
                while (reader.ReadNextRecord())
                {
                    foreach (
                        string modName in
                            OmssaModification.SplitModificationLine(reader["Mods"]))
                    {
                        _variableModifications.Add(modName);
                    }
                }
            }
            checkedListBox1.Items.Clear();
            foreach (string modName in _variableModifications)
            {
                checkedListBox1.Items.Add(modName);
            }
        }

        public void LoadPeptides(string filename)
        {
            textBox1.Text = filename;
            if (string.IsNullOrEmpty(textBox2.Text))
                textBox2.Text = Path.GetDirectoryName(filename);
            if(string.IsNullOrEmpty(textBox3.Text))
                textBox3.Text = Path.GetDirectoryName(filename);
            UpdateVariableMods();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Run();
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
                    MainThread = null;
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
                textBox3.Text = rawD.SelectedPath;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (outputD.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = outputD.SelectedPath;
            }
        }

        private void LoadFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                switch (Path.GetExtension(file))
                {
                    case ".csv":
                        LoadPeptides(file);
                        break;
                    case ".xml":
                        LoadUserMods(file);
                        break;
                }
            }
        }

        private void lotorForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void lotorForm_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            LoadFiles(files);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = listBox1.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int index = listBox1.SelectedIndices[i];
                listBox2.Items.Add(listBox1.Items[index]);
                listBox1.Items.RemoveAt(index);
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            for (int i = listBox2.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int index = listBox2.SelectedIndices[i];
                listBox1.Items.Add(listBox2.Items[index]);
                listBox2.Items.RemoveAt(index);
            }
        }
     
    }
}
