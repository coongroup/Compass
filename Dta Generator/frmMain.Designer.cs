namespace Coon.Compass.DtaGenerator
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.fbdOutput = new System.Windows.Forms.FolderBrowserDialog();
            this.prgProgress = new System.Windows.Forms.ProgressBar();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstRawFiles = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.grpOutputOptions = new System.Windows.Forms.GroupBox();
            this.includeLogCB = new System.Windows.Forms.CheckBox();
            this.chkMascotMgfOutput = new System.Windows.Forms.CheckBox();
            this.chkSequestDtaOutput = new System.Windows.Forms.CheckBox();
            this.chkOmssaTxtOutput = new System.Windows.Forms.CheckBox();
            this.chkGroupByActivationEnergyTime = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.grpPeakFilteringOptions = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.grpIsobaricLabelCleaning = new System.Windows.Forms.GroupBox();
            this.chkCleanItraq8Plex = new System.Windows.Forms.CheckBox();
            this.chkCleanTmt6Plex = new System.Windows.Forms.CheckBox();
            this.chkCleanTmtDuplex = new System.Windows.Forms.CheckBox();
            this.chkCleanItraq4Plex = new System.Windows.Forms.CheckBox();
            this.chkCleanPrecursor = new System.Windows.Forms.CheckBox();
            this.chkEnableEtdPreProcessing = new System.Windows.Forms.CheckBox();
            this.grpAssumedPrecursorChargeStateRange = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.numMaximumAssumedPrecursorChargeState = new System.Windows.Forms.NumericUpDown();
            this.numMinimumAssumedPrecursorChargeState = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ofdRawFiles = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip1.SuspendLayout();
            this.grpOutputOptions.SuspendLayout();
            this.grpPeakFilteringOptions.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpIsobaricLabelCleaning.SuspendLayout();
            this.grpAssumedPrecursorChargeStateRange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumAssumedPrecursorChargeState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinimumAssumedPrecursorChargeState)).BeginInit();
            this.SuspendLayout();
            // 
            // prgProgress
            // 
            this.prgProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgProgress.Location = new System.Drawing.Point(12, 507);
            this.prgProgress.Maximum = 1000;
            this.prgProgress.Name = "prgProgress";
            this.prgProgress.Size = new System.Drawing.Size(479, 23);
            this.prgProgress.Step = 1;
            this.prgProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.prgProgress.TabIndex = 21;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(497, 507);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 38;
            this.btnOK.Text = "Run";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(497, 137);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 33;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstRawFiles
            // 
            this.lstRawFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstRawFiles.ContextMenuStrip = this.contextMenuStrip1;
            this.lstRawFiles.FormattingEnabled = true;
            this.lstRawFiles.HorizontalScrollbar = true;
            this.lstRawFiles.Location = new System.Drawing.Point(12, 25);
            this.lstRawFiles.Name = "lstRawFiles";
            this.lstRawFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstRawFiles.Size = new System.Drawing.Size(479, 134);
            this.lstRawFiles.TabIndex = 32;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearAllToolStripMenuItem,
            this.clearSelectedToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 48);
            // 
            // clearAllToolStripMenuItem
            // 
            this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
            this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.clearAllToolStripMenuItem.Text = "Clear All";
            this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.clearAllToolStripMenuItem_Click);
            // 
            // clearSelectedToolStripMenuItem
            // 
            this.clearSelectedToolStripMenuItem.Name = "clearSelectedToolStripMenuItem";
            this.clearSelectedToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.clearSelectedToolStripMenuItem.Text = "Clear Selected";
            this.clearSelectedToolStripMenuItem.Click += new System.EventHandler(this.clearSelectedToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(177, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Thermo .raw MS/MS Data Filepaths";
            // 
            // grpOutputOptions
            // 
            this.grpOutputOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOutputOptions.Controls.Add(this.includeLogCB);
            this.grpOutputOptions.Controls.Add(this.chkMascotMgfOutput);
            this.grpOutputOptions.Controls.Add(this.chkSequestDtaOutput);
            this.grpOutputOptions.Controls.Add(this.chkOmssaTxtOutput);
            this.grpOutputOptions.Controls.Add(this.chkGroupByActivationEnergyTime);
            this.grpOutputOptions.Location = new System.Drawing.Point(12, 427);
            this.grpOutputOptions.Name = "grpOutputOptions";
            this.grpOutputOptions.Size = new System.Drawing.Size(553, 74);
            this.grpOutputOptions.TabIndex = 108;
            this.grpOutputOptions.TabStop = false;
            this.grpOutputOptions.Text = "Output Options";
            // 
            // includeLogCB
            // 
            this.includeLogCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.includeLogCB.AutoSize = true;
            this.includeLogCB.Location = new System.Drawing.Point(438, 19);
            this.includeLogCB.Name = "includeLogCB";
            this.includeLogCB.Size = new System.Drawing.Size(106, 17);
            this.includeLogCB.TabIndex = 107;
            this.includeLogCB.Text = "Include Log Files";
            this.includeLogCB.UseVisualStyleBackColor = true;
            // 
            // chkMascotMgfOutput
            // 
            this.chkMascotMgfOutput.AutoSize = true;
            this.chkMascotMgfOutput.Checked = global::Coon.Compass.DtaGenerator.Properties.Settings.Default.mascotMgfOutput;
            this.chkMascotMgfOutput.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.DtaGenerator.Properties.Settings.Default, "mascotMgfOutput", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkMascotMgfOutput.Location = new System.Drawing.Point(258, 51);
            this.chkMascotMgfOutput.Name = "chkMascotMgfOutput";
            this.chkMascotMgfOutput.Size = new System.Drawing.Size(119, 17);
            this.chkMascotMgfOutput.TabIndex = 106;
            this.chkMascotMgfOutput.Text = "Mascot .mgf Output";
            this.chkMascotMgfOutput.UseVisualStyleBackColor = true;
            // 
            // chkSequestDtaOutput
            // 
            this.chkSequestDtaOutput.AutoSize = true;
            this.chkSequestDtaOutput.Checked = global::Coon.Compass.DtaGenerator.Properties.Settings.Default.sequestDtaOutput;
            this.chkSequestDtaOutput.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.DtaGenerator.Properties.Settings.Default, "sequestDtaOutput", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkSequestDtaOutput.Location = new System.Drawing.Point(9, 51);
            this.chkSequestDtaOutput.Name = "chkSequestDtaOutput";
            this.chkSequestDtaOutput.Size = new System.Drawing.Size(121, 17);
            this.chkSequestDtaOutput.TabIndex = 102;
            this.chkSequestDtaOutput.Text = "Sequest .dta Output";
            this.chkSequestDtaOutput.UseVisualStyleBackColor = true;
            // 
            // chkOmssaTxtOutput
            // 
            this.chkOmssaTxtOutput.AutoSize = true;
            this.chkOmssaTxtOutput.Checked = global::Coon.Compass.DtaGenerator.Properties.Settings.Default.omssaTxtOutput;
            this.chkOmssaTxtOutput.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOmssaTxtOutput.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.DtaGenerator.Properties.Settings.Default, "omssaTxtOutput", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkOmssaTxtOutput.Location = new System.Drawing.Point(136, 51);
            this.chkOmssaTxtOutput.Name = "chkOmssaTxtOutput";
            this.chkOmssaTxtOutput.Size = new System.Drawing.Size(116, 17);
            this.chkOmssaTxtOutput.TabIndex = 101;
            this.chkOmssaTxtOutput.Text = "OMSSA .txt Output";
            this.chkOmssaTxtOutput.UseVisualStyleBackColor = true;
            // 
            // chkGroupByActivationEnergyTime
            // 
            this.chkGroupByActivationEnergyTime.AutoSize = true;
            this.chkGroupByActivationEnergyTime.Checked = global::Coon.Compass.DtaGenerator.Properties.Settings.Default.groupByActivationEnergyTime;
            this.chkGroupByActivationEnergyTime.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.DtaGenerator.Properties.Settings.Default, "groupByActivationEnergyTime", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkGroupByActivationEnergyTime.Location = new System.Drawing.Point(9, 22);
            this.chkGroupByActivationEnergyTime.Name = "chkGroupByActivationEnergyTime";
            this.chkGroupByActivationEnergyTime.Size = new System.Drawing.Size(189, 17);
            this.chkGroupByActivationEnergyTime.TabIndex = 100;
            this.chkGroupByActivationEnergyTime.Text = "Group by Activation Energy / Time";
            this.chkGroupByActivationEnergyTime.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 177);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 105;
            this.label2.Text = "Output Folder";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(497, 174);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 103;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputFolder.Location = new System.Drawing.Point(86, 174);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(405, 20);
            this.txtOutputFolder.TabIndex = 104;
            // 
            // grpPeakFilteringOptions
            // 
            this.grpPeakFilteringOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPeakFilteringOptions.Controls.Add(this.groupBox1);
            this.grpPeakFilteringOptions.Controls.Add(this.linkLabel1);
            this.grpPeakFilteringOptions.Controls.Add(this.grpIsobaricLabelCleaning);
            this.grpPeakFilteringOptions.Controls.Add(this.chkCleanPrecursor);
            this.grpPeakFilteringOptions.Controls.Add(this.chkEnableEtdPreProcessing);
            this.grpPeakFilteringOptions.Location = new System.Drawing.Point(12, 260);
            this.grpPeakFilteringOptions.Name = "grpPeakFilteringOptions";
            this.grpPeakFilteringOptions.Size = new System.Drawing.Size(560, 161);
            this.grpPeakFilteringOptions.TabIndex = 106;
            this.grpPeakFilteringOptions.TabStop = false;
            this.grpPeakFilteringOptions.Text = "Peak Filtering Options";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkedListBox2);
            this.groupBox1.Location = new System.Drawing.Point(202, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(352, 102);
            this.groupBox1.TabIndex = 119;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Neutral Loss Cleaning";
            // 
            // checkedListBox2
            // 
            this.checkedListBox2.CheckOnClick = true;
            this.checkedListBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBox2.FormattingEnabled = true;
            this.checkedListBox2.Location = new System.Drawing.Point(3, 16);
            this.checkedListBox2.Name = "checkedListBox2";
            this.checkedListBox2.Size = new System.Drawing.Size(346, 83);
            this.checkedListBox2.TabIndex = 95;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(295, 26);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(119, 13);
            this.linkLabel1.TabIndex = 121;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "(i.e. the Good algorithm)";
            this.linkLabel1.Click += new System.EventHandler(this.linkLabel1_Click);
            // 
            // grpIsobaricLabelCleaning
            // 
            this.grpIsobaricLabelCleaning.Controls.Add(this.chkCleanItraq8Plex);
            this.grpIsobaricLabelCleaning.Controls.Add(this.chkCleanTmt6Plex);
            this.grpIsobaricLabelCleaning.Controls.Add(this.chkCleanTmtDuplex);
            this.grpIsobaricLabelCleaning.Controls.Add(this.chkCleanItraq4Plex);
            this.grpIsobaricLabelCleaning.Location = new System.Drawing.Point(9, 53);
            this.grpIsobaricLabelCleaning.Name = "grpIsobaricLabelCleaning";
            this.grpIsobaricLabelCleaning.Size = new System.Drawing.Size(189, 102);
            this.grpIsobaricLabelCleaning.TabIndex = 118;
            this.grpIsobaricLabelCleaning.TabStop = false;
            this.grpIsobaricLabelCleaning.Text = "Isobaric Label Cleaning";
            // 
            // chkCleanItraq8Plex
            // 
            this.chkCleanItraq8Plex.AutoSize = true;
            this.chkCleanItraq8Plex.Checked = global::Coon.Compass.DtaGenerator.Properties.Settings.Default.cleanItraq8Plex;
            this.chkCleanItraq8Plex.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.DtaGenerator.Properties.Settings.Default, "cleanItraq8Plex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkCleanItraq8Plex.Location = new System.Drawing.Point(98, 45);
            this.chkCleanItraq8Plex.Name = "chkCleanItraq8Plex";
            this.chkCleanItraq8Plex.Size = new System.Drawing.Size(89, 17);
            this.chkCleanItraq8Plex.TabIndex = 104;
            this.chkCleanItraq8Plex.Text = "iTRAQ 8-plex";
            this.chkCleanItraq8Plex.UseVisualStyleBackColor = true;
            // 
            // chkCleanTmt6Plex
            // 
            this.chkCleanTmt6Plex.AutoSize = true;
            this.chkCleanTmt6Plex.Checked = global::Coon.Compass.DtaGenerator.Properties.Settings.Default.cleanTmt6Plex;
            this.chkCleanTmt6Plex.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.DtaGenerator.Properties.Settings.Default, "cleanTmt6Plex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkCleanTmt6Plex.Location = new System.Drawing.Point(6, 45);
            this.chkCleanTmt6Plex.Name = "chkCleanTmt6Plex";
            this.chkCleanTmt6Plex.Size = new System.Drawing.Size(81, 17);
            this.chkCleanTmt6Plex.TabIndex = 93;
            this.chkCleanTmt6Plex.Text = "TMT 6-Plex";
            this.chkCleanTmt6Plex.UseVisualStyleBackColor = true;
            // 
            // chkCleanTmtDuplex
            // 
            this.chkCleanTmtDuplex.AutoSize = true;
            this.chkCleanTmtDuplex.Checked = global::Coon.Compass.DtaGenerator.Properties.Settings.Default.cleanTmtDuplex;
            this.chkCleanTmtDuplex.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.DtaGenerator.Properties.Settings.Default, "cleanTmtDuplex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkCleanTmtDuplex.Location = new System.Drawing.Point(6, 22);
            this.chkCleanTmtDuplex.Name = "chkCleanTmtDuplex";
            this.chkCleanTmtDuplex.Size = new System.Drawing.Size(85, 17);
            this.chkCleanTmtDuplex.TabIndex = 92;
            this.chkCleanTmtDuplex.Text = "TMT Duplex";
            this.chkCleanTmtDuplex.UseVisualStyleBackColor = true;
            // 
            // chkCleanItraq4Plex
            // 
            this.chkCleanItraq4Plex.AutoSize = true;
            this.chkCleanItraq4Plex.Checked = global::Coon.Compass.DtaGenerator.Properties.Settings.Default.cleanItraq4Plex;
            this.chkCleanItraq4Plex.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.DtaGenerator.Properties.Settings.Default, "cleanItraq4Plex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkCleanItraq4Plex.Location = new System.Drawing.Point(98, 22);
            this.chkCleanItraq4Plex.Name = "chkCleanItraq4Plex";
            this.chkCleanItraq4Plex.Size = new System.Drawing.Size(89, 17);
            this.chkCleanItraq4Plex.TabIndex = 91;
            this.chkCleanItraq4Plex.Text = "iTRAQ 4-plex";
            this.chkCleanItraq4Plex.UseVisualStyleBackColor = true;
            // 
            // chkCleanPrecursor
            // 
            this.chkCleanPrecursor.AutoSize = true;
            this.chkCleanPrecursor.Checked = global::Coon.Compass.DtaGenerator.Properties.Settings.Default.cleanPrecursor;
            this.chkCleanPrecursor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCleanPrecursor.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.DtaGenerator.Properties.Settings.Default, "cleanPrecursor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkCleanPrecursor.Location = new System.Drawing.Point(9, 25);
            this.chkCleanPrecursor.Name = "chkCleanPrecursor";
            this.chkCleanPrecursor.Size = new System.Drawing.Size(101, 17);
            this.chkCleanPrecursor.TabIndex = 105;
            this.chkCleanPrecursor.Text = "Clean Precursor";
            this.chkCleanPrecursor.UseVisualStyleBackColor = true;
            // 
            // chkEnableEtdPreProcessing
            // 
            this.chkEnableEtdPreProcessing.AutoSize = true;
            this.chkEnableEtdPreProcessing.Checked = global::Coon.Compass.DtaGenerator.Properties.Settings.Default.enableEtdPreProcessing;
            this.chkEnableEtdPreProcessing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableEtdPreProcessing.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.DtaGenerator.Properties.Settings.Default, "enableEtdPreProcessing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkEnableEtdPreProcessing.Location = new System.Drawing.Point(144, 25);
            this.chkEnableEtdPreProcessing.Name = "chkEnableEtdPreProcessing";
            this.chkEnableEtdPreProcessing.Size = new System.Drawing.Size(158, 17);
            this.chkEnableEtdPreProcessing.TabIndex = 103;
            this.chkEnableEtdPreProcessing.Text = "Enable ETD Pre-Processing";
            this.chkEnableEtdPreProcessing.UseVisualStyleBackColor = true;
            // 
            // grpAssumedPrecursorChargeStateRange
            // 
            this.grpAssumedPrecursorChargeStateRange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAssumedPrecursorChargeStateRange.Controls.Add(this.checkBox1);
            this.grpAssumedPrecursorChargeStateRange.Controls.Add(this.numMaximumAssumedPrecursorChargeState);
            this.grpAssumedPrecursorChargeStateRange.Controls.Add(this.numMinimumAssumedPrecursorChargeState);
            this.grpAssumedPrecursorChargeStateRange.Controls.Add(this.label6);
            this.grpAssumedPrecursorChargeStateRange.Controls.Add(this.label5);
            this.grpAssumedPrecursorChargeStateRange.Enabled = false;
            this.grpAssumedPrecursorChargeStateRange.Location = new System.Drawing.Point(12, 200);
            this.grpAssumedPrecursorChargeStateRange.Name = "grpAssumedPrecursorChargeStateRange";
            this.grpAssumedPrecursorChargeStateRange.Size = new System.Drawing.Size(560, 54);
            this.grpAssumedPrecursorChargeStateRange.TabIndex = 94;
            this.grpAssumedPrecursorChargeStateRange.TabStop = false;
            this.grpAssumedPrecursorChargeStateRange.Text = "Charge State Determiniation";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(6, 23);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(139, 17);
            this.checkBox1.TabIndex = 106;
            this.checkBox1.Text = "Determine Automatically";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // numMaximumAssumedPrecursorChargeState
            // 
            this.numMaximumAssumedPrecursorChargeState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numMaximumAssumedPrecursorChargeState.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Coon.Compass.DtaGenerator.Properties.Settings.Default, "maximumAssumedPrecursorChargeState", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numMaximumAssumedPrecursorChargeState.Location = new System.Drawing.Point(514, 20);
            this.numMaximumAssumedPrecursorChargeState.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numMaximumAssumedPrecursorChargeState.Minimum = new decimal(new int[] {
            99,
            0,
            0,
            -2147483648});
            this.numMaximumAssumedPrecursorChargeState.Name = "numMaximumAssumedPrecursorChargeState";
            this.numMaximumAssumedPrecursorChargeState.Size = new System.Drawing.Size(40, 20);
            this.numMaximumAssumedPrecursorChargeState.TabIndex = 3;
            this.numMaximumAssumedPrecursorChargeState.Value = global::Coon.Compass.DtaGenerator.Properties.Settings.Default.maximumAssumedPrecursorChargeState;
            // 
            // numMinimumAssumedPrecursorChargeState
            // 
            this.numMinimumAssumedPrecursorChargeState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numMinimumAssumedPrecursorChargeState.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Coon.Compass.DtaGenerator.Properties.Settings.Default, "minimumAssumedPrecursorChargeState", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numMinimumAssumedPrecursorChargeState.Location = new System.Drawing.Point(446, 19);
            this.numMinimumAssumedPrecursorChargeState.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numMinimumAssumedPrecursorChargeState.Minimum = new decimal(new int[] {
            99,
            0,
            0,
            -2147483648});
            this.numMinimumAssumedPrecursorChargeState.Name = "numMinimumAssumedPrecursorChargeState";
            this.numMinimumAssumedPrecursorChargeState.Size = new System.Drawing.Size(40, 20);
            this.numMinimumAssumedPrecursorChargeState.TabIndex = 2;
            this.numMinimumAssumedPrecursorChargeState.Value = global::Coon.Compass.DtaGenerator.Properties.Settings.Default.minimumAssumedPrecursorChargeState;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(492, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "to";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(366, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Charge States";
            // 
            // ofdRawFiles
            // 
            this.ofdRawFiles.Filter = "Thermo .raw data files (*.raw)|*.raw";
            this.ofdRawFiles.Multiselect = true;
            // 
            // frmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 536);
            this.Controls.Add(this.grpOutputOptions);
            this.Controls.Add(this.grpPeakFilteringOptions);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.grpAssumedPrecursorChargeStateRange);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lstRawFiles);
            this.Controls.Add(this.txtOutputFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.prgProgress);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "DTA Generator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
            this.contextMenuStrip1.ResumeLayout(false);
            this.grpOutputOptions.ResumeLayout(false);
            this.grpOutputOptions.PerformLayout();
            this.grpPeakFilteringOptions.ResumeLayout(false);
            this.grpPeakFilteringOptions.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.grpIsobaricLabelCleaning.ResumeLayout(false);
            this.grpIsobaricLabelCleaning.PerformLayout();
            this.grpAssumedPrecursorChargeStateRange.ResumeLayout(false);
            this.grpAssumedPrecursorChargeStateRange.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumAssumedPrecursorChargeState)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinimumAssumedPrecursorChargeState)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog ofdRawFiles;
        private System.Windows.Forms.FolderBrowserDialog fbdOutput;
        private System.Windows.Forms.ProgressBar prgProgress;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox lstRawFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkCleanItraq4Plex;
        private System.Windows.Forms.CheckBox chkCleanTmtDuplex;
        private System.Windows.Forms.CheckBox chkCleanTmt6Plex;
        private System.Windows.Forms.CheckBox chkSequestDtaOutput;
        private System.Windows.Forms.CheckBox chkOmssaTxtOutput;
        private System.Windows.Forms.CheckBox chkGroupByActivationEnergyTime;
        private System.Windows.Forms.GroupBox grpAssumedPrecursorChargeStateRange;
        private System.Windows.Forms.NumericUpDown numMaximumAssumedPrecursorChargeState;
        private System.Windows.Forms.NumericUpDown numMinimumAssumedPrecursorChargeState;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkEnableEtdPreProcessing;
        private System.Windows.Forms.CheckBox chkCleanItraq8Plex;
        private System.Windows.Forms.CheckBox chkCleanPrecursor;
        private System.Windows.Forms.GroupBox grpPeakFilteringOptions;
        private System.Windows.Forms.GroupBox grpOutputOptions;
        private System.Windows.Forms.CheckBox chkMascotMgfOutput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.GroupBox grpIsobaricLabelCleaning;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox checkedListBox2;
        private System.Windows.Forms.CheckBox includeLogCB;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem clearAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearSelectedToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

