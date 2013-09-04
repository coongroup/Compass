using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSMSL;
using CSMSL.Analysis.Quantitation;
using System.Text;
using System.Threading.Tasks;
using CSMSL.IO;
using CSMSL.IO.Thermo;
using CSMSL.Spectral;
using LumenWorks.Framework.IO.Csv;

namespace TagQuant
{
    public class TagQuant
    {
        public bool DontQuantifyETD;
        public bool NoisebandCap;
        public SortedList<double, TagInformation> UsedTags;
        public MassTolerance ItMassTolerance;
        public MassTolerance FtmMassTolerance;
        public string OutputDirectory;
        public string RawFileDirectory;
        public int ETDQuantPosition;
        public List<string> InputFiles;
        public Dictionary<string, ThermoRawFile> RawFiles;
        private StreamWriter logWriter;

        private const double Carbon1213Difference = Constants.Carbon13 - Constants.Carbon;

        public TagQuant(string outputDirectory, string rawFileDirectory, IEnumerable<string> inputFiles, IEnumerable<TagInformation> tags, MassTolerance itTolerance, MassTolerance ftTolerance, int etdQuantPosition = 0, bool nosiebasecap = false, bool isETDQuantified = true)
        {
            OutputDirectory = outputDirectory;
            UsedTags = new SortedList<double, TagInformation>(tags.ToDictionary(t => t.MassCAD));
            InputFiles = inputFiles.ToList();
            NoisebandCap = nosiebasecap;
            DontQuantifyETD = isETDQuantified;

            ItMassTolerance = itTolerance;
            FtmMassTolerance = ftTolerance;
            ETDQuantPosition = etdQuantPosition;

            logWriter = new StreamWriter(Path.Combine(outputDirectory,"tagquant_log.txt"));
            logWriter.AutoFlush = true;

            RawFiles =
                Directory.EnumerateFiles(rawFileDirectory, "*.raw")
                    .ToDictionary(file => Path.GetFileNameWithoutExtension(file), file => new ThermoRawFile(file));

            
        }

        public void Run()
        {
            //WriteLog();

            BuildPurityMatrixes(UsedTags.Values);

            List<QuantFile> quantFiles = LoadFiles(InputFiles).ToList();

            Normalize(quantFiles);

            WriteOutputFiles(quantFiles);


            logWriter.Close();
        }

        private void BuildPurityMatrixes(IEnumerable<TagInformation> inputTags)
        {
            List<TagInformation> tags = inputTags.ToList();

         
            double[,] data = new double[10,4];
            for (int i = 0; i < tags.Count; i++)
            {
                var tag = tags[i];
                data[i, 0] = tag.M2;
                data[i, 1] = tag.M1;
                data[i, 2] = tag.P1;
                data[i, 3] = tag.P2;
            }

            IsobaricTagPurityCorrection correction_set1 = IsobaricTagPurityCorrection.Create(data);
            double det = correction_set1.Determinant();
        }

        private void Normalize(IEnumerable<QuantFile> quantFiles)
        {
            double maxSignal = (from TagInformation tag in UsedTags.Values select tag.TotalSignal).Concat(new double[] {0}).Max();
            
            Log("Tag\tSample\tTotal Signal\tNormalizedSignal");

            foreach (TagInformation tag in UsedTags.Values)
            {
                // Divide by max so that everything is less than or equal to 1
                tag.NormalizedTotalSignal = tag.TotalSignal/maxSignal;

                Log(string.Format("{0}\t{1}\t{2}\t{3}", tag.TagName, tag.SampleName, tag.TotalSignal, tag.NormalizedTotalSignal));
            }
            

            foreach (QuantFile quantFile in quantFiles)
            {
                foreach (PSM psm in quantFile.Psms.Values)
                {
                    foreach (QuantPeak qpeak in psm.QuantPeaks.Values)
                    {
                        // Divide by the tags Normalized value again to normalize all channels to 1
                        qpeak.PurityCorrectedNormalizedIntensity = qpeak.PurityCorrectedIntensity/
                                                                   qpeak.Tag.NormalizedTotalSignal;
                    }
                }
            }
        }

        private void Log(string msg)
        {
            logWriter.WriteLine(msg);
        }

