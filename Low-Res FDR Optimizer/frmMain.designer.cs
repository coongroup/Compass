namespace LowResFdrOptimizer
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
            this.ofdOmssaCsvFiles = new System.Windows.Forms.OpenFileDialog();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.btnBrowseOutputFolder = new System.Windows.Forms.Button();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkUnique = new System.Windows.Forms.CheckBox();
            this.chkOverallOutputs = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.chkPhosphopeptideOutputs = new System.Windows.Forms.CheckBox();
            this.chkHigherScoresAreBetter = new System.Windows.Forms.CheckBox();
            this.numMaximumFalseDiscoveryRate = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstOmssaCsvFiles = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fbdOutputFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.prgProgress = new System.Windows.Forms.ProgressBar();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumFalseDiscoveryRate)).BeginInit();
            this.SuspendLayout();
            // 
            // ofdOmssaCsvFiles
            // 
            this.ofdOmssaCsvFiles.Filter = "OMSSA comma-separated value output files (.csv)|*.csv";
            this.ofdOmssaCsvFiles.Multiselect = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(8, 216);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 38;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.btnBrowseOutputFolder);
            this.pnlMain.Controls.Add(this.txtOutputFolder);
            this.pnlMain.Controls.Add(this.label3);
            this.pnlMain.Controls.Add(this.chkUnique);
            this.pnlMain.Controls.Add(this.chkOverallOutputs);
            this.pnlMain.Controls.Add(this.label10);
            this.pnlMain.Controls.Add(this.chkPhosphopeptideOutputs);
            this.pnlMain.Controls.Add(this.chkHigherScoresAreBetter);
            this.pnlMain.Controls.Add(this.numMaximumFalseDiscoveryRate);
            this.pnlMain.Controls.Add(this.label8);
            this.pnlMain.Controls.Add(this.btnClear);
            this.pnlMain.Controls.Add(this.btnRemove);
            this.pnlMain.Controls.Add(this.btnAdd);
            this.pnlMain.Controls.Add(this.btnOK);
            this.pnlMain.Location = new System.Drawing.Point(4, 139);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(471, 247);
            this.pnlMain.TabIndex = 45;
            // 
            // btnBrowseOutputFolder
            // 
            this.btnBrowseOutputFolder.Location = new System.Drawing.Point(387, 182);
            this.btnBrowseOutputFolder.Name = "btnBrowseOutputFolder";
            this.btnBrowseOutputFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseOutputFolder.TabIndex = 162;
            this.btnBrowseOutputFolder.Text = "Browse";
            this.btnBrowseOutputFolder.UseVisualStyleBackColor = true;
            this.btnBrowseOutputFolder.Click += new System.EventHandler(this.btnBrowseOverallOutputFolder_Click);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(8, 184);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(373, 20);
            this.txtOutputFolder.TabIndex = 161;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 160;
            this.label3.Text = "Output Folder";
            // 
            // chkUnique
            // 
            this.chkUnique.AutoSize = true;
            this.chkUnique.Checked = global::LowResFdrOptimizer.Properties.Settings.Default.unique;
            this.chkUnique.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnique.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::LowResFdrOptimizer.Properties.Settings.Default, "unique", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkUnique.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUnique.Location = new System.Drawing.Point(8, 105);
            this.chkUnique.Name = "chkUnique";
            this.chkUnique.Size = new System.Drawing.Size(431, 17);
            this.chkUnique.TabIndex = 123;
            this.chkUnique.Text = "FDR Calculation and Optimization Based on Unique Peptide Sequences";
            this.chkUnique.UseVisualStyleBackColor = true;
            // 
            // chkOverallOutputs
            // 
            this.chkOverallOutputs.AutoSize = true;
            this.chkOverallOutputs.Checked = global::LowResFdrOptimizer.Properties.Settings.Default.overallOutputs;
            this.chkOverallOutputs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOverallOutputs.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::LowResFdrOptimizer.Properties.Settings.Default, "overallOutputs", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkOverallOutputs.Location = new System.Drawing.Point(8, 137);
            this.chkOverallOutputs.Name = "chkOverallOutputs";
            this.chkOverallOutputs.Size = new System.Drawing.Size(99, 17);
            this.chkOverallOutputs.TabIndex = 122;
            this.chkOverallOutputs.Text = "Overall Outputs";
            this.chkOverallOutputs.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(272, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(15, 13);
            this.label10.TabIndex = 97;
            this.label10.Text = "%";
            // 
            // chkPhosphopeptideOutputs
            // 
            this.chkPhosphopeptideOutputs.AutoSize = true;
            this.chkPhosphopeptideOutputs.Checked = global::LowResFdrOptimizer.Properties.Settings.Default.phosphopeptideOutputs;
            this.chkPhosphopeptideOutputs.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::LowResFdrOptimizer.Properties.Settings.Default, "phosphopeptideOutputs", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkPhosphopeptideOutputs.Location = new System.Drawing.Point(197, 137);
            this.chkPhosphopeptideOutputs.Name = "chkPhosphopeptideOutputs";
            this.chkPhosphopeptideOutputs.Size = new System.Drawing.Size(143, 17);
            this.chkPhosphopeptideOutputs.TabIndex = 119;
            this.chkPhosphopeptideOutputs.Text = "Phosphopeptide Outputs";
            this.chkPhosphopeptideOutputs.UseVisualStyleBackColor = true;
            // 
            // chkHigherScoresAreBetter
            // 
            this.chkHigherScoresAreBetter.AutoSize = true;
            this.chkHigherScoresAreBetter.Checked = global::LowResFdrOptimizer.Properties.Settings.Default.higherScoresAreBetter;
            this.chkHigherScoresAreBetter.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::LowResFdrOptimizer.Properties.Settings.Default, "higherScoresAreBetter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkHigherScoresAreBetter.Location = new System.Drawing.Point(8, 53);
            this.chkHigherScoresAreBetter.Name = "chkHigherScoresAreBetter";
            this.chkHigherScoresAreBetter.Size = new System.Drawing.Size(142, 17);
            this.chkHigherScoresAreBetter.TabIndex = 116;
            this.chkHigherScoresAreBetter.Text = "Higher Scores are Better";
            this.chkHigherScoresAreBetter.UseVisualStyleBackColor = true;
            // 
            // numMaximumFalseDiscoveryRate
            // 
            this.numMaximumFalseDiscoveryRate.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::LowResFdrOptimizer.Properties.Settings.Default, "maximumFalseDiscoveryRate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numMaximumFalseDiscoveryRate.DecimalPlaces = 2;
            this.numMaximumFalseDiscoveryRate.Location = new System.Drawing.Point(197, 70);
            this.numMaximumFalseDiscoveryRate.Name = "numMaximumFalseDiscoveryRate";
            this.numMaximumFalseDiscoveryRate.Size = new System.Drawing.Size(75, 20);
            this.numMaximumFalseDiscoveryRate.TabIndex = 96;
            this.numMaximumFalseDiscoveryRate.Value = global::LowResFdrOptimizer.Properties.Settings.Default.maximumFalseDiscoveryRate;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(194, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(186, 13);
            this.label8.TabIndex = 95;
            this.label8.Text = "Maximum False Discovery Rate (FDR)";
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
            this.prgProgress.Location = new System.Drawing.Point(12, 392);
            this.prgProgress.Name = "prgProgress";
            this.prgProgress.Size = new System.Drawing.Size(454, 23);
            this.prgProgress.TabIndex = 70;
            // 
            // frmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 427);
            this.Controls.Add(this.prgProgress);
            this.Controls.Add(this.lstOmssaCsvFiles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Low-Res FDR Optimizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumFalseDiscoveryRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog ofdOmssaCsvFiles;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox lstOmssaCsvFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numMaximumFalseDiscoveryRate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkHigherScoresAreBetter;
        private System.Windows.Forms.CheckBox chkPhosphopeptideOutputs;
        private System.Windows.Forms.CheckBox chkOverallOutputs;
        private System.Windows.Forms.CheckBox chkUnique;
        private System.Windows.Forms.Button btnBrowseOutputFolder;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FolderBrowserDialog fbdOutputFolder;
        private System.Windows.Forms.ProgressBar prgProgress;
    }
}

