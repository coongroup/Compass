using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CSMSL;
using LumenWorks.Framework.IO.Csv;

namespace Coon.Compass.TagQuant
{
    public partial class TagQuantForm : Form
    {
        BindingList<TagInformation> allTags  = new BindingList<TagInformation>();

        private List<TagInformation> tmtTags = new List<TagInformation>
        {
            new TagInformation(126, "TMT 126C", "", 126.1277261, 114.1279, TagSetType.TMTC, 0, 0, 7, 0),
            new TagInformation(127, "TMT 127N", "", 127.1247610, 114.1279, TagSetType.TMTN, 0, 2.1, 6.5, 0),
            new TagInformation(127, "TMT 127C", "", 127.1310809, 114.1279, TagSetType.TMTC, 0, 0.4, 6.3, 0),
            new TagInformation(128, "TMT 128N", "", 128.1281158, 114.1279, TagSetType.TMTN, 0, 0.6, 4.7, 0),
            new TagInformation(128, "TMT 128C", "", 128.1344357, 114.1279, TagSetType.TMTC, 0, 1.1, 5.1, 0),
            new TagInformation(129, "TMT 129N", "", 129.1314706, 114.1279, TagSetType.TMTN, 0, 1.6, 4.7, 0.1),
            new TagInformation(129, "TMT 129C", "", 129.1377905, 114.1279, TagSetType.TMTC, 0.2, 0.1, 0, 0),
            new TagInformation(130, "TMT 130N", "", 130.1348254, 114.1279, TagSetType.TMTN, 0, 1.3, 2.4, 3.2),
            new TagInformation(130, "TMT 130C", "", 130.1411453, 118.1415, TagSetType.TMTC, 0.1, 2.9, 2.9, 0),
            new TagInformation(131, "TMT 131N", "", 131.1381802, 119.1384, TagSetType.TMTN, 0.2, 3.3, 3, 0)
        };

        List<TagInformation> itraqTags = new List<TagInformation>
        {
            new TagInformation(113, "iTRAQ 113", "", 113.107325, 114.1279,TagSetType.iTRAQ,0,0,7,0),
            new TagInformation(114, "iTRAQ 114", "", 114.11068, 114.1279,TagSetType.iTRAQ,0,0,7,0),
            new TagInformation(115, "iTRAQ 115", "", 115.107715, 114.1279,TagSetType.iTRAQ,0,0,7,0),
            new TagInformation(116, "iTRAQ 116", "", 116.111069, 114.1279,TagSetType.iTRAQ,0,0,7,0),
            new TagInformation(117, "iTRAQ 117", "", 117.114424, 114.1279,TagSetType.iTRAQ,0,0,7,0),
            new TagInformation(118, "iTRAQ 118", "", 118.111459, 114.1279,TagSetType.iTRAQ,0,0,7,0),
            new TagInformation(119, "iTRAQ 119", "", 119.114814, 114.1279,TagSetType.iTRAQ,0,0,7,0),
            new TagInformation(121, "iTRAQ 121", "", 121.121524, 114.1279,TagSetType.iTRAQ,0,0,7,0)
        };

        List<TagInformation> userTags = new List<TagInformation>();
        
        public TagQuantForm()
        {
            InitializeComponent();
            this.Text = "Tag Quant v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            DataGridViewCheckBoxColumn useColumn = new DataGridViewCheckBoxColumn();
            useColumn.DataPropertyName = "IsUsed";
            useColumn.HeaderText = "Use";
            useColumn.Frozen = true;
            useColumn.ContextMenuStrip = new ContextMenuStrip();
            useColumn.ContextMenuStrip.Items.Add("Select All").Click += CheckBoxColumnSelectAll;
            useColumn.ContextMenuStrip.Items.Add("Deselect All").Click += CheckBoxColumnDeselectAll;
            dataGridView2.Columns.Add(useColumn);

            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.DataPropertyName = "TagName";
            nameColumn.HeaderText = "Tag";
            nameColumn.Frozen = true;
            nameColumn.ReadOnly = true;
            dataGridView2.Columns.Add(nameColumn);

            DataGridViewTextBoxColumn sampleColumn = new DataGridViewTextBoxColumn();
            sampleColumn.DataPropertyName = "SampleName";
            sampleColumn.HeaderText = "Sample Name";
            dataGridView2.Columns.Add(sampleColumn);

            DataGridViewTextBoxColumn mzColumn = new DataGridViewTextBoxColumn();
            mzColumn.DataPropertyName = "MassCAD";
            mzColumn.HeaderText = "m/z (CAD)";
            dataGridView2.Columns.Add(mzColumn);

            DataGridViewTextBoxColumn m2Column = new DataGridViewTextBoxColumn();
            m2Column.DataPropertyName = "M2";
            m2Column.HeaderText = "-2";
            dataGridView2.Columns.Add(m2Column);

            DataGridViewTextBoxColumn m1Column = new DataGridViewTextBoxColumn();
            m1Column.DataPropertyName = "M1";
            m1Column.HeaderText = "-1";
            dataGridView2.Columns.Add(m1Column);

            DataGridViewTextBoxColumn p1Column = new DataGridViewTextBoxColumn();
            p1Column.DataPropertyName = "P1";
            p1Column.HeaderText = "+1";
            dataGridView2.Columns.Add(p1Column);

            DataGridViewTextBoxColumn p2Column = new DataGridViewTextBoxColumn();
            p2Column.DataPropertyName = "P2";
            p2Column.HeaderText = "+2";
            dataGridView2.Columns.Add(p2Column);

            radioButton2.Checked = true;
            //allTags = new BindingList<TagInformation>(tmtTags);

            dataGridView2.DataSource = allTags;
        }

        private void CheckBoxColumnDeselectAll(object sender, EventArgs e)
        {
            foreach (TagInformation tags in allTags)
            {
                tags.IsUsed = false;
            }
        }

        private void CheckBoxColumnSelectAll(object sender, EventArgs e)
        {
            foreach (TagInformation tags in allTags)
            {
                tags.IsUsed = true;
            }
        }

