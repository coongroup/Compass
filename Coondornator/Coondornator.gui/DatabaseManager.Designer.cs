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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.browseButtonOne = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
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
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(11, 25);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(449, 20);
            this.textBox1.TabIndex = 0;
            // 
            // browseButtonOne
            // 
            this.browseButtonOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButtonOne.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browseButtonOne.Location = new System.Drawing.Point(469, 24);
            this.browseButtonOne.Name = "browseButtonOne";
            this.browseButtonOne.Size = new System.Drawing.Size(115, 23);
            this.browseButtonOne.TabIndex = 1;
            this.browseButtonOne.Text = "Browse";
            this.browseButtonOne.UseVisualStyleBackColor = true;
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
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
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
            this.groupBox2.Controls.Add(this.checkBox2);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.radioButton6);
            this.groupBox2.Controls.Add(this.radioButton5);
            this.groupBox2.Controls.Add(this.radioButton4);
            this.groupBox2.Location = new System.Drawing.Point(201, 54);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(383, 95);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Decoy Database Settings";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(11, 19);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(80, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Target Only";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(11, 42);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(80, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Decoy Only";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(11, 65);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(169, 17);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Concatenated Target + Decoy";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(10, 20);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(65, 17);
            this.radioButton4.TabIndex = 3;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "Reverse";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Location = new System.Drawing.Point(10, 43);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(58, 17);
            this.radioButton5.TabIndex = 4;
            this.radioButton5.TabStop = true;
            this.radioButton5.Text = "Shuffle";
            this.radioButton5.UseVisualStyleBackColor = true;
            // 
            // radioButton6
            // 
            this.radioButton6.AutoSize = true;
            this.radioButton6.Location = new System.Drawing.Point(10, 65);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(65, 17);
            this.radioButton6.TabIndex = 5;
            this.radioButton6.TabStop = true;
            this.radioButton6.Text = "Random";
            this.radioButton6.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(88, 21);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(216, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Exclude N-Terminal Amino Acid Residue";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Enabled = false;
            this.checkBox2.Location = new System.Drawing.Point(88, 45);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(216, 17);
            this.checkBox2.TabIndex = 7;
            this.checkBox2.Text = "Only if N-Terminal Residue is Methionine";
            this.checkBox2.UseVisualStyleBackColor = true;
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
            this.groupBox3.Size = new System.Drawing.Size(572, 197);
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
            this.localMachineOutputBrowseButton.Location = new System.Drawing.Point(458, 44);
            this.localMachineOutputBrowseButton.Name = "localMachineOutputBrowseButton";
            this.localMachineOutputBrowseButton.Size = new System.Drawing.Size(107, 23);
            this.localMachineOutputBrowseButton.TabIndex = 7;
            this.localMachineOutputBrowseButton.Text = "Browse";
            this.localMachineOutputBrowseButton.UseVisualStyleBackColor = true;
            // 
            // localMachineOutputDirectoryBox
            // 
            this.localMachineOutputDirectoryBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.localMachineOutputDirectoryBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.localMachineOutputDirectoryBox.Enabled = false;
            this.localMachineOutputDirectoryBox.Location = new System.Drawing.Point(10, 45);
            this.localMachineOutputDirectoryBox.Name = "localMachineOutputDirectoryBox";
            this.localMachineOutputDirectoryBox.Size = new System.Drawing.Size(439, 20);
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
            this.genusTextBox.Location = new System.Drawing.Point(133, 114);
            this.genusTextBox.Name = "genusTextBox";
            this.genusTextBox.Size = new System.Drawing.Size(147, 20);
            this.genusTextBox.TabIndex = 11;
            // 
            // speciesTextBox
            // 
            this.speciesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.speciesTextBox.Enabled = false;
            this.speciesTextBox.Location = new System.Drawing.Point(287, 114);
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
            this.isoformsComboBox.Location = new System.Drawing.Point(11, 164);
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
            this.consortiumTextBox.Size = new System.Drawing.Size(107, 20);
            this.consortiumTextBox.TabIndex = 14;
            // 
            // infoTextBox
            // 
            this.infoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.infoTextBox.Enabled = false;
            this.infoTextBox.Location = new System.Drawing.Point(134, 165);
            this.infoTextBox.Name = "infoTextBox";
            this.infoTextBox.Size = new System.Drawing.Size(315, 20);
            this.infoTextBox.TabIndex = 15;
            // 
            // monthTextBox
            // 
            this.monthTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.monthTextBox.Enabled = false;
            this.monthTextBox.Location = new System.Drawing.Point(459, 165);
            this.monthTextBox.Name = "monthTextBox";
            this.monthTextBox.Size = new System.Drawing.Size(31, 20);
            this.monthTextBox.TabIndex = 16;
            this.monthTextBox.Text = "MM";
            this.monthTextBox.Click += new System.EventHandler(this.monthTextBox_Click);
            // 
            // dayTextBox
            // 
            this.dayTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dayTextBox.Enabled = false;
            this.dayTextBox.Location = new System.Drawing.Point(497, 165);
            this.dayTextBox.Name = "dayTextBox";
            this.dayTextBox.Size = new System.Drawing.Size(31, 20);
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
            this.yearTextBox.Location = new System.Drawing.Point(535, 165);
            this.yearTextBox.Name = "yearTextBox";
            this.yearTextBox.Size = new System.Drawing.Size(31, 20);
            this.yearTextBox.TabIndex = 18;
            this.yearTextBox.Text = "YY";
            this.yearTextBox.Click += new System.EventHandler(this.yearTextBox_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button2.Location = new System.Drawing.Point(10, 358);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(573, 23);
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
            this.isoformLabel.Location = new System.Drawing.Point(9, 145);
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
            this.infoLabel.Location = new System.Drawing.Point(132, 146);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(198, 13);
            this.infoLabel.TabIndex = 7;
            this.infoLabel.Text = "Additional Information (Omit Whitespace)";
            // 
            // downloadLabel
            // 
            this.downloadLabel.AutoSize = true;
            this.downloadLabel.Enabled = false;
            this.downloadLabel.Location = new System.Drawing.Point(457, 146);
            this.downloadLabel.Name = "downloadLabel";
            this.downloadLabel.Size = new System.Drawing.Size(81, 13);
            this.downloadLabel.TabIndex = 25;
            this.downloadLabel.Text = "Download Date";
            // 
            // DatabaseManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 390);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.browseButtonOne);
            this.Controls.Add(this.textBox1);
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

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button browseButtonOne;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton6;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
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
    }
}