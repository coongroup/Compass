using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Compass.Coondornator.Properties;

namespace Compass.Coondornator
{
    public partial class CoondornatorForm : Form
    {
        public static readonly BindingList<OmssaParameterLine> ParameterLines = new BindingList<OmssaParameterLine>();
        public static readonly BindingList<DatabaseFile> Databases = new BindingList<DatabaseFile>();
        public static readonly BindingList<DtaFile> DtaFiles = new BindingList<DtaFile>();

        private ServerConnection _connection;

        public CoondornatorForm()
        {
            InitializeComponent();
        }

        public async Task<bool> ConnectAsync(string name, string host, string password)
        {
            bool result = false;
            ServerConnection connection = null;
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(password))
            {
                connection = new ServerConnection(host, name);
                Task<bool> connectionTask = connection.ConnectAsync(password);

                SetStatusLabel(string.Format("Attempting to connect to: {0}@{1}", name, host));
                toolStripProgressBar1.ProgressBar.Style = ProgressBarStyle.Marquee;
                result = await connectionTask;
            }

            if (result)
            {
                _connection = connection;
                _connection.UploadProgress += (sender, e) => UpdateProgress(e);
                _connection.UploadStart += (sender, e) => SetStatusLabel("Uploading file " + e.FilePath + "...");
                SetStatusLabel("Loading Databases from server...");
                await RefreshDatabases();
                SetStatusLabel();
                SetTitle(string.Format("{0}@{1}", _connection.UserName, _connection.Host));
                toolStripProgressBar1.ProgressBar.Style = ProgressBarStyle.Continuous;
                submitToolStripMenuItem.Enabled = true;
            }
            else
            {
                submitToolStripMenuItem.Enabled = false;
                toolStripProgressBar1.ProgressBar.Style = ProgressBarStyle.Continuous;
                SetStatusLabel(string.Format("Unable to Connected To: {0}@{1}", name, host));
            }

            return result;
        }

