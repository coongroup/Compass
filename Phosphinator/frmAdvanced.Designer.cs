namespace Phosphinator
{
    partial class frmAdvanced
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
            this.grpQuantitation = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numPrecursorInterferenceThreshold = new System.Windows.Forms.NumericUpDown();
            this.chkEliminatePrecursorInterference = new System.Windows.Forms.CheckBox();
            this.grpMotifX = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnBrowseFasta = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numWindowSize = new System.Windows.Forms.NumericUpDown();
            this.txtFasta = new System.Windows.Forms.TextBox();
            this.chkMotifXOutputs = new System.Windows.Forms.CheckBox();
            this.ofdFasta = new System.Windows.Forms.OpenFileDialog();
            this.btnOK = new System.Windows.Forms.Button();
            this.grpQuantitation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecursorInterferenceThreshold)).BeginInit();
            this.grpMotifX.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWindowSize)).BeginInit();
            this.SuspendLayout();
            // 
            // grpQuantitation
            // 
            this.grpQuantitation.Controls.Add(this.label5);
            this.grpQuantitation.Controls.Add(this.label2);
            this.grpQuantitation.Controls.Add(this.numPrecursorInterferenceThreshold);
            this.grpQuantitation.Controls.Add(this.chkEliminatePrecursorInterference);
            this.grpQuantitation.Location = new System.Drawing.Point(12, 12);
            this.grpQuantitation.Name = "grpQuantitation";
            this.grpQuantitation.Size = new System.Drawing.Size(454, 70);
            this.grpQuantitation.TabIndex = 0;
            this.grpQuantitation.TabStop = false;
            this.grpQuantitation.Text = "Quantitation";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(260, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 13);
            this.label5.TabIndex = 47;
            this.label5.Text = "%";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(217, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(231, 13);
            this.label2.TabIndex = 46;
            this.label2.Text = "Threshold Relative to Maximum Precursor Peak";
            // 
            // numPrecursorInterferenceThreshold
            // 
            this.numPrecursorInterferenceThreshold.Location = new System.Drawing.Point(220, 38);
            this.numPrecursorInterferenceThreshold.Name = "numPrecursorInterferenceThreshold";
            this.numPrecursorInterferenceThreshold.Size = new System.Drawing.Size(40, 20);
            this.numPrecursorInterferenceThreshold.TabIndex = 44;
            this.numPrecursorInterferenceThreshold.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // chkEliminatePrecursorInterference
            // 
            this.chkEliminatePrecursorInterference.AutoSize = true;
            this.chkEliminatePrecursorInterference.Enabled = false;
            this.chkEliminatePrecursorInterference.Location = new System.Drawing.Point(9, 28);
            this.chkEliminatePrecursorInterference.Name = "chkEliminatePrecursorInterference";
            this.chkEliminatePrecursorInterference.Size = new System.Drawing.Size(176, 17);
            this.chkEliminatePrecursorInterference.TabIndex = 43;
            this.chkEliminatePrecursorInterference.Text = "Eliminate Precursor Interference";
            this.chkEliminatePrecursorInterference.UseVisualStyleBackColor = true;
            this.chkEliminatePrecursorInterference.CheckedChanged += new System.EventHandler(this.chkEliminatePrecursorInterference_CheckedChanged);
            // 
            // grpMotifX
            // 
            this.grpMotifX.Controls.Add(this.label4);
            this.grpMotifX.Controls.Add(this.btnBrowseFasta);
            this.grpMotifX.Controls.Add(this.label3);
            this.grpMotifX.Controls.Add(this.numWindowSize);
            this.grpMotifX.Controls.Add(this.txtFasta);
            this.grpMotifX.Controls.Add(this.chkMotifXOutputs);
            this.grpMotifX.Location = new System.Drawing.Point(12, 88);
            this.grpMotifX.Name = "grpMotifX";
            this.grpMotifX.Size = new System.Drawing.Size(454, 100);
            this.grpMotifX.TabIndex = 1;
            this.grpMotifX.TabStop = false;
            this.grpMotifX.Text = "Motif-X";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 13);
            this.label4.TabIndex = 47;
            this.label4.Text = "FASTA Protein Database Filepath";
            // 
            // btnBrowseFasta
            // 
            this.btnBrowseFasta.Enabled = false;
            this.btnBrowseFasta.Location = new System.Drawing.Point(373, 63);
            this.btnBrowseFasta.Name = "btnBrowseFasta";
            this.btnBrowseFasta.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFasta.TabIndex = 46;
            this.btnBrowseFasta.Text = "Browse";
            this.btnBrowseFasta.UseVisualStyleBackColor = true;
            this.btnBrowseFasta.Click += new System.EventHandler(this.btnBrowseFasta_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(326, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 45;
            this.label3.Text = "Window Size";
            // 
            // numWindowSize
            // 
            this.numWindowSize.Enabled = false;
            this.numWindowSize.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numWindowSize.Location = new System.Drawing.Point(401, 13);
            this.numWindowSize.Maximum = new decimal(new int[] {
            101,
            0,
            0,
            0});
            this.numWindowSize.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numWindowSize.Name = "numWindowSize";
            this.numWindowSize.Size = new System.Drawing.Size(47, 20);
            this.numWindowSize.TabIndex = 44;
            this.numWindowSize.Value = new decimal(new int[] {
            51,
            0,
            0,
            0});
            // 
            // txtFasta
            // 
            this.txtFasta.Enabled = false;
            this.txtFasta.Location = new System.Drawing.Point(9, 65);
            this.txtFasta.Name = "txtFasta";
            this.txtFasta.Size = new System.Drawing.Size(358, 20);
            this.txtFasta.TabIndex = 42;
            // 
            // chkMotifXOutputs
            // 
            this.chkMotifXOutputs.AutoSize = true;
            this.chkMotifXOutputs.Location = new System.Drawing.Point(9, 19);
            this.chkMotifXOutputs.Name = "chkMotifXOutputs";
            this.chkMotifXOutputs.Size = new System.Drawing.Size(127, 17);
            this.chkMotifXOutputs.TabIndex = 41;
            this.chkMotifXOutputs.Text = "Write Motif-X Outputs";
            this.chkMotifXOutputs.UseVisualStyleBackColor = true;
            this.chkMotifXOutputs.CheckedChanged += new System.EventHandler(this.chkMotifXOutputs_CheckedChanged);
            // 
            // ofdFasta
            // 
            this.ofdFasta.Filter = "FASTA proteome database files (.fa, .mpfa, .fna, .fsa, .fas, .fasta)|*.fa;*.mpfa;" +
    "*.fna;*.fsa;*.fas;*.fasta";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 194);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 47;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmAdvanced
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 229);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpMotifX);
            this.Controls.Add(this.grpQuantitation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmAdvanced";
            this.Text = "Phosphinator - Advanced Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAdvanced_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmAdvanced_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmAdvanced_DragEnter);
            this.grpQuantitation.ResumeLayout(false);
            this.grpQuantitation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecursorInterferenceThreshold)).EndInit();
            this.grpMotifX.ResumeLayout(false);
            this.grpMotifX.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWindowSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpQuantitation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numPrecursorInterferenceThreshold;
        private System.Windows.Forms.CheckBox chkEliminatePrecursorInterference;
        private System.Windows.Forms.GroupBox grpMotifX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numWindowSize;
        private System.Windows.Forms.TextBox txtFasta;
        private System.Windows.Forms.CheckBox chkMotifXOutputs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnBrowseFasta;
        private System.Windows.Forms.OpenFileDialog ofdFasta;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label5;

    }
}