namespace Compass
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.coonResearchGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crgMainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crgSoftwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.universityOfWisconsinMadisonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uwmMainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uwmDepartmentOfChemistryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutCOMPASSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbDatabaseMaker = new System.Windows.Forms.ToolStripButton();
            this.tsbDtaGenerator = new System.Windows.Forms.ToolStripButton();
            this.tsbCoondornator = new System.Windows.Forms.ToolStripButton();
            this.tsddbFdrOptimizer = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiNonBatchFdrOptimizers = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLowResFdrOptimizer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFdrOptimizer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBatchFdrOptimizers = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBatchLowResFdrOptimizer = new System.Windows.Forms.ToolStripMenuItem();
            this.highresolutionMS1ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbTagQuant = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowsToolStripMenuItem,
            this.linksToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(984, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cascadeToolStripMenuItem,
            this.closeAllToolStripMenuItem});
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.windowsToolStripMenuItem.Text = "&Windows";
            // 
            // cascadeToolStripMenuItem
            // 
            this.cascadeToolStripMenuItem.Name = "cascadeToolStripMenuItem";
            this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.cascadeToolStripMenuItem.Text = "&Cascade";
            this.cascadeToolStripMenuItem.Click += new System.EventHandler(this.cascadeToolStripMenuItem_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.closeAllToolStripMenuItem.Text = "C&lose All";
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.closeAllToolStripMenuItem_Click);
            // 
            // linksToolStripMenuItem
            // 
            this.linksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.coonResearchGroupToolStripMenuItem,
            this.universityOfWisconsinMadisonToolStripMenuItem});
            this.linksToolStripMenuItem.Name = "linksToolStripMenuItem";
            this.linksToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.linksToolStripMenuItem.Text = "&Links";
            // 
            // coonResearchGroupToolStripMenuItem
            // 
            this.coonResearchGroupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.crgMainToolStripMenuItem,
            this.crgSoftwareToolStripMenuItem});
            this.coonResearchGroupToolStripMenuItem.Name = "coonResearchGroupToolStripMenuItem";
            this.coonResearchGroupToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.coonResearchGroupToolStripMenuItem.Text = "&Coon Research Group";
            // 
            // crgMainToolStripMenuItem
            // 
            this.crgMainToolStripMenuItem.Name = "crgMainToolStripMenuItem";
            this.crgMainToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.crgMainToolStripMenuItem.Text = "&Main";
            this.crgMainToolStripMenuItem.Click += new System.EventHandler(this.crgMainToolStripMenuItem_Click);
            // 
            // crgSoftwareToolStripMenuItem
            // 
            this.crgSoftwareToolStripMenuItem.Name = "crgSoftwareToolStripMenuItem";
            this.crgSoftwareToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.crgSoftwareToolStripMenuItem.Text = "&Software";
            this.crgSoftwareToolStripMenuItem.Click += new System.EventHandler(this.crgSoftwareToolStripMenuItem_Click);
            // 
            // universityOfWisconsinMadisonToolStripMenuItem
            // 
            this.universityOfWisconsinMadisonToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uwmMainToolStripMenuItem,
            this.uwmDepartmentOfChemistryToolStripMenuItem});
            this.universityOfWisconsinMadisonToolStripMenuItem.Name = "universityOfWisconsinMadisonToolStripMenuItem";
            this.universityOfWisconsinMadisonToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.universityOfWisconsinMadisonToolStripMenuItem.Text = "&University of Wisconsin–Madison";
            // 
            // uwmMainToolStripMenuItem
            // 
            this.uwmMainToolStripMenuItem.Name = "uwmMainToolStripMenuItem";
            this.uwmMainToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.uwmMainToolStripMenuItem.Text = "&Main";
            this.uwmMainToolStripMenuItem.Click += new System.EventHandler(this.uwmMainToolStripMenuItem_Click);
            // 
            // uwmDepartmentOfChemistryToolStripMenuItem
            // 
            this.uwmDepartmentOfChemistryToolStripMenuItem.Name = "uwmDepartmentOfChemistryToolStripMenuItem";
            this.uwmDepartmentOfChemistryToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.uwmDepartmentOfChemistryToolStripMenuItem.Text = "&Department of Chemistry";
            this.uwmDepartmentOfChemistryToolStripMenuItem.Click += new System.EventHandler(this.uwmDepartmentOfChemistryToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutCOMPASSToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutCOMPASSToolStripMenuItem
            // 
            this.aboutCOMPASSToolStripMenuItem.Name = "aboutCOMPASSToolStripMenuItem";
            this.aboutCOMPASSToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.aboutCOMPASSToolStripMenuItem.Text = "&About COMPASS";
            this.aboutCOMPASSToolStripMenuItem.Click += new System.EventHandler(this.aboutCompassToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbDatabaseMaker,
            this.tsbDtaGenerator,
            this.tsbCoondornator,
            this.tsddbFdrOptimizer,
            this.tsbTagQuant,
            this.toolStripButton1,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(984, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbDatabaseMaker
            // 
            this.tsbDatabaseMaker.Image = ((System.Drawing.Image)(resources.GetObject("tsbDatabaseMaker.Image")));
            this.tsbDatabaseMaker.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDatabaseMaker.Name = "tsbDatabaseMaker";
            this.tsbDatabaseMaker.Size = new System.Drawing.Size(111, 22);
            this.tsbDatabaseMaker.Text = "Database Maker";
            this.tsbDatabaseMaker.Click += new System.EventHandler(this.tsbDatabaseMaker_Click);
            // 
            // tsbDtaGenerator
            // 
            this.tsbDtaGenerator.Image = ((System.Drawing.Image)(resources.GetObject("tsbDtaGenerator.Image")));
            this.tsbDtaGenerator.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDtaGenerator.Name = "tsbDtaGenerator";
            this.tsbDtaGenerator.Size = new System.Drawing.Size(105, 22);
            this.tsbDtaGenerator.Text = "DTA Generator";
            this.tsbDtaGenerator.Click += new System.EventHandler(this.tsbDtaGenerator_Click);
            // 
            // tsbCoondornator
            // 
            this.tsbCoondornator.Image = ((System.Drawing.Image)(resources.GetObject("tsbCoondornator.Image")));
            this.tsbCoondornator.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCoondornator.Name = "tsbCoondornator";
            this.tsbCoondornator.Size = new System.Drawing.Size(102, 22);
            this.tsbCoondornator.Text = "Coondornator";
            this.tsbCoondornator.Click += new System.EventHandler(this.tsbCoondornator_Click);
            // 
            // tsddbFdrOptimizer
            // 
            this.tsddbFdrOptimizer.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNonBatchFdrOptimizers,
            this.tsmiBatchFdrOptimizers});
            this.tsddbFdrOptimizer.Image = ((System.Drawing.Image)(resources.GetObject("tsddbFdrOptimizer.Image")));
            this.tsddbFdrOptimizer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbFdrOptimizer.Name = "tsddbFdrOptimizer";
            this.tsddbFdrOptimizer.Size = new System.Drawing.Size(112, 22);
            this.tsddbFdrOptimizer.Text = "FDR Optimizer";
            // 
            // tsmiNonBatchFdrOptimizers
            // 
            this.tsmiNonBatchFdrOptimizers.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLowResFdrOptimizer,
            this.tsmiFdrOptimizer});
            this.tsmiNonBatchFdrOptimizers.Name = "tsmiNonBatchFdrOptimizers";
            this.tsmiNonBatchFdrOptimizers.Size = new System.Drawing.Size(130, 22);
            this.tsmiNonBatchFdrOptimizers.Text = "non-batch";
            // 
            // tsmiLowResFdrOptimizer
            // 
            this.tsmiLowResFdrOptimizer.Name = "tsmiLowResFdrOptimizer";
            this.tsmiLowResFdrOptimizer.Size = new System.Drawing.Size(147, 22);
            this.tsmiLowResFdrOptimizer.Text = "non-FT MS^1";
            this.tsmiLowResFdrOptimizer.Click += new System.EventHandler(this.tsmiLowResFdrOptimizer_Click);
            // 
            // tsmiFdrOptimizer
            // 
            this.tsmiFdrOptimizer.Name = "tsmiFdrOptimizer";
            this.tsmiFdrOptimizer.Size = new System.Drawing.Size(147, 22);
            this.tsmiFdrOptimizer.Text = "FT MS^1";
            this.tsmiFdrOptimizer.Click += new System.EventHandler(this.tsmiFdrOptimizer_Click);
            // 
            // tsmiBatchFdrOptimizers
            // 
            this.tsmiBatchFdrOptimizers.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiBatchLowResFdrOptimizer,
            this.highresolutionMS1ToolStripMenuItem1});
            this.tsmiBatchFdrOptimizers.Name = "tsmiBatchFdrOptimizers";
            this.tsmiBatchFdrOptimizers.Size = new System.Drawing.Size(130, 22);
            this.tsmiBatchFdrOptimizers.Text = "batch";
            // 
            // tsmiBatchLowResFdrOptimizer
            // 
            this.tsmiBatchLowResFdrOptimizer.Name = "tsmiBatchLowResFdrOptimizer";
            this.tsmiBatchLowResFdrOptimizer.Size = new System.Drawing.Size(147, 22);
            this.tsmiBatchLowResFdrOptimizer.Text = "non-FT MS^1";
            this.tsmiBatchLowResFdrOptimizer.Click += new System.EventHandler(this.tsmiBatchLowResFdrOptimizer_Click);
            // 
            // highresolutionMS1ToolStripMenuItem1
            // 
            this.highresolutionMS1ToolStripMenuItem1.Name = "highresolutionMS1ToolStripMenuItem1";
            this.highresolutionMS1ToolStripMenuItem1.Size = new System.Drawing.Size(147, 22);
            this.highresolutionMS1ToolStripMenuItem1.Text = "FT MS^1";
            this.highresolutionMS1ToolStripMenuItem1.Click += new System.EventHandler(this.tsmiBatchFdrOptimizer_Click);
            // 
            // tsbTagQuant
            // 
            this.tsbTagQuant.Image = ((System.Drawing.Image)(resources.GetObject("tsbTagQuant.Image")));
            this.tsbTagQuant.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbTagQuant.Name = "tsbTagQuant";
            this.tsbTagQuant.Size = new System.Drawing.Size(80, 22);
            this.tsbTagQuant.Text = "TagQuant";
            this.tsbTagQuant.Click += new System.EventHandler(this.tsbTagQuant_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(111, 22);
            this.toolStripButton1.Text = "Protein Hoarder";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::Compass.Properties.Resources.coon_blue;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(55, 22);
            this.toolStripButton2.Text = "Lotor";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 762);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "frmMain";
            this.Text = "COMPASS (internal)";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbDatabaseMaker;
        private System.Windows.Forms.ToolStripButton tsbDtaGenerator;
        private System.Windows.Forms.ToolStripButton tsbCoondornator;
        private System.Windows.Forms.ToolStripDropDownButton tsddbFdrOptimizer;
        private System.Windows.Forms.ToolStripMenuItem tsmiNonBatchFdrOptimizers;
        private System.Windows.Forms.ToolStripMenuItem tsmiBatchFdrOptimizers;
        private System.Windows.Forms.ToolStripButton tsbTagQuant;
        private System.Windows.Forms.ToolStripMenuItem linksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiLowResFdrOptimizer;
        private System.Windows.Forms.ToolStripMenuItem tsmiFdrOptimizer;
        private System.Windows.Forms.ToolStripMenuItem tsmiBatchLowResFdrOptimizer;
        private System.Windows.Forms.ToolStripMenuItem highresolutionMS1ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem coonResearchGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem universityOfWisconsinMadisonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem crgSoftwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uwmDepartmentOfChemistryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutCOMPASSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cascadeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem crgMainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uwmMainToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;

    }
}