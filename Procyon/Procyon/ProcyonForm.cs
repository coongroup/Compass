using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using LumenWorks.Framework.IO.Csv;
using Meta.Numerics.Statistics;
using CSMSL.IO;

namespace Coon.Compass.Procyon
{
    public partial class ProcyonForm : Form
    {
        BindingList<string> InputFiles;
        BindingList<string> Headers;
        BindingList<string> UniqueList;
        BindingList<string> UniqueList2;
        BindingList<string> UniprotHeaderList;
        BindingList<AnalysisGroupLine> AnalysisGroups;
        BindingList<string> Numerators;
        BindingList<string> Denominators;
        List<string> PrintHeaderList;
        Dictionary<string, string> FileNameAndHeadertoUniqueGroupName;
        Dictionary<string, string> UniqueGroupNametoGroupString;
        Dictionary<string, AnnotationEntry> AnnotationEntryDict;
        Dictionary<string, Dictionary<AnnotationType, List<string>>> UniprotIDtoAnnotationList;
        Dictionary<string, Dictionary<string,List<string>>> FiletoGroupNametoListofHeaders;
        Dictionary<string, List<string>> FileNametoHeadersToQuantify;
        List<Comparison> Comparisons;
        Dictionary<string, List<string>> HeadersToIncludeByFile;
        List<AnnotationType> AnnotationsToAdd;
        string UniprotHeader;
        string OutputFolder;
        string NormalizationSubetString;
        ValueType ValueForComparison;
        bool TestSignificance;
        bool UseFoldChange = false;
        bool PrintAnnotations = false;
        string CurrentOrganism;
        string SigTestingString;
        string MultipleCorrectionString;
        string ThresholdType;
        double ThresholdValue;
        string GOString;
        string AnnotationDatabase;

        public ProcyonForm()
        {
            InitializeComponent();

            InputFiles = new BindingList<string>();
            Headers = new BindingList<string>();
            UniqueList = new BindingList<string>();
            UniqueList2 = new BindingList<string>();
            PrintHeaderList = new List<string>();
            AnnotationsToAdd = new List<AnnotationType>();
            UniprotHeaderList = new BindingList<string>();
            AnalysisGroups = new BindingList<AnalysisGroupLine>();
            Numerators = new BindingList<string>();
            Denominators = new BindingList<string>();
            Comparisons = new List<Comparison>();
            HeadersToIncludeByFile = new Dictionary<string,List<string>>();
            FiletoGroupNametoListofHeaders = new Dictionary<string, Dictionary<string, List<string>>>();
            FileNametoHeadersToQuantify = new Dictionary<string, List<string>>();
            TestSignificance = false;
            UseFoldChange = false;
            CurrentOrganism = "None";
            NormalizationSubetString = "All Entries";
            SigTestingString = "None";
            MultipleCorrectionString = "None";
            ThresholdType = "Fold Change";
            ThresholdValue = 0;
            GOString = "None";
            AnnotationDatabase = "Mouse";

            PrintHeaderList.Add("Protein Group Name");
            PrintHeaderList.Add("Representative Protein Description");
            PrintHeaderList.Add("Sequence Coverage (%)");
            PrintHeaderList.Add("UniprotIDs");
            PrintHeaderList.Add("Gene Names");
            PrintHeaderList.Add("Numbers of Proteins");
            PrintHeaderList.Add("# Quantified Peptides");

            FileNameAndHeadertoUniqueGroupName = new Dictionary<string, string>();
            UniqueGroupNametoGroupString = new Dictionary<string, string>();
            AnnotationEntryDict = new Dictionary<string, AnnotationEntry>();
            UniprotIDtoAnnotationList = new Dictionary<string, Dictionary<AnnotationType, List<string>>>();
            headerList.DataSource = Headers;
            inputFiles.DataSource = InputFiles;
            headerComboBox.DataSource = UniqueList;
            headerComboBox2.DataSource = UniqueList2;
            uniprotIDHeader.DataSource = UniprotHeaderList;

            #region Group Grid
            //Set up the Analysis Group Data Grid View Box
            groupDataGrid.AutoGenerateColumns = false;

            DataGridViewTextBoxColumn fileNameColumn = new DataGridViewTextBoxColumn();
            fileNameColumn.DataPropertyName = "FileName";
            fileNameColumn.HeaderText = "File Name";
            fileNameColumn.ReadOnly = true;
            fileNameColumn.Width = 100;
            groupDataGrid.Columns.Add(fileNameColumn);

            DataGridViewTextBoxColumn headerNameColumn = new DataGridViewTextBoxColumn();
            headerNameColumn.DataPropertyName = "HeaderName";
            headerNameColumn.HeaderText = "Header Name";
            headerNameColumn.ReadOnly = true;
            headerNameColumn.Width = 100;
            groupDataGrid.Columns.Add(headerNameColumn);

            DataGridViewTextBoxColumn uniqueNameColumn = new DataGridViewTextBoxColumn();
            uniqueNameColumn.DataPropertyName = "UniqueName";
            uniqueNameColumn.HeaderText = "Unique Name";
            uniqueNameColumn.Width = 100;
            groupDataGrid.Columns.Add(uniqueNameColumn);

            DataGridViewTextBoxColumn groupNumberColumn = new DataGridViewTextBoxColumn();
            groupNumberColumn.DataPropertyName = "GroupString";
            groupNumberColumn.HeaderText = "Group Name";
            groupNumberColumn.Width = 80;
            groupDataGrid.Columns.Add(groupNumberColumn);

            groupDataGrid.DataSource = AnalysisGroups;
            #endregion

            #region Comparison Grid
            comparisonDataGridView.AutoGenerateColumns = false;
            DataGridViewComboBoxColumn numeratorColumn = new DataGridViewComboBoxColumn();
            numeratorColumn.DataPropertyName = "Numerator";
            numeratorColumn.HeaderText = "Numerator Group";
            numeratorColumn.Width = 187;
            numeratorColumn.DataSource = Numerators;
            comparisonDataGridView.Columns.Add(numeratorColumn);

            DataGridViewComboBoxColumn denominatorColumn = new DataGridViewComboBoxColumn();
            denominatorColumn.DataPropertyName = "Denominator";
            denominatorColumn.HeaderText = "Denominator Group";
            denominatorColumn.Width = 187;
            denominatorColumn.DataSource = Denominators;
            comparisonDataGridView.Columns.Add(denominatorColumn);

            #endregion
        }

        #region GUI Code
        private void addToOne_Click(object sender, EventArgs e)
        {
            if (Headers.Count > 0)
            {
                List<string> headersToRemove = new List<string>();
                foreach (string inputFile in inputFiles.Items)
                {
                    using (CsvReader reader = new CsvReader(new StreamReader(inputFile), true))
                    {
                        foreach (string header in headerList.SelectedItems)
                        {
                            if (reader.GetFieldHeaders().ToList().Contains(header))
                            {
                                //Make the Analysis Group Line Object here
                                AnalysisGroups.Add(new AnalysisGroupLine(inputFile, header, ""));

                            }

                            headersToRemove.Add(header);
                        }
                    }
                }

                foreach (string header in headersToRemove)
                {
                    Headers.Remove(header);
                }
            }
        }

        private void clearInputFile_Click(object sender, EventArgs e)
        {
            InputFiles.Clear();
            Headers.Clear();
            UniqueList.Clear();
            UniqueList2.Clear();
            UniprotHeaderList.Clear();
            AnalysisGroups.Clear();
            PrintHeaderList.Clear();
            FileNameAndHeadertoUniqueGroupName.Clear();
            UniqueGroupNametoGroupString.Clear();
            headersToPrintListBox.Items.Clear();
            outputFolderTextBox.Clear();
            AnalysisGroups.Clear();

            intensityFoldChange.Enabled = true;
            intensityLog2.Enabled = true;

            comparisonDataGridView.Rows.Clear();
        }

        private void userDatabase_CheckedChanged(object sender, EventArgs e)
        {
            customDatabaseText.Enabled = !customDatabaseText.Enabled;
            goDatabaseComboBox.Enabled = !goDatabaseComboBox.Enabled;
            clearAnnotaionDatabase.Enabled = !clearAnnotaionDatabase.Enabled;

            if (customDatabaseText.Enabled)
            {
                goTermsCheckListBox.SetItemCheckState(3, CheckState.Unchecked);
            }
            else
            {
                goTermsCheckListBox.SetItemCheckState(3, CheckState.Unchecked);
            }
        }

        private void clearAnnotaionDatabase_Click(object sender, EventArgs e)
        {
            customDatabaseText.Clear();
        }

