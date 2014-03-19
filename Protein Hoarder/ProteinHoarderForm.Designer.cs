namespace Coon.Compass.ProteinHoarder
{
    partial class ProteinHoarderForm
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
            if (disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProteinHoarderForm));
            this.logGB = new System.Windows.Forms.GroupBox();
            this.logTB = new System.Windows.Forms.RichTextBox();
            this.hoardB = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.inputGB = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.csvclearB = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.csvsB = new System.Windows.Forms.Button();
            this.maxMissedCleavagedUD = new System.Windows.Forms.NumericUpDown();
            this.csvDGV = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.databaseB = new System.Windows.Forms.Button();
            this.databaseTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.optionsGB = new System.Windows.Forms.GroupBox();
            this.proteinsPerMinCB = new System.Windows.Forms.CheckBox();
            this.semiCB = new System.Windows.Forms.CheckBox();
            this.includeUnfliterCB = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.proteinscoringCB = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.minpepspergroupUD = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.fdrUD = new System.Windows.Forms.NumericUpDown();
            this.conservativeCB = new System.Windows.Forms.CheckBox();
            this.outputGB = new System.Windows.Forms.GroupBox();
            this.outputdirectoryB = new System.Windows.Forms.Button();
            this.outputTB = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.quantGB = new System.Windows.Forms.GroupBox();
            this.useNBCCB = new System.Windows.Forms.CheckBox();
            this.medianvalueCB = new System.Windows.Forms.CheckBox();
            this.duplexCB = new System.Windows.Forms.CheckBox();
            this.ignorePepMissingCB = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ignoreModsCLB = new System.Windows.Forms.CheckedListBox();
            this.enableQuantCB = new System.Windows.Forms.CheckBox();
            this.csvD = new System.Windows.Forms.OpenFileDialog();
            this.databaseD = new System.Windows.Forms.OpenFileDialog();
            this.outputD = new System.Windows.Forms.FolderBrowserDialog();
            this.sequenceMapCB = new System.Windows.Forms.CheckBox();
            this.logGB.SuspendLayout();
            this.inputGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxMissedCleavagedUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.csvDGV)).BeginInit();
            this.optionsGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minpepspergroupUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fdrUD)).BeginInit();
            this.outputGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.quantGB.SuspendLayout();
            this.SuspendLayout();
            // 
            // logGB
            // 
            this.logGB.Controls.Add(this.logTB);
            this.logGB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logGB.Location = new System.Drawing.Point(3, 3);
            this.logGB.Name = "logGB";
            this.logGB.Padding = new System.Windows.Forms.Padding(5);
            this.logGB.Size = new System.Drawing.Size(794, 163);
            this.logGB.TabIndex = 0;
            this.logGB.TabStop = false;
            this.logGB.Text = "Log";
            // 
            // logTB
            // 
            this.logTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTB.Location = new System.Drawing.Point(5, 18);
            this.logTB.Margin = new System.Windows.Forms.Padding(5);
            this.logTB.Name = "logTB";
            this.logTB.ReadOnly = true;
            this.logTB.Size = new System.Drawing.Size(784, 140);
            this.logTB.TabIndex = 0;
            this.logTB.Text = "";
            this.logTB.WordWrap = false;
            // 
            // hoardB
            // 
            this.hoardB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.hoardB.Location = new System.Drawing.Point(726, 469);
            this.hoardB.Name = "hoardB";
            this.hoardB.Size = new System.Drawing.Size(66, 23);
            this.hoardB.TabIndex = 1;
            this.hoardB.Text = "Hoard";
            this.hoardB.UseVisualStyleBackColor = true;
            this.hoardB.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(5, 469);
            this.progressBar.Maximum = 10000;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(708, 23);
            this.progressBar.TabIndex = 2;
            // 
            // inputGB
            // 
            this.inputGB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputGB.Controls.Add(this.label7);
            this.inputGB.Controls.Add(this.csvclearB);
            this.inputGB.Controls.Add(this.comboBox1);
            this.inputGB.Controls.Add(this.csvsB);
            this.inputGB.Controls.Add(this.maxMissedCleavagedUD);
            this.inputGB.Controls.Add(this.csvDGV);
            this.inputGB.Controls.Add(this.label5);
            this.inputGB.Controls.Add(this.databaseB);
            this.inputGB.Controls.Add(this.databaseTB);
            this.inputGB.Controls.Add(this.label1);
            this.inputGB.Location = new System.Drawing.Point(3, 3);
            this.inputGB.Name = "inputGB";
            this.inputGB.Size = new System.Drawing.Size(794, 227);
            this.inputGB.TabIndex = 3;
            this.inputGB.TabStop = false;
            this.inputGB.Text = "Input Files";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(200, 201);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Database Type";
            // 
            // csvclearB
            // 
            this.csvclearB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.csvclearB.Location = new System.Drawing.Point(723, 48);
            this.csvclearB.Name = "csvclearB";
            this.csvclearB.Size = new System.Drawing.Size(66, 23);
            this.csvclearB.TabIndex = 7;
            this.csvclearB.Text = "Clear";
            this.csvclearB.UseVisualStyleBackColor = true;
            this.csvclearB.Click += new System.EventHandler(this.csvclearB_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(282, 197);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(63, 21);
            this.comboBox1.TabIndex = 15;
            // 
            // csvsB
            // 
            this.csvsB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.csvsB.Location = new System.Drawing.Point(722, 19);
            this.csvsB.Name = "csvsB";
            this.csvsB.Size = new System.Drawing.Size(66, 23);
            this.csvsB.TabIndex = 6;
            this.csvsB.Text = "Browse";
            this.csvsB.UseVisualStyleBackColor = true;
            this.csvsB.Click += new System.EventHandler(this.csvsB_Click);
            // 
            // maxMissedCleavagedUD
            // 
            this.maxMissedCleavagedUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.maxMissedCleavagedUD.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Coon.Compass.ProteinHoarder.Properties.Settings.Default, "maxMissedCleavages", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.maxMissedCleavagedUD.Location = new System.Drawing.Point(148, 197);
            this.maxMissedCleavagedUD.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.maxMissedCleavagedUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxMissedCleavagedUD.Name = "maxMissedCleavagedUD";
            this.maxMissedCleavagedUD.Size = new System.Drawing.Size(46, 20);
            this.maxMissedCleavagedUD.TabIndex = 9;
            this.maxMissedCleavagedUD.Value = global::Coon.Compass.ProteinHoarder.Properties.Settings.Default.maxMissedCleavages;
            // 
            // csvDGV
            // 
            this.csvDGV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.csvDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.csvDGV.Location = new System.Drawing.Point(9, 19);
            this.csvDGV.Name = "csvDGV";
            this.csvDGV.Size = new System.Drawing.Size(707, 173);
            this.csvDGV.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(134, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Maximum Missed Clevages";
            // 
            // databaseB
            // 
            this.databaseB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseB.Location = new System.Drawing.Point(723, 196);
            this.databaseB.Name = "databaseB";
            this.databaseB.Size = new System.Drawing.Size(66, 23);
            this.databaseB.TabIndex = 4;
            this.databaseB.Text = "Browse";
            this.databaseB.UseVisualStyleBackColor = true;
            this.databaseB.Click += new System.EventHandler(this.databaseB_Click);
            // 
            // databaseTB
            // 
            this.databaseTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseTB.Location = new System.Drawing.Point(461, 197);
            this.databaseTB.Name = "databaseTB";
            this.databaseTB.Size = new System.Drawing.Size(255, 20);
            this.databaseTB.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(351, 201);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Database file (.fasta)";
            // 
            // optionsGB
            // 
            this.optionsGB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.optionsGB.Controls.Add(this.sequenceMapCB);
            this.optionsGB.Controls.Add(this.proteinsPerMinCB);
            this.optionsGB.Controls.Add(this.semiCB);
            this.optionsGB.Controls.Add(this.includeUnfliterCB);
            this.optionsGB.Controls.Add(this.label4);
            this.optionsGB.Controls.Add(this.proteinscoringCB);
            this.optionsGB.Controls.Add(this.label3);
            this.optionsGB.Controls.Add(this.minpepspergroupUD);
            this.optionsGB.Controls.Add(this.label2);
            this.optionsGB.Controls.Add(this.fdrUD);
            this.optionsGB.Controls.Add(this.conservativeCB);
            this.optionsGB.Location = new System.Drawing.Point(5, 287);
            this.optionsGB.Name = "optionsGB";
            this.optionsGB.Size = new System.Drawing.Size(276, 176);
            this.optionsGB.TabIndex = 4;
            this.optionsGB.TabStop = false;
            this.optionsGB.Text = "Options";
            // 
            // proteinsPerMinCB
            // 
            this.proteinsPerMinCB.AutoSize = true;
            this.proteinsPerMinCB.Checked = global::Coon.Compass.ProteinHoarder.Properties.Settings.Default.proteinsPerMinute;
            this.proteinsPerMinCB.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.ProteinHoarder.Properties.Settings.Default, "proteinsPerMinute", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.proteinsPerMinCB.Location = new System.Drawing.Point(149, 135);
            this.proteinsPerMinCB.Name = "proteinsPerMinCB";
            this.proteinsPerMinCB.Size = new System.Drawing.Size(117, 17);
            this.proteinsPerMinCB.TabIndex = 14;
            this.proteinsPerMinCB.Text = "Proteins per Minute";
            this.proteinsPerMinCB.UseVisualStyleBackColor = true;
            // 
            // semiCB
            // 
            this.semiCB.AutoSize = true;
            this.semiCB.Checked = global::Coon.Compass.ProteinHoarder.Properties.Settings.Default.semiDigestion;
            this.semiCB.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.ProteinHoarder.Properties.Settings.Default, "semiDigestion", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.semiCB.Location = new System.Drawing.Point(8, 135);
            this.semiCB.Name = "semiCB";
            this.semiCB.Size = new System.Drawing.Size(135, 17);
            this.semiCB.TabIndex = 13;
            this.semiCB.Text = "Perform Semi-Digestion";
            this.semiCB.UseVisualStyleBackColor = true;
            // 
            // includeUnfliterCB
            // 
            this.includeUnfliterCB.AutoSize = true;
            this.includeUnfliterCB.Checked = global::Coon.Compass.ProteinHoarder.Properties.Settings.Default.includeUnfilterResults;
            this.includeUnfliterCB.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.ProteinHoarder.Properties.Settings.Default, "includeUnfilterResults", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.includeUnfliterCB.Location = new System.Drawing.Point(26, 114);
            this.includeUnfliterCB.Name = "includeUnfliterCB";
            this.includeUnfliterCB.Size = new System.Drawing.Size(219, 17);
            this.includeUnfliterCB.TabIndex = 5;
            this.includeUnfliterCB.Text = "Write Unfiltered Protein Groups to Output";
            this.includeUnfliterCB.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Protein Scoring Method";
            // 
            // proteinscoringCB
            // 
            this.proteinscoringCB.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.proteinscoringCB.DataBindings.Add(new System.Windows.Forms.Binding("Name", global::Coon.Compass.ProteinHoarder.Properties.Settings.Default, "ProteinScoringValue", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.proteinscoringCB.FormattingEnabled = true;
            this.proteinscoringCB.Location = new System.Drawing.Point(130, 18);
            this.proteinscoringCB.Name = global::Coon.Compass.ProteinHoarder.Properties.Settings.Default.ProteinScoringValue;
            this.proteinscoringCB.Size = new System.Drawing.Size(140, 21);
            this.proteinscoringCB.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Minimum Peptides per Protein";
            // 
            // minpepspergroupUD
            // 
            this.minpepspergroupUD.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Coon.Compass.ProteinHoarder.Properties.Settings.Default, "minPeptidesPerProteinGroup", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.minpepspergroupUD.Location = new System.Drawing.Point(224, 67);
            this.minpepspergroupUD.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.minpepspergroupUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.minpepspergroupUD.Name = "minpepspergroupUD";
            this.minpepspergroupUD.Size = new System.Drawing.Size(46, 20);
            this.minpepspergroupUD.TabIndex = 6;
            this.minpepspergroupUD.Value = global::Coon.Compass.ProteinHoarder.Properties.Settings.Default.minPeptidesPerProteinGroup;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Maximum FDR (%)";
            // 
            // fdrUD
            // 
            this.fdrUD.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Coon.Compass.ProteinHoarder.Properties.Settings.Default, "maxFDR", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.fdrUD.DecimalPlaces = 2;
            this.fdrUD.Location = new System.Drawing.Point(224, 93);
            this.fdrUD.Name = "fdrUD";
            this.fdrUD.Size = new System.Drawing.Size(46, 20);
            this.fdrUD.TabIndex = 5;
            this.fdrUD.Value = global::Coon.Compass.ProteinHoarder.Properties.Settings.Default.maxFDR;
            // 
            // conservativeCB
            // 
            this.conservativeCB.AutoSize = true;
            this.conservativeCB.Checked = global::Coon.Compass.ProteinHoarder.Properties.Settings.Default.conservativePScore;
            this.conservativeCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.conservativeCB.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.ProteinHoarder.Properties.Settings.Default, "conservativePScore", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.conservativeCB.Location = new System.Drawing.Point(26, 45);
            this.conservativeCB.Name = "conservativeCB";
            this.conservativeCB.Size = new System.Drawing.Size(229, 17);
            this.conservativeCB.TabIndex = 5;
            this.conservativeCB.Text = "Conservative Protein P-Value (Use Lowest)";
            this.conservativeCB.UseVisualStyleBackColor = true;
            // 
            // outputGB
            // 
            this.outputGB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputGB.Controls.Add(this.outputdirectoryB);
            this.outputGB.Controls.Add(this.outputTB);
            this.outputGB.Location = new System.Drawing.Point(5, 236);
            this.outputGB.Name = "outputGB";
            this.outputGB.Size = new System.Drawing.Size(794, 45);
            this.outputGB.TabIndex = 8;
            this.outputGB.TabStop = false;
            this.outputGB.Text = "Output Location";
            // 
            // outputdirectoryB
            // 
            this.outputdirectoryB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputdirectoryB.Location = new System.Drawing.Point(723, 15);
            this.outputdirectoryB.Name = "outputdirectoryB";
            this.outputdirectoryB.Size = new System.Drawing.Size(66, 23);
            this.outputdirectoryB.TabIndex = 8;
            this.outputdirectoryB.Text = "Browse";
            this.outputdirectoryB.UseVisualStyleBackColor = true;
            this.outputdirectoryB.Click += new System.EventHandler(this.outputdirectoryB_Click);
            // 
            // outputTB
            // 
            this.outputTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTB.Location = new System.Drawing.Point(9, 16);
            this.outputTB.Name = "outputTB";
            this.outputTB.Size = new System.Drawing.Size(707, 20);
            this.outputTB.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.quantGB);
            this.splitContainer1.Panel1.Controls.Add(this.progressBar);
            this.splitContainer1.Panel1.Controls.Add(this.inputGB);
            this.splitContainer1.Panel1.Controls.Add(this.outputGB);
            this.splitContainer1.Panel1.Controls.Add(this.hoardB);
            this.splitContainer1.Panel1.Controls.Add(this.optionsGB);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.logGB);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainer1.Size = new System.Drawing.Size(804, 676);
            this.splitContainer1.SplitterDistance = 499;
            this.splitContainer1.TabIndex = 9;
            // 
            // quantGB
            // 
            this.quantGB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.quantGB.Controls.Add(this.useNBCCB);
            this.quantGB.Controls.Add(this.medianvalueCB);
            this.quantGB.Controls.Add(this.duplexCB);
            this.quantGB.Controls.Add(this.ignorePepMissingCB);
            this.quantGB.Controls.Add(this.label6);
            this.quantGB.Controls.Add(this.ignoreModsCLB);
            this.quantGB.Controls.Add(this.enableQuantCB);
            this.quantGB.Location = new System.Drawing.Point(287, 287);
            this.quantGB.Name = "quantGB";
            this.quantGB.Size = new System.Drawing.Size(510, 176);
            this.quantGB.TabIndex = 9;
            this.quantGB.TabStop = false;
            this.quantGB.Text = "Protein Quantitation";
            // 
            // useNBCCB
            // 
            this.useNBCCB.AutoSize = true;
            this.useNBCCB.Enabled = false;
            this.useNBCCB.Location = new System.Drawing.Point(149, 153);
            this.useNBCCB.Name = "useNBCCB";
            this.useNBCCB.Size = new System.Drawing.Size(151, 17);
            this.useNBCCB.TabIndex = 15;
            this.useNBCCB.Text = "Use NBC only if necessary";
            this.useNBCCB.UseVisualStyleBackColor = true;
            // 
            // medianvalueCB
            // 
            this.medianvalueCB.AutoSize = true;
            this.medianvalueCB.Enabled = false;
            this.medianvalueCB.Location = new System.Drawing.Point(25, 153);
            this.medianvalueCB.Name = "medianvalueCB";
            this.medianvalueCB.Size = new System.Drawing.Size(118, 17);
            this.medianvalueCB.TabIndex = 13;
            this.medianvalueCB.Text = "Use Median Values";
            this.medianvalueCB.UseVisualStyleBackColor = true;
            // 
            // duplexCB
            // 
            this.duplexCB.AutoSize = true;
            this.duplexCB.Enabled = false;
            this.duplexCB.Location = new System.Drawing.Point(6, 130);
            this.duplexCB.Name = "duplexCB";
            this.duplexCB.Size = new System.Drawing.Size(119, 17);
            this.duplexCB.TabIndex = 14;
            this.duplexCB.Text = "Duplex Quantitation";
            this.duplexCB.UseVisualStyleBackColor = true;
            this.duplexCB.CheckedChanged += new System.EventHandler(this.duplexCB_CheckedChanged);
            // 
            // ignorePepMissingCB
            // 
            this.ignorePepMissingCB.AutoSize = true;
            this.ignorePepMissingCB.Checked = true;
            this.ignorePepMissingCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ignorePepMissingCB.Enabled = false;
            this.ignorePepMissingCB.Location = new System.Drawing.Point(243, 40);
            this.ignorePepMissingCB.Name = "ignorePepMissingCB";
            this.ignorePepMissingCB.Size = new System.Drawing.Size(186, 17);
            this.ignorePepMissingCB.TabIndex = 12;
            this.ignorePepMissingCB.Text = "Ignore Peptides with Missing Data";
            this.ignorePepMissingCB.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(215, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Ignore Peptides with Following Modifications";
            // 
            // ignoreModsCLB
            // 
            this.ignoreModsCLB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ignoreModsCLB.CheckOnClick = true;
            this.ignoreModsCLB.Enabled = false;
            this.ignoreModsCLB.FormattingEnabled = true;
            this.ignoreModsCLB.Location = new System.Drawing.Point(25, 60);
            this.ignoreModsCLB.Name = "ignoreModsCLB";
            this.ignoreModsCLB.Size = new System.Drawing.Size(478, 64);
            this.ignoreModsCLB.TabIndex = 1;
            // 
            // enableQuantCB
            // 
            this.enableQuantCB.AutoSize = true;
            this.enableQuantCB.Location = new System.Drawing.Point(6, 21);
            this.enableQuantCB.Name = "enableQuantCB";
            this.enableQuantCB.Size = new System.Drawing.Size(125, 17);
            this.enableQuantCB.TabIndex = 0;
            this.enableQuantCB.Text = "Enabled Quantitation";
            this.enableQuantCB.UseVisualStyleBackColor = true;
            this.enableQuantCB.CheckedChanged += new System.EventHandler(this.enableQuantCB_CheckedChanged);
            // 
            // csvD
            // 
            this.csvD.Filter = "Omssa CSV|*.csv";
            this.csvD.InitialDirectory = global::Coon.Compass.ProteinHoarder.Properties.Settings.Default.csvDirectory;
            this.csvD.Multiselect = true;
            this.csvD.Title = "Select csvs to herd";
            // 
            // databaseD
            // 
            this.databaseD.Filter = "FASTA|*.fa*|All Files|*.*";
            this.databaseD.InitialDirectory = global::Coon.Compass.ProteinHoarder.Properties.Settings.Default.databaseDirectory;
            this.databaseD.Title = "Select the protein database used to search the data with";
            // 
            // outputD
            // 
            this.outputD.Description = "Select the Output Directory";
            this.outputD.SelectedPath = global::Coon.Compass.ProteinHoarder.Properties.Settings.Default.outputDirectory;
            // 
            // sequenceMapCB
            // 
            this.sequenceMapCB.AutoSize = true;
            this.sequenceMapCB.Checked = global::Coon.Compass.ProteinHoarder.Properties.Settings.Default.coverageMap;
            this.sequenceMapCB.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Coon.Compass.ProteinHoarder.Properties.Settings.Default, "coverageMap", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sequenceMapCB.Location = new System.Drawing.Point(8, 153);
            this.sequenceMapCB.Name = "sequenceMapCB";
            this.sequenceMapCB.Size = new System.Drawing.Size(153, 17);
            this.sequenceMapCB.TabIndex = 15;
            this.sequenceMapCB.Text = "Sequence Coverage Maps";
            this.sequenceMapCB.UseVisualStyleBackColor = true;
            // 
            // ProteinHoarderForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 676);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(600, 480);
            this.Name = "ProteinHoarderForm";
            this.Text = "Protein Hoarder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.logGB.ResumeLayout(false);
            this.inputGB.ResumeLayout(false);
            this.inputGB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxMissedCleavagedUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.csvDGV)).EndInit();
            this.optionsGB.ResumeLayout(false);
            this.optionsGB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minpepspergroupUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fdrUD)).EndInit();
            this.outputGB.ResumeLayout(false);
            this.outputGB.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.quantGB.ResumeLayout(false);
            this.quantGB.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox logGB;
        private System.Windows.Forms.RichTextBox logTB;
        private System.Windows.Forms.Button hoardB;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.GroupBox inputGB;
        private System.Windows.Forms.Button csvclearB;
        private System.Windows.Forms.Button csvsB;
        private System.Windows.Forms.DataGridView csvDGV;
        private System.Windows.Forms.Button databaseB;
        private System.Windows.Forms.TextBox databaseTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox optionsGB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown minpepspergroupUD;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown fdrUD;
        private System.Windows.Forms.CheckBox conservativeCB;
        private System.Windows.Forms.GroupBox outputGB;
        private System.Windows.Forms.Button outputdirectoryB;
        private System.Windows.Forms.TextBox outputTB;
        private System.Windows.Forms.FolderBrowserDialog outputD;
        private System.Windows.Forms.OpenFileDialog databaseD;
        private System.Windows.Forms.OpenFileDialog csvD;
        private System.Windows.Forms.NumericUpDown maxMissedCleavagedUD;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox proteinscoringCB;
        private System.Windows.Forms.GroupBox quantGB;
        private System.Windows.Forms.CheckBox enableQuantCB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckedListBox ignoreModsCLB;
        private System.Windows.Forms.CheckBox includeUnfliterCB;
        private System.Windows.Forms.CheckBox ignorePepMissingCB;
        private System.Windows.Forms.CheckBox semiCB;
        private System.Windows.Forms.CheckBox proteinsPerMinCB;
        private System.Windows.Forms.CheckBox medianvalueCB;
        private System.Windows.Forms.CheckBox duplexCB;
        private System.Windows.Forms.CheckBox useNBCCB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox sequenceMapCB;
    }
}