        private void WriteOutputFiles(IEnumerable<QuantFile> quantFiles)
        {
            foreach (QuantFile file in quantFiles)
            {
                string filePath = file.FilePath;
                string outputFilePath = filePath.Replace(".csv", "_quant.csv");
                using (CsvReader reader = new CsvReader(new StreamReader(filePath), true))
                {
                    using (StreamWriter writer = new StreamWriter(outputFilePath))
                    {
                        StringBuilder sb = new StringBuilder();
                        string[] headerColumns = reader.GetFieldHeaders();
                        int headerCount = headerColumns.Length;
                        string header = string.Join(",", headerColumns);
                        sb.Append(header);

                        // Raw Intensities
                        foreach (TagInformation tag in UsedTags.Values)
                        {
                            sb.AppendFormat(",{0} ({1} NL)", tag.SampleName, tag.TagName);
                        }

                        // Denormalized Intensities
                        foreach (TagInformation tag in UsedTags.Values)
                        {
                            sb.AppendFormat(",{0} ({1} dNL)", tag.SampleName, tag.TagName);
                        }

                        // Purity Corrected Intensities
                        foreach (TagInformation tag in UsedTags.Values)
                        {
                            sb.AppendFormat(",{0} ({1} PC)", tag.SampleName, tag.TagName);
                        }

                        // Purity Corrected Normalized Intensities
                        foreach (TagInformation tag in UsedTags.Values)
                        {
                            sb.AppendFormat(",{0} ({1} PCN)", tag.SampleName, tag.TagName);
                        }

                        // Number of Channels Detected
                        sb.Append(",Channels Detected");

                        writer.WriteLine(sb.ToString());

                        while (reader.ReadNextRecord())
                        {
                            sb.Clear();

                            string[] inputData = new string[headerCount];
                            reader.CopyCurrentRecordTo(inputData);
                            sb.Append(string.Join(",", inputData));

                            int scanNumber = int.Parse(reader["Spectrum number"]);
                            PSM psm = file[scanNumber];

                            List<QuantPeak> peaks = (from TagInformation tag in UsedTags.Values select psm[tag]).ToList();
                            
                            // Raw Intensities
                            foreach (QuantPeak peak in peaks)
                            {
                                sb.Append(',');
                                sb.Append(peak.RawIntensity);
                            }

                            // Denormalized Intensities
                            foreach (QuantPeak peak in peaks)
                            {
                                sb.Append(',');
                                sb.Append(peak.DeNormalizedIntensity);
                            }

                            // Purity Corrected Intensities
                            foreach (QuantPeak peak in peaks)
                            {
                                sb.Append(',');
                                sb.Append(peak.PurityCorrectedIntensity);
                            }

                            // Purity Corrected Normalized Intensities
                            foreach (QuantPeak peak in peaks)
                            {
                                sb.Append(',');
                                sb.Append(peak.PurityCorrectedNormalizedIntensity);
                            }

                            // Number of Channels Detected (positive raw intensity)
                            int channelsDetected = peaks.Count(p => p.RawIntensity > 0);
                            sb.Append(',');
                            sb.Append(channelsDetected);

                            writer.WriteLine(sb.ToString());
                        }
                    }
                }
            }
        }
        
        private IEnumerable<QuantFile> LoadFiles(IEnumerable<string> filePaths)
        {
            foreach (string filePath in filePaths)
            {
                QuantFile quantFile = new QuantFile(filePath);
                StreamReader basestreamReader = new StreamReader(filePath);
                using (CsvReader reader = new CsvReader(basestreamReader, true))
                {
                    while (reader.ReadNextRecord()) // go through csv and raw file to extract the info we want
                    {
                        int scanNumber = int.Parse(reader["Spectrum number"]);
                        string filenameID = reader["Filename/id"];
                        string rawFileName = filenameID.Split('.')[0];
                        ThermoRawFile rawFile;
                        if (!RawFiles.TryGetValue(rawFileName, out rawFile))
                        {
                            throw new ArgumentException("Cannot find this raw file: "+rawFileName + ".raw");
                        }
                        if(!rawFile.IsOpen)
                            rawFile.Open();
                        
                        // Set default fragmentation to CAD / HCD
                        FragmentationMethod ScanFragMethod = filenameID.Contains(".ETD.") ? FragmentationMethod.ETD : FragmentationMethod.CAD;

                        if (ScanFragMethod == FragmentationMethod.ETD)
                        {
                            ScanFragMethod = FragmentationMethod.CAD;
                            scanNumber += ETDQuantPosition;
                        }
                        
                        MassTolerance massTolerance = filenameID.Contains(".FTMS") ? FtmMassTolerance : ItMassTolerance;

                        // Get the scan object for the sequence ms2 scan
                        MsnDataScan seqScan = rawFile[scanNumber] as MsnDataScan;

                        if (seqScan == null)
                        {
                            throw new ArgumentException("Not an MS2 scan");
                        }

                        double injectionTime = seqScan.InjectionTime;
                        var massSpectrum = seqScan.MassSpectrum;
                        double noise = 0;
                        if (NoisebandCap)
                        {
                            MassRange range = new MassRange(UsedTags.Keys[0], UsedTags.Keys[UsedTags.Count - 1]);
                            ThermoLabeledPeak peak = massSpectrum.GetClosestPeak(range) as ThermoLabeledPeak;
                            if (peak != null )
                            {
                                noise = peak.Noise;
                            }
                        }

                        Dictionary<TagInformation, QuantPeak> peaks = new Dictionary<TagInformation, QuantPeak>();

                        // Read in the peak data
                        foreach (TagInformation tag in UsedTags.Values)
                        {
                            double tagMz = ScanFragMethod == FragmentationMethod.ETD
                                ? tag.MassEtd
                                : tag.MassCAD;
                          
                            var peak = massSpectrum.GetClosestPeak(tagMz, massTolerance);

                            QuantPeak qPeak = new QuantPeak(tag, peak, injectionTime, seqScan, noise,
                                peak == null && NoisebandCap);                               

                            peaks.Add(tag, qPeak);
                        }

                        PurityCorrect(peaks.Values);

                        PSM psm = new PSM(filePath, scanNumber, peaks);
                        quantFile.AddPSM(psm);
                    }
                }
                yield return quantFile;
            }
        }

        private void PurityCorrect(IEnumerable<QuantPeak> quantPeaks)
        {
            foreach (QuantPeak quantPeak in quantPeaks)
            {
                //TODO
                quantPeak.PurityCorrectedIntensity = quantPeak.DeNormalizedIntensity;

                if(!quantPeak.IsNoisedCapped)
                    quantPeak.Tag.TotalSignal += quantPeak.PurityCorrectedIntensity;
            }
        }

    }
}