        private void buildComparisons_Click(object sender, EventArgs e)
        {
            comparisonDataGridView.Rows.Clear();
            Numerators.Clear();
            Denominators.Clear();

            Numerators.Add("All");

            List<string> addList = new List<string>();

            foreach (AnalysisGroupLine groupLine in AnalysisGroups)
            {
                if (!addList.Contains(groupLine.GroupString))
                {
                    addList.Add(groupLine.GroupString);
                }
            }

            addList.Sort();

            foreach (string groupNumber in addList)
            {
                Numerators.Add(groupNumber);
                Denominators.Add(groupNumber);
            }
        }

        private void sortAnalysisGroups_Click(object sender, EventArgs e)
        {
            SortedList<string, AnalysisGroupLine> sortDict = new SortedList<string, AnalysisGroupLine>();

            if (!TestUniqueNames(AnalysisGroups))
            {
                foreach (AnalysisGroupLine groupLine in AnalysisGroups)
                {
                    sortDict.Add(groupLine.UniqueName, groupLine);
                }

                AnalysisGroups.Clear();

                foreach (KeyValuePair<string, AnalysisGroupLine> kvp in sortDict)
                {
                    AnalysisGroups.Add(kvp.Value);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Unique Group Names Contains Non-unique Name!");
            }
        }

        private void clearAnalysisGroups_Click(object sender, EventArgs e)
        {
            List<string> tempList = new List<string>();
            foreach (AnalysisGroupLine groupLine in AnalysisGroups)
            {
                if (!tempList.Contains(groupLine.HeaderName))
                {
                    tempList.Add(groupLine.HeaderName);
                }
            }

            tempList.Sort();

            foreach(string header in tempList)
            {
                Headers.Add(header);
            }

            AnalysisGroups.Clear();
            comparisonDataGridView.Rows.Clear();
            Numerators.Clear();
            Denominators.Clear();
        }

        private void clearNormTextBox_Click(object sender, EventArgs e)
        {
            normalizationTextBox.Clear();
        }

        #endregion

        #region Add File Code

        public void AddTxt(string filename)
        {
            if (!InputFiles.Contains(filename))
            {
                InputFiles.Add(filename);
                Application.DoEvents();
            }
        }

        public void AddXml(string filename)
        {
            customDatabaseText.Text = filename;
            Application.DoEvents();
        }

        public void AddCSV(string filename)
        {
            using (CsvReader reader = new CsvReader(new StreamReader(filename), true))
            {
                if (reader.GetFieldHeaders().ToList().Contains("Uniprot"))
                {
                    normalizationTextBox.Text = filename;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("File Must Have Header Labeled as 'Uniprot'!");
                }

                Application.DoEvents();
            }
        }

        private void AddFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                switch (Path.GetExtension(file))
                {
                    case ".csv":
                        AddTxt(file);
                        break;
                    default:
                        break;
                }
            }
        }

