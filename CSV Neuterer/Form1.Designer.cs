namespace CsvNeuterer
{
    partial class Form1
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
            this.grpFixedModifications = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLight = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.btnLightMoveLeft = new System.Windows.Forms.Button();
            this.btnLightMoveRight = new System.Windows.Forms.Button();
            this.lstSelectedLightFixedModifications = new System.Windows.Forms.ListBox();
            this.lstAllLightModifications = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabMedium = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMediumMoveLeft = new System.Windows.Forms.Button();
            this.btnMediumMoveRight = new System.Windows.Forms.Button();
            this.lstSelectedMediumFixedModifications = new System.Windows.Forms.ListBox();
            this.lstAllMediumModifications = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabHeavy = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.lstSelectedHeavyFixedModifications = new System.Windows.Forms.ListBox();
            this.lstAllHeavyModifications = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnBrowseMods = new System.Windows.Forms.Button();
            this.ofdModsXml = new System.Windows.Forms.OpenFileDialog();
            this.btnClear = new System.Windows.Forms.Button();
            this.lstOmssaCsvFiles = new System.Windows.Forms.ListBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.ofdOmssaCsvFiles = new System.Windows.Forms.OpenFileDialog();
            this.btnOK = new System.Windows.Forms.Button();
            this.grpFixedModifications.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabLight.SuspendLayout();
            this.tabMedium.SuspendLayout();
            this.tabHeavy.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpFixedModifications
            // 
            this.grpFixedModifications.Controls.Add(this.tabControl1);
            this.grpFixedModifications.Controls.Add(this.btnBrowseMods);
            this.grpFixedModifications.Location = new System.Drawing.Point(12, 205);
            this.grpFixedModifications.Name = "grpFixedModifications";
            this.grpFixedModifications.Size = new System.Drawing.Size(509, 251);
            this.grpFixedModifications.TabIndex = 174;
            this.grpFixedModifications.TabStop = false;
            this.grpFixedModifications.Text = "Fixed Modifications (* = user modification)";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabLight);
            this.tabControl1.Controls.Add(this.tabMedium);
            this.tabControl1.Controls.Add(this.tabHeavy);
            this.tabControl1.Location = new System.Drawing.Point(6, 19);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(492, 188);
            this.tabControl1.TabIndex = 173;
            // 
            // tabLight
            // 
            this.tabLight.Controls.Add(this.label5);
            this.tabLight.Controls.Add(this.btnLightMoveLeft);
            this.tabLight.Controls.Add(this.btnLightMoveRight);
            this.tabLight.Controls.Add(this.lstSelectedLightFixedModifications);
            this.tabLight.Controls.Add(this.lstAllLightModifications);
            this.tabLight.Controls.Add(this.label4);
            this.tabLight.Location = new System.Drawing.Point(4, 22);
            this.tabLight.Name = "tabLight";
            this.tabLight.Padding = new System.Windows.Forms.Padding(3);
            this.tabLight.Size = new System.Drawing.Size(484, 162);
            this.tabLight.TabIndex = 0;
            this.tabLight.Text = "Light";
            this.tabLight.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(242, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 177;
            this.label5.Text = "Selected";
            // 
            // btnLightMoveLeft
            // 
            this.btnLightMoveLeft.Location = new System.Drawing.Point(215, 85);
            this.btnLightMoveLeft.Name = "btnLightMoveLeft";
            this.btnLightMoveLeft.Size = new System.Drawing.Size(21, 23);
            this.btnLightMoveLeft.TabIndex = 176;
            this.btnLightMoveLeft.Text = "<";
            this.btnLightMoveLeft.UseVisualStyleBackColor = true;
            this.btnLightMoveLeft.Click += new System.EventHandler(this.btnLightMoveLeft_Click);
            // 
            // btnLightMoveRight
            // 
            this.btnLightMoveRight.Location = new System.Drawing.Point(215, 56);
            this.btnLightMoveRight.Name = "btnLightMoveRight";
            this.btnLightMoveRight.Size = new System.Drawing.Size(21, 23);
            this.btnLightMoveRight.TabIndex = 175;
            this.btnLightMoveRight.Text = ">";
            this.btnLightMoveRight.UseVisualStyleBackColor = true;
            this.btnLightMoveRight.Click += new System.EventHandler(this.btnLightMoveRight_Click);
            // 
            // lstSelectedLightFixedModifications
            // 
            this.lstSelectedLightFixedModifications.FormattingEnabled = true;
            this.lstSelectedLightFixedModifications.Location = new System.Drawing.Point(242, 28);
            this.lstSelectedLightFixedModifications.Name = "lstSelectedLightFixedModifications";
            this.lstSelectedLightFixedModifications.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSelectedLightFixedModifications.Size = new System.Drawing.Size(200, 108);
            this.lstSelectedLightFixedModifications.TabIndex = 174;
            // 
            // lstAllLightModifications
            // 
            this.lstAllLightModifications.FormattingEnabled = true;
            this.lstAllLightModifications.Location = new System.Drawing.Point(9, 28);
            this.lstAllLightModifications.Name = "lstAllLightModifications";
            this.lstAllLightModifications.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAllLightModifications.Size = new System.Drawing.Size(200, 108);
            this.lstAllLightModifications.Sorted = true;
            this.lstAllLightModifications.TabIndex = 173;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 13);
            this.label4.TabIndex = 172;
            this.label4.Text = "All";
            // 
            // tabMedium
            // 
            this.tabMedium.Controls.Add(this.label1);
            this.tabMedium.Controls.Add(this.btnMediumMoveLeft);
            this.tabMedium.Controls.Add(this.btnMediumMoveRight);
            this.tabMedium.Controls.Add(this.lstSelectedMediumFixedModifications);
            this.tabMedium.Controls.Add(this.lstAllMediumModifications);
            this.tabMedium.Controls.Add(this.label2);
            this.tabMedium.Location = new System.Drawing.Point(4, 22);
            this.tabMedium.Name = "tabMedium";
            this.tabMedium.Padding = new System.Windows.Forms.Padding(3);
            this.tabMedium.Size = new System.Drawing.Size(484, 162);
            this.tabMedium.TabIndex = 1;
            this.tabMedium.Text = "Medium";
            this.tabMedium.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(242, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 183;
            this.label1.Text = "Selected";
            // 
            // btnMediumMoveLeft
            // 
            this.btnMediumMoveLeft.Location = new System.Drawing.Point(215, 85);
            this.btnMediumMoveLeft.Name = "btnMediumMoveLeft";
            this.btnMediumMoveLeft.Size = new System.Drawing.Size(21, 23);
            this.btnMediumMoveLeft.TabIndex = 182;
            this.btnMediumMoveLeft.Text = "<";
            this.btnMediumMoveLeft.UseVisualStyleBackColor = true;
            this.btnMediumMoveLeft.Click += new System.EventHandler(this.btnMediumMoveLeft_Click);
            // 
            // btnMediumMoveRight
            // 
            this.btnMediumMoveRight.Location = new System.Drawing.Point(215, 56);
            this.btnMediumMoveRight.Name = "btnMediumMoveRight";
            this.btnMediumMoveRight.Size = new System.Drawing.Size(21, 23);
            this.btnMediumMoveRight.TabIndex = 181;
            this.btnMediumMoveRight.Text = ">";
            this.btnMediumMoveRight.UseVisualStyleBackColor = true;
            this.btnMediumMoveRight.Click += new System.EventHandler(this.btnMediumMoveRight_Click);
            // 
            // lstSelectedMediumFixedModifications
            // 
            this.lstSelectedMediumFixedModifications.FormattingEnabled = true;
            this.lstSelectedMediumFixedModifications.Location = new System.Drawing.Point(242, 28);
            this.lstSelectedMediumFixedModifications.Name = "lstSelectedMediumFixedModifications";
            this.lstSelectedMediumFixedModifications.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSelectedMediumFixedModifications.Size = new System.Drawing.Size(200, 108);
            this.lstSelectedMediumFixedModifications.TabIndex = 180;
            // 
            // lstAllMediumModifications
            // 
            this.lstAllMediumModifications.FormattingEnabled = true;
            this.lstAllMediumModifications.Location = new System.Drawing.Point(9, 28);
            this.lstAllMediumModifications.Name = "lstAllMediumModifications";
            this.lstAllMediumModifications.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAllMediumModifications.Size = new System.Drawing.Size(200, 108);
            this.lstAllMediumModifications.Sorted = true;
            this.lstAllMediumModifications.TabIndex = 179;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 178;
            this.label2.Text = "All";
            // 
            // tabHeavy
            // 
            this.tabHeavy.Controls.Add(this.label3);
            this.tabHeavy.Controls.Add(this.button3);
            this.tabHeavy.Controls.Add(this.button4);
            this.tabHeavy.Controls.Add(this.lstSelectedHeavyFixedModifications);
            this.tabHeavy.Controls.Add(this.lstAllHeavyModifications);
            this.tabHeavy.Controls.Add(this.label6);
            this.tabHeavy.Location = new System.Drawing.Point(4, 22);
            this.tabHeavy.Name = "tabHeavy";
            this.tabHeavy.Padding = new System.Windows.Forms.Padding(3);
            this.tabHeavy.Size = new System.Drawing.Size(484, 162);
            this.tabHeavy.TabIndex = 2;
            this.tabHeavy.Text = "Heavy";
            this.tabHeavy.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(242, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 183;
            this.label3.Text = "Selected";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(215, 85);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(21, 23);
            this.button3.TabIndex = 182;
            this.button3.Text = "<";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnHeavyMoveLeft_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(215, 56);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(21, 23);
            this.button4.TabIndex = 181;
            this.button4.Text = ">";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.btnHeavyMoveRight_Click);
            // 
            // lstSelectedHeavyFixedModifications
            // 
            this.lstSelectedHeavyFixedModifications.FormattingEnabled = true;
            this.lstSelectedHeavyFixedModifications.Location = new System.Drawing.Point(242, 28);
            this.lstSelectedHeavyFixedModifications.Name = "lstSelectedHeavyFixedModifications";
            this.lstSelectedHeavyFixedModifications.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSelectedHeavyFixedModifications.Size = new System.Drawing.Size(200, 108);
            this.lstSelectedHeavyFixedModifications.TabIndex = 180;
            // 
            // lstAllHeavyModifications
            // 
            this.lstAllHeavyModifications.FormattingEnabled = true;
            this.lstAllHeavyModifications.Location = new System.Drawing.Point(9, 28);
            this.lstAllHeavyModifications.Name = "lstAllHeavyModifications";
            this.lstAllHeavyModifications.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAllHeavyModifications.Size = new System.Drawing.Size(200, 108);
            this.lstAllHeavyModifications.Sorted = true;
            this.lstAllHeavyModifications.TabIndex = 179;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 178;
            this.label6.Text = "All";
            // 
            // btnBrowseMods
            // 
            this.btnBrowseMods.Location = new System.Drawing.Point(11, 222);
            this.btnBrowseMods.Name = "btnBrowseMods";
            this.btnBrowseMods.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseMods.TabIndex = 172;
            this.btnBrowseMods.Text = "Browse";
            this.btnBrowseMods.UseVisualStyleBackColor = true;
            this.btnBrowseMods.Click += new System.EventHandler(this.btnBrowseMods_Click);
            // 
            // ofdModsXml
            // 
            this.ofdModsXml.Filter = "OMSSA mods files (.xml)|*.xml";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(446, 146);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 179;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lstOmssaCsvFiles
            // 
            this.lstOmssaCsvFiles.FormattingEnabled = true;
            this.lstOmssaCsvFiles.HorizontalScrollbar = true;
            this.lstOmssaCsvFiles.Location = new System.Drawing.Point(12, 32);
            this.lstOmssaCsvFiles.Name = "lstOmssaCsvFiles";
            this.lstOmssaCsvFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstOmssaCsvFiles.Size = new System.Drawing.Size(509, 108);
            this.lstOmssaCsvFiles.TabIndex = 178;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(229, 146);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 177;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(253, 13);
            this.label7.TabIndex = 175;
            this.label7.Text = "OMSSA Comma-Separated Value Output Files (.csv)";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 146);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 176;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // ofdOmssaCsvFiles
            // 
            this.ofdOmssaCsvFiles.Filter = "OMSSA comma-separated value output files (.csv)|*.csv";
            this.ofdOmssaCsvFiles.Multiselect = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 481);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 174;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 516);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lstOmssaCsvFiles);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.grpFixedModifications);
            this.Name = "Form1";
            this.Text = "CSV Neuterer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.grpFixedModifications.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabLight.ResumeLayout(false);
            this.tabLight.PerformLayout();
            this.tabMedium.ResumeLayout(false);
            this.tabMedium.PerformLayout();
            this.tabHeavy.ResumeLayout(false);
            this.tabHeavy.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpFixedModifications;
        private System.Windows.Forms.Button btnBrowseMods;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabLight;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnLightMoveLeft;
        private System.Windows.Forms.Button btnLightMoveRight;
        private System.Windows.Forms.ListBox lstSelectedLightFixedModifications;
        private System.Windows.Forms.ListBox lstAllLightModifications;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabMedium;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMediumMoveLeft;
        private System.Windows.Forms.Button btnMediumMoveRight;
        private System.Windows.Forms.ListBox lstSelectedMediumFixedModifications;
        private System.Windows.Forms.ListBox lstAllMediumModifications;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabHeavy;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListBox lstSelectedHeavyFixedModifications;
        private System.Windows.Forms.ListBox lstAllHeavyModifications;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.OpenFileDialog ofdModsXml;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ListBox lstOmssaCsvFiles;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.OpenFileDialog ofdOmssaCsvFiles;
        private System.Windows.Forms.Button btnOK;
    }
}