        private void SaveTags()
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) 
                return;
            string fileName = saveFileDialog1.FileName;
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Tag,TagType,Nominal Mass,Use?,Sample Name,m/z (CAD),m/z (ETD),M2,M1,P1,P2");
                foreach (TagInformation tag in allTags)
                {
                    writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}"
                        , tag.TagName
                        , tag.TagSet
                        , tag.NominalMass
                        , tag.IsUsed
                        , tag.SampleName
                        , tag.MassCAD
                        , tag.MassEtd
                        , tag.M2
                        , tag.M1
                        , tag.P1
                        , tag.P2
                        );
                }
            }
        }

        private void OpenSavedTags()
        {
            if (openFileDialog2.ShowDialog() != DialogResult.OK)
                return;
            string fileName = openFileDialog2.FileName;
            userTags.Clear();
            using (CsvReader reader = new CsvReader(new StreamReader(fileName), true))
            {
                while (reader.ReadNextRecord())
                {
                   
                    string tagName = reader["Tag"];
                    string tagTypeStr = reader["TagType"];

                    TagSetType tagType;
                    switch (tagTypeStr)
                    {
                        default:
                        case "TMTC":
                            tagType = TagSetType.TMTC;
                            break;
                        case "TMTN":
                            tagType = TagSetType.TMTN;
                            break;
                        case "iTRAQ":
                            tagType = TagSetType.iTRAQ;
                            break;
                    }
                    int nominalMass = int.Parse(reader["Nominal Mass"]);
                    bool use = bool.Parse(reader["Use?"]);
                    string sampleName = reader["Sample Name"];
                    double mzCAD = double.Parse(reader["m/z (CAD)"]);
                    double mzETD = double.Parse(reader["m/z (ETD)"]);
                    double m2 = double.Parse(reader["M2"]);
                    double m1 = double.Parse(reader["M1"]);
                    double p1 = double.Parse(reader["P1"]);
                    double p2 = double.Parse(reader["P2"]);
                    userTags.Add(new TagInformation(nominalMass, tagName, sampleName, mzCAD, mzETD, tagType, m2, m1, p1, p2, use));
                }
            }
            allTags.Clear();
            foreach (TagInformation tag in userTags)
            {
                allTags.Add(tag);
            }
        }

        private void TagQuantForm_Load(object sender, EventArgs e)
        {
         
        }

        // Drag & enter files into listbox1
        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach(string filepath in filepaths)
                {
                    if(Path.GetExtension(filepath).Equals(".csv", StringComparison.InvariantCultureIgnoreCase) &&
                        !listBox1.Items.Contains(filepath))
                    {
                        e.Effect = DragDropEffects.Link;
                        break;
                    }
                }
            }
        }

        //Drag & drop files into listbox1
        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach(string filepath in filepaths)
            {
                if(Path.GetExtension(filepath).Equals(".csv", StringComparison.InvariantCultureIgnoreCase) &&
                    !listBox1.Items.Contains(filepath))
                {
                    listBox1.Items.Add(filepath);
                    UpdateRawFileFolder(filepath);
                    UpdateOutputFolder(filepath);
                }
            }
        }

        // Add files into listbox1
        private void Add_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach(string filename in openFileDialog1.FileNames)
                {
                    listBox1.Items.Add(filename);
                    UpdateRawFileFolder(filename);
                    UpdateOutputFolder(filename);
                }
            }
        }

        // Update the output folder
        private void UpdateOutputFolder(string filepath)
        {
            if(textOutputFolder.Text == string.Empty)
            {
                textOutputFolder.Text = Path.GetDirectoryName(filepath);
            }
        }

        private void UpdateRawFileFolder(string filepath)
        {
            if (string.IsNullOrEmpty(textRawFolder.Text))
            {
                textRawFolder.Text = Path.GetDirectoryName(filepath);
            }
        }

        // Remove files from listbox1
        private void Remove_Click(object sender, EventArgs e)
        {
            while(listBox1.SelectedItems.Count > 0)
            {
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
        }

        // Clear all files from listbox1
        private void Clear_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        // Find folder for raw files
        private void BrowseRaw_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textRawFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        // Find output folder
        private void BrowseOutput_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                textOutputFolder.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private const double C12_C13_MASS_DIFFERENCE = 1.0033548378;
        private const double N14_N15_MASS_DIFFERENCE = 0.99999991;

        // Run TagQuant Program
        private void Quantify_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("No files are selected for analysis");
                return;
            }

            var tagsToUse = allTags.Where(t => t.IsUsed).ToList();

            if (tagsToUse.Count == 0)
            {
                MessageBox.Show("At least one tag must be selected for analysis");
                return;
            }

            toolStripStatusLabel1.Text = "Running...";
            Quantify.Enabled = false;

            double ITerror = (double)numericUpDown1.Value;
            double FTerror = (double)numericUpDown2.Value;

            //bool DontQuantifyETD = false;// ComboBoxETDoptions.Text == "Don't Quantify";
            bool noisebandCap = noisebandcapCB.Checked; // Check for noise-band capping
            bool ms3Quant = checkBox1.Checked;
            bool calculatePurity = checkBox2.Checked;

            TagQuant tagQuant = new TagQuant(textOutputFolder.Text, textRawFolder.Text, listBox1.Items.OfType<string>(), tagsToUse, Tolerance.FromDA(ITerror), Tolerance.FromDA(FTerror), ms3Quant, nosiebasecap: noisebandCap, calculatePurity: calculatePurity);
            tagQuant.ProgressChanged += tagQuant_ProgressChanged;
            tagQuant.OnFinished += tagQuant_OnFinished;
            tagQuant.UpdateLog += tagQuant_UpdateLog;
            richTextBox1.Clear();
            Thread thread = new Thread(tagQuant.Run);
            thread.IsBackground = true;
            thread.Start();
           

            /*
            panel1.Enabled = false;
            toolStripStatusLabel1.Text = "Running";
            Application.DoEvents();
            int FilesCount = listBox1.Items.Count;
            int FileCounter = 1;

           

            int ExceptionCount = 0;
            double IsoWindow = 0;
            double PreErrorAllowed = 0;
            //IsoWindow = double.Parse(windowTextBox.Text);   // SetSite isolation window
            //PreErrorAllowed = double.Parse(ppmTextBox.Text);    // SetSite allowed precursor error

            List<int> TagsInUse = new List<int>();
            double TagCount = 0;
            
            double TotalETDSignal = 0;
            double TotalCADSignal = 0;
            //string CleavagePattern = "\"" + textBoxCleavage.Text + "\"";

            // Purity correction factors for each of 10 possible rows and isotopic impurities (-2, -1, +1, +2)
            double pcf_R1_m2 = 0;
            double pcf_R2_m2 = 0;
            double pcf_R3_m2 = 0;
            double pcf_R4_m2 = 0;
            double pcf_R5_m2 = 0;
            double pcf_R6_m2 = 0;
            double pcf_R7_m2 = 0;
            double pcf_R8_m2 = 0;
            double pcf_R9_m2 = 0;
            double pcf_R10_m2 = 0;

            double pcf_R1_m1 = 0;
            double pcf_R2_m1 = 0;
            double pcf_R3_m1 = 0;
            double pcf_R4_m1 = 0;
            double pcf_R5_m1 = 0;
            double pcf_R6_m1 = 0;
            double pcf_R7_m1 = 0;
            double pcf_R8_m1 = 0;
            double pcf_R9_m1 = 0;
            double pcf_R10_m1 = 0;

            double pcf_R1_p1 = 0;
            double pcf_R2_p1 = 0;
            double pcf_R3_p1 = 0;
            double pcf_R4_p1 = 0;
            double pcf_R5_p1 = 0;
            double pcf_R6_p1 = 0;
            double pcf_R7_p1 = 0;
            double pcf_R8_p1 = 0;
            double pcf_R9_p1 = 0;
            double pcf_R10_p1 = 0;

            double pcf_R1_p2 = 0;
            double pcf_R2_p2 = 0;
            double pcf_R3_p2 = 0;
            double pcf_R4_p2 = 0;
            double pcf_R5_p2 = 0;
            double pcf_R6_p2 = 0;
            double pcf_R7_p2 = 0;
            double pcf_R8_p2 = 0;
            double pcf_R9_p2 = 0;
            double pcf_R10_p2 = 0;

            // Calculated value for each row (after purity correction)
            double AcP_R1 = 0;
            double AcP_R2 = 0;
            double AcP_R3 = 0;
            double AcP_R4 = 0;
            double AcP_R5 = 0;
            double AcP_R6 = 0;
            double AcP_R7 = 0;
            double AcP_R8 = 0;
            double AcP_R9 = 0;
            double AcP_R10 = 0;

            double CADCoefficientMatrixDeterminant = 0.0;
            double TMT10_CADCoefficientMatrixDeterminant = 0.0;
            //double ETDCoefficientMatrixDeterminant = 0.0;

            //Purity Correction Factors end

            // Create dictionary for peptide tag signals
            Dictionary<string, Dictionary<FragmentationMethod, double>> TotalTagSignal = new Dictionary<string, Dictionary<FragmentationMethod, double>>();
           
            double peptidecount = 0;
            //double ITerror = (double)numericUpDown1.Value;
            //double FTerror = (double)numericUpDown2.Value;
            //double error;

            // Create log file
            string LogOutPutName = Path.Combine(textOutputFolder.Text, "TagQuant_log.txt");
            StreamWriter log = new StreamWriter(LogOutPutName);
            log.AutoFlush = true;
            log.WriteLine("TagQuant PARAMETERS");
            log.WriteLine("Labeling Reagent: " + (radioBox_TMT2.Checked ? radioBox_TMT2.Text : (radioBox_iTRAQ4.Checked ? radioBox_iTRAQ4.Text : (radioBox_TMT6.Checked ? radioBox_TMT6.Text : radioBox_iTRAQ8.Text))));
            log.WriteLine();

            log.WriteLine("Tags and Samples Used");

            SortedDictionary<int, TagInformation> Samples = new SortedDictionary<int, TagInformation>();

            string sample = "unused";

            SortedList<double, TagInformation> SamplesAndTags = new SortedList<double, TagInformation>();

           

            log.WriteLine("Input Files for Quantitation");
            

            foreach(string filename in listBox1.Items)
            {
                toolStripStatusLabel1.Text = "Working on File " + Convert.ToString(FileCounter) + " of " + Convert.ToString(FilesCount);
                Application.DoEvents();
                FileCounter++;
                //OPEN INPUT AND OUTPUT FILES

                StreamReader baseSteam = new StreamReader(filename);
                using (CsvReader reader = new CsvReader(baseSteam, true))
                {

                    string outputname = Path.Combine(textOutputFolder.Text,
                        Path.GetFileNameWithoutExtension(filename) + "_quant_temp.csv");
                    using (StreamWriter QuantTempFile = new StreamWriter(outputname))
                    {

                        log.WriteLine(filename);

                        //open raw file

                        string[] headerColumns = reader.GetFieldHeaders();
                        int headerCount = headerColumns.Length;
                        string header = string.Join(",", headerColumns);

                        StringBuilder sb = new StringBuilder();
                        sb.Append(header);
                        sb.Append(",Interference");
                       
                        foreach (TagInformation tag in SamplesAndTags.Values)
                        {
                            sb.AppendFormat(",{0} ({1} NL)", tag.SampleName, tag.TagName);
                        }

                        foreach (TagInformation tag in SamplesAndTags.Values)
                        {
                            sb.AppendFormat(",{0} ({1} dNL)", tag.SampleName, tag.TagName);
                        }
                        
                        //QuantTempFile.Write(",TQ_Total_int(all_tags)");

                        foreach (TagInformation tag in SamplesAndTags.Values)
                        {
                            sb.AppendFormat(",{0} ({1} PC)", tag.SampleName, tag.TagName);
                        }
                        

                        QuantTempFile.WriteLine(sb.ToString());

                        ThermoRawFile rawFile = null;

                        while (reader.ReadNextRecord()) // go through csv and raw file to extract the info we want
                        {
                            int scanNumber = int.Parse(reader["Spectrum number"]);
                            string filenameID = reader["Filename/id"];


                            string rawFileName = Path.Combine(textRawFolder.Text, filenameID.Split('.')[0] + ".raw");

                            // FIX ME

                            if (rawFile != null)
                            {
                                string RawFileCheck = rawFile.FilePath;
                                if (!RawFileCheck.Equals(rawFileName))
                                {
                                    rawFile.Dispose();
                                    if (!File.Exists(rawFileName))
                                    {
                                        throw new FileNotFoundException("Can't find file " + rawFileName);
                                    }

                                    rawFile = new ThermoRawFile(rawFileName);
                                }
                            }
                            else
                            {
                                if (!File.Exists(rawFileName))
                                {
                                    throw new FileNotFoundException("Can't find file " + rawFileName);
                                }

                                rawFile = new ThermoRawFile(rawFileName);
                            }

                            rawFile.Open();

                            // SetSite default fragmentation to CAD / HCD
                            FragmentationMethod ScanFragMethod = FragmentationMethod.CAD;

                            if (filenameID.Contains(".ETD."))
                            {
                                ScanFragMethod = FragmentationMethod.ETD;
                                if (ComboBoxETDoptions.Text == "Use Scan Before")
                                {
                                    scanNumber = scanNumber - 1;
                                    ScanFragMethod = FragmentationMethod.CAD;
                                }
                                if (ComboBoxETDoptions.Text == "Use Scan After")
                                {
                                    scanNumber = scanNumber + 1;
                                    ScanFragMethod = FragmentationMethod.CAD;
                                }
                            }

                            error = ITerror;
                            if (filenameID.Contains(".FTMS"))
                            {
                                error = FTerror;
                            }

                            // perform functions on raw file

                            // 1.) Determine Purity
                            double Purity = 0;
                            double PreInt = 0;
                            double TotalInt = 0;

                            // Get the scan object for the sequence ms2 scan
                            MsnDataScan seqScan = rawFile[scanNumber] as MsnDataScan;

                            if (seqScan == null)
                            {
                                throw new ArgumentException("Not an MS2 scan");
                            }

                            double injectionTime = seqScan.InjectionTime;
                            var massSpectrum = seqScan.MassSpectrum;

                            // Find the closest parent survery scan


                            //if(surveyScan != null)
                            //{   
                            //    double Charge = Convert.ToDouble(column_values[11]);
                            //    double IsoPreMZ = Convert.ToDouble(column_values[15]);
                            //    Range<double> range = new Range<double>(IsoPreMZ - 0.5 * IsoWindow, IsoPreMZ + 0.5 * IsoWindow);
                            //    List<MZPeak> peaks = null;

                            //    if (surveyScan.MassSpectrum.TryGetPeaks(out peaks, range))
                            //    {
                            //        foreach (MZPeak peak in peaks)
                            //        {
                            //            double difference = (peak.MZ - IsoPreMZ) * Charge;
                            //            double difference_Rounded = Math.Round(difference);
                            //            double expected_difference = difference_Rounded * C12_C13_MASS_DIFFERENCE;
                            //            double Difference_ppm = (Math.Abs((expected_difference - difference) / (IsoPreMZ * Charge))) * 1000000;

                            //            if (Difference_ppm <= PreErrorAllowed)
                            //            {
                            //                PreInt += peak.Intensity;
                            //            }
                            //            TotalInt += peak.Intensity;
                            //        }
                            //    }

                            //    Purity = PreInt / TotalInt;
                            //}


                            Dictionary<string, double> nlScanIntensityHash = new Dictionary<string, double>();
                            Dictionary<string, double> dNlScanIntensityHash = new Dictionary<string, double>();
                            Dictionary<string, double> pcfScanIntensityHash = new Dictionary<string, double>();

                            double TotalScanIntCounts = 0;

                            foreach (TagInformation tag in SamplesAndTags.Values)
                            {
                                nlScanIntensityHash.Add(tag.TagName, 0);
                                dNlScanIntensityHash.Add(tag.TagName, 0);
                                pcfScanIntensityHash.Add(tag.TagName, 0);
                            }

                            foreach (TagInformation tag in SamplesAndTags.Values)
                            {

                                double tagvalue = ScanFragMethod == FragmentationMethod.ETD
                                    ? tag.MassEtd
                                    : tag.MassCAD;
                            
                                var peak = massSpectrum.GetClosestPeak(tagvalue, error);

                                double nlCounts = peak != null ? peak.Intensity : 0;
                                if(noisebandCap) {

}
                                double dNlCounts = nlCounts*injectionTime;

                                nlScanIntensityHash[tag.TagName] += nlCounts;
                                dNlScanIntensityHash[tag.TagName] += dNlCounts;
                            }

                            switch (ScanFragMethod)
                            {
                                case FragmentationMethod.CAD:
                                    //if (radioBox_iTRAQ4.Checked)
                                    //{
                                    //    Matrix Delta_R1_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {
                                    //            dNlScanIntensityHash[114], dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116],
                                    //            dNlScanIntensityHash[117]
                                    //        },
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4}
                                    //    });

                                    //    Matrix Delta_R2_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0},
                                    //        {
                                    //            dNlScanIntensityHash[114], dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116],
                                    //            dNlScanIntensityHash[117]
                                    //        },
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4}
                                    //    });

                                    //    Matrix Delta_R3_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2},
                                    //        {
                                    //            dNlScanIntensityHash[114], dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116],
                                    //            dNlScanIntensityHash[117]
                                    //        },
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4}
                                    //    });

                                    //    Matrix Delta_R4_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1},
                                    //        {
                                    //            dNlScanIntensityHash[114], dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116],
                                    //            dNlScanIntensityHash[117]
                                    //        }
                                    //    });

                                    //    try
                                    //    {
                                    //        double DeterminantR1 = Delta_R1_CAD.Determinant();
                                    //        double DeterminantR2 = Delta_R2_CAD.Determinant();
                                    //        double DeterminantR3 = Delta_R3_CAD.Determinant();
                                    //        double DeterminantR4 = Delta_R4_CAD.Determinant();

                                    //        // Calculate the purity correction factor for each channel
                                    //        pcfScanIntensityHash[114] = (DeterminantR1/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[115] = (DeterminantR2/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[116] = (DeterminantR3/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[117] = (DeterminantR4/CADCoefficientMatrixDeterminant);
                                    //    }
                                    //    catch (DivideByZeroException)
                                    //    {
                                    //        pcfScanIntensityHash[114] = dNlScanIntensityHash[114];
                                    //        pcfScanIntensityHash[115] = dNlScanIntensityHash[115];
                                    //        pcfScanIntensityHash[116] = dNlScanIntensityHash[116];
                                    //        pcfScanIntensityHash[117] = dNlScanIntensityHash[117];
                                    //    }

                                    //    TotalScanIntCounts += pcfScanIntensityHash[114] + pcfScanIntensityHash[115] +
                                    //                          pcfScanIntensityHash[116] + pcfScanIntensityHash[117];
                                    //}
                                    //if (radioBox_iTRAQ8.Checked)
                                    //{
                                    //    Matrix Delta_R1_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R2_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R3_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R4_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R5_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R6_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R7_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {

                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R8_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        }
                                    //    });

                                    //    try
                                    //    {
                                    //        double DeterminantR1 = Delta_R1_CAD.Determinant();
                                    //        double DeterminantR2 = Delta_R2_CAD.Determinant();
                                    //        double DeterminantR3 = Delta_R3_CAD.Determinant();
                                    //        double DeterminantR4 = Delta_R4_CAD.Determinant();
                                    //        double DeterminantR5 = Delta_R5_CAD.Determinant();
                                    //        double DeterminantR6 = Delta_R6_CAD.Determinant();
                                    //        double DeterminantR7 = Delta_R7_CAD.Determinant();
                                    //        double DeterminantR8 = Delta_R8_CAD.Determinant();

                                    //        pcfScanIntensityHash[113] = (DeterminantR1/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[114] = (DeterminantR2/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[115] = (DeterminantR3/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[116] = (DeterminantR4/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[117] = (DeterminantR5/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[118] = (DeterminantR6/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[119] = (DeterminantR7/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[121] = (DeterminantR8/CADCoefficientMatrixDeterminant);
                                    //    }
                                    //    catch (DivideByZeroException)
                                    //    {
                                    //        pcfScanIntensityHash[113] = dNlScanIntensityHash[113];
                                    //        pcfScanIntensityHash[114] = dNlScanIntensityHash[114];
                                    //        pcfScanIntensityHash[115] = dNlScanIntensityHash[115];
                                    //        pcfScanIntensityHash[116] = dNlScanIntensityHash[116];
                                    //        pcfScanIntensityHash[117] = dNlScanIntensityHash[117];
                                    //        pcfScanIntensityHash[118] = dNlScanIntensityHash[118];
                                    //        pcfScanIntensityHash[119] = dNlScanIntensityHash[119];
                                    //        pcfScanIntensityHash[121] = dNlScanIntensityHash[121];
                                    //    }

                                    //    TotalScanIntCounts += pcfScanIntensityHash[113] + pcfScanIntensityHash[114] +
                                    //                          pcfScanIntensityHash[115] + pcfScanIntensityHash[116] +
                                    //                          pcfScanIntensityHash[117] + pcfScanIntensityHash[118] +
                                    //                          pcfScanIntensityHash[119] + pcfScanIntensityHash[121];
                                    //}
                                    //if (radioBox_TMT6.Checked)
                                    //{
                                    //    Matrix Delta_R1_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R2_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R3_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R4_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R5_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R6_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        }
                                    //    });

                                    //    try
                                    //    {
                                    //        double DeterminantR1 = Delta_R1_CAD.Determinant();
                                    //        double DeterminantR2 = Delta_R2_CAD.Determinant();
                                    //        double DeterminantR3 = Delta_R3_CAD.Determinant();
                                    //        double DeterminantR4 = Delta_R4_CAD.Determinant();
                                    //        double DeterminantR5 = Delta_R5_CAD.Determinant();
                                    //        double DeterminantR6 = Delta_R6_CAD.Determinant();

                                    //        pcfScanIntensityHash[126] = (DeterminantR1/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[127] = (DeterminantR2/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[128] = (DeterminantR3/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[129] = (DeterminantR4/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[130] = (DeterminantR5/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[131] = (DeterminantR6/CADCoefficientMatrixDeterminant);

                                    //    }
                                    //    catch (DivideByZeroException)
                                    //    {
                                    //        ExceptionCount++;
                                    //        pcfScanIntensityHash[126] = dNlScanIntensityHash[126];
                                    //        pcfScanIntensityHash[127] = dNlScanIntensityHash[127];
                                    //        pcfScanIntensityHash[128] = dNlScanIntensityHash[128];
                                    //        pcfScanIntensityHash[129] = dNlScanIntensityHash[129];
                                    //        pcfScanIntensityHash[130] = dNlScanIntensityHash[130];
                                    //        pcfScanIntensityHash[131] = dNlScanIntensityHash[131];
                                    //    }

                                    //    TotalScanIntCounts += pcfScanIntensityHash[126] + pcfScanIntensityHash[127] +
                                    //                          pcfScanIntensityHash[128] + pcfScanIntensityHash[129] +
                                    //                          pcfScanIntensityHash[130] + pcfScanIntensityHash[131];
                                    //}
                                    //if (radioBox_TMT8.Checked)
                                    //{
                                    //    Matrix Delta_R1_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R2_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R3_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R4_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R5_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R6_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        }
                                    //    });

                                    //    Matrix Delta_R7_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {dNlScanIntensityHash[132], dNlScanIntensityHash[133]},
                                    //        {pcf_R8_m2, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R8_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R7, pcf_R7_p2},
                                    //        {dNlScanIntensityHash[132], dNlScanIntensityHash[133]}
                                    //    });

                                 
                                    //    TotalScanIntCounts +=  pcfScanIntensityHash[126] = (Delta_R1_CAD.Determinant()/CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[127] = (Delta_R2_CAD.Determinant()/CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[128] = (Delta_R3_CAD.Determinant()/CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[129] = (Delta_R4_CAD.Determinant()/CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[130] = (Delta_R5_CAD.Determinant()/CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[131] = (Delta_R6_CAD.Determinant()/CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[132] = (Delta_R7_CAD.Determinant()/TMT10_CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[133] = (Delta_R8_CAD.Determinant()/TMT10_CADCoefficientMatrixDeterminant);

                                        
                                        
                                    //    //ExceptionCount++;
                                    //    //pcfScanIntensityHash[126] = dNlScanIntensityHash[126];
                                    //    //pcfScanIntensityHash[127] = dNlScanIntensityHash[127];
                                    //    //pcfScanIntensityHash[128] = dNlScanIntensityHash[128];
                                    //    //pcfScanIntensityHash[129] = dNlScanIntensityHash[129];
                                    //    //pcfScanIntensityHash[130] = dNlScanIntensityHash[130];
                                    //    //pcfScanIntensityHash[131] = dNlScanIntensityHash[131];
                                    //    //pcfScanIntensityHash[132] = dNlScanIntensityHash[132];
                                    //    //pcfScanIntensityHash[133] = dNlScanIntensityHash[133];
                                    //}
                                    if (radioBox_TMT10.Checked)
                                    {
                                        Matrix Delta_R1_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {
                                                dNlScanIntensityHash["126"], dNlScanIntensityHash["127C"],
                                                dNlScanIntensityHash["128C"], dNlScanIntensityHash["129C"],
                                                dNlScanIntensityHash["130C"], dNlScanIntensityHash["131"]
                                            },
                                            {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                            {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                            {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                            {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                            {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                        });

                                        Matrix Delta_R2_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                            {
                                                dNlScanIntensityHash["126"], dNlScanIntensityHash["127C"],
                                                dNlScanIntensityHash["128C"], dNlScanIntensityHash["129C"],
                                                dNlScanIntensityHash["130C"], dNlScanIntensityHash["131"]
                                            },
                                            {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                            {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                            {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                            {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                        });

                                        Matrix Delta_R3_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                            {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                            {
                                                dNlScanIntensityHash["126"], dNlScanIntensityHash["127C"],
                                                dNlScanIntensityHash["128C"], dNlScanIntensityHash["129C"],
                                                dNlScanIntensityHash["130C"], dNlScanIntensityHash["131"]
                                            },
                                            {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                            {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                            {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                        });

                                        Matrix Delta_R4_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                            {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                            {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                            {
                                                dNlScanIntensityHash["126"], dNlScanIntensityHash["127C"],
                                                dNlScanIntensityHash["128C"], dNlScanIntensityHash["129C"],
                                                dNlScanIntensityHash["130C"], dNlScanIntensityHash["131"]
                                            },
                                            {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                            {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                        });

                                        Matrix Delta_R5_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                            {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                            {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                            {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                            {
                                                dNlScanIntensityHash["126"], dNlScanIntensityHash["127C"],
                                                dNlScanIntensityHash["128C"], dNlScanIntensityHash["129C"],
                                                dNlScanIntensityHash["130C"], dNlScanIntensityHash["131"]
                                            },
                                            {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                        });

                                        Matrix Delta_R6_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                            {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                            {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                            {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                            {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                            {
                                                dNlScanIntensityHash["126"], dNlScanIntensityHash["127C"],
                                                dNlScanIntensityHash["128C"], dNlScanIntensityHash["129C"],
                                                dNlScanIntensityHash["130C"], dNlScanIntensityHash["131"]
                                            }
                                        });

                                        Matrix Delta_R7_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {
                                                dNlScanIntensityHash["127N"], dNlScanIntensityHash["128N"],
                                                dNlScanIntensityHash["129N"], dNlScanIntensityHash["130N"]
                                            },
                                            {pcf_R7_p1, AcP_R8, pcf_R9_m1, pcf_R10_m2},
                                            {pcf_R7_p2, pcf_R8_p1, AcP_R9, pcf_R10_m1},
                                            {0, pcf_R8_p2, pcf_R9_p1, AcP_R10},
                                        });

                                        Matrix Delta_R8_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R7, pcf_R8_m1, pcf_R9_m2, 0},
                                            {
                                                dNlScanIntensityHash["127N"], dNlScanIntensityHash["128N"],
                                                dNlScanIntensityHash["129N"], dNlScanIntensityHash["130N"]
                                            },
                                            {pcf_R7_p2, pcf_R8_p1, AcP_R9, pcf_R10_m1},
                                            {0, pcf_R8_p2, pcf_R9_p1, AcP_R10},
                                        });

                                        Matrix Delta_R9_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R7, pcf_R8_m1, pcf_R9_m2, 0},
                                            {pcf_R7_p1, AcP_R8, pcf_R9_m1, pcf_R10_m2},
                                            {
                                                dNlScanIntensityHash["127N"], dNlScanIntensityHash["128N"],
                                                dNlScanIntensityHash["129N"], dNlScanIntensityHash["130N"]
                                            },
                                            {0, pcf_R8_p2, pcf_R9_p1, AcP_R10},
                                        });

                                        Matrix Delta_R10_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R7, pcf_R8_m1, pcf_R9_m2, 0},
                                            {pcf_R7_p1, AcP_R8, pcf_R9_m1, pcf_R10_m2},
                                            {pcf_R7_p2, pcf_R8_p1, AcP_R9, pcf_R10_m1},
                                            {
                                                dNlScanIntensityHash["127N"], dNlScanIntensityHash["128N"],
                                                dNlScanIntensityHash["129N"], dNlScanIntensityHash["130N"]
                                            },
                                        });

                                        TotalScanIntCounts += pcfScanIntensityHash["126"] = (Delta_R1_CAD.Determinant() / CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["127N"] = (Delta_R2_CAD.Determinant() / CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["127C"] = (Delta_R7_CAD.Determinant() / TMT10_CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["128N"] = (Delta_R3_CAD.Determinant() / CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["128C"] = (Delta_R8_CAD.Determinant() / TMT10_CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["129N"] = (Delta_R4_CAD.Determinant() / CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["129C"] = (Delta_R9_CAD.Determinant() / TMT10_CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["130N"] = (Delta_R5_CAD.Determinant() / CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["130C"] = (Delta_R10_CAD.Determinant() / TMT10_CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["131"] = (Delta_R6_CAD.Determinant() / CADCoefficientMatrixDeterminant);
                                    

                                    }
                                    break;
                                    /*
                                case FragmentationMethod.ETD:
                                    if (radioBox_iTRAQ4.Checked)
                                    {
                                        pcfScanIntensityHash[114] = dNlScanIntensityHash[114];
                                        pcfScanIntensityHash[115] = dNlScanIntensityHash[115];
                                        pcfScanIntensityHash[116] = dNlScanIntensityHash[116];
                                        pcfScanIntensityHash[117] = dNlScanIntensityHash[117];
                                    }
                                    if (radioBox_iTRAQ8.Checked)
                                    {
                                        pcfScanIntensityHash[114] = dNlScanIntensityHash[113];
                                        pcfScanIntensityHash[114] = dNlScanIntensityHash[114];
                                        pcfScanIntensityHash[115] = dNlScanIntensityHash[115];
                                        pcfScanIntensityHash[116] = dNlScanIntensityHash[116];
                                        pcfScanIntensityHash[117] = dNlScanIntensityHash[117];
                                        pcfScanIntensityHash[118] = dNlScanIntensityHash[118];
                                        pcfScanIntensityHash[119] = dNlScanIntensityHash[119];
                                        pcfScanIntensityHash[121] = dNlScanIntensityHash[121];
                                    }
                                    if (radioBox_TMT6.Checked)
                                    {
                                        pcfScanIntensityHash[126] = dNlScanIntensityHash[126];
                                        pcfScanIntensityHash[127] = dNlScanIntensityHash[127];
                                        pcfScanIntensityHash[128] = dNlScanIntensityHash[128];
                                        pcfScanIntensityHash[129] = dNlScanIntensityHash[129];
                                        pcfScanIntensityHash[130] = dNlScanIntensityHash[130];
                                        pcfScanIntensityHash[131] = dNlScanIntensityHash[131];
                                    }
                                    break;
                                     
                            }

                            peptidecount++;

                            sb.Clear();

                            for (int i = 0; i < headerCount; i++)
                            {
                                sb.Append(reader[i]);
                                sb.Append(',');
                            }
                            sb.Append(Purity);

                            // Write out NL values
                            foreach (TagInformation tag in SamplesAndTags.Values)
                            {
                                sb.Append(',');
                                if (DontQuantifyETD && ScanFragMethod == FragmentationMethod.ETD)
                                {
                                    sb.Append("NA");
                                }
                                else
                                {
                                    sb.Append(nlScanIntensityHash[tag.TagName]);
                                }
                            }

                            // Write out dNL
                            foreach (TagInformation tag in SamplesAndTags.Values)
                            {
                                sb.Append(',');
                                if (DontQuantifyETD && ScanFragMethod == FragmentationMethod.ETD)
                                {
                                    sb.Append("NA");
                                }
                                else
                                {
                                    sb.Append(dNlScanIntensityHash[tag.TagName]);
                                }
                            }

                            //sb.Append(",");
                            //// Write in Total Tag Intensity
                            //if (DontQuantifyETD && ScanFragMethod == FragmentationMethod.ETD)
                            //{
                            //    sb.Append("NA");
                            //}
                            //else
                            //{
                            //    sb.Append(TotalScanIntCounts);
                            //}

                            //Add the purity corrected signal from each channel if it is in use to the total counts for that channel
                            MassRange noiseRange = new MassRange(100, 250);
                            List<MZPeak> noisePeaks = null;
                            int labeledNoisePeakCount;

                            foreach (TagInformation tag in SamplesAndTags.Values)
                            {
                                noisePeaks = null;
                                labeledNoisePeakCount = 0;
                                sb.Append(',');
                                if (DontQuantifyETD && ScanFragMethod == FragmentationMethod.ETD)
                                {
                                    sb.Append("NA");
                                }
                                else
                                {
                                    if (noisebandCap)
                                    {
                                        if (nlScanIntensityHash[tag.TagName] == 0)
                                        {
                                            pcfScanIntensityHash[tag.TagName] = 0;
                                            if (seqScan.MassSpectrum.TryGetPeaks(noiseRange, out noisePeaks))
                                            {
                                                foreach (MZPeak peak in noisePeaks)
                                                {
                                                    if (peak is ThermoLabeledPeak)
                                                    {
                                                        pcfScanIntensityHash[tag.TagName] +=
                                                            ((ThermoLabeledPeak) peak).Noise;
                                                        labeledNoisePeakCount++;
                                                    }
                                                }
                                                pcfScanIntensityHash[tag.TagName] = (pcfScanIntensityHash[tag.TagName] /
                                                                                   labeledNoisePeakCount)*injectionTime;
                                            }
                                        }
                                    }

                                    sb.Append(pcfScanIntensityHash[tag.TagName]);
                                }

                               
                                // Add to total channel signal for that fragmentation method
                                TotalTagSignal[tag.TagName][ScanFragMethod] += pcfScanIntensityHash[tag.TagName];

                                // Add to the total signal for that fragmentation method
                                if (ScanFragMethod == FragmentationMethod.ETD)
                                {
                                    TotalETDSignal += pcfScanIntensityHash[tag.TagName];
                                }
                                if (ScanFragMethod == FragmentationMethod.CAD)
                                {
                                    TotalCADSignal += pcfScanIntensityHash[tag.TagName];
                                }
                                
                            }

                            QuantTempFile.WriteLine(sb.ToString());
                            progressBar1.Value = (int) (((double) baseSteam.BaseStream.Position/baseSteam.BaseStream.Length)*100);

                        }
                    }
                }
            }

            double ExpectedRatio = 1 / TagCount;
            Application.DoEvents();
            Dictionary<string, Dictionary<FragmentationMethod, double>> NormalizationHash = new Dictionary<string, Dictionary<FragmentationMethod, double>>();
            double maxValue = 0;
            toolStripStatusLabel1.Text = "Calculating Normalization Values";

            foreach (TagInformation tag in SamplesAndTags.Values)
            {
                NormalizationHash.Add(tag.TagName, new Dictionary<FragmentationMethod, double>());
                NormalizationHash[tag.TagName].Add(FragmentationMethod.CAD, ExpectedRatio / (TotalTagSignal[tag.TagName][FragmentationMethod.CAD] / TotalCADSignal));
                NormalizationHash[tag.TagName].Add(FragmentationMethod.ETD, 0);
                //NormalizationHash[TagUsed][FragmentationMethod.CAD] = ExpectedRatio / (TotalTagSignal[TagUsed][FragmentationMethod.CAD] / TotalCADSignal);
                
                // Keep track of max channel signal
                if (NormalizationHash[tag.TagName][FragmentationMethod.CAD] > maxValue)
                {
                    maxValue = NormalizationHash[tag.TagName][FragmentationMethod.CAD];
                }
            }

            log.WriteLine();
            log.WriteLine("Tag m/z \tTotal Signal \tNoramlization Value \tNormalized to Max");

            foreach (TagInformation tag in SamplesAndTags.Values)
            {                 
                double percentValue =  (NormalizationHash[tag.TagName][FragmentationMethod.CAD]) / maxValue;

                StringBuilder sb = new StringBuilder();
                sb.Append(tag.TagName);
                sb.Append("\t ");
                sb.Append(TotalTagSignal[tag.TagName][FragmentationMethod.CAD].ToString("g4"));
                sb.Append("\t ");
                sb.Append(NormalizationHash[tag.TagName][FragmentationMethod.CAD].ToString("f4"));
                sb.Append("\t " );
                sb.Append(percentValue.ToString("f4"));
                log.WriteLine(sb.ToString());          

                if(TotalETDSignal > 0)
                {
                    NormalizationHash[tag.TagName][FragmentationMethod.ETD] = ExpectedRatio / (TotalTagSignal[tag.TagName][FragmentationMethod.ETD] / TotalETDSignal);

                    log.WriteLine(tag.TagName + " Total Signal ETD = " + TotalTagSignal[tag.TagName][FragmentationMethod.ETD] + ", Normalization value (ETD) = " + NormalizationHash[tag.TagName][FragmentationMethod.ETD]);
                }
                else
                {
                    //log.WriteLine(TagUsed + " Total Signal ETD = " + TotalTagSignal[TagUsed][FragmentationMethod.ETD] + "," + " Normalization value (ETD) = NaN");
                }
            }
            log.WriteLine();

            log.Close();


            int tagnumber = 0;
            if (radioBox_iTRAQ4.Checked)
            {
                tagnumber = 4;
            }
            if (radioBox_iTRAQ8.Checked)
            {
                tagnumber = 8;
            }
            if (radioBox_TMT6.Checked)
            {
                tagnumber = 6;
            }
            if (radioBox_TMT8.Checked)
            {
                tagnumber = 8;
            }
            if (radioBox_TMT10.Checked)
            {
                tagnumber = 10;
            }

            ////Re open files and perform normalization
            foreach(string filename in listBox1.Items)
            {
                toolStripStatusLabel1.Text = "Applying Normalization Values";
                string quantfile = Path.Combine(textOutputFolder.Text, Path.GetFileNameWithoutExtension(filename) + "_quant_temp.csv");
                StreamReader sr = new StreamReader(quantfile);
                using (CsvReader reader = new CsvReader(sr, true))
                {

                    string outputname = Path.Combine(textOutputFolder.Text, Path.GetFileNameWithoutExtension(filename) + "_quant.csv");
                    using (StreamWriter sw = new StreamWriter(outputname))
                    {
                        string[] headerFields = reader.GetFieldHeaders();
                        int headerCount = headerFields.Length;
                        int firstQuantColumn = headerCount - tagnumber;
                        string header = string.Join(",", headerFields);
                        sw.Write(header);

                        foreach (TagInformation tag in SamplesAndTags.Values)
                        {
                            sw.Write(",{0} ({1} PCN)", tag.SampleName, tag.TagName);
                        }

                        //sw.Write(",Cleavage Site");
                        sw.WriteLine();


                        while (reader.ReadNextRecord()) // go through csv and raw file to extract the info we want
                        {
                            StringBuilder sb = new StringBuilder();
                            StringBuilder sb2 = new StringBuilder();

                            string fileNameID = reader["Filename/id"];
                            FragmentationMethod ScanFragMethod = FragmentationMethod.CAD;
                            if (fileNameID.Contains(".ETD."))
                            {
                                ScanFragMethod = FragmentationMethod.ETD;
                                if ((ComboBoxETDoptions.Text == "Use Scan Before") ||
                                    (ComboBoxETDoptions.Text == "Use Scan After"))
                                {
                                    ScanFragMethod = FragmentationMethod.CAD;
                                }
                            }
                            int j = 0;
                            for (int i = 0; i < headerCount; i++)
                            {
                                string value = reader[i];
                                sb.Append(value);
                                sb.Append(',');
                                if (i < firstQuantColumn) 
                                    continue;
                                
                                if (value == "NA")
                                {
                                    sb2.Append("NA");
                                }
                                else
                                {
                                    double UnnormalizedValue = double.Parse(value);
                                    double NormalizedValue = UnnormalizedValue;

                                    TagInformation tag = SamplesAndTags.Values[j++];
                                    NormalizedValue = UnnormalizedValue * NormalizationHash[tag.TagName][ScanFragMethod];
                                    
                                    sb2.Append(NormalizedValue);
                                }
                                sb2.Append(',');
                            }
                            sb2.Remove(sb2.Length - 1, 1);
                            //sb2.Append(',');
                            //sb2.Append(CleavagePattern);
                            sw.WriteLine(sb.ToString() + sb2.ToString());
                        }
                    }
                }
                File.Delete(quantfile);
            }
            panel1.Enabled = true;
            toolStripStatusLabel1.Text = "done";
            */
        }

        private void tagQuant_UpdateLog(object sender, StatusEventArgs e)
        {
            MethodInvoker method = delegate
            {
                richTextBox1.AppendText(string.Format("[{0}]\t{1}\n", DateTime.Now.ToLongTimeString(), e.Message));
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            };

            if (InvokeRequired)
                BeginInvoke(method);
            else
                method.Invoke();
        }

        private void tagQuant_OnFinished(object sender, EventArgs e)
        {
            MethodInvoker method = delegate
            {
                Quantify.Enabled = true;
                //toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
                //toolStripProgressBar1.Value = 100;
                toolStripStatusLabel1.Text = "Complete";
            };

            if (InvokeRequired)
                BeginInvoke(method);
            else
                method.Invoke();
        }

        private void tagQuant_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgress(e.ProgressPercentage);
        }

        private void UpdateProgress(int p)
        {
            MethodInvoker method = delegate
            {
                toolStripProgressBar1.Value = p;
            };

            if (InvokeRequired)
                BeginInvoke(method);
            else
                method.Invoke();
        }

        private static int sortByAscendingIntensity(KeyValuePair<double, double> left, KeyValuePair<double, double> right)
        {
            return left.Key.CompareTo(right.Key);
        }

        private void TagQuantForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        private void TagQuantForm_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) 
                return;

            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            foreach (string filename in files)
            {
                listBox1.Items.Add(filename);
                UpdateRawFileFolder(filename);
                UpdateOutputFolder(filename);
            }
        }

        private void textBox113_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = (DataGridView) sender;
            if (dgv.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton2.Checked) 
                return;
            allTags.Clear();
            foreach (TagInformation tag in tmtTags)
            {
                allTags.Add(tag);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton1.Checked)
                return;
            allTags.Clear();
            foreach (TagInformation tag in itraqTags)
            {
                allTags.Add(tag);
            }
        }

        private void masstolerancelabel_Click(object sender, EventArgs e)
        {

        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filename in files)
            {
                listBox1.Items.Add(filename);
                UpdateRawFileFolder(filename);
                UpdateOutputFolder(filename);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveTags();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenSavedTags();
        }

        private void dataGridView2_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView2.IsCurrentCellDirty)
            {
                dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}