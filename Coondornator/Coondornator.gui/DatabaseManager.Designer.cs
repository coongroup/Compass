namespace Compass.Coondornator
{
    partial class DatabaseManager
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
            this.fastaTextBox = new System.Windows.Forms.TextBox();
            this.browseButtonOne = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.targetOnlyRadio = new System.Windows.Forms.RadioButton();
            this.decoyOnlyRadio = new System.Windows.Forms.RadioButton();
            this.concatRadio = new System.Windows.Forms.RadioButton();
            this.reverseRadio = new System.Windows.Forms.RadioButton();
            this.shuffleRadio = new System.Windows.Forms.RadioButton();
            this.randomRadio = new System.Windows.Forms.RadioButton();
            this.excludeNTermCheckBox = new System.Windows.Forms.CheckBox();
            this.nTermMethCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.localMachineCheckBox = new System.Windows.Forms.CheckBox();
            this.localMachineOutputBrowseButton = new System.Windows.Forms.Button();
            this.localMachineOutputDirectoryBox = new System.Windows.Forms.TextBox();
            this.serverCheckBox = new System.Windows.Forms.CheckBox();
            this.taxIDTextBox = new System.Windows.Forms.TextBox();
            this.genusTextBox = new System.Windows.Forms.TextBox();
            this.speciesTextBox = new System.Windows.Forms.TextBox();
            this.isoformsComboBox = new System.Windows.Forms.ComboBox();
            this.consortiumTextBox = new System.Windows.Forms.TextBox();
            this.infoTextBox = new System.Windows.Forms.TextBox();
            this.monthTextBox = new System.Windows.Forms.TextBox();
            this.dayTextBox = new System.Windows.Forms.TextBox();
            this.yearTextBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.taxIDLabel = new System.Windows.Forms.LinkLabel();
            this.genusLabel = new System.Windows.Forms.LinkLabel();
            this.speciesLabel = new System.Windows.Forms.LinkLabel();
            this.consortiumLabel = new System.Windows.Forms.LinkLabel();
            this.isoformLabel = new System.Windows.Forms.LinkLabel();
            this.infoLabel = new System.Windows.Forms.Label();
            this.downloadLabel = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // fastaTextBox
            // 
            this.fastaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fastaTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fastaTextBox.Location = new System.Drawing.Point(11, 25);
            this.fastaTextBox.Name = "fastaTextBox";
            this.fastaTextBox.Size = new System.Drawing.Size(467, 20);
            this.fastaTextBox.TabIndex = 0;
            // 
            // browseButtonOne
            // 
            this.browseButtonOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButtonOne.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browseButtonOne.Location = new System.Drawing.Point(487, 24);
            this.browseButtonOne.Name = "browseButtonOne";
            this.browseButtonOne.Size = new System.Drawing.Size(115, 23);
            this.browseButtonOne.TabIndex = 1;
            this.browseButtonOne.Text = "Browse";
            this.browseButtonOne.UseVisualStyleBackColor = true;
            this.browseButtonOne.Click += new System.EventHandler(this.browseButtonOne_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "FASTA Protein Database File (*.fasta)";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.concatRadio);
            this.groupBox1.Controls.Add(this.decoyOnlyRadio);
            this.groupBox1.Controls.Add(this.targetOnlyRadio);
            this.groupBox1.Location = new System.Drawing.Point(11, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(184, 96);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database Type";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.nTermMethCheckBox);
            this.groupBox2.Controls.Add(this.excludeNTermCheckBox);
            this.groupBox2.Controls.Add(this.randomRadio);
            this.groupBox2.Controls.Add(this.shuffleRadio);
            this.groupBox2.Controls.Add(this.reverseRadio);
            this.groupBox2.Location = new System.Drawing.Point(201, 54);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(401, 95);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Decoy Database Settings";
            // 
            // targetOnlyRadio
            // 
            this.targetOnlyRadio.AutoSize = true;
            this.targetOnlyRadio.Location = new System.Drawing.Point(11, 19);
            this.targetOnlyRadio.Name = "targetOnlyRadio";
            this.targetOnlyRadio.Size = new System.Drawing.Size(80, 17);
            this.targetOnlyRadio.TabIndex = 0;
            this.targetOnlyRadio.Text = "Target Only";
            this.targetOnlyRadio.UseVisualStyleBackColor = true;
            // 
            // decoyOnlyRadio
            // 
            this.decoyOnlyRadio.AutoSize = true;
            this.decoyOnlyRadio.Location = new System.Drawing.Point(11, 42);
            this.decoyOnlyRadio.Name = "decoyOnlyRadio";
            this.decoyOnlyRadio.Size = new System.Drawing.Size(80, 17);
            this.decoyOnlyRadio.TabIndex = 1;
            this.decoyOnlyRadio.Text = "Decoy Only";
            this.decoyOnlyRadio.UseVisualStyleBackColor = true;
            // 
            // concatRadio
            // 
            this.concatRadio.AutoSize = true;
            this.concatRadio.Checked = true;
            this.concatRadio.Location = new System.Drawing.Point(11, 65);
            this.concatRadio.Name = "concatRadio";
            this.concatRadio.Size = new System.Drawing.Size(169, 17);
            this.concatRadio.TabIndex = 2;
            this.concatRadio.TabStop = true;
            this.concatRadio.Text = "Concatenated Target + Decoy";
            this.concatRadio.UseVisualStyleBackColor = true;
            // 
            // reverseRadio
            // 
            this.reverseRadio.AutoSize = true;
            this.reverseRadio.Checked = true;
            this.reverseRadio.Location = new System.Drawing.Point(10, 20);
            this.reverseRadio.Name = "reverseRadio";
            this.reverseRadio.Size = new System.Drawing.Size(65, 17);
            this.reverseRadio.TabIndex = 3;
            this.reverseRadio.TabStop = true;
            this.reverseRadio.Text = "Reverse";
            this.reverseRadio.UseVisualStyleBackColor = true;
            // 
            // shuffleRadio
            // 
            this.shuffleRadio.AutoSize = true;
            this.shuffleRadio.Location = new System.Drawing.Point(10, 43);
            this.shuffleRadio.Name = "shuffleRadio";
            this.shuffleRadio.Size = new System.Drawing.Size(58, 17);
            this.shuffleRadio.TabIndex = 4;
            this.shuffleRadio.Text = "Shuffle";
            this.shuffleRadio.UseVisualStyleBackColor = true;
            // 
            // randomRadio
            // 
            this.randomRadio.AutoSize = true;
            this.randomRadio.Location = new System.Drawing.Point(10, 65);
            this.randomRadio.Name = "randomRadio";
            this.randomRadio.Size = new System.Drawing.Size(65, 17);
            this.randomRadio.TabIndex = 5;
            this.randomRadio.Text = "Random";
            this.randomRadio.UseVisualStyleBackColor = true;
            // 
            // excludeNTermCheckBox
            // 
            this.excludeNTermCheckBox.AutoSize = true;
            this.excludeNTermCheckBox.Location = new System.Drawing.Point(88, 21);
            this.excludeNTermCheckBox.Name = "excludeNTermCheckBox";
            this.excludeNTermCheckBox.Size = new System.Drawing.Size(216, 17);
            this.excludeNTermCheckBox.TabIndex = 6;
            this.excludeNTermCheckBox.Text = "Exclude N-Terminal Amino Acid Residue";
            this.excludeNTermCheckBox.UseVisualStyleBackColor = true;
            this.excludeNTermCheckBox.CheckedChanged += new System.EventHandler(this.excludeNTermCheckBox_CheckedChanged);
            // 
            // nTermMethCheckBox
            // 
            this.nTermMethCheckBox.AutoSize = true;
            this.nTermMethCheckBox.Enabled = false;
            this.nTermMethCheckBox.Location = new System.Drawing.Point(88, 45);
            this.nTermMethCheckBox.Name = "nTermMethCheckBox";
            this.nTermMethCheckBox.Size = new System.Drawing.Size(216, 17);
            this.nTermMethCheckBox.TabIndex = 7;
            this.nTermMethCheckBox.Text = "Only if N-Terminal Residue is Methionine";
            this.nTermMethCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.downloadLabel);
            this.groupBox3.Controls.Add(this.infoLabel);
            this.groupBox3.Controls.Add(this.isoformLabel);
            this.groupBox3.Controls.Add(this.consortiumLabel);
            this.groupBox3.Controls.Add(this.speciesLabel);
            this.groupBox3.Controls.Add(this.genusLabel);
            this.groupBox3.Controls.Add(this.taxIDLabel);
            this.groupBox3.Controls.Add(this.yearTextBox);
            this.groupBox3.Controls.Add(this.dayTextBox);
            this.groupBox3.Controls.Add(this.monthTextBox);
            this.groupBox3.Controls.Add(this.infoTextBox);
            this.groupBox3.Controls.Add(this.consortiumTextBox);
            this.groupBox3.Controls.Add(this.isoformsComboBox);
            this.groupBox3.Controls.Add(this.speciesTextBox);
            this.groupBox3.Controls.Add(this.genusTextBox);
            this.groupBox3.Controls.Add(this.taxIDTextBox);
            this.groupBox3.Controls.Add(this.serverCheckBox);
            this.groupBox3.Controls.Add(this.localMachineOutputBrowseButton);
            this.groupBox3.Controls.Add(this.localMachineCheckBox);
            this.groupBox3.Controls.Add(this.localMachineOutputDirectoryBox);
            this.groupBox3.Location = new System.Drawing.Point(11, 154);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(590, 201);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output Options";
            // 
            // localMachineCheckBox
            // 
            this.localMachineCheckBox.AutoSize = true;
            this.localMachineCheckBox.Location = new System.Drawing.Point(11, 21);
            this.localMachineCheckBox.Name = "localMachineCheckBox";
            this.localMachineCheckBox.Size = new System.Drawing.Size(185, 17);
            this.localMachineCheckBox.TabIndex = 8;
            this.localMachineCheckBox.Text = "Write Database to Local Machine";
            this.localMachineCheckBox.UseVisualStyleBackColor = true;
            this.localMachineCheckBox.CheckedChanged += new System.EventHandler(this.localMachineCheckBox_CheckedChanged);
            // 
            // localMachineOutputBrowseButton
            // 
            this.localMachineOutputBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.localMachineOutputBrowseButton.Enabled = false;
            this.localMachineOutputBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.localMachineOutputBrowseButton.Location = new System.Drawing.Point(476, 44);
            this.localMachineOutputBrowseButton.Name = "localMachineOutputBrowseButton";
            this.localMachineOutputBrowseButton.Size = new System.Drawing.Size(107, 23);
            this.localMachineOutputBrowseButton.TabIndex = 7;
            this.localMachineOutputBrowseButton.Text = "Browse";
            this.localMachineOutputBrowseButton.UseVisualStyleBackColor = true;
            this.localMachineOutputBrowseButton.Click += new System.EventHandler(this.localMachineOutputBrowseButton_Click);
            // 
            // localMachineOutputDirectoryBox
            // 
            this.localMachineOutputDirectoryBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.localMachineOutputDirectoryBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.localMachineOutputDirectoryBox.Enabled = false;
            this.localMachineOutputDirectoryBox.Location = new System.Drawing.Point(10, 45);
            this.localMachineOutputDirectoryBox.Name = "localMachineOutputDirectoryBox";
            this.localMachineOutputDirectoryBox.Size = new System.Drawing.Size(457, 20);
            this.localMachineOutputDirectoryBox.TabIndex = 6;
            this.localMachineOutputDirectoryBox.Text = "Output File Location";
            this.localMachineOutputDirectoryBox.Click += new System.EventHandler(this.localMachineOutputDirectoryBox_Click);
            // 
            // serverCheckBox
            // 
            this.serverCheckBox.AutoSize = true;
            this.serverCheckBox.Location = new System.Drawing.Point(11, 73);
            this.serverCheckBox.Name = "serverCheckBox";
            this.serverCheckBox.Size = new System.Drawing.Size(150, 17);
            this.serverCheckBox.TabIndex = 9;
            this.serverCheckBox.Text = "Write Database To Server";
            this.serverCheckBox.UseVisualStyleBackColor = true;
            this.serverCheckBox.CheckedChanged += new System.EventHandler(this.serverCheckBox_CheckedChanged);
            // 
            // taxIDTextBox
            // 
            this.taxIDTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.taxIDTextBox.Enabled = false;
            this.taxIDTextBox.Location = new System.Drawing.Point(11, 114);
            this.taxIDTextBox.Name = "taxIDTextBox";
            this.taxIDTextBox.Size = new System.Drawing.Size(115, 20);
            this.taxIDTextBox.TabIndex = 10;
            // 
            // genusTextBox
            // 
            this.genusTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.genusTextBox.Enabled = false;
            this.genusTextBox.Location = new System.Drawing.Point(134, 114);
            this.genusTextBox.Name = "genusTextBox";
            this.genusTextBox.Size = new System.Drawing.Size(147, 20);
            this.genusTextBox.TabIndex = 11;
            // 
            // speciesTextBox
            // 
            this.speciesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.speciesTextBox.Enabled = false;
            this.speciesTextBox.Location = new System.Drawing.Point(289, 114);
            this.speciesTextBox.Name = "speciesTextBox";
            this.speciesTextBox.Size = new System.Drawing.Size(162, 20);
            this.speciesTextBox.TabIndex = 12;
            // 
            // isoformsComboBox
            // 
            this.isoformsComboBox.Enabled = false;
            this.isoformsComboBox.FormattingEnabled = true;
            this.isoformsComboBox.Items.AddRange(new object[] {
            "YES",
            "NO"});
            this.isoformsComboBox.Location = new System.Drawing.Point(11, 170);
            this.isoformsComboBox.Name = "isoformsComboBox";
            this.isoformsComboBox.Size = new System.Drawing.Size(115, 21);
            this.isoformsComboBox.TabIndex = 13;
            // 
            // consortiumTextBox
            // 
            this.consortiumTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.consortiumTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.consortiumTextBox.Enabled = false;
            this.consortiumTextBox.Location = new System.Drawing.Point(459, 114);
            this.consortiumTextBox.Name = "consortiumTextBox";
            this.consortiumTextBox.Size = new System.Drawing.Size(122, 20);
            this.consortiumTextBox.TabIndex = 14;
            // 
            // infoTextBox
            // 
            this.infoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.infoTextBox.Enabled = false;
            this.infoTextBox.Location = new System.Drawing.Point(134, 171);
            this.infoTextBox.Name = "infoTextBox";
            this.infoTextBox.Size = new System.Drawing.Size(317, 20);
            this.infoTextBox.TabIndex = 15;
            // 
            // monthTextBox
            // 
            this.monthTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.monthTextBox.Enabled = false;
            this.monthTextBox.Location = new System.Drawing.Point(460, 171);
            this.monthTextBox.Name = "monthTextBox";
            this.monthTextBox.Size = new System.Drawing.Size(36, 20);
            this.monthTextBox.TabIndex = 16;
            this.monthTextBox.Text = "MM";
            this.monthTextBox.Click += new System.EventHandler(this.monthTextBox_Click);
            // 
            // dayTextBox
            // 
            this.dayTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dayTextBox.Enabled = false;
            this.dayTextBox.Location = new System.Drawing.Point(500, 171);
            this.dayTextBox.Name = "dayTextBox";
            this.dayTextBox.Size = new System.Drawing.Size(36, 20);
            this.dayTextBox.TabIndex = 17;
            this.dayTextBox.Text = "DD";
            this.dayTextBox.Click += new System.EventHandler(this.dayTextBox_Click);
            // 
            // yearTextBox
            // 
            this.yearTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.yearTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.yearTextBox.Enabled = false;
            this.yearTextBox.Location = new System.Drawing.Point(540, 171);
            this.yearTextBox.Name = "yearTextBox";
            this.yearTextBox.Size = new System.Drawing.Size(36, 20);
            this.yearTextBox.TabIndex = 18;
            this.yearTextBox.Text = "YY";
            this.yearTextBox.Click += new System.EventHandler(this.yearTextBox_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button2.Location = new System.Drawing.Point(12, 361);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(589, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Create Database";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // taxIDLabel
            // 
            this.taxIDLabel.AutoSize = true;
            this.taxIDLabel.Enabled = false;
            this.taxIDLabel.Location = new System.Drawing.Point(9, 96);
            this.taxIDLabel.Name = "taxIDLabel";
            this.taxIDLabel.Size = new System.Drawing.Size(102, 13);
            this.taxIDLabel.TabIndex = 20;
            this.taxIDLabel.TabStop = true;
            this.taxIDLabel.Text = "NCBI Taxnomic ID#";
            this.taxIDLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.taxIDLabel_LinkClicked);
            // 
            // genusLabel
            // 
            this.genusLabel.AutoSize = true;
            this.genusLabel.Enabled = false;
            this.genusLabel.Location = new System.Drawing.Point(132, 96);
            this.genusLabel.Name = "genusLabel";
            this.genusLabel.Size = new System.Drawing.Size(38, 13);
            this.genusLabel.TabIndex = 21;
            this.genusLabel.TabStop = true;
            this.genusLabel.Text = "Genus";
            this.genusLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.genusLabel_LinkClicked);
            // 
            // speciesLabel
            // 
            this.speciesLabel.AutoSize = true;
            this.speciesLabel.Enabled = false;
            this.speciesLabel.Location = new System.Drawing.Point(285, 96);
            this.speciesLabel.Name = "speciesLabel";
            this.speciesLabel.Size = new System.Drawing.Size(45, 13);
            this.speciesLabel.TabIndex = 22;
            this.speciesLabel.TabStop = true;
            this.speciesLabel.Text = "Species";
            this.speciesLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.speciesLabel_LinkClicked);
            // 
            // consortiumLabel
            // 
            this.consortiumLabel.AutoSize = true;
            this.consortiumLabel.Enabled = false;
            this.consortiumLabel.Location = new System.Drawing.Point(457, 96);
            this.consortiumLabel.Name = "consortiumLabel";
            this.consortiumLabel.Size = new System.Drawing.Size(59, 13);
            this.consortiumLabel.TabIndex = 23;
            this.consortiumLabel.TabStop = true;
            this.consortiumLabel.Text = "Consortium";
            this.consortiumLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.consortiumLabel_LinkClicked);
            // 
            // isoformLabel
            // 
            this.isoformLabel.AutoSize = true;
            this.isoformLabel.Enabled = false;
            this.isoformLabel.Location = new System.Drawing.Point(9, 152);
            this.isoformLabel.Name = "isoformLabel";
            this.isoformLabel.Size = new System.Drawing.Size(46, 13);
            this.isoformLabel.TabIndex = 24;
            this.isoformLabel.TabStop = true;
            this.isoformLabel.Text = "Isoforms";
            this.isoformLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.isoformLabel_LinkClicked);
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Enabled = false;
            this.infoLabel.Location = new System.Drawing.Point(132, 152);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(198, 13);
            this.infoLabel.TabIndex = 7;
            this.infoLabel.Text = "Additional Information (Omit Whitespace)";
            // 
            // downloadLabel
            // 
            this.downloadLabel.AutoSize = true;
            this.downloadLabel.Enabled = false;
            this.downloadLabel.Location = new System.Drawing.Point(457, 139);
            this.downloadLabel.Name = "downloadLabel";
            this.downloadLabel.Size = new System.Drawing.Size(81, 26);
            this.downloadLabel.TabIndex = 25;
            this.downloadLabel.Text = "Download Date\r\n(MM/DD/YY)";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // DatabaseManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 396);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.browseButtonOne);
            this.Controls.Add(this.fastaTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximumSize = new System.Drawing.Size(1000, 500);
            this.MinimumSize = new System.Drawing.Size(611, 395);
            this.Name = "DatabaseManager";
            this.Text = "Database Manager";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox fastaTextBox;
        private System.Windows.Forms.Button browseButtonOne;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton concatRadio;
        private System.Windows.Forms.RadioButton decoyOnlyRadio;
        private System.Windows.Forms.RadioButton targetOnlyRadio;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton randomRadio;
        private System.Windows.Forms.RadioButton shuffleRadio;
        private System.Windows.Forms.RadioButton reverseRadio;
        private System.Windows.Forms.CheckBox nTermMethCheckBox;
        private System.Windows.Forms.CheckBox excludeNTermCheckBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox yearTextBox;
        private System.Windows.Forms.TextBox dayTextBox;
        private System.Windows.Forms.TextBox monthTextBox;
        private System.Windows.Forms.TextBox infoTextBox;
        private System.Windows.Forms.TextBox consortiumTextBox;
        private System.Windows.Forms.ComboBox isoformsComboBox;
        private System.Windows.Forms.TextBox speciesTextBox;
        private System.Windows.Forms.TextBox genusTextBox;
        private System.Windows.Forms.TextBox taxIDTextBox;
        private System.Windows.Forms.CheckBox serverCheckBox;
        private System.Windows.Forms.Button localMachineOutputBrowseButton;
        private System.Windows.Forms.CheckBox localMachineCheckBox;
        private System.Windows.Forms.TextBox localMachineOutputDirectoryBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.LinkLabel isoformLabel;
        private System.Windows.Forms.LinkLabel consortiumLabel;
        private System.Windows.Forms.LinkLabel speciesLabel;
        private System.Windows.Forms.LinkLabel genusLabel;
        private System.Windows.Forms.LinkLabel taxIDLabel;
        private System.Windows.Forms.Label downloadLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}