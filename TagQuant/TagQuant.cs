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
        public Dictionary<TagSetType, IsobaricTagPurityCorrection> PurityMatrices;

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

            PurityMatrices = new Dictionary<TagSetType, IsobaricTagPurityCorrection>();
            foreach (TagSetType tagSet in Enum.GetValues(typeof(TagSetType)))
            {
                List<TagInformation> usedTags = tags.Where(t => t.TagSet == tagSet).ToList();
                if (usedTags.Count > 0)
                {
                    usedTags = usedTags.OrderBy(t => t.NominalMass).ToList();
                    int max = usedTags[usedTags.Count - 1].NominalMass - usedTags[0].NominalMass + 1;
                    int minint = usedTags[0].NominalMass;

                    TagInformation[] tarray = new TagInformation[max];
                    double[,] data = new double[max, 4];

                    foreach (var tag in usedTags)
                    {
                        tarray[tag.NominalMass - minint] = tag;
                    }
                  
                    for (int i = 0; i < max; i++)
                    {
                        var tag = tarray[i];
                        if (tag != null)
                        {
                            data[i, 0] = tag.M2;
                            data[i, 1] = tag.M1;
                            data[i, 2] = tag.P1;
                            data[i, 3] = tag.P2;
                        }
                        else
                        {
                            for (int j = 0; j < 4; j++) data[i, j] = 0;
                        }
                    }
                    IsobaricTagPurityCorrection correction = IsobaricTagPurityCorrection.Create(data);
                    PurityMatrices.Add(tagSet, correction);
                }
            }
        }

        private void Normalize(IEnumerable<QuantFile> quantFiles)
        {
            double maxSignal = (from TagInformation tag in UsedTags.Values where tag.IsUsed select tag.TotalSignal).Concat(new double[] {0}).Max();
            
            Log("Tag\tSample\tTotal Signal\tNormalizedSignal");

            foreach (TagInformation tag in UsedTags.Values)
            {
                // Divide by max so that everything is less than or equal to 1
                tag.NormalizedTotalSignal = tag.TotalSignal/maxSignal;

                Log(string.Format("{0}\t{1}\t{2}\t{3}", tag.TagName, tag.SampleName, tag.TotalSignal,
                    tag.NormalizedTotalSignal));
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
                            // Noise is pretty constant over a small region, find the noise of the center of all isobaric tags
                            MassRange range = new MassRange(UsedTags.Keys[0], UsedTags.Keys[UsedTags.Count - 1]);
                            ThermoLabeledPeak peak = massSpectrum.GetClosestPeak(range) as ThermoLabeledPeak;
                            if (peak != null)
                            {
                                noise = peak.Noise;
                            }
                            else
                            {
                                throw new ArgumentException("Low resolution data has no noise associated with it");
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
            
            // Dispose of all raw files
            foreach (ThermoRawFile rawFile in RawFiles.Values)
            {
                rawFile.Dispose();
            }
        }

        private void PurityCorrect(IEnumerable<QuantPeak> peaks)
        {
            List<QuantPeak> quantPeaks = peaks.ToList();
        
            foreach (TagSetType tagSet in PurityMatrices.Keys)
            {
                QuantPeak[] tagSetPeaks = quantPeaks.Where(t => t.Tag.TagSet == tagSet).OrderBy(t => t.Tag.NominalMass).ToArray();

                int minint = tagSetPeaks[0].Tag.NominalMass;
                int max = tagSetPeaks[tagSetPeaks.Length - 1].Tag.NominalMass - minint + 1;

                QuantPeak[] finalArray = new QuantPeak[max];
                foreach (var qpeak in tagSetPeaks)
                {
                    finalArray[qpeak.Tag.NominalMass - minint] = qpeak;
                }

                double[] dNLvalues = new double[max];
                for (int i = 0; i < max; i++)
                {
                    var qpeak = finalArray[i];
                    if (qpeak == null || qpeak.IsNoisedCapped)
                    {
                        dNLvalues[i] = 0;
                    }
                    else
                    {
                        dNLvalues[i] = qpeak.DeNormalizedIntensity;
                    }
                }

                double[] correctedData = PurityMatrices[tagSet].ApplyPurityCorrection(dNLvalues);

                for (int i = 0; i < correctedData.Length; i++)
                {
                    var qpeak = finalArray[i];
                    if (qpeak == null || qpeak.IsNoisedCapped)
                        continue;
                    
                    qpeak.Tag.TotalSignal += qpeak.PurityCorrectedIntensity = correctedData[i];
                }
            }
        }
    }
}
