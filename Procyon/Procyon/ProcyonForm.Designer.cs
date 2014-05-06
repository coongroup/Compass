namespace Coon.Compass.Procyon
{
    partial class ProcyonForm
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
            this.inputFiles = new System.Windows.Forms.ListBox();
            this.headerList = new System.Windows.Forms.ListBox();
            this.addToOne = new System.Windows.Forms.Button();
            this.clearInputFile = new System.Windows.Forms.Button();
            this.inputLabel = new System.Windows.Forms.Label();
            this.headerLabel = new System.Windows.Forms.Label();
            this.groupDataGrid = new System.Windows.Forms.DataGridView();
            this.headerComboBox = new System.Windows.Forms.ComboBox();
            this.analyze = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.headersToPrintListBox = new System.Windows.Forms.CheckedListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.headerComboBox2 = new System.Windows.Forms.ComboBox();
            this.normalizationComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.sigTestingComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.sigThershold = new System.Windows.Forms.TextBox();
            this.thresholdType = new System.Windows.Forms.ComboBox();
            this.multCompCorr = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.goTermsCheckListBox = new System.Windows.Forms.CheckedListBox();
            this.label8 = new System.Windows.Forms.Label();
            this.annotationComboBox = new System.Windows.Forms.ComboBox();
            this.goDatabaseComboBox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.userDatabase = new System.Windows.Forms.CheckBox();
            this.annotationSigTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.clearAnnotaionDatabase = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.perseusOutput = new System.Windows.Forms.CheckBox();
            this.comparisonDataGridView = new System.Windows.Forms.DataGridView();
            this.label14 = new System.Windows.Forms.Label();
            this.buildComparisons = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.uniprotIDHeader = new System.Windows.Forms.ComboBox();
            this.clearNormTextBox = new System.Windows.Forms.Button();
            this.normalizationTextBox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.clearAnalysisGroups = new System.Windows.Forms.Button();
            this.sortAnalysisGroups = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.cleanDefline = new System.Windows.Forms.CheckBox();
            this.medianNorm = new System.Windows.Forms.RadioButton();
            this.sumNorm = new System.Windows.Forms.RadioButton();
            this.customDatabaseText = new System.Windows.Forms.TextBox();
            this.noNorm = new System.Windows.Forms.RadioButton();
            this.outputFolderTextBox = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.meanNormFoldChange = new System.Windows.Forms.RadioButton();
            this.meanNormLog2 = new System.Windows.Forms.RadioButton();
            this.intensityFoldChange = new System.Windows.Forms.RadioButton();
            this.intensityLog2 = new System.Windows.Forms.RadioButton();
            this.logBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.groupDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comparisonDataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputFiles
            // 
            this.inputFiles.AllowDrop = true;
            this.inputFiles.FormattingEnabled = true;
            this.inputFiles.Location = new System.Drawing.Point(12, 24);
            this.inputFiles.Name = "inputFiles";
            this.inputFiles.Size = new System.Drawing.Size(791, 43);
            this.inputFiles.TabIndex = 0;
            this.inputFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.inputFiles_DragDrop);
            this.inputFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.inputFiles_DragEnter);
            // 
            // headerList
            // 
            this.headerList.FormattingEnabled = true;
            this.headerList.Location = new System.Drawing.Point(12, 124);
            this.headerList.Name = "headerList";
            this.headerList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.headerList.Size = new System.Drawing.Size(111, 186);
            this.headerList.TabIndex = 1;
            // 
            // addToOne
            // 
            this.addToOne.Location = new System.Drawing.Point(12, 313);
            this.addToOne.Name = "addToOne";
            this.addToOne.Size = new System.Drawing.Size(111, 23);
            this.addToOne.TabIndex = 5;
            this.addToOne.Text = "Add Columns";
            this.addToOne.UseVisualStyleBackColor = true;
            this.addToOne.Click += new System.EventHandler(this.addToOne_Click);
            // 
            // clearInputFile
            // 
            this.clearInputFile.Location = new System.Drawing.Point(728, 73);
            this.clearInputFile.Name = "clearInputFile";
            this.clearInputFile.Size = new System.Drawing.Size(75, 23);
            this.clearInputFile.TabIndex = 4;
            this.clearInputFile.Text = "Clear";
            this.clearInputFile.UseVisualStyleBackColor = true;
            this.clearInputFile.Click += new System.EventHandler(this.clearInputFile_Click);
            // 
            // inputLabel
            // 
            this.inputLabel.AutoSize = true;
            this.inputLabel.Location = new System.Drawing.Point(12, 8);
            this.inputLabel.Name = "inputLabel";
            this.inputLabel.Size = new System.Drawing.Size(84, 13);
            this.inputLabel.TabIndex = 8;
            this.inputLabel.Text = "Input Files (.csv)";
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Location = new System.Drawing.Point(21, 108);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(93, 13);
            this.headerLabel.TabIndex = 9;
            this.headerLabel.Text = "Expression Values";
            // 
            // groupDataGrid
            // 
            this.groupDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.groupDataGrid.Location = new System.Drawing.Point(129, 124);
            this.groupDataGrid.Name = "groupDataGrid";
            this.groupDataGrid.Size = new System.Drawing.Size(443, 183);
            this.groupDataGrid.TabIndex = 10;
            this.groupDataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // headerComboBox
            // 
            this.headerComboBox.FormattingEnabled = true;
            this.headerComboBox.Location = new System.Drawing.Point(451, 313);
            this.headerComboBox.Name = "headerComboBox";
            this.headerComboBox.Size = new System.Drawing.Size(121, 21);
            this.headerComboBox.TabIndex = 12;
            // 
            // analyze
            // 
            this.analyze.Location = new System.Drawing.Point(694, 598);
            this.analyze.Name = "analyze";
            this.analyze.Size = new System.Drawing.Size(109, 23);
            this.analyze.TabIndex = 14;
            this.analyze.Text = "Analyze";
            this.analyze.UseVisualStyleBackColor = true;
            this.analyze.Click += new System.EventHandler(this.analyze_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(324, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Analysis Groups";
            // 
            // headersToPrintListBox
            // 
            this.headersToPrintListBox.CheckOnClick = true;
            this.headersToPrintListBox.FormattingEnabled = true;
            this.headersToPrintListBox.Location = new System.Drawing.Point(581, 391);
            this.headersToPrintListBox.Name = "headersToPrintListBox";
            this.headersToPrintListBox.Size = new System.Drawing.Size(221, 199);
            this.headersToPrintListBox.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(645, 376);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Headers To Print";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(424, 345);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "and";
            // 
            // headerComboBox2
            // 
            this.headerComboBox2.FormattingEnabled = true;
            this.headerComboBox2.Location = new System.Drawing.Point(451, 342);
            this.headerComboBox2.Name = "headerComboBox2";
            this.headerComboBox2.Size = new System.Drawing.Size(121, 21);
            this.headerComboBox2.TabIndex = 19;
            // 
            // normalizationComboBox
            // 
            this.normalizationComboBox.FormattingEnabled = true;
            this.normalizationComboBox.Items.AddRange(new object[] {
            "All Entries",
            "Subset - Uniprot (.csv)",
            "Subset - GO-Cellular Component",
            "Subset - GO-Molecular Function",
            "Subset - GO-Biological Processes",
            "Subset - Keywords"});
            this.normalizationComboBox.Location = new System.Drawing.Point(8, 550);
            this.normalizationComboBox.Name = "normalizationComboBox";
            this.normalizationComboBox.Size = new System.Drawing.Size(174, 21);
            this.normalizationComboBox.TabIndex = 20;
            this.normalizationComboBox.Text = "All Entries";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(5, 533);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(166, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Normalization and Protein Filtering";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(277, 533);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Significance Testing";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // sigTestingComboBox
            // 
            this.sigTestingComboBox.FormattingEnabled = true;
            this.sigTestingComboBox.Items.AddRange(new object[] {
            "None",
            "T-Test (Unequal Var)",
            "Fold Change"});
            this.sigTestingComboBox.Location = new System.Drawing.Point(279, 549);
            this.sigTestingComboBox.Name = "sigTestingComboBox";
            this.sigTestingComboBox.Size = new System.Drawing.Size(139, 21);
            this.sigTestingComboBox.TabIndex = 24;
            this.sigTestingComboBox.Text = "None";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(433, 533);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "Significance Threshold";
            // 
            // sigThershold
            // 
            this.sigThershold.Location = new System.Drawing.Point(436, 550);
            this.sigThershold.Name = "sigThershold";
            this.sigThershold.Size = new System.Drawing.Size(46, 20);
            this.sigThershold.TabIndex = 26;
            this.sigThershold.Text = "0.05";
            this.sigThershold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // thresholdType
            // 
            this.thresholdType.FormattingEnabled = true;
            this.thresholdType.Items.AddRange(new object[] {
            "p-Value",
            "P-Value",
            "Fold Change"});
            this.thresholdType.Location = new System.Drawing.Point(491, 549);
            this.thresholdType.Name = "thresholdType";
            this.thresholdType.Size = new System.Drawing.Size(84, 21);
            this.thresholdType.TabIndex = 27;
            this.thresholdType.Text = "p-Value";
            this.thresholdType.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // multCompCorr
            // 
            this.multCompCorr.FormattingEnabled = true;
            this.multCompCorr.Items.AddRange(new object[] {
            "None",
            "Benjamini-Hochberg"});
            this.multCompCorr.Location = new System.Drawing.Point(280, 598);
            this.multCompCorr.Name = "multCompCorr";
            this.multCompCorr.Size = new System.Drawing.Size(139, 21);
            this.multCompCorr.TabIndex = 28;
            this.multCompCorr.Text = "None";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(277, 582);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(131, 13);
            this.label7.TabIndex = 29;
            this.label7.Text = "Multiple Comparisons Corr.";
            // 
            // goTermsCheckListBox
            // 
            this.goTermsCheckListBox.CheckOnClick = true;
            this.goTermsCheckListBox.FormattingEnabled = true;
            this.goTermsCheckListBox.Items.AddRange(new object[] {
            "GO-Cellular Component",
            "GO-Molecular Function",
            "GO-Biological Processes",
            "KEGG Pathway (Not Available)",
            "Protein Interaction",
            "Keywords"});
            this.goTermsCheckListBox.Location = new System.Drawing.Point(583, 179);
            this.goTermsCheckListBox.Name = "goTermsCheckListBox";
            this.goTermsCheckListBox.Size = new System.Drawing.Size(221, 94);
            this.goTermsCheckListBox.TabIndex = 30;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(613, 107);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(159, 13);
            this.label8.TabIndex = 31;
            this.label8.Text = "Annotations and GO Enrichment";
            // 
            // annotationComboBox
            // 
            this.annotationComboBox.FormattingEnabled = true;
            this.annotationComboBox.Items.AddRange(new object[] {
            "None",
            "Annotate Only",
            "Annotate,Test Enrichment (Significant)",
            "Annotate,Test Enrichment (Sig. Up)",
            "Annotate,Test Enrichment (Sig. Down)",
            "Annotate,Test Enrichment (All Three)"});
            this.annotationComboBox.Location = new System.Drawing.Point(581, 123);
            this.annotationComboBox.Name = "annotationComboBox";
            this.annotationComboBox.Size = new System.Drawing.Size(222, 21);
            this.annotationComboBox.TabIndex = 32;
            this.annotationComboBox.Text = "None";
            // 
            // goDatabaseComboBox
            // 
            this.goDatabaseComboBox.FormattingEnabled = true;
            this.goDatabaseComboBox.Items.AddRange(new object[] {
            "Mouse",
            "Human",
            "Yeast"});
            this.goDatabaseComboBox.Location = new System.Drawing.Point(581, 295);
            this.goDatabaseComboBox.Name = "goDatabaseComboBox";
            this.goDatabaseComboBox.Size = new System.Drawing.Size(104, 21);
            this.goDatabaseComboBox.TabIndex = 33;
            this.goDatabaseComboBox.Text = "Mouse";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(579, 279);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(107, 13);
            this.label9.TabIndex = 34;
            this.label9.Text = "Annotation Database";
            // 
            // userDatabase
            // 
            this.userDatabase.Location = new System.Drawing.Point(581, 323);
            this.userDatabase.Name = "userDatabase";
            this.userDatabase.Size = new System.Drawing.Size(186, 17);
            this.userDatabase.TabIndex = 37;
            this.userDatabase.Text = "Use Custom Annotation Database";
            this.userDatabase.UseVisualStyleBackColor = true;
            this.userDatabase.CheckedChanged += new System.EventHandler(this.userDatabase_CheckedChanged);
            // 
            // annotationSigTextBox
            // 
            this.annotationSigTextBox.Location = new System.Drawing.Point(691, 295);
            this.annotationSigTextBox.Name = "annotationSigTextBox";
            this.annotationSigTextBox.Size = new System.Drawing.Size(60, 20);
            this.annotationSigTextBox.TabIndex = 38;
            this.annotationSigTextBox.Text = "0.02";
            this.annotationSigTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(688, 279);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(115, 13);
            this.label10.TabIndex = 39;
            this.label10.Text = "Significance Threshold";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(755, 298);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 13);
            this.label11.TabIndex = 40;
            this.label11.Text = "P-Value";
            this.label11.Click += new System.EventHandler(this.label11_Click);
            // 
            // clearAnnotaionDatabase
            // 
            this.clearAnnotaionDatabase.Enabled = false;
            this.clearAnnotaionDatabase.Location = new System.Drawing.Point(739, 341);
            this.clearAnnotaionDatabase.Name = "clearAnnotaionDatabase";
            this.clearAnnotaionDatabase.Size = new System.Drawing.Size(67, 23);
            this.clearAnnotaionDatabase.TabIndex = 41;
            this.clearAnnotaionDatabase.Text = "Clear";
            this.clearAnnotaionDatabase.UseVisualStyleBackColor = true;
            this.clearAnnotaionDatabase.Click += new System.EventHandler(this.clearAnnotaionDatabase_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(433, 583);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(70, 13);
            this.label12.TabIndex = 43;
            this.label12.Text = "Extra Options";
            // 
            // perseusOutput
            // 
            this.perseusOutput.AutoSize = true;
            this.perseusOutput.Enabled = false;
            this.perseusOutput.Location = new System.Drawing.Point(437, 604);
            this.perseusOutput.Name = "perseusOutput";
            this.perseusOutput.Size = new System.Drawing.Size(99, 17);
            this.perseusOutput.TabIndex = 44;
            this.perseusOutput.Text = "Perseus Output";
            this.perseusOutput.UseVisualStyleBackColor = true;
            // 
            // comparisonDataGridView
            // 
            this.comparisonDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.comparisonDataGridView.Location = new System.Drawing.Point(155, 391);
            this.comparisonDataGridView.Name = "comparisonDataGridView";
            this.comparisonDataGridView.Size = new System.Drawing.Size(417, 137);
            this.comparisonDataGridView.TabIndex = 46;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(327, 377);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(67, 13);
            this.label14.TabIndex = 48;
            this.label14.Text = "Comparisons";
            // 
            // buildComparisons
            // 
            this.buildComparisons.Location = new System.Drawing.Point(8, 391);
            this.buildComparisons.Name = "buildComparisons";
            this.buildComparisons.Size = new System.Drawing.Size(139, 23);
            this.buildComparisons.TabIndex = 49;
            this.buildComparisons.Text = "Add Comparisons";
            this.buildComparisons.UseVisualStyleBackColor = true;
            this.buildComparisons.Click += new System.EventHandler(this.buildComparisons_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(580, 153);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(93, 13);
            this.label15.TabIndex = 50;
            this.label15.Text = "Uniprot ID Header";
            // 
            // uniprotIDHeader
            // 
            this.uniprotIDHeader.FormattingEnabled = true;
            this.uniprotIDHeader.Location = new System.Drawing.Point(679, 150);
            this.uniprotIDHeader.Name = "uniprotIDHeader";
            this.uniprotIDHeader.Size = new System.Drawing.Size(124, 21);
            this.uniprotIDHeader.TabIndex = 51;
            // 
            // clearNormTextBox
            // 
            this.clearNormTextBox.Location = new System.Drawing.Point(192, 598);
            this.clearNormTextBox.Name = "clearNormTextBox";
            this.clearNormTextBox.Size = new System.Drawing.Size(72, 23);
            this.clearNormTextBox.TabIndex = 53;
            this.clearNormTextBox.Text = "Clear";
            this.clearNormTextBox.UseVisualStyleBackColor = true;
            this.clearNormTextBox.Click += new System.EventHandler(this.clearNormTextBox_Click);
            // 
            // normalizationTextBox
            // 
            this.normalizationTextBox.AllowDrop = true;
            this.normalizationTextBox.Location = new System.Drawing.Point(8, 599);
            this.normalizationTextBox.Name = "normalizationTextBox";
            this.normalizationTextBox.Size = new System.Drawing.Size(174, 20);
            this.normalizationTextBox.TabIndex = 54;
            this.normalizationTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.normalizationTextBox_DragDrop);
            this.normalizationTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.normalizationTextBox_DragEnter);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(6, 583);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(144, 13);
            this.label16.TabIndex = 55;
            this.label16.Text = "Protein Filtering Term/Subset";
            // 
            // clearAnalysisGroups
            // 
            this.clearAnalysisGroups.Location = new System.Drawing.Point(207, 313);
            this.clearAnalysisGroups.Name = "clearAnalysisGroups";
            this.clearAnalysisGroups.Size = new System.Drawing.Size(72, 23);
            this.clearAnalysisGroups.TabIndex = 56;
            this.clearAnalysisGroups.Text = "Clear";
            this.clearAnalysisGroups.UseVisualStyleBackColor = true;
            this.clearAnalysisGroups.Click += new System.EventHandler(this.clearAnalysisGroups_Click);
            // 
            // sortAnalysisGroups
            // 
            this.sortAnalysisGroups.Location = new System.Drawing.Point(129, 313);
            this.sortAnalysisGroups.Name = "sortAnalysisGroups";
            this.sortAnalysisGroups.Size = new System.Drawing.Size(72, 23);
            this.sortAnalysisGroups.TabIndex = 57;
            this.sortAnalysisGroups.Text = "Sort";
            this.sortAnalysisGroups.UseVisualStyleBackColor = true;
            this.sortAnalysisGroups.Click += new System.EventHandler(this.sortAnalysisGroups_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(312, 318);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(137, 13);
            this.label13.TabIndex = 58;
            this.label13.Text = "Combine Multiple Entries on";
            // 
            // cleanDefline
            // 
            this.cleanDefline.AutoSize = true;
            this.cleanDefline.Location = new System.Drawing.Point(542, 605);
            this.cleanDefline.Name = "cleanDefline";
            this.cleanDefline.Size = new System.Drawing.Size(89, 17);
            this.cleanDefline.TabIndex = 59;
            this.cleanDefline.Text = "Clean Defline";
            this.cleanDefline.UseVisualStyleBackColor = true;
            // 
            // medianNorm
            // 
            this.medianNorm.AutoSize = true;
            this.medianNorm.Location = new System.Drawing.Point(200, 554);
            this.medianNorm.Name = "medianNorm";
            this.medianNorm.Size = new System.Drawing.Size(60, 17);
            this.medianNorm.TabIndex = 60;
            this.medianNorm.Text = "Median";
            this.medianNorm.UseVisualStyleBackColor = true;
            // 
            // sumNorm
            // 
            this.sumNorm.AutoSize = true;
            this.sumNorm.Location = new System.Drawing.Point(200, 573);
            this.sumNorm.Name = "sumNorm";
            this.sumNorm.Size = new System.Drawing.Size(46, 17);
            this.sumNorm.TabIndex = 61;
            this.sumNorm.Text = "Sum";
            this.sumNorm.UseVisualStyleBackColor = true;
            // 
            // customDatabaseText
            // 
            this.customDatabaseText.AllowDrop = true;
            this.customDatabaseText.Enabled = false;
            this.customDatabaseText.Location = new System.Drawing.Point(581, 342);
            this.customDatabaseText.Name = "customDatabaseText";
            this.customDatabaseText.Size = new System.Drawing.Size(152, 20);
            this.customDatabaseText.TabIndex = 62;
            this.customDatabaseText.DragDrop += new System.Windows.Forms.DragEventHandler(this.customDatabaseText_DragDrop);
            this.customDatabaseText.DragEnter += new System.Windows.Forms.DragEventHandler(this.customDatabaseText_DragEnter);
            // 
            // noNorm
            // 
            this.noNorm.AutoSize = true;
            this.noNorm.Checked = true;
            this.noNorm.Location = new System.Drawing.Point(200, 534);
            this.noNorm.Name = "noNorm";
            this.noNorm.Size = new System.Drawing.Size(51, 17);
            this.noNorm.TabIndex = 63;
            this.noNorm.TabStop = true;
            this.noNorm.Text = "None";
            this.noNorm.UseVisualStyleBackColor = true;
            // 
            // outputFolderTextBox
            // 
            this.outputFolderTextBox.AllowDrop = true;
            this.outputFolderTextBox.Location = new System.Drawing.Point(86, 74);
            this.outputFolderTextBox.Name = "outputFolderTextBox";
            this.outputFolderTextBox.Size = new System.Drawing.Size(636, 20);
            this.outputFolderTextBox.TabIndex = 64;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(9, 77);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(71, 13);
            this.label17.TabIndex = 65;
            this.label17.Text = "Output Folder";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.meanNormFoldChange);
            this.groupBox1.Controls.Add(this.meanNormLog2);
            this.groupBox1.Controls.Add(this.intensityFoldChange);
            this.groupBox1.Controls.Add(this.intensityLog2);
            this.groupBox1.Location = new System.Drawing.Point(5, 419);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(145, 108);
            this.groupBox1.TabIndex = 73;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Values for Comparison";
            // 
            // meanNormFoldChange
            // 
            this.meanNormFoldChange.AutoSize = true;
            this.meanNormFoldChange.Location = new System.Drawing.Point(3, 85);
            this.meanNormFoldChange.Name = "meanNormFoldChange";
            this.meanNormFoldChange.Size = new System.Drawing.Size(136, 17);
            this.meanNormFoldChange.TabIndex = 3;
            this.meanNormFoldChange.Text = "Mean Normalized (Fold)";
            this.meanNormFoldChange.UseVisualStyleBackColor = true;
            // 
            // meanNormLog2
            // 
            this.meanNormLog2.AutoSize = true;
            this.meanNormLog2.Location = new System.Drawing.Point(3, 62);
            this.meanNormLog2.Name = "meanNormLog2";
            this.meanNormLog2.Size = new System.Drawing.Size(140, 17);
            this.meanNormLog2.TabIndex = 2;
            this.meanNormLog2.Text = "Mean Normalized (Log2)";
            this.meanNormLog2.UseVisualStyleBackColor = true;
            // 
            // intensityFoldChange
            // 
            this.intensityFoldChange.AutoSize = true;
            this.intensityFoldChange.Location = new System.Drawing.Point(3, 39);
            this.intensityFoldChange.Name = "intensityFoldChange";
            this.intensityFoldChange.Size = new System.Drawing.Size(93, 17);
            this.intensityFoldChange.TabIndex = 1;
            this.intensityFoldChange.Text = "Intensity (Fold)";
            this.intensityFoldChange.UseVisualStyleBackColor = true;
            // 
            // intensityLog2
            // 
            this.intensityLog2.AutoSize = true;
            this.intensityLog2.Checked = true;
            this.intensityLog2.Location = new System.Drawing.Point(3, 16);
            this.intensityLog2.Name = "intensityLog2";
            this.intensityLog2.Size = new System.Drawing.Size(97, 17);
            this.intensityLog2.TabIndex = 0;
            this.intensityLog2.TabStop = true;
            this.intensityLog2.Text = "Intensity (Log2)";
            this.intensityLog2.UseVisualStyleBackColor = true;
            // 
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(8, 627);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(795, 75);
            this.logBox.TabIndex = 74;
            this.logBox.Text = "";
            // 
            // ProcyonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 707);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.outputFolderTextBox);
            this.Controls.Add(this.noNorm);
            this.Controls.Add(this.customDatabaseText);
            this.Controls.Add(this.sumNorm);
            this.Controls.Add(this.medianNorm);
            this.Controls.Add(this.cleanDefline);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.sortAnalysisGroups);
            this.Controls.Add(this.clearAnalysisGroups);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.normalizationTextBox);
            this.Controls.Add(this.clearNormTextBox);
            this.Controls.Add(this.uniprotIDHeader);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.buildComparisons);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.comparisonDataGridView);
            this.Controls.Add(this.perseusOutput);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.clearAnnotaionDatabase);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.annotationSigTextBox);
            this.Controls.Add(this.userDatabase);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.goDatabaseComboBox);
            this.Controls.Add(this.annotationComboBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.goTermsCheckListBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.multCompCorr);
            this.Controls.Add(this.thresholdType);
            this.Controls.Add(this.sigThershold);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.sigTestingComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.normalizationComboBox);
            this.Controls.Add(this.headerComboBox2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.headersToPrintListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.analyze);
            this.Controls.Add(this.headerComboBox);
            this.Controls.Add(this.groupDataGrid);
            this.Controls.Add(this.headerLabel);
            this.Controls.Add(this.inputLabel);
            this.Controls.Add(this.addToOne);
            this.Controls.Add(this.clearInputFile);
            this.Controls.Add(this.headerList);
            this.Controls.Add(this.inputFiles);
            this.Name = "ProcyonForm";
            this.Text = " Procyon";
            this.Load += new System.EventHandler(this.Procyon_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comparisonDataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox inputFiles;
        private System.Windows.Forms.ListBox headerList;
        private System.Windows.Forms.Button addToOne;
        private System.Windows.Forms.Button clearInputFile;
        private System.Windows.Forms.Label inputLabel;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.DataGridView groupDataGrid;
        private System.Windows.Forms.ComboBox headerComboBox;
        private System.Windows.Forms.Button analyze;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox headersToPrintListBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox headerComboBox2;
        private System.Windows.Forms.ComboBox normalizationComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox sigTestingComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox sigThershold;
        private System.Windows.Forms.ComboBox thresholdType;
        private System.Windows.Forms.ComboBox multCompCorr;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckedListBox goTermsCheckListBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox annotationComboBox;
        private System.Windows.Forms.ComboBox goDatabaseComboBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox userDatabase;
        private System.Windows.Forms.TextBox annotationSigTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button clearAnnotaionDatabase;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox perseusOutput;
        private System.Windows.Forms.DataGridView comparisonDataGridView;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button buildComparisons;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox uniprotIDHeader;
        private System.Windows.Forms.Button clearNormTextBox;
        private System.Windows.Forms.TextBox normalizationTextBox;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button clearAnalysisGroups;
        private System.Windows.Forms.Button sortAnalysisGroups;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox cleanDefline;
        private System.Windows.Forms.RadioButton medianNorm;
        private System.Windows.Forms.RadioButton sumNorm;
        private System.Windows.Forms.TextBox customDatabaseText;
        private System.Windows.Forms.RadioButton noNorm;
        private System.Windows.Forms.TextBox outputFolderTextBox;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton meanNormFoldChange;
        private System.Windows.Forms.RadioButton meanNormLog2;
        private System.Windows.Forms.RadioButton intensityFoldChange;
        private System.Windows.Forms.RadioButton intensityLog2;
        private System.Windows.Forms.RichTextBox logBox;

    }
}