        private void UpdateProgress(ProgressEventArgs progressEventArgs)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ProgressEventArgs>(UpdateProgress), progressEventArgs);
                return;
            }

            toolStripStatusLabel1.Text = string.Format("{0}/{1} KB", progressEventArgs.Position/(1024*1024), progressEventArgs.Length/(1024*1024));
            toolStripProgressBar1.ProgressBar.Value = (int) (toolStripProgressBar1.ProgressBar.Maximum*progressEventArgs.Percent);
        }

        private void UpdateProgress(double percent)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<double>(UpdateProgress), percent);
                return;
            }

            toolStripProgressBar1.ProgressBar.Value = (int) (toolStripProgressBar1.ProgressBar.Maximum*percent);
        }

        private void SetStatusLabel(string message = "")
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetStatusLabel), message);
                return;
            }

            toolStripStatusLabel2.Text = message;
        }

        protected override async void OnLoad(EventArgs e)
        {
            //if (Settings.Default.UpdateSettings)
            //{
            //    Settings.Default.Upgrade();
            //    Settings.Default.UpdateSettings = false;
            //    Settings.Default.Save();
            //}

            SetTitle("Connecting...");
            OmssaParameterLine.Changed += (sender, e2) => RefreshParameterLines();
            RefreshParameterLines();

            checkedListBox1.DataSource = Databases;
            checkedListBox1.DisplayMember = "Name";

            listBox1.DataSource = ParameterLines;
            listBox1.DisplayMember = "Name";

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = true;

            DataGridViewTextBoxColumn dtaName = new DataGridViewTextBoxColumn();
            dtaName.HeaderText = "DTA Name";
            dtaName.ReadOnly = true;
            dtaName.DataPropertyName = "Name";
            dtaName.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dtaName.Frozen = true;
            dataGridView1.Columns.Add(dtaName);

            DataGridViewTextBoxColumn argumentLine = new DataGridViewTextBoxColumn();
            argumentLine.HeaderText = "Parameters";
            argumentLine.DataPropertyName = "ParameterLine";
            argumentLine.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add(argumentLine);

            dataGridView1.DataSource = DtaFiles;

            string name = Settings.Default.UserName;
            string host = Settings.Default.Host;
            string password = Settings.Default.Password;

            bool result = await ConnectAsync(name, host, password);

            if (!result)
            {
                // Show the server connection form if auto login doesn't work
                ServerConnectionForm form = new ServerConnectionForm(this);
                form.ShowDialog();
            }

            textBox1.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
           

            base.OnLoad(e);
        }

        private void RefreshParameterLines()
        {
            ParameterLines.RaiseListChangedEvents = false;
            ParameterLines.Clear();
            foreach (var parameterLine in OmssaParameterLine.GetAllParameterLines())
                ParameterLines.Add(parameterLine);
            ParameterLines.RaiseListChangedEvents = true;
            ParameterLines.ResetBindings();
        }

        private async Task RefreshDatabases()
        {
            Databases.RaiseListChangedEvents = false;
            Databases.Clear();
            foreach (var database in await _connection.GetBlastDatabases())
                Databases.Add(database);
            Databases.RaiseListChangedEvents = true;
            Databases.ResetBindings();
        }

        private void AddDtaFile(string filePath)
        {
            DtaFile file = new DtaFile(filePath);
            DtaFiles.Add(file);
        }

        private async void Submit()
        {
            try
            {
                bool splitSpectra = checkBox1.Checked;
                int spectraToSplit = (int)numericUpDown1.Value;

                submitToolStripMenuItem.Enabled = false;
                dataGridView1.Enabled = false;
                string jobName = textBox1.Text;

                if (DtaFiles.Count == 0)
                {
                    throw new ArgumentNullException("DTA Files", "Please supply at least one DTA file for the submission");
                }

                if (string.IsNullOrWhiteSpace(jobName))
                {
                    throw new ArgumentNullException("Job Name", "Please supply a job name for the submission");
                }
                
                DatabaseFile[] dataBases = checkedListBox1.CheckedItems.Cast<DatabaseFile>().ToArray();
                if (dataBases.Length == 0)
                {
                    throw new ArgumentNullException("Database", "Please selected at least one database for the submission");
                }

                UserModFile userModFile = null;
                string userModFilePath = textBox2.Text;
                if (!string.IsNullOrWhiteSpace(userModFilePath))
                {
                    if (!System.IO.File.Exists(userModFilePath))
                    {
                        throw new FileNotFoundException("Unable to locate user mod file", userModFilePath);
                    }
                    userModFile = new UserModFile(userModFilePath);
                }

                if (!_connection.DirectoryExists(_connection.CondorFolder))
                {
                    _connection.CreateDirectory(_connection.CondorFolder);
                }

                string remoteJobDirectory = _connection.CreateDirectory(_connection.CondorFolder + jobName);
                
                if (userModFile != null)
                {
                    SetStatusLabel("Uploading User Mod File...");
                    await _connection.PutFileAsync(userModFile, remoteJobDirectory);
                }
                
                List<DtaFile> dtaFiles = new List<DtaFile>();
              
                if (splitSpectra)
                {
                    SetStatusLabel("Splitting Dta Files to contain " + spectraToSplit + " spectra each...");
                    Task t = new Task(() =>
                    {
                        foreach (DtaFile dtaFile in DtaFiles.Distinct())
                        {
                            dtaFiles.AddRange(dtaFile.Split(spectraToSplit));
                        }
                    });
                    t.Start();
                    await t;
                }
                else
                {
                    dtaFiles = DtaFiles.ToList();
                }

                SetStatusLabel("Uploading Files to " + remoteJobDirectory + "...");
                await _connection.PutFilesAsync(dtaFiles.Distinct(), remoteJobDirectory);
                
                SetStatusLabel("Uploading Submit File...");
                CondorSubmitFile submitFile = new CondorSubmitFile(dtaFiles, userModFile, dataBases);
                submitFile.WriteToDisk();
                
                await _connection.PutFileAsync(submitFile, remoteJobDirectory, "submitFile.condor");
                
                if (splitSpectra)
                {
                    foreach (DtaFile dtaFile in dtaFiles)
                    {
                        System.IO.File.Delete(dtaFile.FilePath);
                    }
                }

                System.IO.File.Delete(submitFile.FilePath);

                string result = _connection.RunSubmission(remoteJobDirectory, "submitFile.condor");
                
                SetStatusLabel(result.Replace("\n"," "));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error Submitting Job", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                SetStatusLabel("Error");
            }
            finally
            {
                dataGridView1.Enabled = true;
                toolStripStatusLabel1.Text = "";
                submitToolStripMenuItem.Enabled = true;
                UpdateProgress(0);
            }
        }

        private void SetTitle(string msg = "")
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                Text = string.Format("Coondornator (v{0})", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            }
            else
            {
                Text = string.Format("Coondornator (v{0}) - {1}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version, msg);
            }
        }

        private void LoadUserModFile(string filePath)
        {
            textBox2.Text = filePath;
        }

        #region Event Handlers

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null)
                return;

            OmssaParameterLine line = listBox.SelectedValue as OmssaParameterLine;
            if (line == null)
                return;

            textBox3.Text = line.ToString();
            saveLineTB.Text = line.Name;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string name = saveLineTB.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("You must provide a non-null name to save this parameter line");
                return;
            }

            string line = textBox3.Text;
            if (!OmssaParameterLine.Validate(line))
            {
                MessageBox.Show("The provided parameter line cannot be correctly parsed. Please fix before saving");
                return;
            }

            OmssaParameterLine.AddLine(name, line);
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete)
                return;

            var listBox = sender as ListBox;
            if (listBox == null)
                return;

            OmssaParameterLine line = listBox.SelectedValue as OmssaParameterLine;
            if (line == null)
                return;

            OmssaParameterLine.RemoveLine(line.Name);

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            string line = textBox3.Text;
            if (OmssaParameterLine.Validate(line))
            {
                textBox3.BackColor = SystemColors.Window;
            }
            else
            {
                textBox3.BackColor = SystemColors.Info;
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null)
                return;

            if (files.Any(f => Path.GetExtension(f).Equals(".txt") || Path.GetExtension(f).Equals(".xml")))
                e.Effect = DragDropEffects.Link;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null)
                return;

            foreach (string file in files.Where(f => Path.GetExtension(f).Equals(".txt")))
            {
                AddDtaFile(file);
            }

            foreach (string file in files.Where(f => Path.GetExtension(f).Equals(".xml")))
            {
                LoadUserModFile(file);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DtaFiles.Count == 0)
                return;

            string line = textBox3.Text;
            if (!OmssaParameterLine.Validate(line))
            {
                MessageBox.Show("Unable to apply parameter line to all dtas because the line cannot be parsed correctly");
                return;
            }

            foreach (DtaFile file in DtaFiles)
            {
                file.ParameterLine = line;
            }
        }

        private void submitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Submit();
        }

        private void loadDTAFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openDtaDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openDtaDialog.FileNames)
                {
                    AddDtaFile(file);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerConnectionForm form = new ServerConnectionForm(this);
            form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openUserModFileD.ShowDialog() == DialogResult.OK)
            {
                LoadUserModFile(openUserModFileD.FileName);
            }
        }

        #endregion
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = checkBox1.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabPage2.BringToFront();
        }
    }
}
      
