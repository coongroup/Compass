namespace Coon.Compass.DatabaseMaker
{
    partial class DatabaseMakerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseMakerForm));
            this.fastaD = new System.Windows.Forms.OpenFileDialog();
            this.btnBrowseFasta = new System.Windows.Forms.Button();
            this.radTarget = new System.Windows.Forms.RadioButton();
            this.radDecoy = new System.Windows.Forms.RadioButton();
            this.radConcatenated = new System.Windows.Forms.RadioButton();
            this.grpDatabaseType = new System.Windows.Forms.GroupBox();
            this.grpDecoyDatabaseMethod = new System.Windows.Forms.GroupBox();
            this.chkOnlyIfNTerminusIsMethionine = new System.Windows.Forms.CheckBox();
            this.chkExcludeNTerminus = new System.Windows.Forms.CheckBox();
            this.radRandom = new System.Windows.Forms.RadioButton();
            this.radReverse = new System.Windows.Forms.RadioButton();
            this.radShuffle = new System.Windows.Forms.RadioButton();
            this.chkBlast = new System.Windows.Forms.CheckBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.outputD = new System.Windows.Forms.FolderBrowserDialog();
            this.fastaLB = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.clearBtn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.grpDatabaseType.SuspendLayout();
            this.grpDecoyDatabaseMethod.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // fastaD
            // 
            this.fastaD.DefaultExt = "fasta";
            this.fastaD.Filter = "FASTA protein database files|*.fa*|All Files|*.*";
            this.fastaD.Multiselect = true;
            this.fastaD.Title = "Add FASTA Protein Databases";
            // 
            // btnBrowseFasta
            // 
            this.btnBrowseFasta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseFasta.Location = new System.Drawing.Point(401, 81);
            this.btnBrowseFasta.Name = "btnBrowseFasta";
            this.btnBrowseFasta.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFasta.TabIndex = 2;
            this.btnBrowseFasta.Text = "Add";
            this.btnBrowseFasta.UseVisualStyleBackColor = true;
            this.btnBrowseFasta.Click += new System.EventHandler(this.btnBrowseFasta_Click);
            // 
            // radTarget
            // 
            this.radTarget.AutoSize = true;
            this.radTarget.Location = new System.Drawing.Point(6, 19);
            this.radTarget.Name = "radTarget";
            this.radTarget.Size = new System.Drawing.Size(80, 17);
            this.radTarget.TabIndex = 3;
            this.radTarget.Text = "Target Only";
            this.radTarget.UseVisualStyleBackColor = true;
            this.radTarget.CheckedChanged += new System.EventHandler(this.radTarget_CheckedChanged);
            // 
            // radDecoy
            // 
            this.radDecoy.AutoSize = true;
            this.radDecoy.Location = new System.Drawing.Point(6, 42);
            this.radDecoy.Name = "radDecoy";
            this.radDecoy.Size = new System.Drawing.Size(80, 17);
            this.radDecoy.TabIndex = 4;
            this.radDecoy.Text = "Decoy Only";
            this.radDecoy.UseVisualStyleBackColor = true;
            this.radDecoy.CheckedChanged += new System.EventHandler(this.radDecoy_CheckedChanged);
            // 
            // radConcatenated
            // 
            this.radConcatenated.AutoSize = true;
            this.radConcatenated.Checked = true;
            this.radConcatenated.Location = new System.Drawing.Point(5, 65);
            this.radConcatenated.Name = "radConcatenated";
            this.radConcatenated.Size = new System.Drawing.Size(169, 17);
            this.radConcatenated.TabIndex = 5;
            this.radConcatenated.TabStop = true;
            this.radConcatenated.Text = "Concatenated Target + Decoy";
            this.radConcatenated.UseVisualStyleBackColor = true;
            this.radConcatenated.CheckedChanged += new System.EventHandler(this.radConcatenated_CheckedChanged);
            // 
            // grpDatabaseType
            // 
            this.grpDatabaseType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpDatabaseType.Controls.Add(this.radTarget);
            this.grpDatabaseType.Controls.Add(this.radConcatenated);
            this.grpDatabaseType.Controls.Add(this.radDecoy);
            this.grpDatabaseType.Location = new System.Drawing.Point(8, 133);
            this.grpDatabaseType.Name = "grpDatabaseType";
            this.grpDatabaseType.Size = new System.Drawing.Size(180, 91);
            this.grpDatabaseType.TabIndex = 6;
            this.grpDatabaseType.TabStop = false;
            this.grpDatabaseType.Text = "Database Type";
            // 
            // grpDecoyDatabaseMethod
            // 
            this.grpDecoyDatabaseMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDecoyDatabaseMethod.Controls.Add(this.chkOnlyIfNTerminusIsMethionine);
            this.grpDecoyDatabaseMethod.Controls.Add(this.chkExcludeNTerminus);
            this.grpDecoyDatabaseMethod.Controls.Add(this.radRandom);
            this.grpDecoyDatabaseMethod.Controls.Add(this.radReverse);
            this.grpDecoyDatabaseMethod.Controls.Add(this.radShuffle);
            this.grpDecoyDatabaseMethod.Location = new System.Drawing.Point(189, 133);
            this.grpDecoyDatabaseMethod.Name = "grpDecoyDatabaseMethod";
            this.grpDecoyDatabaseMethod.Size = new System.Drawing.Size(301, 91);
            this.grpDecoyDatabaseMethod.TabIndex = 7;
            this.grpDecoyDatabaseMethod.TabStop = false;
            this.grpDecoyDatabaseMethod.Text = "Decoy Database Method";
            // 
            // chkOnlyIfNTerminusIsMethionine
            // 
            this.chkOnlyIfNTerminusIsMethionine.AutoSize = true;
            this.chkOnlyIfNTerminusIsMethionine.Enabled = false;
            this.chkOnlyIfNTerminusIsMethionine.Location = new System.Drawing.Point(79, 42);
            this.chkOnlyIfNTerminusIsMethionine.Name = "chkOnlyIfNTerminusIsMethionine";
            this.chkOnlyIfNTerminusIsMethionine.Size = new System.Drawing.Size(218, 17);
            this.chkOnlyIfNTerminusIsMethionine.TabIndex = 8;
            this.chkOnlyIfNTerminusIsMethionine.Text = "Only If N-Terminal Residue Is Methionine";
            this.chkOnlyIfNTerminusIsMethionine.UseVisualStyleBackColor = true;
            // 
            // chkExcludeNTerminus
            // 
            this.chkExcludeNTerminus.AutoSize = true;
            this.chkExcludeNTerminus.Location = new System.Drawing.Point(79, 20);
            this.chkExcludeNTerminus.Name = "chkExcludeNTerminus";
            this.chkExcludeNTerminus.Size = new System.Drawing.Size(216, 17);
            this.chkExcludeNTerminus.TabIndex = 7;
            this.chkExcludeNTerminus.Text = "Exclude N-Terminal Amino Acid Residue";
            this.chkExcludeNTerminus.UseVisualStyleBackColor = true;
            this.chkExcludeNTerminus.CheckedChanged += new System.EventHandler(this.chkExcludeNTerminus_CheckedChanged);
            // 
            // radRandom
            // 
            this.radRandom.AutoSize = true;
            this.radRandom.Location = new System.Drawing.Point(6, 65);
            this.radRandom.Name = "radRandom";
            this.radRandom.Size = new System.Drawing.Size(65, 17);
            this.radRandom.TabIndex = 6;
            this.radRandom.TabStop = true;
            this.radRandom.Text = "Random";
            this.radRandom.UseVisualStyleBackColor = true;
            // 
            // radReverse
            // 
            this.radReverse.AutoSize = true;
            this.radReverse.Checked = true;
            this.radReverse.Location = new System.Drawing.Point(6, 19);
            this.radReverse.Name = "radReverse";
            this.radReverse.Size = new System.Drawing.Size(65, 17);
            this.radReverse.TabIndex = 3;
            this.radReverse.TabStop = true;
            this.radReverse.Text = "Reverse";
            this.radReverse.UseVisualStyleBackColor = true;
            // 
            // radShuffle
            // 
            this.radShuffle.AutoSize = true;
            this.radShuffle.Location = new System.Drawing.Point(6, 42);
            this.radShuffle.Name = "radShuffle";
            this.radShuffle.Size = new System.Drawing.Size(58, 17);
            this.radShuffle.TabIndex = 5;
            this.radShuffle.TabStop = true;
            this.radShuffle.Text = "Shuffle";
            this.radShuffle.UseVisualStyleBackColor = true;
            // 
            // chkBlast
            // 
            this.chkBlast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkBlast.AutoSize = true;
            this.chkBlast.Checked = true;
            this.chkBlast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBlast.Location = new System.Drawing.Point(6, 51);
            this.chkBlast.Name = "chkBlast";
            this.chkBlast.Size = new System.Drawing.Size(223, 17);
            this.chkBlast.TabIndex = 8;
            this.chkBlast.Text = "Create BLAST Database Files for OMSSA";
            this.chkBlast.UseVisualStyleBackColor = true;
            // 
            // btnBrowseOutput
            // 
            this.btnBrowseOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOutput.Location = new System.Drawing.Point(401, 45);
            this.btnBrowseOutput.Name = "btnBrowseOutput";
            this.btnBrowseOutput.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseOutput.TabIndex = 11;
            this.btnBrowseOutput.Text = "Browse";
            this.btnBrowseOutput.UseVisualStyleBackColor = true;
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(6, 19);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(470, 20);
            this.txtOutput.TabIndex = 9;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(411, 315);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "Create";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // fastaLB
            // 
            this.fastaLB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fastaLB.FormattingEnabled = true;
            this.fastaLB.HorizontalScrollbar = true;
            this.fastaLB.Location = new System.Drawing.Point(6, 19);
            this.fastaLB.Name = "fastaLB";
            this.fastaLB.Size = new System.Drawing.Size(470, 56);
            this.fastaLB.TabIndex = 13;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.clearBtn);
            this.groupBox1.Controls.Add(this.fastaLB);
            this.groupBox1.Controls.Add(this.btnBrowseFasta);
            this.groupBox1.Location = new System.Drawing.Point(8, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(482, 115);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FASTA Protein Database Files";
            // 
            // clearBtn
            // 
            this.clearBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearBtn.Location = new System.Drawing.Point(320, 81);
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(75, 23);
            this.clearBtn.TabIndex = 15;
            this.clearBtn.Text = "Clear";
            this.clearBtn.UseVisualStyleBackColor = true;
            this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtOutput);
            this.groupBox2.Controls.Add(this.btnBrowseOutput);
            this.groupBox2.Controls.Add(this.chkBlast);
            this.groupBox2.Location = new System.Drawing.Point(8, 230);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(482, 79);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output Folder";
            // 
            // DatabaseMakerForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 345);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpDecoyDatabaseMethod);
            this.Controls.Add(this.grpDatabaseType);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DatabaseMakerForm";
            this.Text = "Database Maker";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
            this.grpDatabaseType.ResumeLayout(false);
            this.grpDatabaseType.PerformLayout();
            this.grpDecoyDatabaseMethod.ResumeLayout(false);
            this.grpDecoyDatabaseMethod.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog fastaD;
        private System.Windows.Forms.Button btnBrowseFasta;
        private System.Windows.Forms.RadioButton radTarget;
        private System.Windows.Forms.RadioButton radDecoy;
        private System.Windows.Forms.RadioButton radConcatenated;
        private System.Windows.Forms.GroupBox grpDatabaseType;
        private System.Windows.Forms.GroupBox grpDecoyDatabaseMethod;
        private System.Windows.Forms.RadioButton radReverse;
        private System.Windows.Forms.RadioButton radShuffle;
        private System.Windows.Forms.RadioButton radRandom;
        private System.Windows.Forms.CheckBox chkBlast;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.FolderBrowserDialog outputD;
        private System.Windows.Forms.CheckBox chkExcludeNTerminus;
        private System.Windows.Forms.CheckBox chkOnlyIfNTerminusIsMethionine;
        private System.Windows.Forms.ListBox fastaLB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button clearBtn;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

