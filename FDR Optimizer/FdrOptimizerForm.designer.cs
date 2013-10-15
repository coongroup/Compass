namespace Coon.Compass.FdrOptimizer
{
    partial class FdrOptimizerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FdrOptimizerForm));
            this.fbdRawFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.ofdOmssaCsvFiles = new System.Windows.Forms.OpenFileDialog();
            this.grpFixedModifications = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnMoveLeft = new System.Windows.Forms.Button();
            this.btnMoveRight = new System.Windows.Forms.Button();
            this.lstSelectedFixedModifications = new System.Windows.Forms.ListBox();
            this.lstAllModifications = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnBrowseOutputFolder = new System.Windows.Forms.Button();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtRawFolder = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.numMaximumFalseDiscoveryRate = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstOmssaCsvFiles = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.prgProgress = new System.Windows.Forms.ProgressBar();
            this.fbdOutputFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.ofdModsXml = new System.Windows.Forms.OpenFileDialog();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.twoDCB = new System.Windows.Forms.CheckBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.grpFixedModifications.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumFalseDiscoveryRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ofdOmssaCsvFiles
            // 
            this.ofdOmssaCsvFiles.Filter = "OMSSA comma-separated value output files (.csv)|*.csv";
            this.ofdOmssaCsvFiles.Multiselect = true;
            // 
            // grpFixedModifications
            // 
            this.grpFixedModifications.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFixedModifications.Controls.Add(this.button1);
            this.grpFixedModifications.Controls.Add(this.checkBox2);
            this.grpFixedModifications.Controls.Add(this.label5);
            this.grpFixedModifications.Controls.Add(this.btnMoveLeft);
            this.grpFixedModifications.Controls.Add(this.btnMoveRight);
            this.grpFixedModifications.Controls.Add(this.lstSelectedFixedModifications);
            this.grpFixedModifications.Controls.Add(this.lstAllModifications);
            this.grpFixedModifications.Controls.Add(this.label4);
            this.grpFixedModifications.Location = new System.Drawing.Point(10, 228);
            this.grpFixedModifications.Name = "grpFixedModifications";
            this.grpFixedModifications.Size = new System.Drawing.Size(551, 154);
            this.grpFixedModifications.TabIndex = 173;
            this.grpFixedModifications.TabStop = false;
            this.grpFixedModifications.Text = "Fixed Modifications (* = user modification)";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(470, 125);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 173;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(11, 131);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(196, 17);
            this.checkBox2.TabIndex = 172;
            this.checkBox2.Text = "Include Fixed Mods in Mods Column";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(282, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 171;
            this.label5.Text = "Selected";
            // 
            // btnMoveLeft
            // 
            this.btnMoveLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMoveLeft.Location = new System.Drawing.Point(250, 95);
            this.btnMoveLeft.Name = "btnMoveLeft";
            this.btnMoveLeft.Size = new System.Drawing.Size(21, 23);
            this.btnMoveLeft.TabIndex = 170;
            this.btnMoveLeft.Text = "<";
            this.btnMoveLeft.UseVisualStyleBackColor = true;
            this.btnMoveLeft.Click += new System.EventHandler(this.btnMoveLeft_Click);
            // 
            // btnMoveRight
            // 
            this.btnMoveRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMoveRight.Location = new System.Drawing.Point(250, 30);
            this.btnMoveRight.Name = "btnMoveRight";
            this.btnMoveRight.Size = new System.Drawing.Size(21, 23);
            this.btnMoveRight.TabIndex = 169;
            this.btnMoveRight.Text = ">";
            this.btnMoveRight.UseVisualStyleBackColor = true;
            this.btnMoveRight.Click += new System.EventHandler(this.btnMoveRight_Click);
            // 
            // lstSelectedFixedModifications
            // 
            this.lstSelectedFixedModifications.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSelectedFixedModifications.FormattingEnabled = true;
            this.lstSelectedFixedModifications.Location = new System.Drawing.Point(282, 33);
            this.lstSelectedFixedModifications.Name = "lstSelectedFixedModifications";
            this.lstSelectedFixedModifications.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSelectedFixedModifications.Size = new System.Drawing.Size(263, 82);
            this.lstSelectedFixedModifications.TabIndex = 168;
            // 
            // lstAllModifications
            // 
            this.lstAllModifications.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstAllModifications.FormattingEnabled = true;
            this.lstAllModifications.Location = new System.Drawing.Point(11, 33);
            this.lstAllModifications.Name = "lstAllModifications";
            this.lstAllModifications.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAllModifications.Size = new System.Drawing.Size(230, 82);
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
            this.btnBrowseOutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOutputFolder.Location = new System.Drawing.Point(476, 115);
            this.btnBrowseOutputFolder.Name = "btnBrowseOutputFolder";
            this.btnBrowseOutputFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseOutputFolder.TabIndex = 159;
            this.btnBrowseOutputFolder.Text = "Browse";
            this.btnBrowseOutputFolder.UseVisualStyleBackColor = true;
            this.btnBrowseOutputFolder.Click += new System.EventHandler(this.btnBrowseOverallOutputFolder_Click);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputFolder.Location = new System.Drawing.Point(87, 117);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(383, 20);
            this.txtOutputFolder.TabIndex = 158;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 157;
            this.label3.Text = "Output Folder";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(252, 184);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(15, 13);
            this.label10.TabIndex = 97;
            this.label10.Text = "%";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(479, 177);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 24);
            this.btnBrowse.TabIndex = 111;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtRawFolder
            // 
            this.txtRawFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRawFolder.Location = new System.Drawing.Point(119, 180);
            this.txtRawFolder.Name = "txtRawFolder";
            this.txtRawFolder.Size = new System.Drawing.Size(351, 20);
            this.txtRawFolder.TabIndex = 110;
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(10, 183);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(98, 13);
            this.label17.TabIndex = 71;
            this.label17.Text = "Thermo .raw Folder";
            // 
            // numMaximumFalseDiscoveryRate
            // 
            this.numMaximumFalseDiscoveryRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numMaximumFalseDiscoveryRate.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Coon.Compass.FdrOptimizer.Properties.Settings.Default, "maximumFalseDiscoveryRate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numMaximumFalseDiscoveryRate.DecimalPlaces = 2;
            this.numMaximumFalseDiscoveryRate.Location = new System.Drawing.Point(202, 202);
            this.numMaximumFalseDiscoveryRate.Name = "numMaximumFalseDiscoveryRate";
            this.numMaximumFalseDiscoveryRate.Size = new System.Drawing.Size(44, 20);
            this.numMaximumFalseDiscoveryRate.TabIndex = 96;
            this.numMaximumFalseDiscoveryRate.Value = global::Coon.Compass.FdrOptimizer.Properties.Settings.Default.maximumFalseDiscoveryRate;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 204);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(186, 13);
            this.label8.TabIndex = 95;
            this.label8.Text = "Maximum False Discovery Rate (FDR)";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(477, 86);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 70;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(476, 51);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 69;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(476, 25);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 68;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstOmssaCsvFiles
            // 
            this.lstOmssaCsvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstOmssaCsvFiles.FormattingEnabled = true;
            this.lstOmssaCsvFiles.HorizontalScrollbar = true;
            this.lstOmssaCsvFiles.Location = new System.Drawing.Point(10, 25);
            this.lstOmssaCsvFiles.Name = "lstOmssaCsvFiles";
            this.lstOmssaCsvFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstOmssaCsvFiles.Size = new System.Drawing.Size(460, 82);
            this.lstOmssaCsvFiles.TabIndex = 69;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(253, 13);
            this.label1.TabIndex = 68;
            this.label1.Text = "OMSSA Comma-Separated Value Output Files (.csv)";
            // 
            // prgProgress
            // 
            this.prgProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgProgress.Location = new System.Drawing.Point(10, 388);
            this.prgProgress.Name = "prgProgress";
            this.prgProgress.Size = new System.Drawing.Size(460, 23);
            this.prgProgress.TabIndex = 70;
            // 
            // ofdModsXml
            // 
            this.ofdModsXml.Filter = "OMSSA mods files (.xml)|*.xml";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(255, 149);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(123, 13);
            this.label6.TabIndex = 179;
            this.label6.Text = "Reduce PSMs based on";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(384, 146);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(167, 21);
            this.comboBox1.TabIndex = 178;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(168, 148);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(84, 17);
            this.checkBox1.TabIndex = 177;
            this.checkBox1.Text = "Batch Mode";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(477, 388);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(78, 23);
            this.btnOK.TabIndex = 178;
            this.btnOK.Text = "Start";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click_1);
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
            this.splitContainer1.Panel1.Controls.Add(this.label17);
            this.splitContainer1.Panel1.Controls.Add(this.txtRawFolder);
            this.splitContainer1.Panel1.Controls.Add(this.btnBrowse);
            this.splitContainer1.Panel1.Controls.Add(this.comboBox1);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.twoDCB);
            this.splitContainer1.Panel1.Controls.Add(this.label10);
            this.splitContainer1.Panel1.Controls.Add(this.numMaximumFalseDiscoveryRate);
            this.splitContainer1.Panel1.Controls.Add(this.label8);
            this.splitContainer1.Panel1.Controls.Add(this.checkBox1);
            this.splitContainer1.Panel1.Controls.Add(this.grpFixedModifications);
            this.splitContainer1.Panel1.Controls.Add(this.prgProgress);
            this.splitContainer1.Panel1.Controls.Add(this.lstOmssaCsvFiles);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.btnClear);
            this.splitContainer1.Panel1.Controls.Add(this.btnBrowseOutputFolder);
            this.splitContainer1.Panel1.Controls.Add(this.txtOutputFolder);
            this.splitContainer1.Panel1.Controls.Add(this.btnOK);
            this.splitContainer1.Panel1.Controls.Add(this.btnAdd);
            this.splitContainer1.Panel1.Controls.Add(this.btnRemove);
            this.splitContainer1.Panel1MinSize = 420;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer1.Size = new System.Drawing.Size(568, 617);
            this.splitContainer1.SplitterDistance = 420;
            this.splitContainer1.TabIndex = 179;
            // 
            // twoDCB
            // 
            this.twoDCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.twoDCB.AutoSize = true;
            this.twoDCB.Checked = true;
            this.twoDCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.twoDCB.Location = new System.Drawing.Point(13, 148);
            this.twoDCB.Name = "twoDCB";
            this.twoDCB.Size = new System.Drawing.Size(149, 17);
            this.twoDCB.TabIndex = 180;
            this.twoDCB.Text = "High Resolution (2D FDR)";
            this.twoDCB.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(564, 189);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // FdrOptimizerForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 617);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(584, 562);
            this.Name = "FdrOptimizerForm";
            this.Text = "FDR Optimizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
            this.grpFixedModifications.ResumeLayout(false);
            this.grpFixedModifications.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumFalseDiscoveryRate)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog ofdOmssaCsvFiles;
        private System.Windows.Forms.FolderBrowserDialog fbdRawFolder;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox lstOmssaCsvFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar prgProgress;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numMaximumFalseDiscoveryRate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtRawFolder;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btnBrowseOutputFolder;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FolderBrowserDialog fbdOutputFolder;
        private System.Windows.Forms.GroupBox grpFixedModifications;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnMoveLeft;
        private System.Windows.Forms.Button btnMoveRight;
        private System.Windows.Forms.ListBox lstSelectedFixedModifications;
        private System.Windows.Forms.ListBox lstAllModifications;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog ofdModsXml;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox twoDCB;
    }
}

