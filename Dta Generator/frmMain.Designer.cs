namespace DtaGenerator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.fbdOutput = new System.Windows.Forms.FolderBrowserDialog();
            this.prgProgress = new System.Windows.Forms.ProgressBar();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstRawFiles = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.grpOutputOptions = new System.Windows.Forms.GroupBox();
            this.chkMascotMgfOutput = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.chkSequestDtaOutput = new System.Windows.Forms.CheckBox();
            this.chkOmssaTxtOutput = new System.Windows.Forms.CheckBox();
            this.chkGroupByActivationEnergyTime = new System.Windows.Forms.CheckBox();
            this.grpPeakFilteringOptions = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.grpIsobaricLabelCleaning = new System.Windows.Forms.GroupBox();
            this.chkCleanItraq8Plex = new System.Windows.Forms.CheckBox();
            this.chkCleanTmt6Plex = new System.Windows.Forms.CheckBox();
            this.chkCleanTmtDuplex = new System.Windows.Forms.CheckBox();
            this.chkCleanItraq4Plex = new System.Windows.Forms.CheckBox();
            this.chkCleanPrecursor = new System.Windows.Forms.CheckBox();
            this.chkEnableEtdPreProcessing = new System.Windows.Forms.CheckBox();
            this.grpAssumedPrecursorChargeStateRange = new System.Windows.Forms.GroupBox();
            this.numMaximumAssumedPrecursorChargeState = new System.Windows.Forms.NumericUpDown();
            this.numMinimumAssumedPrecursorChargeState = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ofdRawFiles = new System.Windows.Forms.OpenFileDialog();
            this.pnlMain.SuspendLayout();
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
            this.prgProgress.Location = new System.Drawing.Point(12, 614);
            this.prgProgress.Name = "prgProgress";
            this.prgProgress.Size = new System.Drawing.Size(438, 23);
            this.prgProgress.TabIndex = 21;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(9, 394);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 38;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(372, 2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 35;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(183, 2);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 34;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(9, 2);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 33;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstRawFiles
            // 
            this.lstRawFiles.FormattingEnabled = true;
            this.lstRawFiles.HorizontalScrollbar = true;
            this.lstRawFiles.Location = new System.Drawing.Point(12, 25);
            this.lstRawFiles.Name = "lstRawFiles";
            this.lstRawFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstRawFiles.Size = new System.Drawing.Size(438, 160);
            this.lstRawFiles.TabIndex = 32;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(177, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Thermo .raw MS/MS Data Filepaths";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.grpOutputOptions);
            this.pnlMain.Controls.Add(this.grpPeakFilteringOptions);
            this.pnlMain.Controls.Add(this.grpAssumedPrecursorChargeStateRange);
            this.pnlMain.Controls.Add(this.btnOK);
            this.pnlMain.Controls.Add(this.btnClear);
            this.pnlMain.Controls.Add(this.btnRemove);
            this.pnlMain.Controls.Add(this.btnAdd);
            this.pnlMain.Location = new System.Drawing.Point(3, 188);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(454, 420);
            this.pnlMain.TabIndex = 45;
            // 
            // grpOutputOptions
            // 
            this.grpOutputOptions.Controls.Add(this.chkMascotMgfOutput);
            this.grpOutputOptions.Controls.Add(this.label2);
            this.grpOutputOptions.Controls.Add(this.btnBrowse);
            this.grpOutputOptions.Controls.Add(this.txtOutputFolder);
            this.grpOutputOptions.Controls.Add(this.chkSequestDtaOutput);
            this.grpOutputOptions.Controls.Add(this.chkOmssaTxtOutput);
            this.grpOutputOptions.Controls.Add(this.chkGroupByActivationEnergyTime);
            this.grpOutputOptions.Location = new System.Drawing.Point(9, 268);
            this.grpOutputOptions.Name = "grpOutputOptions";
            this.grpOutputOptions.Size = new System.Drawing.Size(438, 120);
            this.grpOutputOptions.TabIndex = 108;
            this.grpOutputOptions.TabStop = false;
            this.grpOutputOptions.Text = "Output Options";
            // 
            // chkMascotMgfOutput
            // 
            this.chkMascotMgfOutput.AutoSize = true;
            this.chkMascotMgfOutput.Checked = global::DtaGenerator.Properties.Settings.Default.mascotMgfOutput;
            this.chkMascotMgfOutput.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::DtaGenerator.Properties.Settings.Default, "mascotMgfOutput", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkMascotMgfOutput.Location = new System.Drawing.Point(313, 51);
            this.chkMascotMgfOutput.Name = "chkMascotMgfOutput";
            this.chkMascotMgfOutput.Size = new System.Drawing.Size(119, 17);
            this.chkMascotMgfOutput.TabIndex = 106;
            this.chkMascotMgfOutput.Text = "Mascot .mgf Output";
            this.chkMascotMgfOutput.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 105;
            this.label2.Text = "Output Folder";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(357, 92);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 103;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(6, 94);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(345, 20);
            this.txtOutputFolder.TabIndex = 104;
            // 
            // chkSequestDtaOutput
            // 
            this.chkSequestDtaOutput.AutoSize = true;
            this.chkSequestDtaOutput.Checked = global::DtaGenerator.Properties.Settings.Default.sequestDtaOutput;
            this.chkSequestDtaOutput.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::DtaGenerator.Properties.Settings.Default, "sequestDtaOutput", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.chkOmssaTxtOutput.Checked = global::DtaGenerator.Properties.Settings.Default.omssaTxtOutput;
            this.chkOmssaTxtOutput.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOmssaTxtOutput.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::DtaGenerator.Properties.Settings.Default, "omssaTxtOutput", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkOmssaTxtOutput.Location = new System.Drawing.Point(163, 51);
            this.chkOmssaTxtOutput.Name = "chkOmssaTxtOutput";
            this.chkOmssaTxtOutput.Size = new System.Drawing.Size(116, 17);
            this.chkOmssaTxtOutput.TabIndex = 101;
            this.chkOmssaTxtOutput.Text = "OMSSA .txt Output";
            this.chkOmssaTxtOutput.UseVisualStyleBackColor = true;
            // 
            // chkGroupByActivationEnergyTime
            // 
            this.chkGroupByActivationEnergyTime.AutoSize = true;
            this.chkGroupByActivationEnergyTime.Checked = global::DtaGenerator.Properties.Settings.Default.groupByActivationEnergyTime;
            this.chkGroupByActivationEnergyTime.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::DtaGenerator.Properties.Settings.Default, "groupByActivationEnergyTime", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkGroupByActivationEnergyTime.Location = new System.Drawing.Point(9, 22);
            this.chkGroupByActivationEnergyTime.Name = "chkGroupByActivationEnergyTime";
            this.chkGroupByActivationEnergyTime.Size = new System.Drawing.Size(189, 17);
            this.chkGroupByActivationEnergyTime.TabIndex = 100;
            this.chkGroupByActivationEnergyTime.Text = "Group by Activation Energy / Time";
            this.chkGroupByActivationEnergyTime.UseVisualStyleBackColor = true;
            // 
            // grpPeakFilteringOptions
            // 
            this.grpPeakFilteringOptions.Controls.Add(this.groupBox1);
            this.grpPeakFilteringOptions.Controls.Add(this.linkLabel1);
            this.grpPeakFilteringOptions.Controls.Add(this.grpIsobaricLabelCleaning);
            this.grpPeakFilteringOptions.Controls.Add(this.chkCleanPrecursor);
            this.grpPeakFilteringOptions.Controls.Add(this.chkEnableEtdPreProcessing);
            this.grpPeakFilteringOptions.Location = new System.Drawing.Point(9, 101);
            this.grpPeakFilteringOptions.Name = "grpPeakFilteringOptions";
            this.grpPeakFilteringOptions.Size = new System.Drawing.Size(438, 161);
            this.grpPeakFilteringOptions.TabIndex = 106;
            this.grpPeakFilteringOptions.TabStop = false;
            this.grpPeakFilteringOptions.Text = "Peak Filtering Options";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkedListBox2);
            this.groupBox1.Controls.Add(this.checkedListBox1);
            this.groupBox1.Location = new System.Drawing.Point(202, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 102);
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
            this.checkedListBox2.Size = new System.Drawing.Size(224, 83);
            this.checkedListBox2.TabIndex = 95;
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(239, 16);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(120, 94);
            this.checkedListBox1.TabIndex = 94;
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
            this.chkCleanItraq8Plex.Checked = global::DtaGenerator.Properties.Settings.Default.cleanItraq8Plex;
            this.chkCleanItraq8Plex.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::DtaGenerator.Properties.Settings.Default, "cleanItraq8Plex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.chkCleanTmt6Plex.Checked = global::DtaGenerator.Properties.Settings.Default.cleanTmt6Plex;
            this.chkCleanTmt6Plex.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::DtaGenerator.Properties.Settings.Default, "cleanTmt6Plex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.chkCleanTmtDuplex.Checked = global::DtaGenerator.Properties.Settings.Default.cleanTmtDuplex;
            this.chkCleanTmtDuplex.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::DtaGenerator.Properties.Settings.Default, "cleanTmtDuplex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.chkCleanItraq4Plex.Checked = global::DtaGenerator.Properties.Settings.Default.cleanItraq4Plex;
            this.chkCleanItraq4Plex.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::DtaGenerator.Properties.Settings.Default, "cleanItraq4Plex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkCleanItraq4Plex.Location = new System.Drawing.Point(98, 22);
            this.chkCleanItraq4Plex.Name = "chkCleanItraq4Plex";
            this.chkCleanItraq4Plex.Size = new System.Drawing.Size(89, 17);
            this.chkCleanItraq4Plex.TabIndex = 91;
            this.chkCleanItraq4Plex.Text = "iTRAQ 4-plex";
            this.chkCleanItraq4Plex.UseVisualStyleBackColor = true;
            this.chkCleanItraq4Plex.CheckedChanged += new System.EventHandler(this.chkCleanItraq4Plex_CheckedChanged);
            // 
            // chkCleanPrecursor
            // 
            this.chkCleanPrecursor.AutoSize = true;
            this.chkCleanPrecursor.Checked = global::DtaGenerator.Properties.Settings.Default.cleanPrecursor;
            this.chkCleanPrecursor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCleanPrecursor.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::DtaGenerator.Properties.Settings.Default, "cleanPrecursor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.chkEnableEtdPreProcessing.Checked = global::DtaGenerator.Properties.Settings.Default.enableEtdPreProcessing;
            this.chkEnableEtdPreProcessing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableEtdPreProcessing.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::DtaGenerator.Properties.Settings.Default, "enableEtdPreProcessing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkEnableEtdPreProcessing.Location = new System.Drawing.Point(144, 25);
            this.chkEnableEtdPreProcessing.Name = "chkEnableEtdPreProcessing";
            this.chkEnableEtdPreProcessing.Size = new System.Drawing.Size(158, 17);
            this.chkEnableEtdPreProcessing.TabIndex = 103;
            this.chkEnableEtdPreProcessing.Text = "Enable ETD Pre-Processing";
            this.chkEnableEtdPreProcessing.UseVisualStyleBackColor = true;
            // 
            // grpAssumedPrecursorChargeStateRange
            // 
            this.grpAssumedPrecursorChargeStateRange.Controls.Add(this.numMaximumAssumedPrecursorChargeState);
            this.grpAssumedPrecursorChargeStateRange.Controls.Add(this.numMinimumAssumedPrecursorChargeState);
            this.grpAssumedPrecursorChargeStateRange.Controls.Add(this.label6);
            this.grpAssumedPrecursorChargeStateRange.Controls.Add(this.label5);
            this.grpAssumedPrecursorChargeStateRange.Location = new System.Drawing.Point(9, 41);
            this.grpAssumedPrecursorChargeStateRange.Name = "grpAssumedPrecursorChargeStateRange";
            this.grpAssumedPrecursorChargeStateRange.Size = new System.Drawing.Size(438, 54);
            this.grpAssumedPrecursorChargeStateRange.TabIndex = 94;
            this.grpAssumedPrecursorChargeStateRange.TabStop = false;
            this.grpAssumedPrecursorChargeStateRange.Text = "Assumed Precursor Charge State Range";
            // 
            // numMaximumAssumedPrecursorChargeState
            // 
            this.numMaximumAssumedPrecursorChargeState.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::DtaGenerator.Properties.Settings.Default, "maximumAssumedPrecursorChargeState", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numMaximumAssumedPrecursorChargeState.Location = new System.Drawing.Point(183, 23);
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
            this.numMaximumAssumedPrecursorChargeState.Value = global::DtaGenerator.Properties.Settings.Default.maximumAssumedPrecursorChargeState;
            // 
            // numMinimumAssumedPrecursorChargeState
            // 
            this.numMinimumAssumedPrecursorChargeState.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::DtaGenerator.Properties.Settings.Default, "minimumAssumedPrecursorChargeState", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numMinimumAssumedPrecursorChargeState.Location = new System.Drawing.Point(63, 23);
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
            this.numMinimumAssumedPrecursorChargeState.Value = global::DtaGenerator.Properties.Settings.Default.minimumAssumedPrecursorChargeState;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(126, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Maximum";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Minimum";
            // 
            // ofdRawFiles
            // 
            this.ofdRawFiles.Filter = "Thermo .raw data files (*.raw)|*.raw|MzML File (.mzML, .xml)|*.mzml;*.xml";
            this.ofdRawFiles.Multiselect = true;
            // 
            // frmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 649);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.lstRawFiles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.prgProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "DTA Generator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
            this.pnlMain.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox lstRawFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlMain;
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
        private System.Windows.Forms.CheckedListBox checkedListBox1;
    }
}

