namespace BatchFdrOptimizer
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
            this.fbdRawFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.ofdOmssaCsvFiles = new System.Windows.Forms.OpenFileDialog();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.grpFixedModifications = new System.Windows.Forms.GroupBox();
            this.btnBrowseMods = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnMoveLeft = new System.Windows.Forms.Button();
            this.btnMoveRight = new System.Windows.Forms.Button();
            this.lstSelectedFixedModifications = new System.Windows.Forms.ListBox();
            this.lstAllModifications = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnBrowseOutputFolder = new System.Windows.Forms.Button();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.numMaximumFalseDiscoveryRate = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.chkHigherScoresAreBetter = new System.Windows.Forms.CheckBox();
            this.chkUnique = new System.Windows.Forms.CheckBox();
            this.chkOverallOutputs = new System.Windows.Forms.CheckBox();
            this.chkPhosphopeptideOutputs = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtRawFolder = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.numMaximumPrecursorMassError = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numPrecursorMassErrorIncrement = new System.Windows.Forms.NumericUpDown();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstOmssaCsvFiles = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.prgProgress = new System.Windows.Forms.ProgressBar();
            this.fbdOutputFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.ofdModsXml = new System.Windows.Forms.OpenFileDialog();
            this.pnlMain.SuspendLayout();
            this.grpFixedModifications.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumFalseDiscoveryRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumPrecursorMassError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecursorMassErrorIncrement)).BeginInit();
            this.SuspendLayout();
            // 
            // ofdOmssaCsvFiles
            // 
            this.ofdOmssaCsvFiles.Filter = "OMSSA comma-separated value output files (.csv)|*.csv";
            this.ofdOmssaCsvFiles.Multiselect = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(8, 508);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 38;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.grpFixedModifications);
            this.pnlMain.Controls.Add(this.btnBrowseOutputFolder);
            this.pnlMain.Controls.Add(this.txtOutputFolder);
            this.pnlMain.Controls.Add(this.label3);
            this.pnlMain.Controls.Add(this.label2);
            this.pnlMain.Controls.Add(this.label10);
            this.pnlMain.Controls.Add(this.numMaximumFalseDiscoveryRate);
            this.pnlMain.Controls.Add(this.label8);
            this.pnlMain.Controls.Add(this.chkHigherScoresAreBetter);
            this.pnlMain.Controls.Add(this.chkUnique);
            this.pnlMain.Controls.Add(this.chkOverallOutputs);
            this.pnlMain.Controls.Add(this.chkPhosphopeptideOutputs);
            this.pnlMain.Controls.Add(this.btnBrowse);
            this.pnlMain.Controls.Add(this.txtRawFolder);
            this.pnlMain.Controls.Add(this.label17);
            this.pnlMain.Controls.Add(this.label12);
            this.pnlMain.Controls.Add(this.label14);
            this.pnlMain.Controls.Add(this.numMaximumPrecursorMassError);
            this.pnlMain.Controls.Add(this.label6);
            this.pnlMain.Controls.Add(this.label7);
            this.pnlMain.Controls.Add(this.numPrecursorMassErrorIncrement);
            this.pnlMain.Controls.Add(this.btnClear);
            this.pnlMain.Controls.Add(this.btnRemove);
            this.pnlMain.Controls.Add(this.btnAdd);
            this.pnlMain.Controls.Add(this.btnOK);
            this.pnlMain.Location = new System.Drawing.Point(4, 139);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(471, 534);
            this.pnlMain.TabIndex = 45;
            // 
            // grpFixedModifications
            // 
            this.grpFixedModifications.Controls.Add(this.btnBrowseMods);
            this.grpFixedModifications.Controls.Add(this.label5);
            this.grpFixedModifications.Controls.Add(this.btnMoveLeft);
            this.grpFixedModifications.Controls.Add(this.btnMoveRight);
            this.grpFixedModifications.Controls.Add(this.lstSelectedFixedModifications);
            this.grpFixedModifications.Controls.Add(this.lstAllModifications);
            this.grpFixedModifications.Controls.Add(this.label4);
            this.grpFixedModifications.Location = new System.Drawing.Point(8, 93);
            this.grpFixedModifications.Name = "grpFixedModifications";
            this.grpFixedModifications.Size = new System.Drawing.Size(454, 179);
            this.grpFixedModifications.TabIndex = 172;
            this.grpFixedModifications.TabStop = false;
            this.grpFixedModifications.Text = "Fixed Modifications (* = user modification)";
            // 
            // btnBrowseMods
            // 
            this.btnBrowseMods.Location = new System.Drawing.Point(11, 147);
            this.btnBrowseMods.Name = "btnBrowseMods";
            this.btnBrowseMods.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseMods.TabIndex = 172;
            this.btnBrowseMods.Text = "Browse";
            this.btnBrowseMods.UseVisualStyleBackColor = true;
            this.btnBrowseMods.Click += new System.EventHandler(this.btnBrowseMods_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(244, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 171;
            this.label5.Text = "Selected";
            // 
            // btnMoveLeft
            // 
            this.btnMoveLeft.Location = new System.Drawing.Point(217, 90);
            this.btnMoveLeft.Name = "btnMoveLeft";
            this.btnMoveLeft.Size = new System.Drawing.Size(21, 23);
            this.btnMoveLeft.TabIndex = 170;
            this.btnMoveLeft.Text = "<";
            this.btnMoveLeft.UseVisualStyleBackColor = true;
            this.btnMoveLeft.Click += new System.EventHandler(this.btnMoveLeft_Click);
            // 
            // btnMoveRight
            // 
            this.btnMoveRight.Location = new System.Drawing.Point(217, 61);
            this.btnMoveRight.Name = "btnMoveRight";
            this.btnMoveRight.Size = new System.Drawing.Size(21, 23);
            this.btnMoveRight.TabIndex = 169;
            this.btnMoveRight.Text = ">";
            this.btnMoveRight.UseVisualStyleBackColor = true;
            this.btnMoveRight.Click += new System.EventHandler(this.btnMoveRight_Click);
            // 
            // lstSelectedFixedModifications
            // 
            this.lstSelectedFixedModifications.FormattingEnabled = true;
            this.lstSelectedFixedModifications.Location = new System.Drawing.Point(244, 33);
            this.lstSelectedFixedModifications.Name = "lstSelectedFixedModifications";
            this.lstSelectedFixedModifications.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSelectedFixedModifications.Size = new System.Drawing.Size(200, 108);
            this.lstSelectedFixedModifications.TabIndex = 168;
            // 
            // lstAllModifications
            // 
            this.lstAllModifications.FormattingEnabled = true;
            this.lstAllModifications.Location = new System.Drawing.Point(11, 33);
            this.lstAllModifications.Name = "lstAllModifications";
            this.lstAllModifications.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAllModifications.Size = new System.Drawing.Size(200, 108);
            this.lstAllModifications.Sorted = true;
            this.lstAllModifications.TabIndex = 167;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 13);
            this.label4.TabIndex = 166;
            this.label4.Text = "All";
            // 
            // btnBrowseOutputFolder
            // 
            this.btnBrowseOutputFolder.Location = new System.Drawing.Point(387, 474);
            this.btnBrowseOutputFolder.Name = "btnBrowseOutputFolder";
            this.btnBrowseOutputFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseOutputFolder.TabIndex = 156;
            this.btnBrowseOutputFolder.Text = "Browse";
            this.btnBrowseOutputFolder.UseVisualStyleBackColor = true;
            this.btnBrowseOutputFolder.Click += new System.EventHandler(this.btnBrowseOverallOutputFolder_Click);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(8, 476);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(373, 20);
            this.txtOutputFolder.TabIndex = 155;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 460);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 154;
            this.label3.Text = "Output Folder";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 302);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 153;
            this.label2.Text = "±";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(246, 358);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(15, 13);
            this.label10.TabIndex = 152;
            this.label10.Text = "%";
            // 
            // numMaximumFalseDiscoveryRate
            // 
            this.numMaximumFalseDiscoveryRate.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::BatchFdrOptimizer.Properties.Settings.Default, "maximumFalseDiscoveryRate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numMaximumFalseDiscoveryRate.DecimalPlaces = 2;
            this.numMaximumFalseDiscoveryRate.Location = new System.Drawing.Point(171, 354);
            this.numMaximumFalseDiscoveryRate.Name = "numMaximumFalseDiscoveryRate";
            this.numMaximumFalseDiscoveryRate.Size = new System.Drawing.Size(75, 20);
            this.numMaximumFalseDiscoveryRate.TabIndex = 151;
            this.numMaximumFalseDiscoveryRate.Value = global::BatchFdrOptimizer.Properties.Settings.Default.maximumFalseDiscoveryRate;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(168, 338);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(186, 13);
            this.label8.TabIndex = 150;
            this.label8.Text = "Maximum False Discovery Rate (FDR)";
            // 
            // chkHigherScoresAreBetter
            // 
            this.chkHigherScoresAreBetter.AutoSize = true;
            this.chkHigherScoresAreBetter.Checked = global::BatchFdrOptimizer.Properties.Settings.Default.higherScoresAreBetter;
            this.chkHigherScoresAreBetter.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::BatchFdrOptimizer.Properties.Settings.Default, "higherScoresAreBetter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkHigherScoresAreBetter.Location = new System.Drawing.Point(8, 344);
            this.chkHigherScoresAreBetter.Name = "chkHigherScoresAreBetter";
            this.chkHigherScoresAreBetter.Size = new System.Drawing.Size(142, 17);
            this.chkHigherScoresAreBetter.TabIndex = 149;
            this.chkHigherScoresAreBetter.Text = "Higher Scores are Better";
            this.chkHigherScoresAreBetter.UseVisualStyleBackColor = true;
            // 
            // chkUnique
            // 
            this.chkUnique.AutoSize = true;
            this.chkUnique.Checked = global::BatchFdrOptimizer.Properties.Settings.Default.unique;
            this.chkUnique.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnique.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::BatchFdrOptimizer.Properties.Settings.Default, "unique", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkUnique.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUnique.Location = new System.Drawing.Point(8, 396);
            this.chkUnique.Name = "chkUnique";
            this.chkUnique.Size = new System.Drawing.Size(431, 17);
            this.chkUnique.TabIndex = 141;
            this.chkUnique.Text = "FDR Calculation and Optimization Based on Unique Peptide Sequences";
            this.chkUnique.UseVisualStyleBackColor = true;
            // 
            // chkOverallOutputs
            // 
            this.chkOverallOutputs.AutoSize = true;
            this.chkOverallOutputs.Checked = global::BatchFdrOptimizer.Properties.Settings.Default.overallOutputs;
            this.chkOverallOutputs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOverallOutputs.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::BatchFdrOptimizer.Properties.Settings.Default, "overallOutputs", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkOverallOutputs.Location = new System.Drawing.Point(8, 429);
            this.chkOverallOutputs.Name = "chkOverallOutputs";
            this.chkOverallOutputs.Size = new System.Drawing.Size(99, 17);
            this.chkOverallOutputs.TabIndex = 122;
            this.chkOverallOutputs.Text = "Overall Outputs";
            this.chkOverallOutputs.UseVisualStyleBackColor = true;
            // 
            // chkPhosphopeptideOutputs
            // 
            this.chkPhosphopeptideOutputs.AutoSize = true;
            this.chkPhosphopeptideOutputs.Checked = global::BatchFdrOptimizer.Properties.Settings.Default.phosphopeptideOutputs;
            this.chkPhosphopeptideOutputs.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::BatchFdrOptimizer.Properties.Settings.Default, "phosphopeptideOutputs", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkPhosphopeptideOutputs.Location = new System.Drawing.Point(171, 429);
            this.chkPhosphopeptideOutputs.Name = "chkPhosphopeptideOutputs";
            this.chkPhosphopeptideOutputs.Size = new System.Drawing.Size(143, 17);
            this.chkPhosphopeptideOutputs.TabIndex = 119;
            this.chkPhosphopeptideOutputs.Text = "Phosphopeptide Outputs";
            this.chkPhosphopeptideOutputs.UseVisualStyleBackColor = true;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(387, 58);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 111;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtRawFolder
            // 
            this.txtRawFolder.Location = new System.Drawing.Point(8, 60);
            this.txtRawFolder.Name = "txtRawFolder";
            this.txtRawFolder.Size = new System.Drawing.Size(373, 20);
            this.txtRawFolder.TabIndex = 110;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(5, 44);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(205, 13);
            this.label17.TabIndex = 71;
            this.label17.Text = "Thermo LC-MS/MS Data (.raw) File Folder";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(93, 302);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(27, 13);
            this.label12.TabIndex = 103;
            this.label12.Text = "ppm";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(5, 284);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(152, 13);
            this.label14.TabIndex = 102;
            this.label14.Text = "Maximum Precursor Mass Error";
            // 
            // numMaximumPrecursorMassError
            // 
            this.numMaximumPrecursorMassError.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::BatchFdrOptimizer.Properties.Settings.Default, "maximumPrecursorMassError", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numMaximumPrecursorMassError.DecimalPlaces = 2;
            this.numMaximumPrecursorMassError.Location = new System.Drawing.Point(18, 300);
            this.numMaximumPrecursorMassError.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMaximumPrecursorMassError.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numMaximumPrecursorMassError.Name = "numMaximumPrecursorMassError";
            this.numMaximumPrecursorMassError.Size = new System.Drawing.Size(75, 20);
            this.numMaximumPrecursorMassError.TabIndex = 101;
            this.numMaximumPrecursorMassError.Value = global::BatchFdrOptimizer.Properties.Settings.Default.maximumPrecursorMassError;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(246, 302);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 83;
            this.label6.Text = "ppm";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(168, 284);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(155, 13);
            this.label7.TabIndex = 82;
            this.label7.Text = "Precursor Mass Error Increment";
            // 
            // numPrecursorMassErrorIncrement
            // 
            this.numPrecursorMassErrorIncrement.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::BatchFdrOptimizer.Properties.Settings.Default, "precursorMassErrorIncrement", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numPrecursorMassErrorIncrement.DecimalPlaces = 2;
            this.numPrecursorMassErrorIncrement.Location = new System.Drawing.Point(171, 300);
            this.numPrecursorMassErrorIncrement.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPrecursorMassErrorIncrement.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numPrecursorMassErrorIncrement.Name = "numPrecursorMassErrorIncrement";
            this.numPrecursorMassErrorIncrement.Size = new System.Drawing.Size(75, 20);
            this.numPrecursorMassErrorIncrement.TabIndex = 81;
            this.numPrecursorMassErrorIncrement.Value = global::BatchFdrOptimizer.Properties.Settings.Default.precursorMassErrorIncrement;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(387, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 70;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(197, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 69;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(8, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 68;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstOmssaCsvFiles
            // 
            this.lstOmssaCsvFiles.FormattingEnabled = true;
            this.lstOmssaCsvFiles.HorizontalScrollbar = true;
            this.lstOmssaCsvFiles.Location = new System.Drawing.Point(12, 25);
            this.lstOmssaCsvFiles.Name = "lstOmssaCsvFiles";
            this.lstOmssaCsvFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstOmssaCsvFiles.Size = new System.Drawing.Size(454, 108);
            this.lstOmssaCsvFiles.TabIndex = 69;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(253, 13);
            this.label1.TabIndex = 68;
            this.label1.Text = "OMSSA Comma-Separated Value Output Files (.csv)";
            // 
            // prgProgress
            // 
            this.prgProgress.Location = new System.Drawing.Point(12, 679);
            this.prgProgress.Name = "prgProgress";
            this.prgProgress.Size = new System.Drawing.Size(454, 23);
            this.prgProgress.TabIndex = 70;
            // 
            // ofdModsXml
            // 
            this.ofdModsXml.Filter = "OMSSA mods files (.xml)|*.xml";
            // 
            // frmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 713);
            this.Controls.Add(this.prgProgress);
            this.Controls.Add(this.lstOmssaCsvFiles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Batch FDR Optimizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frmMain_KeyPress);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.grpFixedModifications.ResumeLayout(false);
            this.grpFixedModifications.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumFalseDiscoveryRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumPrecursorMassError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecursorMassErrorIncrement)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog ofdOmssaCsvFiles;
        private System.Windows.Forms.FolderBrowserDialog fbdRawFolder;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox lstOmssaCsvFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar prgProgress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numPrecursorMassErrorIncrement;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numMaximumPrecursorMassError;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtRawFolder;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox chkPhosphopeptideOutputs;
        private System.Windows.Forms.CheckBox chkOverallOutputs;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numMaximumFalseDiscoveryRate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkHigherScoresAreBetter;
        private System.Windows.Forms.CheckBox chkUnique;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowseOutputFolder;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FolderBrowserDialog fbdOutputFolder;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnMoveLeft;
        private System.Windows.Forms.Button btnMoveRight;
        private System.Windows.Forms.ListBox lstSelectedFixedModifications;
        private System.Windows.Forms.ListBox lstAllModifications;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grpFixedModifications;
        private System.Windows.Forms.Button btnBrowseMods;
        private System.Windows.Forms.OpenFileDialog ofdModsXml;
    }
}