        private void AddFiles2(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                switch (Path.GetExtension(file))
                {
                    case ".xml":
                        AddXml(file);
                        break;
                    case ".csv":
                        AddXml(file);
                        break;
                    default:
                        break;
                }
            }
        }

        private void AddFile(string[] files)
        {
            foreach (string file in files)
            {
                switch (Path.GetExtension(file))
                {
                    case ".csv":
                        AddCSV(file);
                        break;
                    default:
                        break;
                }
            }
        }

        private bool CheckHeader(string header)
        {
            if (header.Contains(" NL") || header.Contains(" dNL"))
            {
                return false;
            }
            switch (header)
            {
                case "Total Amino Acids":
                    return false;
                case "Sequence Coverage (%)":
                    return false;
                case "Numbers of Proteins":
                    return false;
                case "Number of PSMs":
                    return false;
                case "Number of Unique Peptides":
                    return false;
                case "# PSMs in Experiment":
                    return false;
                case "# Unique Seq in Experiment":
                    return false;
                case "P-Score":
                    return false;
                case "# Quantified PSMs":
                    return false;
                case "# Quantified Peptides":
                    return false;
                case "Representative Protein Description":
                    return false;
                case "Protein Group Name":
                    return false;
                case "UniprotIDs":
                    return false;
                case "Gene Names":
                    return false;
                case "Spectrum Number":
                    return false;
                case "Filename/id":
                    return false;
                case "Peptide":
                    return false;
                case "E-value":
                    return false;
                case "Mass":
                    return false;
                case "gi":
                    return false;
                case "Accession":
                    return false;
                case "Start":
                    return false;
                case "Stop":
                    return false;
                case "Defline":
                    return false;
                case "Mods":
                    return false;
                case "Charge":
                    return false;
                case "Theo Mass":
                    return false;
                case "P-value":
                    return false;
                case "NIST score":
                    return false;
                case "Precursor Isolation m/z":
                    return false;
                case "Precursor Isolation Mass (Da)":
                    return false;
                case "Precursor Theoretical Neutral Mass (Da)":
                    return false;
                case "Precursor Experimental Neutral Mass (Da)":
                    return false;
                case "Precursor Mass Error (ppm)":
                    return false;
                case "Adjusted Precursor Mass Error (ppm)":
                    return false;
                case "Q-Value (%)":
                    return false;
                case "Channels Detected":
                    return false;
                case "Experiment ID":
                    return false;
                case "# of Sharing PGs":
                    return false;
                case "Best PG Name":
                    return false;
                case " Phosphoisoform":
                    return false;
                case " Phosphoisoform Sites":
                    return false;
                case "Peptides":
                    return false;
                case " PSMs Identified":
                    return false;
                case " PSMs Quantified":
                    return false;
                case " Peptides Identified":
                    return false;
                case " Peptides Quantified":
                    return false;
                case " Phosphoisoform Quantified?":
                    return false;
                case "Protein Descripton":
                    return false;
                case "Protein Group":
                    return false;
                case "Isoform":
                    return false;
                case "Sites":
                    return false;
                case "PSMs Identified":
                    return false;
                case "PSMs Localized":
                    return false;
                default:
                    return true;
            }
        }

        #endregion

        #region Drag and Drop Code
        private void inputFiles_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void inputFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                AddFiles(files);

                foreach (string inputFile in InputFiles)
                {
                    using (CsvReader reader = new CsvReader(new StreamReader(inputFile), true))
                    {
                        int count = 0;
                        foreach (string header in reader.GetFieldHeaders().ToList())
                        {
                            reader.ReadNextRecord();

                            if (CheckHeader(header) && !Headers.Contains(header))
                            {
                                Headers.Add(header);
                            }

                            if (!UniqueList.Contains(header))
                            {
                                UniqueList.Add(header);
                                UniqueList2.Add(header);
                                headersToPrintListBox.Items.Add(header);

                                if (header.Contains("Uniprot"))
                                {
                                    UniprotHeaderList.Add(header);
                                }
                            }

                            count++;
                        }
                    }

                }

                headersToPrintListBox.SetItemCheckState(0, CheckState.Checked);

                int counter = 0;
                foreach (string headerName in UniqueList)
                {
                    if (PrintHeaderList.Contains(headerName))
                    {
                        headersToPrintListBox.SetItemCheckState(counter, CheckState.Checked);
                    }

                    if (headerName.Contains("Protein Description") || headerName.Contains("Defline"))
                    {
                        UniprotHeaderList.Add(headerName);
                    }

                    counter++;
                }

                foreach (string headerName in UniqueList)
                {
                    UniprotHeaderList.Add(headerName);
                }

                outputFolderTextBox.Text = Path.GetDirectoryName(InputFiles.ElementAt(0));

                if (inputFiles.Items.Count > 1)
                {
                    if (intensityFoldChange.Checked || intensityLog2.Checked)
                    {
                        meanNormLog2.Checked = true;
                    }

                    intensityFoldChange.Enabled = false;
                    intensityLog2.Enabled = false;

                    headerComboBox.Enabled = true;
                    headerComboBox2.Enabled = true;
                }
            }
        }

        private void normalizationTextBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileName = (string[])e.Data.GetData(DataFormats.FileDrop);
                AddFile(fileName);
            }
        }

        private void normalizationTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void customDatabaseText_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                AddFiles2(files);
            }
        }

        private void customDatabaseText_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        #endregion

        #region Unused Buttons
        private void customDatabaseBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        #endregion

        private void analyze_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TestUniqueNames(AnalysisGroups))
                {
                    LoadUserInputs();

                    //Import the Quantitation Files
                    List<QuantFile> QuantFiles = ImportQuantFiles();

                    //Apply Normalization to Each Data Set in the QuantFile
                    NormalizeQuantFiles(QuantFiles);

                    //Combine Mulitple QuantFiles into one QuantFile
                    List<QuantFile> QuantFilesToPrint = CombineQuantFiles(QuantFiles);

                    //Group the quantitative data
                    GroupQuantEntryData(QuantFilesToPrint);

                    //Build Comparisons for Each Quant File
                    BuildComparisons(QuantFilesToPrint);

                    //Significance Testing
                    PerformSignificanceTest(QuantFilesToPrint);

                    //Multiple Correction (pvalue FDR)
                    bool printQValue = PerformMultipleComparrisonCorrection(QuantFilesToPrint);

                    //GO Annotation and Enrichment Analysis
                    bool printEnrichedAnnotations = false;
                    bool printAnnotations = AnnotateQuantFileAndTestEnrichment(QuantFilesToPrint, out printEnrichedAnnotations);

                    //Print the files
                    PrintQuantifiedFiles(QuantFilesToPrint, printQValue, printAnnotations);

                    //Print the enriched Annotations
                    if (printEnrichedAnnotations)
                    {
                        PrintEnrichedAnnotations(QuantFilesToPrint);
                    }

                    //Print perseus outputs - needs some work. 
                    if (perseusOutput.Checked)
                    {
                        PrintPerseusOutput(QuantFilesToPrint);
                    }

                    //Print a custom database if the user has loaded an uniprot XML file
                    if (userDatabase.Checked || customDatabaseText.Text.Contains(".xml"))
                    {
                        PrintDatabase();
                    }

                    WriteLogFiles();

                    //Clean up incase the user wants to run the program again. 
                    QuantFiles.Clear();
                    QuantFilesToPrint.Clear();
                    FileNameAndHeadertoUniqueGroupName.Clear();
                    UniqueGroupNametoGroupString.Clear();
                    HeadersToIncludeByFile.Clear();
                    FileNametoHeadersToQuantify.Clear();
                    Comparisons.Clear();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Unique Group Names Contains Non-unique Name!");
                }
            }
            catch (Exception exp)
            {
                UpdateLog("Error! " + exp.Message);
            }
        }

        private void LoadUserInputs()
        {
            //Get the Column that Contains Uniprot Header(s)
            string uniprotHeader = uniprotIDHeader.SelectedItem.ToString();
            if (uniprotHeader.Equals(""))
            {
                uniprotHeader = uniprotIDHeader.Items[0].ToString();
            }

            UniprotHeader = uniprotHeader;

            //Get or create the output file directory
            if (!Directory.Exists(outputFolderTextBox.Text))
            {
                Directory.CreateDirectory(outputFolderTextBox.Text);
            }
            OutputFolder = outputFolderTextBox.Text;


            //Get the value to use for the 
            ValueForComparison = ValueType.MeanNormalizedLog2Change;

            if(intensityFoldChange.Checked) { ValueForComparison = ValueType.IntensityFoldChange; }
            else if (intensityLog2.Checked) { ValueForComparison = ValueType.IntensityLog2Change; }
            if (meanNormFoldChange.Checked) { ValueForComparison = ValueType.MeanNormalizedFoldChange; }
            else if (meanNormLog2.Checked) { ValueForComparison = ValueType.MeanNormalizedLog2Change; }

            foreach (DataGridViewRow row in comparisonDataGridView.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    string numeratorGroupString = row.Cells[0].Value.ToString();
                    string denominatorGroupString = row.Cells[1].Value.ToString();

                    bool testSignificance = true;

                    if (numeratorGroupString.Equals("All"))
                    {
                        foreach (string groupNumber in Numerators)
                        {
                            if (!groupNumber.Equals("All") && !groupNumber.Equals(denominatorGroupString))
                            {
                                numeratorGroupString = groupNumber;
                                Comparison comparison = new Comparison(numeratorGroupString, denominatorGroupString, testSignificance);

                                Comparisons.Add(comparison);
                            }
                        }

                    }
                    else
                    {
                        Comparison comparison = new Comparison(numeratorGroupString, denominatorGroupString, testSignificance);

                        Comparisons.Add(comparison);
                    }
                }
            }

            groupDataGrid.Update();
            foreach (AnalysisGroupLine analysisGroupLine in AnalysisGroups)
            {
                string filename = analysisGroupLine.FileName;
                string headername = analysisGroupLine.HeaderName;
                string filelocation = analysisGroupLine.FileLocation;
                string uniquegroupname = analysisGroupLine.UniqueName;
                string fileAndHeader = filelocation + headername;
                string groupString = analysisGroupLine.GroupString;

                FileNameAndHeadertoUniqueGroupName.Add(fileAndHeader, uniquegroupname);
                UniqueGroupNametoGroupString.Add(uniquegroupname, groupString);

                Dictionary<string, List<string>> outDictionary = null;
                if (FiletoGroupNametoListofHeaders.TryGetValue(filelocation, out outDictionary))
                {
                    List<string> outString = null;
                    if (outDictionary.TryGetValue(groupString, out outString))
                    {
                        outString.Add(headername);
                    }
                    else
                    {
                        List<string> addList = new List<string>();
                        addList.Add(headername);
                        outDictionary.Add(groupString, addList);
                    }
                }
                else
                {
                    List<string> addList = new List<string>();
                    addList.Add(headername);
                    Dictionary<string, List<string>> addDict = new Dictionary<string, List<string>>();
                    addDict.Add(groupString, addList);
                    FiletoGroupNametoListofHeaders.Add(filelocation, addDict);
                }

                //Build a dictionary with the File Location and the headers to Quantify
                List<string> outList = new List<string>();
                if (FileNametoHeadersToQuantify.TryGetValue(filelocation, out outList))
                {
                    outList.Add(headername);
                }
                else
                {
                    List<string> addList = new List<string>();
                    addList.Add(headername);
                    FileNametoHeadersToQuantify.Add(filelocation, addList);
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp in FiletoGroupNametoListofHeaders)
            {
                List<string> headersToAdd = new List<string>();
                HeadersToIncludeByFile.Add(kvp.Key, headersToAdd);

                foreach (Comparison comparison in Comparisons)
                {
                    //Add all the headers to limit to in HeadersToInclude
                    List<string> outList = new List<string>();
                    if (kvp.Value.TryGetValue(comparison.NumeratorGroupString, out outList))
                    {
                        foreach (string header in outList)
                        {
                            if (!HeadersToIncludeByFile[kvp.Key].Contains(header))
                            {
                                HeadersToIncludeByFile[kvp.Key].Add(header);
                            }
                        }
                    }

                    List<string> outList2 = new List<string>();
                    if (kvp.Value.TryGetValue(comparison.DenominatorGroupString, out outList2))
                    {
                        foreach (string header in outList2)
                        {
                            if (!HeadersToIncludeByFile[kvp.Key].Contains(header))
                            {
                                HeadersToIncludeByFile[kvp.Key].Add(header);
                            }
                        }
                    }
                }
            }
           
        }

        private bool TestUniqueNames(BindingList<AnalysisGroupLine> groupLines)
        {
            bool nonUniqueName = false;

            List<string> uniqueNameList = new List<string>();

            foreach (AnalysisGroupLine groupLine in groupLines)
            {
                if (!uniqueNameList.Contains(groupLine.UniqueName))
                {
                    uniqueNameList.Add(groupLine.UniqueName);
                }
                else
                {
                    nonUniqueName = true;
                }
            }

            return nonUniqueName;
        }

        private List<QuantFile> ImportQuantFiles()
        {
            UpdateLog("Importing Quant Files...");

            //This will be the list of Quant Files
            List<QuantFile> retList = new List<QuantFile>();

            //Load the headers that you will want to print in the final document. 
            String[] headersToPrintArray = new string[headersToPrintListBox.CheckedItems.Count];
            headersToPrintListBox.CheckedItems.CopyTo(headersToPrintArray, 0);
            List<String> headersToPrintList = headersToPrintArray.ToList();

            if (!headersToPrintList.Contains(UniprotHeader))
            {
                headersToPrintList.Add(UniprotHeader);
            }

            //Load the two strings that you will use to determine a unique group when combining files
            string uniqueIDString = headerComboBox.SelectedItem.ToString();
            string uniqueIDString2 = headerComboBox2.SelectedItem.ToString();

            //Create Quant Files for each loaded CSV - Files will not be combined at this time
            foreach (KeyValuePair<string, List<string>> kvp in FileNametoHeadersToQuantify)
            {
                //Open the CSV file 
                using (CsvReader reader = new CsvReader(new StreamReader(kvp.Key), true))
                {
                    //Create the Quant File that you will populate
                    QuantFile quantFile = new QuantFile(kvp.Key, kvp.Value);

                    //Set the UniprotHeader
                    quantFile.UniprotHeader = UniprotHeader;

                    //Read in each line and make a Quant Entry
                    while (reader.ReadNextRecord())
                    {
                        //Make sure that there is quantitative data for all measurements in the file
                        bool addQuantData = true;

                        //Do this check if you are mean normalizing
                        if (ValueForComparison == ValueType.MeanNormalizedFoldChange || ValueForComparison == ValueType.MeanNormalizedLog2Change || medianNorm.Checked || sumNorm.Checked)
                        {
                            //Check all of the headers in the file regardless of if they are in a comparison
                            foreach (string header in kvp.Value)
                            {
                                string parseThis = reader[header];
                                double outDouble = 0;
                                if (!double.TryParse(parseThis, out outDouble) || outDouble == 0)
                                {
                                    addQuantData = false;
                                }
                            }
                        }
                        else
                        {
                            //Only check the headers that are involved in a comparison HeadersToIncludeByFile
                            foreach (string header in HeadersToIncludeByFile[kvp.Key])
                            {
                                string parseThis = reader[header];
                                double outDouble = 0;
                                if (!double.TryParse(parseThis, out outDouble) || outDouble == 0)
                                {
                                    addQuantData = false;
                                }
                            }
                        }

                        //If there are not "0" or "-" values then add this Quant Entry
                        if (addQuantData)
                        {
                            //Make a new Quant Entry
                            QuantEntry quantEntry = new QuantEntry(quantFile.FileLoaction, reader[uniqueIDString], reader[uniqueIDString2], UniqueGroupNametoGroupString);

                            //Load up the data in the columns that you will want to print at the end.
                            foreach (string header in headersToPrintList)
                            {
                                string outString = null;
                                if (!quantFile.HeadersToPrint.TryGetValue(header, out outString))
                                {
                                    quantFile.HeadersToPrint.Add(header, header);
                                }

                                quantEntry.HeadersToPrintDict.Add(header, reader[header]);
                            }

                            //Load up the data from each of the columns that you want to quantify. 
                            foreach (string header in kvp.Value)
                            {
                                //Get the Quant Value to Parse
                                string testit = reader[header];

                                double a = double.Parse(reader[header]);

                                if (a == 0)
                                {
                                    int b = 0;
                                }

                                quantFile.HeadertoValueDict[header].Add(double.Parse(reader[header]));
                                quantEntry.SampleValues.Add(header, double.Parse(reader[header]));

                                //Get the file name + header and create a unique hash code that you will map the unique header name
                                string fileAndHeader = quantFile.FileLoaction + header;
                                long fileAndHeaderHash = fileAndHeader.GetHashCode();

                                //Only add these once - this will map the File Location + Header code to the unique name given by the user
                                string outString = null;
                                if (!quantFile.UniqueGroupNamesToPrint.TryGetValue(fileAndHeader, out outString))
                                {
                                    quantFile.UniqueGroupNamesToPrint.Add(fileAndHeader, FileNameAndHeadertoUniqueGroupName[fileAndHeader]);
                                }
                            }

                            //Add that Quant Entry to the List within the Quant File
                            quantFile.QuantEntries.Add(quantEntry);
                        }
                    }

                    retList.Add(quantFile);
                }
            }

            return retList;
        }

        private void NormalizeQuantFiles(List<QuantFile> quantFiles)
        {
            bool noNormalize = noNorm.Checked;
            bool medianNormalize = medianNorm.Checked;
            bool sumNormalize = sumNorm.Checked;

            if (!noNormalize)
            {
                UpdateLog("Normalizing Quant Files...");
            }

            string normalizationString = normalizationComboBox.SelectedItem.ToString();
            if (normalizationString.Equals(""))
            {
                normalizationString = normalizationComboBox.Items[0].ToString();
            }

            NormalizationSubetString = normalizationString;

            foreach (QuantFile quantFile in quantFiles)
            {
                List<string> uniprotIDsForNormalization = new List<string>();
                string normalizationTestBoxString = normalizationTextBox.Text;

                if (normalizationString.Equals("Subset - Uniprot (.csv)"))
                {
                    string inputFile = normalizationTextBox.Text;
                    using (CsvReader reader = new CsvReader(new StreamReader(normalizationTextBox.Text), true))
                    {
                        while (reader.ReadNextRecord())
                        {
                            string uniprotLine = reader["Uniprot"];
                            List<string> uniprotIDs = uniprotLine.Split('|').ToList();

                            foreach (string uniprotID in uniprotIDs)
                            {
                                uniprotIDsForNormalization.Add(uniprotID);
                            }
                        }
                    }
                }
                else if (normalizationString.Contains("Subset"))
                {
                    //PrintAnnotations = true;

                    List<AnnotationType> annotationsToAdd = new List<AnnotationType>();
                    annotationsToAdd.Add(AnnotationType.GOCellularComponent);
                    annotationsToAdd.Add(AnnotationType.GOMolecularFunction);
                    annotationsToAdd.Add(AnnotationType.GOBiologicalProcesses);
                    annotationsToAdd.Add(AnnotationType.Keywords);

                    AnnotationsToAdd = annotationsToAdd;

                    //Decides wether to do a significance test (TTest)
                    string goDatabaseString = goDatabaseComboBox.SelectedItem.ToString();
                    if (goDatabaseString.Equals(""))
                    {
                        goDatabaseString = goDatabaseComboBox.Items[0].ToString();
                    }

                    AnnotationDatabase = goDatabaseString;
                    double annotationThreshold = double.Parse(annotationSigTextBox.Text);

                    //Annotate The File if necessary
                    //Start by building the annotation dictionary
                    if (AnnotationEntryDict.Count == 0 || CurrentOrganism != goDatabaseString)
                    {
                        AnnotationEntryDict.Clear();
                        UniprotIDtoAnnotationList.Clear();

                        if (!userDatabase.Checked)
                        {
                            LoadEmbededDatabase(goDatabaseString);
                        }
                        else if (userDatabase.Checked)
                        {
                            string databaseFile = GetDatabasePath(goDatabaseString);
                            if (databaseFile.Contains(".csv"))
                            {
                                ImportSavedProcyonDatabase(databaseFile);
                            }
                            else if (databaseFile.Contains(".xml"))
                            {
                                BuildAnnotationDictionary(goDatabaseString, annotationsToAdd);
                            }
                        }

                    }

                    foreach (KeyValuePair<string, Dictionary<AnnotationType, List<string>>> kvp in UniprotIDtoAnnotationList)
                    {
                        AnnotationType testType = GetAnnotationType(normalizationString.Replace("Subset - ", ""));
                        List<string> annotationHashes = null;
                        if (kvp.Value.TryGetValue(testType, out annotationHashes))
                        {
                            foreach (string annotationHash in annotationHashes)
                            {
                                AnnotationEntry testEntry = AnnotationEntryDict[annotationHash];
                                if (testEntry.Name.Contains(normalizationTestBoxString))
                                {
                                    if (!uniprotIDsForNormalization.Contains(kvp.Key))
                                    {
                                        uniprotIDsForNormalization.Add(kvp.Key);
                                    }
                                }
                            }
                        }
                    }
                }
                

                quantFile.NormalizeDataSet(noNormalize, sumNormalize, medianNormalize, UniprotHeader, normalizationString, uniprotIDsForNormalization);
                
            }
        }

        private List<QuantFile> CombineQuantFiles(List<QuantFile> quantFiles)
        {
            List<QuantFile> retList = new List<QuantFile>();

            if (quantFiles.Count > 1)
            {
                UpdateLog("Combining Quant Files...");
            }

            QuantFile combinedQuantFile = new QuantFile(quantFiles, FileNameAndHeadertoUniqueGroupName);

            retList.Add(combinedQuantFile);

            return retList;
        }

        private void GroupQuantEntryData(List<QuantFile> quantFiles)
        {
            UpdateLog("Creating Data Groups...");

            foreach (QuantFile quantFile in quantFiles)
            {
                quantFile.GroupQuantData();
            }
        }

        private void BuildComparisons(List<QuantFile> quantFiles)
        {
            if (Comparisons.Count > 0)
            {
                UpdateLog("Making Comparisons...");
            }
            foreach (QuantFile quantFile in quantFiles)
            {
                foreach (Comparison comparison in Comparisons)
                {
                    quantFile.AddComparison(comparison, ValueForComparison);
                }
            }
        }

        private void PerformSignificanceTest(List<QuantFile> quantFiles)
        {
            //Decides wether to do a significance test (TTest)
            string sigTestingString = sigTestingComboBox.SelectedItem.ToString();
            if (sigTestingString.Equals(""))
            {
                sigTestingString = sigTestingComboBox.Items[0].ToString();
            }

            SigTestingString = sigTestingString;

            if (!sigTestingString.Equals("None"))
            {
                TestSignificance = true;
                UpdateLog("Performing Significance Tests...");
            }

            UseFoldChange = false;
            if (sigTestingString.Equals("Fold Change"))
            {
                UseFoldChange = true;
            }

            //Determines the Threshold for significance
            double sigThresholdValue = double.Parse(sigThershold.Text);

            foreach (QuantFile quantFile in quantFiles)
            {
                quantFile.PerformSignificanceTest(sigTestingString, sigThresholdValue, ValueForComparison);
            }
        }

        private bool PerformMultipleComparrisonCorrection(List<QuantFile> quantFiles)
        {
            //Decides wether to do a significance test (TTest)
            string mulCompCorrString = multCompCorr.SelectedItem.ToString();
            if (mulCompCorrString.Equals(""))
            {
                mulCompCorrString = multCompCorr.Items[0].ToString();
            }

            MultipleCorrectionString = mulCompCorrString;

            bool printQValue = true;
            if (mulCompCorrString.Equals("None"))
            {
                printQValue = false;
            }
            else
            {
                UpdateLog("Correcting for Multiple Comparisons...");
            }

            //Decides wether to do a significance test (TTest)
            string thresholdTypeString = thresholdType.SelectedItem.ToString();
            if (thresholdTypeString.Equals(""))
            {
                thresholdTypeString = thresholdType.Items[0].ToString();
            }

            ThresholdType = thresholdTypeString;

            //Determines the Threshold for significance
            double sigThresholdValue = double.Parse(sigThershold.Text);

            ThresholdValue = sigThresholdValue;

            foreach (QuantFile quantFile in quantFiles)
            {
                quantFile.MulitpleComparisonCorrection(mulCompCorrString, thresholdTypeString, sigThresholdValue);
            }


            return printQValue;
        }

        private bool AnnotateQuantFileAndTestEnrichment(List<QuantFile> quantFiles, out bool printEnrichedAnnotations)
        {
            printEnrichedAnnotations = true;
            bool printAnnotaions = false;

            //Decides wether to do a significance test (TTest)
            string annotationString = annotationComboBox.SelectedItem.ToString();
            if (annotationString.Equals(""))
            {
                annotationString = annotationComboBox.Items[0].ToString();
            }

            GOString = annotationString;

            if (annotationString.Equals("None"))
            {
                printEnrichedAnnotations = false;
            }
            else if (annotationString.Equals("Annotate Only"))
            {
                printEnrichedAnnotations = false;
                UpdateLog("Annotating Only...");
            }
            else
            {
                UpdateLog("Annotating and Testing Enrichment...");
            }


            if (!annotationString.Equals("None") || PrintAnnotations)
            {
                printAnnotaions = true;
                string[] annotationsToPrintArray = new string[goTermsCheckListBox.CheckedItems.Count];
                goTermsCheckListBox.CheckedItems.CopyTo(annotationsToPrintArray, 0);
                List<String> annotationStrings = annotationsToPrintArray.ToList();

                List<AnnotationType> annotationsToAdd = new List<AnnotationType>();

                foreach (string annotation in annotationStrings)
                {
                    annotationsToAdd.Add(GetAnnotationType(annotation));
                }

                //Decides wether to do a significance test (TTest)
                string goDatabaseString = goDatabaseComboBox.SelectedItem.ToString();
                if (goDatabaseString.Equals(""))
                {
                    goDatabaseString = goDatabaseComboBox.Items[0].ToString();
                }

                AnnotationDatabase = goDatabaseString;

                double annotationThreshold = double.Parse(annotationSigTextBox.Text);

                //Annotate The File if necessary
                //Start by building the annotation dictionary
                if (AnnotationEntryDict.Count == 0 || CurrentOrganism != goDatabaseString)
                {
                    AnnotationEntryDict.Clear();
                    UniprotIDtoAnnotationList.Clear();

                    if (!userDatabase.Checked)
                    {
                        LoadEmbededDatabase(goDatabaseString);
                    }
                    else if (userDatabase.Checked)
                    {
                        string databaseFile = GetDatabasePath(goDatabaseString);
                        if(databaseFile.Contains(".csv"))
                        {
                            ImportSavedProcyonDatabase(databaseFile);
                        }
                        else if (databaseFile.Contains(".xml"))
                        {
                            BuildAnnotationDictionary(goDatabaseString, annotationsToAdd);
                        }
                    }

                    AnnotationsToAdd = annotationsToAdd;
                }

                //For each file go through and test the enrichment
                foreach (QuantFile quantFile in quantFiles)
                {
                    quantFile.TestAnnotationEnrichment(annotationString, annotationsToAdd, AnnotationEntryDict, UniprotIDtoAnnotationList, annotationThreshold);
                }
            }

            return printAnnotaions;
        }

        private void LoadEmbededDatabase(string databaseString)
        {
            string databaseLocation = null;
            if (databaseString.Equals("Mouse"))
            {
                databaseLocation = "Coon.Compass.Procyon.Procyon_Mouse_GODatabase.csv";
            }
            else if (databaseString.Equals("Human"))
            {
                databaseLocation = "Coon.Compass.Procyon.Procyon_Human_GODatabase.csv";
            }
            else if (databaseString.Equals("Yeast"))
            {
                databaseLocation = "Coon.Compass.Procyon.Procyon_Yeast_GODatabase.csv";
            }

            if (databaseLocation != null)
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(databaseLocation))
                using (CsvReader reader = new CsvReader(new StreamReader(stream), true))
                {
                    while (reader.ReadNextRecord())
                    {
                        if (reader["Linkage"].Equals("Connection"))
                        {
                            string uniprotID = reader["ID"];
                            string goIDList = reader["Contents"];
                            string type = reader["Type"];

                            AnnotationType addType = GetAnnotationType(type);

                            Dictionary<AnnotationType, List<string>> outDict = new Dictionary<AnnotationType, List<string>>();
                            if (UniprotIDtoAnnotationList.TryGetValue(uniprotID, out outDict))
                            {
                                List<string> outStringList = null;
                                if (outDict.TryGetValue(addType, out outStringList))
                                {
                                    foreach (string goID in goIDList.Split('|').ToList())
                                    {
                                        outStringList.Add(goID);
                                    }
                                }
                                else
                                {
                                    List<String> addList = new List<string>();
                                    foreach (string goID in goIDList.Split('|').ToList())
                                    {
                                        addList.Add(goID);
                                    }
                                    outDict.Add(addType, addList);
                                }
                            }
                            else
                            {
                                List<string> addList = new List<string>();
                                foreach (string goID in goIDList.Split('|').ToList())
                                {
                                    addList.Add(goID);
                                }
                                Dictionary<AnnotationType, List<string>> addDict = new Dictionary<AnnotationType, List<string>>();
                                addDict.Add(addType, addList);

                                UniprotIDtoAnnotationList.Add(uniprotID, addDict);
                            }

                        }
                        else if (reader["Linkage"].Equals("Annotation"))
                        {
                            string annotationID = reader["ID"];
                            string annotationName = reader["Contents"];
                            string type = reader["Type"];

                            AnnotationType addType = GetAnnotationType(type);

                            AnnotationEntry addEntry = new AnnotationEntry(annotationID, annotationName, addType);

                            AnnotationEntryDict.Add(addEntry.UniqueID, addEntry);
                        }
                    }
                }
            }
        }

        private void BuildAnnotationDictionary(string databaseString, List<AnnotationType> annotationsToAdd)
        {
            //Fill the AnnotationEntryDict and UniprotIDtoAnnotationList
            CurrentOrganism = databaseString;

            string databaseFile = GetDatabasePath(databaseString);

            UniProtXml uniprotXml = new UniProtXml(databaseFile);

            foreach (entry uniprotEntry in uniprotXml.Entries)
            {
                List<string> uniprotIDs = new List<string>();
                List<AnnotationEntry> annotationEntries = new List<AnnotationEntry>();
                foreach (string acession in uniprotEntry.accession)
                {
                    uniprotIDs.Add(acession);
                }

                if(annotationsToAdd.Contains(AnnotationType.Keywords))
                {
                    foreach (keywordType keyword in uniprotEntry.keyword)
                    {
                        string keywordID = keyword.id;
                        AnnotationEntry outEntry = null;

                        if (AnnotationEntryDict.TryGetValue(keywordID, out outEntry))
                        {
                            annotationEntries.Add(outEntry);
                        }
                        else
                        {
                            AnnotationEntry addEntry = new AnnotationEntry(keywordID, keyword.Value, AnnotationType.Keywords);
                            annotationEntries.Add(addEntry);
                            AnnotationEntryDict.Add(addEntry.UniqueID, addEntry);
                        }
                    }
                }

                foreach (dbReferenceType goTerm in uniprotEntry.dbReference)
                {
                    if (goTerm.type.Equals("GO"))
                    {
                        string goID = goTerm.id;
                        AnnotationEntry outEntry = null;

                        if(AnnotationEntryDict.TryGetValue(goID, out outEntry))
                        {
                            annotationEntries.Add(outEntry);
                        }
                        else
                        {
                            string goName = null;
                            AnnotationType goType = AnnotationType.GOCellularComponent;
                            foreach (propertyType property in goTerm.property)
                            {
                                if (property.type.Equals("term"))
                                {
                                    string goString = property.value;
                                    string[] goArray = goString.Split(':');

                                    if (goArray[0].Equals("C"))
                                    {
                                        goType = AnnotationType.GOCellularComponent;
                                    }
                                    else if (goArray[0].Equals("F"))
                                    {
                                        goType = AnnotationType.GOMolecularFunction;
                                    }
                                    else if (goArray[0].Equals("P"))
                                    {
                                        goType = AnnotationType.GOBiologicalProcesses;
                                    }

                                    goName = goArray[1];
                                }
                            }

                            if (annotationsToAdd.Contains(goType))
                            {
                                AnnotationEntry addEntry = new AnnotationEntry(goID, goName, goType);
                                annotationEntries.Add(addEntry);
                                AnnotationEntryDict.Add(addEntry.UniqueID, addEntry);
                            }
                        }
                    }
                }

                //Build the connections to the UniprotIDs
                foreach (AnnotationEntry annotationEntry in annotationEntries)
                {
                    foreach (string uniprotID in uniprotIDs)
                    {
                        Dictionary<AnnotationType, List<string>> outDict = null;
                        if (UniprotIDtoAnnotationList.TryGetValue(uniprotID, out outDict))
                        {
                            List<string> outList = null;
                            if (outDict.TryGetValue(annotationEntry.Type, out outList))
                            {
                                if(!outList.Contains(annotationEntry.UniqueID))
                                {
                                    outList.Add(annotationEntry.UniqueID);
                                }
                            }
                            else
                            {
                                List<string> addList = new List<string>();
                                addList.Add(annotationEntry.UniqueID);
                                outDict.Add(annotationEntry.Type, addList);
                            }
                        }
                        else
                        {
                            Dictionary<AnnotationType, List<string>> addDict = new Dictionary<AnnotationType, List<string>>();
                            List<string> addList = new List<string>();
                            addList.Add(annotationEntry.UniqueID);
                            addDict.Add(annotationEntry.Type, addList);

                            UniprotIDtoAnnotationList.Add(uniprotID, addDict);
                        }
                    }
                }
            }

            uniprotXml.Dispose();
        }

        private void ImportSavedProcyonDatabase(string databaseFile)
        {
            using (CsvReader reader = new CsvReader(new StreamReader(databaseFile), true))
            {
                while (reader.ReadNextRecord())
                {
                    if (reader["Linkage"].Equals("Connection"))
                    {
                        string uniprotID = reader["ID"];
                        string goIDList = reader["Contents"];
                        string type = reader["Type"];

                        AnnotationType addType = GetAnnotationType(type);

                        Dictionary<AnnotationType, List<string>> outDict = new Dictionary<AnnotationType, List<string>>();
                        if (UniprotIDtoAnnotationList.TryGetValue(uniprotID, out outDict))
                        {
                            List<string> outStringList = null;
                            if (outDict.TryGetValue(addType, out outStringList))
                            {
                                foreach (string goID in goIDList.Split('|').ToList())
                                {
                                    outStringList.Add(goID);
                                }
                            }
                            else
                            {
                                List<String> addList = new List<string>();
                                foreach (string goID in goIDList.Split('|').ToList())
                                {
                                    addList.Add(goID);
                                }
                                outDict.Add(addType, addList);
                            }
                        }
                        else
                        {
                            List<string> addList = new List<string>();
                            foreach (string goID in goIDList.Split('|').ToList())
                            {
                                addList.Add(goID);
                            }
                            Dictionary<AnnotationType, List<string>> addDict = new Dictionary<AnnotationType, List<string>>();
                            addDict.Add(addType, addList);

                            UniprotIDtoAnnotationList.Add(uniprotID, addDict);
                        }

                    }
                    else if (reader["Linkage"].Equals("Annotation"))
                    {
                        string annotationID = reader["ID"];
                        string annotationName = reader["Contents"];
                        string type = reader["Type"];

                        AnnotationType addType = GetAnnotationType(type);

                        AnnotationEntry addEntry = new AnnotationEntry(annotationID, annotationName, addType);

                        AnnotationEntryDict.Add(addEntry.UniqueID, addEntry);
                    }
                }
            }
        }

        private string GetDatabasePath(string databaseString)
        {
            string databaseFile = null;
            if (userDatabase.Checked)
            {
                databaseFile = customDatabaseText.Text;
            }

            return databaseFile;
        }

        private void PrintQuantifiedFiles(List<QuantFile> quantFiles, bool printQValue, bool printAnnotations)
        {
            UpdateLog("Printing Quantified Files...");
            foreach (QuantFile quantFile in quantFiles)
            {
                List<string> uniqueNames = quantFile.CombinedGroupNamesToPrint.Values.ToList();
                uniqueNames.Sort();

                string outputFile = OutputFolder + "\\Procyon_QuantifiedFile.csv";

                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    #region Build and Write Header

                    StringBuilder headerLine = new StringBuilder();

                    foreach (string header in quantFile.CombinedHeadersToPrint.Values)
                    {
                        headerLine.Append(header.Replace(","," "));
                        headerLine.Append(',');
                    }

                    headerLine.Append("Log2 of Normalized Intensity");
                    headerLine.Append(',');

                    foreach (string groupName in uniqueNames)
                    {
                        headerLine.Append(groupName);
                        headerLine.Append(',');
                    }

                    if (ValueForComparison == ValueType.MeanNormalizedFoldChange || ValueForComparison == ValueType.MeanNormalizedLog2Change)
                    {
                        headerLine.Append("Mean Normalized Log2 of Normalized Intensity");
                        headerLine.Append(',');

                        foreach (string groupName in uniqueNames)
                        {
                            headerLine.Append(groupName);
                            headerLine.Append(',');
                        }
                    }

                    headerLine.Append("Ratios");
                    headerLine.Append(',');

                    foreach (Comparison comparison in quantFile.Comparisons.Values)
                    {
                        headerLine.Append(comparison.ComparisonName);
                        headerLine.Append(',');
                    }

                    if (TestSignificance)
                    {
                        if (!UseFoldChange)
                        {
                            headerLine.Append("p-Value");
                            headerLine.Append(',');

                            foreach (Comparison comparison in quantFile.Comparisons.Values)
                            {
                                headerLine.Append(comparison.ComparisonName);
                                headerLine.Append(',');
                            }
                        }

                        if (printQValue)
                        {
                            headerLine.Append("P-Value");
                            headerLine.Append(',');

                            foreach (Comparison comparison in quantFile.Comparisons.Values)
                            {
                                headerLine.Append(comparison.ComparisonName);
                                headerLine.Append(',');
                            }
                        }


                        headerLine.Append("Significant?");
                        headerLine.Append(',');

                        foreach (Comparison comparison in quantFile.Comparisons.Values)
                        {
                            if (comparison.TestSignificance)
                            {
                                headerLine.Append(comparison.ComparisonName);
                                headerLine.Append(',');
                            }
                        }
                    }

                    if (printAnnotations || PrintAnnotations)
                    {
                        foreach (AnnotationType annotation in AnnotationsToAdd)
                        {
                            headerLine.Append('|');
                            headerLine.Append(',');
                            headerLine.Append(annotation.ToString());
                            headerLine.Append(',');
                        }

                        headerLine.Append('|');
                    }

                    writer.WriteLine(headerLine.ToString());

                    #endregion

                    foreach (QuantEntry quantEntry in quantFile.CombinedQuantEntryDict.Values)
                    {
                        StringBuilder quantLine = new StringBuilder();

                        foreach (string header in quantFile.CombinedHeadersToPrint.Values)
                        {
                            List<string> headerList = quantEntry.CombinedHeadersToPrintSampleValues[header];
                            foreach (string value in headerList)
                            {
                                string newValue = value;
                                if (cleanDefline.Checked && header.Equals("Representative Protein Description") || header.Equals("Defline") || header.Equals("Protein Description") || header.Equals("Fasta headers"))
                                {
                                    newValue = CleanDefline(value);
                                }
                                quantLine.Append(newValue.Replace(","," "));
                                quantLine.Append('|');
                            }

                            quantLine.Remove(quantLine.Length - 1, 1);

                            quantLine.Append(',');
                        }

                        quantLine.Append('|');

                        foreach (string groupName in uniqueNames)
                        {
                            double value = 0;
                            if (quantEntry.CombinedLog2NormalizedSampleValues.TryGetValue(groupName, out value))
                            {
                                value = quantEntry.CombinedLog2NormalizedSampleValues[groupName];
                            }

                            if (value.ToString().Contains("Infinity"))
                            {
                                value = 0;
                            }

                            quantLine.Append(',');
                            quantLine.Append(value.ToString("0.##"));
                            
                        }

                        quantLine.Append(',');
                        quantLine.Append('|');

                        if (ValueForComparison == ValueType.MeanNormalizedFoldChange || ValueForComparison == ValueType.MeanNormalizedLog2Change)
                        {
                            foreach (string groupName in uniqueNames)
                            {
                                double value = 0;
                                if (quantEntry.CombinedMeanNormalizedSampleValues.TryGetValue(groupName, out value))
                                {
                                    value = quantEntry.CombinedMeanNormalizedSampleValues[groupName];
                                }
                                quantLine.Append(',');
                                quantLine.Append(value.ToString("0.#####"));
                            }

                            quantLine.Append(',');
                            quantLine.Append('|');
                        }

                        foreach (Comparison comparison in quantFile.Comparisons.Values)
                        {
                            double outDouble = 0;
                            if (quantEntry.ComparisonToPrintAverage.TryGetValue(comparison.ComparisonName, out outDouble))
                            {
                                quantLine.Append(',');
                                quantLine.Append(quantEntry.ComparisonToPrintAverage[comparison.ComparisonName].ToString("0.##"));
                            }
                            else
                            {
                                quantLine.Append(',');
                                quantLine.Append(0);
                            }
                        }

                        if (TestSignificance)
                        {

                            quantLine.Append(',');
                            quantLine.Append('|');

                            if (!UseFoldChange)
                            {
                                foreach (Comparison comparison in quantFile.Comparisons.Values)
                                {
                                    double outDouble = 0;
                                    if (quantEntry.ComparisonToPValue.TryGetValue(comparison.ComparisonName, out outDouble))
                                    {
                                        if (comparison.TestSignificance)
                                        {
                                            quantLine.Append(',');
                                            quantLine.Append(quantEntry.ComparisonToPValue[comparison.ComparisonName].ToString("0.#######"));
                                        }
                                    }
                                    else
                                    {
                                        quantLine.Append(',');
                                        quantLine.Append(0);
                                    }
                                }

                                quantLine.Append(',');
                                quantLine.Append('|');
                            }

                            if (printQValue)
                            {
                                foreach (Comparison comparison in quantFile.Comparisons.Values)
                                {
                                    double outDouble = 0;
                                    if (quantEntry.ComparisonToQValue.TryGetValue(comparison.ComparisonName, out outDouble))
                                    {
                                        if (comparison.TestSignificance)
                                        {
                                            quantLine.Append(',');
                                            quantLine.Append(quantEntry.ComparisonToQValue[comparison.ComparisonName].ToString("0.#######"));
                                        }
                                    }
                                    else
                                    {
                                        quantLine.Append(',');
                                        quantLine.Append(0);
                                    }
                                }

                                quantLine.Append(',');
                                quantLine.Append('|');
                            }

                            foreach (Comparison comparison in quantFile.Comparisons.Values)
                            {
                                bool outBool = false;
                                if (quantEntry.ComparisonToSignificanceDict.TryGetValue(comparison.ComparisonName, out outBool))
                                {
                                    if (comparison.TestSignificance)
                                    {
                                        quantLine.Append(',');
                                        quantLine.Append(quantEntry.ComparisonToSignificanceDict[comparison.ComparisonName]);
                                    }
                                }
                                else
                                {
                                    quantLine.Append(',');
                                    quantLine.Append("FALSE");
                                }
                            }
                        }

                        if (printAnnotations || PrintAnnotations)
                        {
                            foreach (AnnotationType annotation in AnnotationsToAdd)
                            {
                                quantLine.Append(',');
                                quantLine.Append('|');
                                quantLine.Append(',');

                                bool removeLast = false;
                                List<AnnotationEntry> annotations = null;
                                if (quantEntry.AnnotationEntries.TryGetValue(annotation, out annotations))
                                {
                                    foreach (AnnotationEntry entry in annotations)
                                    {
                                        quantLine.Append(entry.Name.Replace(',', '.'));
                                        quantLine.Append(';');
                                    }

                                    removeLast = true;
                                }

                                if (removeLast)
                                {
                                    quantLine.Remove(quantLine.Length - 1, 1);
                                }
                            }

                            quantLine.Append(',');
                            quantLine.Append('|');
                        }


                        writer.WriteLine(quantLine.ToString());
                    }

                }
            }
        }

        private string CleanDefline(string defline)
        {
            string returnDefline = defline;

            string[] firstSplit = defline.Split('_');

            if (firstSplit.Count() > 1)
            {
                string[] secondSplit = firstSplit[1].Split('=');
                string firstRemove = secondSplit[0].Replace("MOUSE ", "");
                string secondRemove = firstRemove.Replace("HUMAN ", "");
                string thirdRemove = secondRemove.Replace(" OS", "");

                returnDefline = thirdRemove;
            }

            return returnDefline;
        }

        private void PrintEnrichedAnnotations(List<QuantFile> quantFiles)
        {
            UpdateLog("Printing Enriched Annotations...");
            foreach (QuantFile quantFile in quantFiles)
            {
                foreach (Comparison comparison in quantFile.Comparisons.Values)
                {
                    foreach (KeyValuePair<string, Dictionary<AnnotationType, Dictionary<string, double>>> kvp in comparison.AnnotationDictToPrint)
                    {
                        string sigType = kvp.Key;
                        string outputFileString = OutputFolder + "\\" + comparison.ComparisonName.Replace("/", "_") + "_" + sigType + "_AnnotationEnrichment.csv";
                        using (StreamWriter writer = new StreamWriter(outputFileString))
                        {
                            writer.WriteLine("GO Type,GO Name,Q-Value");

                            foreach (KeyValuePair<AnnotationType, Dictionary<string, double>> kvp2 in kvp.Value)
                            {
                                AnnotationType goType = kvp2.Key;

                                Dictionary<string, double> sortedDict = kvp2.Value.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

                                foreach (KeyValuePair<string, double> kvp3 in sortedDict)
                                {
                                    string goHash = kvp3.Key;
                                    double qvalue = kvp3.Value;

                                    AnnotationEntry annotationEntry = AnnotationEntryDict[goHash];

                                    StringBuilder sb = new StringBuilder();
                                    sb.Append(goType);
                                    sb.Append(',');
                                    sb.Append(annotationEntry.Name.Replace(',', '.'));
                                    sb.Append(',');
                                    sb.Append(qvalue);

                                    writer.WriteLine(sb.ToString());
                                }
                            }
                        }
                    }
                }
            }
        }

        private void PrintPerseusOutput(List<QuantFile> quantFiles)
        {
            UpdateLog("Printing Perseus Input File...");
            foreach (QuantFile quantFile in quantFiles)
            {
                foreach (Comparison comparison in quantFile.Comparisons.Values)
                {
                    List<string> uniqueNames = quantFile.CombinedGroupNamesToPrint.Values.ToList();
                    uniqueNames.Sort();

                    string outputFileString = OutputFolder + "\\" + comparison.ComparisonName.Replace("/", "_") + "_Perseus_Input.txt";

                    using (StreamWriter writer = new StreamWriter(outputFileString))
                    {
                        StringBuilder headerLine = new StringBuilder();
                        headerLine.Append("Uniprot");

                        foreach (string groupName in uniqueNames)
                        {
                            headerLine.Append("\t");
                            headerLine.Append(groupName);
                        }

                        writer.WriteLine(headerLine.ToString());

                        foreach (QuantEntry quantEntry in quantFile.CombinedQuantEntryDict.Values)
                        {
                            StringBuilder quantLine = new StringBuilder();

                            quantEntry.PopulateUniprotList(UniprotHeader);
                            quantLine.Append(quantEntry.UniprotStringList[0]);

                            bool addQuantLine = true;
                            foreach (string groupName in uniqueNames)
                            {
                                double value = 0;
                                if (!quantEntry.CombinedMeanNormalizedSampleValues.TryGetValue(groupName, out value))
                                {
                                    addQuantLine = false;
                                }
                            }

                            if (addQuantLine)
                            {
                                foreach (string groupName in uniqueNames)
                                {
                                    double value = 0;
                                    if (quantEntry.CombinedMeanNormalizedSampleValues.TryGetValue(groupName, out value))
                                    {
                                        value = quantEntry.CombinedMeanNormalizedSampleValues[groupName];
                                    }

                                    quantLine.Append("\t");
                                    quantLine.Append(value.ToString("0.#####"));
                                }

                                writer.WriteLine(quantLine.ToString());
                            }
                        }

                    }
                }
            }
        }

        private AnnotationType GetAnnotationType(string annotation)
        {
            AnnotationType retType = AnnotationType.None;

            switch (annotation)
            {
                case "GO-Cellular Component":
                    return AnnotationType.GOCellularComponent;
                case "GO-Molecular Function":
                    return AnnotationType.GOMolecularFunction;
                case "GO-Biological Processes":
                    return AnnotationType.GOBiologicalProcesses;
                case "GOCellularComponent":
                    return AnnotationType.GOCellularComponent;
                case "GOMolecularFunction":
                    return AnnotationType.GOMolecularFunction;
                case "GOBiologicalProcesses":
                    return AnnotationType.GOBiologicalProcesses;
                case "KEGG Pathway":
                    return AnnotationType.KEGGPathway;
                case "Protein Interaction":
                    return AnnotationType.ProteinInteraction;
                case "Keywords":
                    return AnnotationType.Keywords;
                default:
                    return retType;
            }
        }

        private void Procyon_Load(object sender, EventArgs e)
        {

        }

        private void PrintDatabase()
        {
            UpdateLog("Printing Custom Database for Future Use...");
            string outputstring = OutputFolder + "\\Procyon_GODatabase.csv";
            using (StreamWriter writer = new StreamWriter(outputstring))
            {
                writer.WriteLine("Linkage,ID,Contents,Type");
                foreach (KeyValuePair<string, Dictionary<AnnotationType, List<string>>> kvp in UniprotIDtoAnnotationList)
                {
                    foreach (KeyValuePair<AnnotationType, List<string>> kvp2 in kvp.Value)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Connection");
                        sb.Append(',');
                        sb.Append(kvp.Key);
                        sb.Append(',');

                        foreach (string addString in kvp2.Value)
                        {
                            sb.Append(addString);
                            sb.Append('|');
                        }

                        sb.Remove(sb.Length - 1, 1);

                        sb.Append(',');
                        sb.Append(kvp2.Key.ToString());

                        writer.WriteLine(sb.ToString());
                    }


                }

                foreach (AnnotationEntry entry in AnnotationEntryDict.Values)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Annotation");
                    sb.Append(',');
                    sb.Append(entry.UniqueID);
                    sb.Append(',');
                    sb.Append(entry.Name.Replace(",", " "));
                    sb.Append(',');
                    sb.Append(entry.Type.ToString());

                    writer.WriteLine(sb.ToString());
                }
            }
        }

        private void UpdateLog(string message)
        {
            logBox.AppendText(string.Format("[{0}]\t{1}\n", DateTime.Now.ToLongTimeString(), message));
            logBox.SelectionStart = logBox.Text.Length;
            logBox.ScrollToCaret();
        }

        private void WriteLogFiles()
        {
            string analysisGroupkey = OutputFolder + "\\Procyon_Log.csv";
            using (StreamWriter writer = new StreamWriter(analysisGroupkey))
            {
                writer.WriteLine("File,Header,Unique Name,Group Name");

                foreach (AnalysisGroupLine groupLine in AnalysisGroups)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(groupLine.FileLocation);
                    sb.Append(',');
                    sb.Append(groupLine.HeaderName);
                    sb.Append(',');
                    sb.Append(groupLine.UniqueName);
                    sb.Append(',');
                    sb.Append(groupLine.GroupString);

                    writer.WriteLine(sb.ToString());
                }

                writer.WriteLine("");

                writer.WriteLine("Category,Value");

                //Wtire the Comparisons
                StringBuilder sb2 = new StringBuilder();
                sb2.Append("Comparisons");
                sb2.Append(",");

                int counter = 1;
                foreach(Comparison comparison in Comparisons)
                {
                    sb2.Append(counter.ToString());
                    sb2.Append(')');
                    sb2.Append(comparison.ComparisonName);
                    sb2.Append(';');
                    counter++;
                }
                sb2.Remove(sb2.Length - 1, 1);
                writer.WriteLine(sb2.ToString());

                //Write the Values for Comparison
                sb2.Clear();
                sb2.Append("Values for Comparison");
                sb2.Append(",");
                sb2.Append(ValueForComparison.ToString());
                writer.WriteLine(sb2.ToString());

                //Write the Normalization Subset
                sb2.Clear();
                sb2.Append("Protein Filtering/Normalization Subset");
                sb2.Append(",");
                sb2.Append(NormalizationSubetString);
                writer.WriteLine(sb2.ToString());

                //Write the filtering term
                sb2.Clear();
                sb2.Append("Filtering File/Term");
                sb2.Append(",");
                sb2.Append(normalizationTextBox.Text);
                writer.WriteLine(sb2.ToString());

                //Write the Unique String to Combine Files
                sb2.Clear();
                sb2.Append("Columns Used to Combine Files");
                sb2.Append(',');
                if (inputFiles.Items.Count > 1)
                {
                    sb2.Append(headerComboBox.SelectedItem.ToString());
                    sb2.Append(" AND ");
                    sb2.Append(headerComboBox2.SelectedItem.ToString());
                }
                writer.WriteLine(sb2.ToString());

                //Write the Type of significance Testing used
                sb2.Clear();
                sb2.Append("Significance Testing");
                sb2.Append(',');
                sb2.Append(SigTestingString);
                writer.WriteLine(sb2.ToString());

                //Write the Type of Multiple Correction used 
                sb2.Clear();
                sb2.Append("Multiple Comparison Correction");
                sb2.Append(',');
                sb2.Append(MultipleCorrectionString);
                writer.WriteLine(sb2.ToString());

                //Write the Significance Threshold
                sb2.Clear();
                sb2.Append("Significance Threshold Type");
                sb2.Append(',');
                sb2.Append(ThresholdType);
                writer.WriteLine(sb2.ToString());

                //Write the Significance Threshold
                sb2.Clear();
                sb2.Append("Significance Threshold Value");
                sb2.Append(',');
                sb2.Append(ThresholdValue);
                writer.WriteLine(sb2.ToString());

                //Write the Annotation String
                sb2.Clear();
                sb2.Append("Annotations and GO Enrichment");
                sb2.Append(',');
                sb2.Append(GOString.Replace(","," and "));
                writer.WriteLine(sb2.ToString());

                //Write the Uniprot Header for the Annotations
                sb2.Clear();
                sb2.Append("Uniprot Header");
                sb2.Append(',');
                sb2.Append(UniprotHeader);
                writer.WriteLine(sb2.ToString());

                //Write the Database for the Annotations
                sb2.Clear();
                sb2.Append("Annotation Database");
                sb2.Append(',');
                if (userDatabase.Checked)
                {
                    sb2.Append(customDatabaseText.Text);
                }
                else
                {
                    sb2.Append(AnnotationDatabase);
                }
                writer.WriteLine(sb2.ToString());

                //Write the Annotations Added
                sb2.Clear();
                sb2.Append("Annotations Added");
                sb2.Append(',');
                foreach (AnnotationType type in AnnotationsToAdd)
                {
                    sb2.Append(type.ToString());
                    sb2.Append(';');
                }
                sb2.Remove(sb2.Length - 1, 1);
                writer.WriteLine(sb2.ToString());

                //Write the Annotation Thresholds
                sb2.Clear();
                sb2.Append("Annotation Threshold: P-Value");
                sb2.Append(',');
                sb2.Append(annotationSigTextBox.Text);
                writer.WriteLine(sb2.ToString());

            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Text += " (" + Assembly.GetExecutingAssembly().GetName().Version + ")";          
            base.OnLoad(e);
        }

    }
}
