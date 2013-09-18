namespace Phosphinator
{
    partial class phosphinatorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(phosphinatorForm));
            this.fbdOutput = new System.Windows.Forms.FolderBrowserDialog();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFragmentIntensityThreshold = new System.Windows.Forms.TextBox();
            this.cboFragmentIntensityThresholdType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.btnMoveLeft = new System.Windows.Forms.Button();
            this.btnMoveRight = new System.Windows.Forms.Button();
            this.lstSelectedFixedModifications = new System.Windows.Forms.ListBox();
            this.lstAllModifications = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numAmbiguityScoreThreshold = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtRawFolder = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numFragmentMZTolerance = new System.Windows.Forms.NumericUpDown();
            this.prgProgress = new System.Windows.Forms.ProgressBar();
            this.lstOmssaCsvFiles = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ofdOmssaCsvFiles = new System.Windows.Forms.OpenFileDialog();
            this.fbdRawFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAmbiguityScoreThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFragmentMZTolerance)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(11, 379);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 38;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(11, 353);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(376, 20);
            this.txtOutputFolder.TabIndex = 40;
            // 
            // btnBrowseOutput
            // 
            this.btnBrowseOutput.Location = new System.Drawing.Point(390, 351);
            this.btnBrowseOutput.Name = "btnBrowseOutput";
            this.btnBrowseOutput.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseOutput.TabIndex = 39;
            this.btnBrowseOutput.Text = "Browse";
            this.btnBrowseOutput.UseVisualStyleBackColor = true;
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 337);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 41;
            this.label2.Text = "Output Folder";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 235);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 13);
            this.label3.TabIndex = 42;
            this.label3.Text = "Fragment Intensity Threshold";
            // 
            // txtFragmentIntensityThreshold
            // 
            this.txtFragmentIntensityThreshold.Location = new System.Drawing.Point(11, 251);
            this.txtFragmentIntensityThreshold.Name = "txtFragmentIntensityThreshold";
            this.txtFragmentIntensityThreshold.Size = new System.Drawing.Size(100, 20);
            this.txtFragmentIntensityThreshold.TabIndex = 43;
            this.txtFragmentIntensityThreshold.Text = "0.0";
            this.txtFragmentIntensityThreshold.TextChanged += new System.EventHandler(this.txtFragmentIntensityThreshold_TextChanged);
            // 
            // cboFragmentIntensityThresholdType
            // 
            this.cboFragmentIntensityThresholdType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFragmentIntensityThresholdType.FormattingEnabled = true;
            this.cboFragmentIntensityThresholdType.Location = new System.Drawing.Point(117, 251);
            this.cboFragmentIntensityThresholdType.Name = "cboFragmentIntensityThresholdType";
            this.cboFragmentIntensityThresholdType.Size = new System.Drawing.Size(96, 21);
            this.cboFragmentIntensityThresholdType.TabIndex = 44;
            this.cboFragmentIntensityThresholdType.SelectedIndexChanged += new System.EventHandler(this.cboIntensityThresholdType_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(254, 235);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 13);
            this.label4.TabIndex = 52;
            this.label4.Text = "Fragment m/z Tolerance";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.btnAdvanced);
            this.pnlMain.Controls.Add(this.label6);
            this.pnlMain.Controls.Add(this.btnMoveLeft);
            this.pnlMain.Controls.Add(this.btnMoveRight);
            this.pnlMain.Controls.Add(this.lstSelectedFixedModifications);
            this.pnlMain.Controls.Add(this.lstAllModifications);
            this.pnlMain.Controls.Add(this.label9);
            this.pnlMain.Controls.Add(this.numAmbiguityScoreThreshold);
            this.pnlMain.Controls.Add(this.label5);
            this.pnlMain.Controls.Add(this.btnBrowse);
            this.pnlMain.Controls.Add(this.txtRawFolder);
            this.pnlMain.Controls.Add(this.label17);
            this.pnlMain.Controls.Add(this.btnClear);
            this.pnlMain.Controls.Add(this.btnRemove);
            this.pnlMain.Controls.Add(this.btnAdd);
            this.pnlMain.Controls.Add(this.label8);
            this.pnlMain.Controls.Add(this.label7);
            this.pnlMain.Controls.Add(this.numFragmentMZTolerance);
            this.pnlMain.Controls.Add(this.label4);
            this.pnlMain.Controls.Add(this.cboFragmentIntensityThresholdType);
            this.pnlMain.Controls.Add(this.txtFragmentIntensityThreshold);
            this.pnlMain.Controls.Add(this.label3);
            this.pnlMain.Controls.Add(this.label2);
            this.pnlMain.Controls.Add(this.btnBrowseOutput);
            this.pnlMain.Controls.Add(this.txtOutputFolder);
            this.pnlMain.Controls.Add(this.btnOK);
            this.pnlMain.Location = new System.Drawing.Point(1, 176);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(476, 405);
            this.pnlMain.TabIndex = 45;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(255, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 13);
            this.label6.TabIndex = 171;
            this.label6.Text = "Selected Fixed Modifications";
            // 
            // btnMoveLeft
            // 
            this.btnMoveLeft.Location = new System.Drawing.Point(228, 168);
            this.btnMoveLeft.Name = "btnMoveLeft";
            this.btnMoveLeft.Size = new System.Drawing.Size(21, 23);
            this.btnMoveLeft.TabIndex = 170;
            this.btnMoveLeft.Text = "<";
            this.btnMoveLeft.UseVisualStyleBackColor = true;
            this.btnMoveLeft.Click += new System.EventHandler(this.btnMoveLeft_Click);
            // 
            // btnMoveRight
            // 
            this.btnMoveRight.Location = new System.Drawing.Point(228, 139);
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
            this.lstSelectedFixedModifications.Location = new System.Drawing.Point(255, 111);
            this.lstSelectedFixedModifications.Name = "lstSelectedFixedModifications";
            this.lstSelectedFixedModifications.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSelectedFixedModifications.Size = new System.Drawing.Size(210, 108);
            this.lstSelectedFixedModifications.TabIndex = 168;
            // 
            // lstAllModifications
            // 
            this.lstAllModifications.FormattingEnabled = true;
            this.lstAllModifications.Location = new System.Drawing.Point(11, 111);
            this.lstAllModifications.Name = "lstAllModifications";
            this.lstAllModifications.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAllModifications.Size = new System.Drawing.Size(210, 108);
            this.lstAllModifications.Sorted = true;
            this.lstAllModifications.TabIndex = 167;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 95);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 13);
            this.label9.TabIndex = 166;
            this.label9.Text = "All Modifications";
            // 
            // numAmbiguityScoreThreshold
            // 
            this.numAmbiguityScoreThreshold.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Phosphinator.Properties.Settings.Default, "ambiguityScoreThreshold", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numAmbiguityScoreThreshold.Location = new System.Drawing.Point(11, 304);
            this.numAmbiguityScoreThreshold.Name = "numAmbiguityScoreThreshold";
            this.numAmbiguityScoreThreshold.Size = new System.Drawing.Size(72, 20);
            this.numAmbiguityScoreThreshold.TabIndex = 124;
            this.numAmbiguityScoreThreshold.Value = global::Phosphinator.Properties.Settings.Default.ambiguityScoreThreshold;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 288);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(133, 13);
            this.label5.TabIndex = 123;
            this.label5.Text = "Ambiguity Score Threshold";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(390, 60);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 114;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtRawFolder
            // 
            this.txtRawFolder.Location = new System.Drawing.Point(11, 62);
            this.txtRawFolder.Name = "txtRawFolder";
            this.txtRawFolder.Size = new System.Drawing.Size(373, 20);
            this.txtRawFolder.TabIndex = 113;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(11, 46);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(205, 13);
            this.label17.TabIndex = 112;
            this.label17.Text = "Thermo LC-MS/MS Data (.raw) File Folder";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(390, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 70;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(202, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 69;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(11, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 68;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(254, 253);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(13, 13);
            this.label8.TabIndex = 62;
            this.label8.Text = "±";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(333, 254);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 13);
            this.label7.TabIndex = 61;
            this.label7.Text = "Th";
            // 
            // numFragmentMZTolerance
            // 
            this.numFragmentMZTolerance.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Phosphinator.Properties.Settings.Default, "fragmentMZTolerance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numFragmentMZTolerance.DecimalPlaces = 3;
            this.numFragmentMZTolerance.Location = new System.Drawing.Point(267, 251);
            this.numFragmentMZTolerance.Name = "numFragmentMZTolerance";
            this.numFragmentMZTolerance.Size = new System.Drawing.Size(66, 20);
            this.numFragmentMZTolerance.TabIndex = 56;
            this.numFragmentMZTolerance.Value = global::Phosphinator.Properties.Settings.Default.fragmentMZTolerance;
            // 
            // prgProgress
            // 
            this.prgProgress.Location = new System.Drawing.Point(12, 587);
            this.prgProgress.Name = "prgProgress";
            this.prgProgress.Size = new System.Drawing.Size(454, 23);
            this.prgProgress.TabIndex = 70;
            // 
            // lstOmssaCsvFiles
            // 
            this.lstOmssaCsvFiles.FormattingEnabled = true;
            this.lstOmssaCsvFiles.HorizontalScrollbar = true;
            this.lstOmssaCsvFiles.Location = new System.Drawing.Point(12, 25);
            this.lstOmssaCsvFiles.Name = "lstOmssaCsvFiles";
            this.lstOmssaCsvFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstOmssaCsvFiles.Size = new System.Drawing.Size(454, 147);
            this.lstOmssaCsvFiles.TabIndex = 72;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(253, 13);
            this.label1.TabIndex = 71;
            this.label1.Text = "OMSSA Comma-Separated Value Output Files (.csv)";
            // 
            // ofdOmssaCsvFiles
            // 
            this.ofdOmssaCsvFiles.Filter = "OMSSA comma-separated value output files (.csv)|*.csv";
            this.ofdOmssaCsvFiles.Multiselect = true;
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Location = new System.Drawing.Point(390, 301);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(75, 23);
            this.btnAdvanced.TabIndex = 172;
            this.btnAdvanced.Text = "Advanced";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // frmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 622);
            this.Controls.Add(this.lstOmssaCsvFiles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.prgProgress);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Phosphinator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frmMain_KeyPress);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAmbiguityScoreThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFragmentMZTolerance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog fbdOutput;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFragmentIntensityThreshold;
        private System.Windows.Forms.ComboBox cboFragmentIntensityThresholdType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numFragmentMZTolerance;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ProgressBar prgProgress;
        private System.Windows.Forms.ListBox lstOmssaCsvFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog ofdOmssaCsvFiles;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtRawFolder;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.FolderBrowserDialog fbdRawFolder;
        private System.Windows.Forms.NumericUpDown numAmbiguityScoreThreshold;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnMoveLeft;
        private System.Windows.Forms.Button btnMoveRight;
        private System.Windows.Forms.ListBox lstSelectedFixedModifications;
        private System.Windows.Forms.ListBox lstAllModifications;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnAdvanced;
    }
}

