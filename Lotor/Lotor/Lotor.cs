using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Coon;
using CoonThermo.IO;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace Lotor
{
    public class Lotor
    {
        private Tolerance PROD_TOLERANCE;
        private Dictionary<string, ThermoRawFile> RAW_FILES;
        private string CSV_FILE;
        private string OUTPUT_DIRECTORY;
        private Dictionary<string, PTM> PTMS;
        private DateTime startTime;
        private double ASCORE_THRESHOLD;
        private FragmentType FRAG_TYPE;

        private int FirstQuantColumn = -1;
        private int LastQuantColumn = -1;
        private string[] headerInfo = null;

        public Lotor(Dictionary<string, ThermoRawFile> rawFiles, string inputcsvFile, string outputDirectory, Dictionary<string, PTM> ptms, Tolerance prod_Tolerance, double ascore_threshold, FragmentType fragType)
        {
            RAW_FILES = rawFiles;
            CSV_FILE = inputcsvFile;
            OUTPUT_DIRECTORY = outputDirectory;
            PTMS = ptms;
            PROD_TOLERANCE = prod_Tolerance;
            ASCORE_THRESHOLD = ascore_threshold;
            FRAG_TYPE = fragType;
        }

        public void Localize()
        {
            startTime = DateTime.Now;
            Log("Localization Started...");

            List<PTM> fixedMods = new List<PTM>();
            List<PTM> varMods = new List<PTM>();
            List<PTM> quantMods = new List<PTM>();
            foreach (PTM ptm in PTMS.Values)
            {
                if (ptm.IsFixed)
                {
                    fixedMods.Add(ptm);
                }
                else
                {
                    varMods.Add(ptm);
                    if (ptm.Quantify)
                    {
                        quantMods.Add(ptm);
                    }
                }
            }

            try
            {

                // 1) Read in all the psms and map them to their respective spectra
                List<PSM> psms = LoadAllPSMs(CSV_FILE, RAW_FILES, fixedMods);

                // 2) Calculate all the best isoforms for all the psms
                List<LocalizedHit> hits = CalculateBestIsoforms(psms, ASCORE_THRESHOLD, FRAG_TYPE, PROD_TOLERANCE);

                // 3) Compile Results
                List<Protein> proteins = CompileResults(hits, CSV_FILE, OUTPUT_DIRECTORY, quantMods);

                // 4) Write out the results
                WriteResults(proteins, CSV_FILE, OUTPUT_DIRECTORY, FirstQuantColumn, LastQuantColumn);
            }
            catch (Exception e)
            {
                Log(e.Message, true);
            }
            finally
            {
                TimeSpan diff = DateTime.Now - startTime;
                Log(string.Format("Finished [{0:D2} hrs, {1:D2} mins, {2:D2} secs]", diff.Hours, diff.Minutes, diff.Seconds));
                Log(string.Format("Lotor v{0}", mainForm.GetRunningVersion()));
                ProgressUpdate(-1);
            }
        }

        private void WriteResults(List<Protein> proteins, string csvFile, string outDirectory, int firstQuant, int lastQuant)
        {
            using (StreamWriter localizeWriter = new StreamWriter(Path.Combine(outDirectory, Path.GetFileNameWithoutExtension(csvFile) + "_localized_reduced.csv")))                
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Protein,Isoform,Sites,PSMs Identified,");
                for (int i = firstQuant; i <= lastQuant; i++)
                {
                    sb.Append(headerInfo[i]);
                    sb.Append(',');
                }
                sb.Remove(sb.Length - 1, 1);
                localizeWriter.WriteLine(sb.ToString());
                foreach (Protein prot in proteins)
                {
                    localizeWriter.WriteLine(prot.Print(firstQuant, lastQuant));
                }
            }
        }

        private List<Protein> CompileResults(List<LocalizedHit> hits, string csvFile, string outputDirectory, List<PTM> quantMods)
        {
            Dictionary<string, LocalizedHit> hitsdict = new Dictionary<string, LocalizedHit>();
            List<string> localizingMods = new List<string>();
            foreach (PTM ptm in quantMods)
            {
                localizingMods.Add(ptm.Name);
            }
            // Group all the localized Hits into proteins
            Dictionary<string, Protein> proteins = new Dictionary<string, Protein>();
            Protein prot = null;
            foreach (LocalizedHit hit in hits)
            {
                hitsdict.Add(hit.PSM.Filename, hit);
                if (proteins.TryGetValue(hit.PSM.ProteinGroup, out prot))
                {
                    prot.AddHit(hit, localizingMods);                  
                }
                else
                {
                    prot = new Protein(hit.PSM.ProteinGroup);
                    prot.AddHit(hit, localizingMods);  
                    proteins.Add(hit.PSM.ProteinGroup, prot);
                }
            }
            using (StreamWriter writer = new StreamWriter(Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(csvFile) + "_localized.csv")))
            {
                using (CsvReader reader = new CsvReader(new StreamReader(csvFile), true))
                {
                    LocalizedHit hit = null;                   
                    headerInfo = reader.GetFieldHeaders();
                    bool tqFound = false;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (headerInfo[i].StartsWith("TQ"))
                        {
                            if (!tqFound)
                            {
                                FirstQuantColumn = i;
                                tqFound = true;
                            }
                            LastQuantColumn = i;
                        }
                    }
                    writer.WriteLine(string.Join(",", headerInfo)+ ",Localized?,Best Isoform,AScore");
                    while (reader.ReadNextRecord())
                    {                       
                        string filename = reader["Filename/id"];
                        bool localized = false;
                        if (localized = hitsdict.TryGetValue(filename, out hit))
                        {
                            prot = proteins[hit.PSM.ProteinGroup];
                            prot.Defline = reader["Defline"];
                            hit.omssapsm = new string[reader.FieldCount];
                            reader.CopyCurrentRecordTo(hit.omssapsm);
                        }                        
                        string[] data = new string[reader.FieldCount];
                        reader.CopyCurrentRecordTo(data);
                        foreach (string datum in data)
                        {                            
                            writer.Write(datum.Replace(",","."));
                            writer.Write(',');
                        }
                        if (localized)
                        {
                            writer.Write(string.Format("{0},{1},{2}\n", localized, hit.LocalizedIsoform.SequenceWithMods, hit.AScore));
                        }
                        else
                        {
                            writer.Write(string.Format("{0},{1},{2}\n", localized, "n/a", "0"));
                        }
                    }
                }
            }
      
            return proteins.Values.ToList();
        }

        private List<LocalizedHit> CalculateBestIsoforms(List<PSM> psms, double ascoreThreshold, FragmentType fragType, Tolerance prod_tolerance)
        {
            Log("Localizing Best Isoforms...");
            int totalisofromscount = 0;
            int count = 0;
            int psm_count = 0;
            int localized_psm = 0;
            double ascore = 0;
            List<LocalizedHit> hits = new List<LocalizedHit>();
            foreach (PSM psm in psms)
            {
                // Generate all the isoforms for the PSM
                totalisofromscount += psm.GenerateIsoforms();   

                // Calculate the probability of success for random matches
                double pvalue = GetPValue(psm, prod_tolerance);

                // Match all the isoforms to the spectrum and log the results
                psm.MatchIsofroms(fragType, prod_tolerance);      
        
                // Perform the localization for all combinations of isoforms
                double[,] res = psm.Calc(fragType, pvalue);

                // Check if the localization is above some threshold               
                int bestIsoform = LocalizedIsoform(res, ascoreThreshold, out ascore);

                // If there is a positive match, record and save it as a LocalizedHit
                if (bestIsoform >= 0)
                {              
                    List<PeptideIsoform> isoforms = psm.PeptideIsoforms.ToList();            
                    hits.Add(new LocalizedHit(psm,  isoforms[bestIsoform], ascore));
                    localized_psm++;
                }

                // No more need for the spectra
                psm.DeleteScan();

                // Progress Bar Stuff
                count++;
                psm_count++;
                if (count > 50) {
                    count = 0;
                    ProgressUpdate((double)psm_count / psms.Count);
                }
            }
            Log(string.Format("Total Number of Possible Isoforms Considered: {0:N0}",totalisofromscount));
            Log(string.Format("Total Number of PSMs Considered: {0:N0}", psm_count));
            Log(string.Format("Total Number of PSMs Localized: {0:N0} ({1:00.00}%)", localized_psm, localized_psm * 100 / psm_count));       
            ProgressUpdate(0);
            return hits;
        }
           
        private List<PSM> LoadAllPSMs(string csvFile, Dictionary<string, ThermoRawFile> rawFiles, List<PTM> fixedMods)
        {
            ProgressUpdate(0.0); //force the progressbar to go into marquee mode  
            Log("Reading PSMs from " + csvFile);
            List<PSM> psms = new List<PSM>();
            ThermoRawFile rawFile = null;
            using (CsvReader reader = new CsvReader(new StreamReader(csvFile), true))
            {               
                while (reader.ReadNextRecord())
                {
                    string filename = reader["Filename/id"];
                    string[] data = filename.Split('.');
                    string rawname = data[0];                                       
                    if (rawFiles.TryGetValue(rawname, out rawFile))
                    {
                        int scan_number = int.Parse(reader["Spectrum number"]);                      
                        PSM psm = new PSM(scan_number, rawFile);
                        psm.StartResidue = int.Parse(reader["Start"]);
                        psm.Charge = int.Parse(reader["Charge"]);      
                        psm.BasePeptide = new Peptide(reader["Peptide"]);
                        psm.ProteinGroup = reader["Defline"];
                        psm.Filename = filename;  
                   
                        // Apply all the fix modifications
                        foreach (PTM fixMod in fixedMods)
                        {
                            psm.BasePeptide.SetFixedModification(fixMod, psm.IsProteinNTerm);
                        }
                        
                        // Save all the variable mod types             
                        psm.Modifications = Modification.ParseModificationLine(reader["Mods"]).ToList(); 
                        psms.Add(psm);
                    }
                    else
                    {
                        throw new NullReferenceException(string.Format("Raw File: {0}.raw was not found! Aborting.",rawname));
                    }     
                }               
            }
            Log(string.Format("{0:N0} PSMs were loaded.", psms.Count));
            return psms;
        }
             
        #region Statics

        public static double GetPValue(PSM psm, Tolerance prod_tolerance)
        {
            double mzTol = Math.Abs(Tolerance.GetThfromPPM(prod_tolerance.Value, psm.IsolationMZ));
            return psm.Spectrum.Count * 2 * mzTol / psm.Spectrum.MzRange.Width;
        }

        public static int LocalizedIsoform(double[,] data, double threshold, out double lowestAscore)
        {
            lowestAscore = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                double minvalue = double.MaxValue;
                for (int j = 0; j < data.GetLength(0); j++)
                {
                    if (i == j) continue;
                    if (data[i, j] < 0)
                    {
                        minvalue = 0;
                        break;
                    }
                    if (data[i, j] <= minvalue) 
                    {
                        minvalue = data[i, j];
                    }
                }
                if (minvalue >= threshold)
                {
                    lowestAscore = minvalue;
                    return i;
                }
            }
            return -1;
        }
        
        #endregion

        #region CallBacks

        public event EventHandler<StatusEventArgs> UpdateLog;
        protected virtual void onUpdateLog(StatusEventArgs e)
        {
            if (UpdateLog != null)
            {
                UpdateLog(this, e);
            }
        }

        public void Log(string message, bool isError = false)
        {
            onUpdateLog(new StatusEventArgs(message, isError));
        }

        public event EventHandler<ProgressEventArgs> UpdateProgress;
        protected virtual void onUpdateProgress(ProgressEventArgs e)
        {
            if (UpdateProgress != null)
            {
                UpdateProgress(this, e);
            }
        }

        public void ProgressUpdate(double percent)
        {
            onUpdateProgress(new ProgressEventArgs(percent));
        }

        #endregion

    }
}
