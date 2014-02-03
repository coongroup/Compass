using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CSMSL;
using CSMSL.Analysis.Identification;
using CSMSL.IO;
using CSMSL.Proteomics;
using LumenWorks.Framework.IO.Csv;

namespace Coon.Compass.ProteinHoarder
{
    public class ProteinHoarder
    {
        public List<CsvFile> CsvFiles;
        public string FastaFile;
        public int MinimumPeptideLength;
        public int MaxMissedCleavages;
        public string OutputDirectory;
        public double MaxFdr;
        public int MinPeptidesPerGroup;

        private DateTime _startTime;
        private int _smallestPeptide = int.MaxValue;
        private int _largestPeptide;
        private List<ProteinGroup> _proteinGroups;
        //private List<ProteinGroup> filteredproteinGroups;
      
        public static Dictionary<Peptide, List<ProteinGroup>> ParsimonyPeptides;
        public static Dictionary<Peptide, List<ProteinGroup>> ParismonyPeptidesFiltered;
        public HashSet<Protease> Proteases;
        public HashSet<Modification> ModificationsToIgnore;

        

        private static Regex omssaRTRegex = new Regex(@"RT_([0-9.]+)_min", RegexOptions.Compiled);

        /// <summary>
        /// Represents all the unique peptide sequences isMapped to all occurrences (PSMs) of that sequence.
        /// </summary>
        internal static Dictionary<string, Peptide> Peptides;

        public static bool UseConservativePScore = true;
        public static PScoreCalculateionMethod PScoreCalculationMethod = PScoreCalculateionMethod.UseAllPeptides;
        public static bool UseQuant = false;
        public static bool FilterQuantInterference = false;
        public static double QuantInterferenceCutoff = 0.25;
        public static bool IncludeNonFilteredResults = false;    
        public static bool IgnorePeptideWithMissingData = false;
        public static bool SemiDigestion = false;
        public static bool ProteinsPerMinute = false;
        public static bool UseMedianForQuantitation = false;
        public static bool DuplexQuantitation = false;
        public static bool UseOnlyCompleteSets = false;
        public static AnnotationType AnnotationType = AnnotationType.None;

        public ProteinHoarder(IEnumerable<CsvFile> csvFiles,
            string fastaFile,
            string outputDirectory,
            int minPeptidesPerGroup = 1,
            int maxMissedCleavages = 3,
            double maxFDR = 1,
            AnnotationType annotationType = AnnotationType.None,
            bool useConservativePScore = true,
            bool useQuant = false,
            bool useMedian = false,
            bool duplexQuantitation = false,
            bool useNoiseBandCap = false,
            HashSet<Modification> modstoignore = null,
            bool filterquantInterference = true,
            double quantInterferenceCutoff = 0.25,
            bool includeUnfilteredResults = false,
            bool ignorePeptideWithMissingData = false,
            bool semiDigestion = false,
            bool proteinPerMinute = false)
        {
            CsvFiles = new List<CsvFile>(csvFiles);
            FastaFile = fastaFile;
            OutputDirectory = outputDirectory;
            MaxMissedCleavages = maxMissedCleavages;
            MaxFdr = maxFDR;
            MinPeptidesPerGroup = minPeptidesPerGroup;
            AnnotationType = annotationType;
            UseConservativePScore = useConservativePScore;
            UseQuant = useQuant;
            UseMedianForQuantitation = useMedian;
            DuplexQuantitation = duplexQuantitation;
            UseOnlyCompleteSets = useNoiseBandCap;
            Quantitation.UseMedian = useMedian;
            ModificationsToIgnore = modstoignore;
            FilterQuantInterference = filterquantInterference;
            QuantInterferenceCutoff = quantInterferenceCutoff;
            IncludeNonFilteredResults = includeUnfilteredResults;
            IgnorePeptideWithMissingData = ignorePeptideWithMissingData;
            SemiDigestion = semiDigestion;
            ProteinsPerMinute = proteinPerMinute;
        }
        
        /// <summary>
        /// Main algorithm for mapping peptides to proteins, and grouping proteins into protein groups
        /// </summary>
        public void Herd()
        {
            // Keep track of how long the hoarding takes
            _startTime = DateTime.Now;

            try
            {
                // 1) Get the unique peptides from all csv files
                Peptides = GetAllUniquePeptides(CsvFiles);

                // 2) Digest Proteins in the Fasta and compare with the Unique Peptides
                List<Protein> proteins = GetMappedProteinsFromFasta(FastaFile, Peptides, Proteases,
                    SemiDigestion);
                
                if (ProteinsPerMinute)
                {
                    _proteinGroups = WriteProteinsPerMinute(Peptides.Values.ToList(), proteins, OutputDirectory);
                }
                else
                {
                    _proteinGroups = GroupProteins(proteins);
                }
   
                // 4) Write out the data
                Dictionary<string, ExperimentGroup> expgroups = GroupExperiments(CsvFiles, UseQuant);

                WritePeptides(expgroups, OutputDirectory);

                WriteGroups(expgroups, OutputDirectory);
                
                // 5) Write summary document
                WriteSummary(expgroups, OutputDirectory);
                
            }
            catch (Exception e)
            {
                Log("[ERROR]\t{0}", e.Message);
            }
            finally
            {               
                CleanUp();
            }
        }

        private List<ProteinGroup> WriteProteinsPerMinute(List<Peptide> allPeptides, List<Protein> proteins, string outputDirectory)
        {
            Log("Writing proteins per minute file...");
            double maxPeptides = allPeptides.Count;
            List<ProteinGroup> groups = null;
            using (StreamWriter writer = new StreamWriter(Path.Combine(outputDirectory, "proteins_per_minute.csv")))
            {
                writer.WriteLine("Time (min),Unique Peptides,Protein Groups");

                double i = 0;
                while(i < 1000000)
                {
                    List<Peptide> currentPeptides =
                        allPeptides.Where(pep => pep.PSMs.Any(psm => psm.RetentionTime <= i)).ToList();
                    HashSet<Protein> currentProteins = new HashSet<Protein>();
                    foreach (Peptide peptide in currentPeptides)
                    {
                        peptide.ProteinGroups.Clear();
                       
                        foreach (Protein protein in proteins)
                        {
                            if (protein.Peptides.Contains(peptide))
                            {
                                currentProteins.Add(protein);
                                break;
                            }
                        }
                    }
                    groups = GroupProteins(currentProteins.ToList(), false);

                    int fdrGroups = groups.Count(g => g.PassesFDR);

                    writer.WriteLine(i + "," + currentPeptides.Count + "," + fdrGroups);
                    ProgressUpdate(currentPeptides.Count / maxPeptides);
                    if (currentPeptides.Count >= maxPeptides)
                        break;
                    i++;
                }
            }
            return groups;
        }

        /// <summary>
        /// Loads all the unique peptide sequences (L / I ambiguous) from the OMSSA csv files
        /// supplied. Keeps track of all the psms for those peptides as well.
        /// </summary>
        /// <param name="csvFiles">The OMSSA .csv Files to read the PSMs from</param>
        /// <returns>A Dictionary of all unique peptide sequences with values of all the PSMs</returns>
        private Dictionary<string, Peptide> GetAllUniquePeptides(IEnumerable<CsvFile> csvFiles)
        {
            Log("Reading in unique peptides sequences from all .csv files...");
            Dictionary<string, Peptide> peptides = new Dictionary<string, Peptide>();
            Proteases = new HashSet<Protease>();
            int psmCount = 0;

            // Loop over each input file and read its contents
            foreach (CsvFile csvfile in csvFiles)
            {
                // Keep a list of all the proteases used
                Proteases.Add(Protease.GetProtease(csvfile.Protease));

                // Counter for the number of PSMs loaded in this csvfile
                int csvPsmCount = 0;

                string sequenceString = "Peptide";
                string pvalueString = "P-value";
                bool proteomeDiscover = false;

                // Open up the csvfile and read its contents, skipping the header
                using (CsvReader reader = new CsvReader(new StreamReader(csvfile.FilePath), true))
                {
                    if (reader.GetFieldHeaders().Contains("XCorr"))
                    {
                        sequenceString = "Sequence";
                        pvalueString = "PEP";
                        proteomeDiscover = true;
                    }

                    // Read each line of the csv
                    while (reader.ReadNextRecord())
                    {
                        // Remove leucine / isoleucine ambiguity                      
                        string leuSeq = reader[sequenceString].ToUpper().Replace('I', 'L');

                        double rt = 0;
                        int specNum = 0;
                        if (proteomeDiscover)
                        {
                            rt = double.Parse(reader["RT [min]"]);
                        }
                        else
                        {
                            specNum = int.Parse(reader["Spectrum number"]);
                            rt = double.Parse(omssaRTRegex.Match(reader["Filename/id"]).Groups[1].Value);
                        }

                        double pvalue = double.Parse(reader[pvalueString]);

                        // Create a new peptide spectral match
                        PSM psm = new PSM(csvfile, specNum, rt, pvalue);

                        // Add to the list of the all the unique peptides
                        Peptide realPep;
                        if (peptides.TryGetValue(leuSeq, out realPep))  // Faster than contains key since you only try to hash once
                        {
                            realPep.PSMs.Add(psm);
                        }
                        else
                        {
                            realPep = new Peptide(leuSeq);
                            realPep.PSMs.Add(psm);

                            peptides.Add(leuSeq, realPep);
                            
                            // Check to see if the peptide was the biggest or smallest
                            if (leuSeq.Length < _smallestPeptide)
                            {
                                _smallestPeptide = leuSeq.Length;
                            }
                            if (leuSeq.Length > _largestPeptide)
                            {
                                _largestPeptide = leuSeq.Length;
                            }
                        }

                        // General psm counters;
                        csvPsmCount++;
                    }
                }

                // Total psms loaded
                psmCount += csvPsmCount;

                Log("{0:N0} PSMs were loaded from {1}", csvPsmCount, csvfile);
            }

            Log("{0:N0} unique peptides were found from the {1:N0} PSMs loaded. (I/L ambiguity removed)", peptides.Count, psmCount);
            return peptides;
        }

        /// <summary>
        /// Performs an in silico digestion of all the proteins found within the fasta file.
        /// </summary>
        /// <param name="fastaFile">The fasta filename to perfrom the digestion on</param>
        /// <param name="uniquePeptides">The unique peptides that were read in from the csv files</param>
        /// <param name="proteases"></param>
        /// <param name="semiDigestion">Perform a Semi Digestion</param>
        /// <returns>True if all the unique peptides get isMapped to at least one protein, false otherwise</returns>
        private List<Protein> GetMappedProteinsFromFasta(string fastaFile, Dictionary<string, Peptide> uniquePeptides, IEnumerable<Protease> proteases, bool semiDigestion = false)
        {
            string fastaFileniceName = Path.GetFileName(fastaFile);
            StringBuilder sb = new StringBuilder();
            foreach (Protease protease in proteases)
            {
                sb.Append(protease.Name);
                sb.Append('/');
            }
            if (sb.Length > 0) { sb.Remove(sb.Length - 1, 1); }
            Log("Performing {0}{1} digestion on {2}...", semiDigestion ? "semi " : "", sb, fastaFileniceName);
            Peptide.MappedCount = 0;
            int forwardProteins = 0, decoyProteins = 0, forwardProteinsMapped = 0, decoyProteinsMapped = 0, fastaCounter = 0;
            long totalBytes = new FileInfo(fastaFile).Length;

            // A hashset of all proteins that have a peptide that was in the input files
            Dictionary<Protein, Protein> proteins = new Dictionary<Protein, Protein>();

            int minLength = semiDigestion ? 1 : _smallestPeptide - 1;
            int maxLength = semiDigestion ? int.MaxValue : _largestPeptide + 1;
            // Open the reader for the protein database in the .fasta format
            using (FastaReader reader = new FastaReader(fastaFile))
            {
                // Read in each protein one-by-one
                foreach (Fasta fasta in reader.ReadNextFasta())
                {
                    // The number of fasta (proteins) read in (for progress bar feedback)
                    fastaCounter++;

                    // Create a new protein from the fasta
                    Protein prot = new Protein(fasta.Description, fasta.Sequence);

                    // Check if the protein is a decoy protein or not
                    if (prot.IsDecoy)
                    {
                        decoyProteins++;
                    }
                    else
                    {
                        forwardProteins++;
                    }

                    // Digest the protein's leucine sequences (all I's are now L's) with the given proteases, max missed cleavages, limiting it to the smallest and largest peptide observed (speed improvement)
                    // *Note each peptide sequence (pep_seq) will be leucine sequences as well
                    foreach (string pepSeq in AminoAcidPolymer.Digest(prot.LeucineSequence, Proteases, MaxMissedCleavages, minLength, maxLength, semiDigestion: semiDigestion))
                    {
                        // Is this one of the unique peptide sequences in the csv files? If not, we don't care about it
                        Peptide pep;
                        if (!uniquePeptides.TryGetValue(pepSeq, out pep)) 
                            continue;

                        // Check to see if this protein has already been added to the list of proteins hit
                        if(!proteins.ContainsKey(prot)) // returns true if the protein is new to the hashset of proteins
                        {
                            proteins.Add(prot, prot);
                            if (prot.IsDecoy)
                            {
                                decoyProteinsMapped++;
                            }
                            else
                            {
                                forwardProteinsMapped++;
                            }
                        }

                        // Add the peptide to the protein
                        prot.AddPeptide(pep);

                        // Mark that this peptide was successfully isMapped, this is for error checking purposes
                        pep.MarkAsMapped();
                    }

                    // Only call every 100 proteins otherwise you are wasting a lot of time refreshing and not doing actual work
                    if (fastaCounter <= 100) 
                        continue;

                    fastaCounter = 0;
                    ProgressUpdate((double)reader.BaseStream.Position / totalBytes);
                }
            }

            // Check to see if every peptide is matched, if not try using a brute force search method instead
            if (uniquePeptides.Count > Peptide.MappedCount)
            {
                List<Peptide> unMapedPeptides = uniquePeptides.Values.Where(p => !p.IsMapped).ToList();

                Log("[WARNING] Couldn't find every peptide using digestion method (wrong enzyme perhaps?), trying brute force search instead on the remaining {0} peptides...", unMapedPeptides.Count);
                //ProgressUpdate(0);
                using (FastaReader reader = new FastaReader(fastaFile))
                {
                    fastaCounter = 0;

                    // Read in each protein one-by-one
                    foreach (Fasta fasta in reader.ReadNextFasta())
                    {
                        string seq = fasta.Sequence.Replace('I', 'L');

                        foreach (Peptide pep2 in unMapedPeptides)
                        {
                            if (!seq.Contains(pep2.LeucineSequence)) 
                                continue;

                            Protein prot = new Protein(fasta.Description, fasta.Sequence);
                            Protein realProt;
                            if (proteins.TryGetValue(prot, out realProt))
                            {
                                // Add the peptide to the protein
                                realProt.AddPeptide(pep2);
                            }
                            else
                            {
                                proteins.Add(prot, prot);
                                if (prot.IsDecoy)
                                {
                                    decoyProteinsMapped++;
                                }
                                else
                                {
                                    forwardProteinsMapped++;
                                }

                                // Add the peptide to the protein
                                prot.AddPeptide(pep2);
                            }

                            // Mark that this peptide was successfully isMapped, this is for error checking purposes
                            pep2.MarkAsMapped();
                        }

                        fastaCounter++;
                        // Only call every 100 proteins otherwise you are wasting a lot of time refreshing and not doing actual work
                        if (fastaCounter <= 100) 
                            continue;
                        fastaCounter = 0;
                        ProgressUpdate((double)reader.BaseStream.Position / totalBytes);
                    }
                }
            
                // Still missing peptides?
                if (unMapedPeptides.Any(p => !p.IsMapped))
                {
                    int count = 0;
                    foreach (Peptide pep2 in unMapedPeptides)
                    {
                        if (pep2.IsMapped)
                            continue;
                        count++;
                        Log("[ERROR]\tPeptide {0} was not isMapped", pep2);
                    }
                    throw new ArgumentException(
                        string.Format(
                            "[ERROR] Unable to map every peptide ({0}) to {1}. You might be using either the wrong database, enzyme, or max missed cleavages!",
                            count, fastaFileniceName));
                }

            }
            Log("Every unique peptide was successfully mapped to at least one protein");
            Log("{0:N0} of {1:N0} ({2:F2}%) target proteins were mapped at least once", forwardProteinsMapped, forwardProteins, 100.0 * (double)forwardProteinsMapped / (double)forwardProteins);
            Log("{0:N0} of {1:N0} ({2:F2}%) decoy proteins were mapped at least once", decoyProteinsMapped, decoyProteins, 100.0 * (double)decoyProteinsMapped / (double)decoyProteins);
            ProgressUpdate(0.0); // force the progress bar to go into marquee mode      
               
            // Return a list of all the proteins that were isMapped at least once
            return proteins.Values.ToList();
        }

        /// <summary>
        /// Groups proteins into groups based on the peptides in the proteins. Combines Proteins if
        /// they contain all the same peptide sequences (Indistinquishable) and removes groups that
        /// can be made up by other groups in its entirety (Subsumable). Lastly, it filters for false
        /// discovery.
        /// </summary>
        /// <param name="proteins">A list of unique proteins to group together</param>
        private List<ProteinGroup> GroupProteins(List<Protein> proteins, bool printMessages = true)
        {
            if (printMessages)
                Log("Grouping proteins into protein groups...");
           
            // A list of protein groups that, at the end of this method, will have distinct protein groups.
            List<ProteinGroup> proteinGroups = new List<ProteinGroup>();
            if (printMessages)
                Log("{0:N0} original proteins (maximum proteins identified)", proteins.Count);

            // 1) Find Indistinguishable Proteins and group them together into Protein Groups
            // If they are not indistinguishable, then they are still converted to Protein Groups
            // but only contain one protein.
            // A 1 2 3 4
            // B 1 2 3 4
            // C 1   3 4
            // Proteins A and B are indistinguisable (have same set of peptides 1,2,3,4), and thus would become a Protein Group (PG1 [a,b])
            // C is distinguishable and would become a Protein Group (PG2 [c]).
            #region Indistinguishable

            // Loop over each protein
            int p1 = 0;
            while (p1 < proteins.Count)
            {
                // Grab the next protein and its associated peptides from the list of all proteins
                Protein protein = proteins[p1];
                HashSet<Peptide> peptides = protein.Peptides;

                // Check to see if this protein has enough peptides to be considered indentified
                //if (peptides.Count < MinPeptidesPerGroup)
                //{
                //    // This protein didn't have enough peptides, so remove it from future consideration
                //    proteins.RemoveAt(p1);

                //    // Increase the counter
                //    numberRemovedForNotEnoughPeptides++;

                //    // Go to the next protein on the list
                //    continue;
                //}

                // Start off making the protein into a protein group with its associated peptides
                ProteinGroup pg = new ProteinGroup(protein, peptides);

                // Start looking at the next protein in the list
                int p2 = p1 + 1;

                // Loop over each other protein skipping the one you just made into the PG
                while (p2 < proteins.Count)
                {
                    // Does the next protein contain the same set of peptides as the protein group?
                    if (proteins[p2].Peptides.SetEquals(peptides))
                    {
                        // Yes they are indistinguishable (i.e. proteins A and B from above), so add this protein to the protein group
                        pg.Add(proteins[p2]);

                        // Then remove this protein from the list of all proteins as not to make it into its own PG later
                        proteins.RemoveAt(p2);
                    }
                    else
                    {
                        // Go to next protein in question
                        p2++;
                    }
                }

                // We have gone through every protein possible and thus have completed the grouping of this PG
                proteinGroups.Add(pg);
                p1++;
            }
            if (printMessages)
                Log("{0:N0} protein groups are left after combining indistinguishable proteins (having the exact same set of peptides)", proteinGroups.Count);

            #endregion Indistinguishable

            // 2) Find Subsumable Proteins
            // Sort proteins from worst to best to remove the worst scoring groups first (note well, lower p-values mean better scores)
            // Case Example: P-Value, Protein Group, Peptides
            // 0.1  A 1 2
            // 0.05 B 1   3
            // 0.01 C   2 3
            // These are subsumable and we remove the worst scoring protein group (in this case, Protein Group A at p-value of 0.1) first. This would leave:
            // 0.05 B 1   3
            // 0.01 C   2 3
            // Which would mean Protein Group B and C are distinct groups, but share a common peptide (3), peptides 1 and 2 would remain unshared.
            #region Subsumable

            // First, make sure all the peptides know which protein groups they belong too, so we can determined shared peptides
            // and thus get correct p-value for the PGs.
            //MappedPeptidesToProteinGroups(proteinGroups);

            // First update each protein's p-value
            foreach (ProteinGroup proteinGroup in proteinGroups)
            {
                proteinGroup.UpdatePValue(PScoreCalculationMethod, UseConservativePScore);
            }

            // Then sort the groups on decreasing p-values
            proteinGroups.Sort(ProteinGroup.CompareDecreasing);
            
            p1 = 0;
            while (p1 < proteinGroups.Count)
            {
                // Get the peptides in the protein group
                ProteinGroup proteinGroup = proteinGroups[p1];
                HashSet<Peptide> referencePeptides = proteinGroup.Peptides;

                // Check if all the peptides are shared, if they are then the protein group is subsumable and should be removed
                if (referencePeptides.All(p => p.IsShared))
                {
                    // Since this protein group is being eliminated, remove its reference from all the peptides
                    foreach (Peptide pep in referencePeptides)
                    {
                        pep.ProteinGroups.Remove(proteinGroup);
                    }

                    // Remove the protein group from the master list
                    proteinGroups.RemoveAt(p1);
                }
                else
                {
                    p1++;
                }
            }

            if (printMessages)
                Log("{0:N0} protein groups are left after removing subsumable groups (peptides can be explain by other groups)", proteinGroups.Count);

            #endregion Subsumable

            // 3) Remove protein groups that do not have enough peptides within them
            #region MinimumGroupSize

            // No need to filter if this is one or less
            if (MinPeptidesPerGroup > 1)
            {
                p1 = 0;
                while (p1 < proteinGroups.Count)
                {
                    ProteinGroup proteinGroup = proteinGroups[p1];

                    // Check to see if this protein has enough peptides to be considered indentified
                    if (proteinGroup.Peptides.Count < MinPeptidesPerGroup)
                    {
                        // Since this protein group is being eliminated, remove its reference from all the peptides
                        foreach (Peptide pep in proteinGroup.Peptides)
                        {
                            pep.ProteinGroups.Remove(proteinGroup);
                        }

                        // This protein didn't have enough peptides, so remove it from future consideration
                        proteinGroups.RemoveAt(p1);
                    }
                    else
                    {
                        p1++;
                    }
                }
                if (printMessages)
                    Log("{0:N0} protein groups are left after removing groups with < {1:N0} peptides [parsimonious proteins]", proteinGroups.Count, MinPeptidesPerGroup);
            }

            #endregion

            // 4) Apply false discovery filtering at the protein level
            #region FDR filtering

            proteinGroups.Sort();
            // Mark each protein group that passes fdr filtering
            int count = 0;
            foreach (ProteinGroup proteinGroup in FalseDiscoveryRate<ProteinGroup, double>.Filter(proteinGroups, MaxFdr / 100, true))
            {
                proteinGroup.PassesFDR = true;
                count++;
            }

            if (printMessages)
                Log("{0:N0} protein groups are left after applying FDR of {1:N2}% [parsimonious proteins filtered]", count, MaxFdr);

            #endregion FDR filtering

            return proteinGroups;
        }

/*
        /// <summary>
        /// Maps the peptide to all the protein groups that it is apart of
        /// </summary>
        /// <param name="proteinGroups">a list of Protein Groups to map too</param>
        /// <returns>a Dictionary of peptides isMapped to a List of Protein Groups</returns>
        private void MappedPeptidesToProteinGroups(List<ProteinGroup> proteinGroups)
        {
            // Clear all the mappings first
            //foreach (ProteinGroup proteinGroup in proteinGroups)
            //{
            //    // Go over each peptide of that protein group
            //    foreach (Peptide peptide in proteinGroup.Peptides)
            //    {
            //        //peptide.ClearProteinGroups();
            //    }
            //}
            // Go over each protein group
            foreach (ProteinGroup proteinGroup in proteinGroups)
            {
                // Go over each peptide of that protein group
                foreach (Peptide peptide in proteinGroup.Peptides)
                {
                    // Add to that peptide that group
                    peptide.AddProteinGroup(proteinGroup);
                }
            }
        }
*/

        private Dictionary<string, ExperimentGroup> GroupExperiments(IEnumerable<CsvFile> csvFiles, bool useQuant)
        {
            Dictionary<string, ExperimentGroup> uniqueNames = new Dictionary<string, ExperimentGroup>();
            foreach (CsvFile file in csvFiles)
            {
                ExperimentGroup experiment;
                if (string.IsNullOrEmpty(file.ExperimentName))
                {
                    file.ExperimentName = "";
                }
                if(uniqueNames.TryGetValue(file.ExperimentName, out experiment))
                {
                    experiment.CsvFiles.Add(file);
                } 
                else
                {
                    experiment = new ExperimentGroup(file.ExperimentName);
                    experiment.CsvFiles.Add(file);
                    uniqueNames.Add(file.ExperimentName, experiment);
                    using (CsvReader reader = new CsvReader(new StreamReader(file.FilePath), true))
                    {
                        // write the header only once to the two outputs
                        string[] headers = reader.GetFieldHeaders();
                        experiment.Header = string.Join(",", headers);

                        if (!useQuant) continue;

                        int headerCount = headers.Length;

                        experiment.TQStart = -1;
                        experiment.TQStop = headerCount - 2;
                        for (int i = 0; i < headerCount; i++)
                        {
                            string header = headers[i];
                            if (!header.Contains("NL)"))
                                continue;
                            experiment.TQStart = i;
                            break;
                        }

                        if (experiment.TQStart < 0)
                        {
                            Log("[WARNING] No quantification data found in {0} for experiment {1}", file, experiment.Name);
                            experiment.UseQuant = false;
                            break;
                        }
                        experiment.UseQuant = true;

                        // Get the experimental Quant headers
                        StringBuilder sb = new StringBuilder();
                        for (int i = experiment.TQStart; i <= experiment.TQStop; i++)
                        {
                            sb.Append(headers[i]);
                            sb.Append(',');
                        }
                        sb.Remove(sb.Length - 1, 1);
                        experiment.QuantHeader = sb.ToString();
                    }
                }

            }

            return uniqueNames;

            //Dictionary<char, ExperimentGroup> expgroups = new Dictionary<char, ExperimentGroup>();
            //foreach (CsvFile csvfile in csvFiles)
            //{
            //    ExperimentGroup exp;
            //    if (expgroups.TryGetValue(csvfile.ExperimentGroup, out exp))
            //    {
            //        exp.CsvFiles.Add(csvfile);
            //    }
            //    else
            //    {
            //        exp = new ExperimentGroup(csvfile.ExperimentGroup, csvfile.ExperimentName);
            //        exp.CsvFiles.Add(csvfile);
            //        expgroups.Add(csvfile.ExperimentGroup, exp);
            //        using (CsvReader reader = new CsvReader(new StreamReader(csvfile.FilePath), true))
            //        {
            //            // write the header only once to the two outputs
            //            string[] headers = reader.GetFieldHeaders();
            //            exp.Header = string.Join(",", headers);

            //            if (!useQuant) continue;

            //            int headerCount = headers.Length;

            //            exp.TQStart = -1;
            //            exp.TQStop = headerCount - 2;
            //            for (int i = 0; i < headerCount; i++)
            //            {
            //                string header = headers[i];
            //                if (!header.Contains("NL)")) 
            //                    continue;
            //                exp.TQStart = i;
            //                break;
            //            }

            //            if (exp.TQStart < 0)
            //            {
            //                Log("[WARNING] No quantification data found in {0} for experiment {1}", csvfile, exp.Name);
            //                exp.UseQuant = false;
            //                break;
            //            }
            //            exp.UseQuant = true;
                      
            //            // Get the experimental Quant headers
            //            StringBuilder sb = new StringBuilder();
            //            for (int i = exp.TQStart; i <= exp.TQStop; i++)
            //            {
            //                sb.Append(headers[i]);
            //                sb.Append(',');
            //            }
            //            sb.Remove(sb.Length - 1, 1);
            //            exp.QuantHeader = sb.ToString();
            //        }
            //    }
            //}
            //return expgroups;
        }

        private void WriteSummary(Dictionary<string, ExperimentGroup> expgroups, string outputDirectory)
        {
            // Only write it for duplex
            if (!DuplexQuantitation)
                return;

            string filePath = Path.Combine(outputDirectory, "Protein_summary.csv");
            Log("Writing file " + filePath);

            List<ExperimentGroup> experiments = expgroups.Values.ToList();

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Header info
                writer.Write("Protein Group,Representative Protein Description");
                if (AnnotationType == AnnotationType.SGD)
                {
                    writer.Write(",SGD Ids,Gene Names");
                } else if (AnnotationType == AnnotationType.UniProt)
                {
                    writer.Write(",UniProt Ids,Gene Names");
                }
                foreach (ExperimentGroup exp in experiments)
                {
                    writer.Write("," + exp.Name + " (log2)");
                }
                writer.WriteLine();

                // Iterate over all protein groups
                foreach(ProteinGroup pg in _proteinGroups.Where(pg => pg.PassesFDR))
                {
                    writer.Write(pg.Name);
                    writer.Write(',');
                    writer.Write(pg.RepresentativeProtein.Description);
                    if (AnnotationType != AnnotationType.None)
                    {
                        writer.Write(',');
                        writer.Write(pg.ProteinIdsString());
                        writer.Write(',');
                        writer.Write(pg.GeneNamesString());
                    }

                    foreach (ExperimentGroup exp in experiments)
                    {
                        writer.Write(',');
                        double log2Ratio = 0;
                        if (exp.ProteinGroups.Contains(pg) && pg.TryGetLog2Ratio(exp, out log2Ratio, UseOnlyCompleteSets))
                        {
                            log2Ratio -= exp.MeidanLog2Ratio;  
                            writer.Write(log2Ratio.ToString("G4"));
                        }
                        else
                        {
                            writer.Write("n/a");
                        }
                    }
                    writer.WriteLine();
                }
            }
        }

        private void WriteGroups(Dictionary<string, ExperimentGroup> expgroups, string outputDirectory)
        {
            foreach (ExperimentGroup exp in expgroups.Values)
            {
                StreamWriter writer = null;
                if (IncludeNonFilteredResults)
                {
                    string filename = "";
                    if (string.IsNullOrEmpty(exp.Name))
                    {
                        filename = Path.Combine(outputDirectory, "Parsimony_proteins.csv");
                    }
                    else
                    {
                        filename = Path.Combine(outputDirectory, string.Format("{0}_parsimony_proteins.csv", exp.Name));
                    }
                    Log("Writing file " + filename);
                    writer = new StreamWriter(filename);
                }

                string filteredfilename = "";
                if (string.IsNullOrEmpty(exp.Name))
                {
                    filteredfilename = Path.Combine(outputDirectory, "Parsimony_proteins_filtered.csv");
                }
                else
                {
                    filteredfilename = Path.Combine(outputDirectory, string.Format("{0}_parsimony_proteins_filtered.csv", exp.Name));   
                }
                
                Log("Writing file " + filteredfilename);
                StreamWriter filteredwriter = new StreamWriter(filteredfilename);
                
                StringBuilder header = new StringBuilder("Protein Group Name,Representative Protein Description,Total Amino Acids,Sequence Coverage (%),Numbers of Proteins,Number of PSMs,Number of Unique Peptides,# PSMs in Experiment,# Unique Seq in Experiment,P-Score,");
                if (exp.UseQuant)
                {
                    header.Append("# Quantified PSMs,# Quantified Peptides,");
                    header.Append(exp.QuantHeader);
                    if (DuplexQuantitation)
                    {
                        header.Append(",Log2 Ratio,Normalized Log2 Ratio");
                    }
                }
                if (AnnotationType == AnnotationType.UniProt)
                {
                    header.Append(",Uniprot IDs,Gene Names");
                } 
                else if (AnnotationType == AnnotationType.SGD)
                {
                    header.Append(",SGD IDs,Gene Names");
                }

                if (IncludeNonFilteredResults)
                {
                    writer.WriteLine(header);
                }
                filteredwriter.WriteLine(header);
                                
                if (DuplexQuantitation && exp.UseQuant)
                {
                    List<double> log2s = new List<double>();
                    foreach (ProteinGroup pg in exp.ProteinGroups.OrderBy(pg => pg.PScore))
                    {
                        double log2Ratio = 0;
                        if (pg.TryGetLog2Ratio(exp, out log2Ratio, UseOnlyCompleteSets))
                        {
                            log2s.Add(log2Ratio);
                        }
                    }
                    log2s.Sort();
                    exp.MeidanLog2Ratio = log2s.Median();
                }

                // Loop over each protein group
                foreach (ProteinGroup pg in exp.ProteinGroups.OrderBy(pg => pg.PScore))
                {
                    string line = pg.ToParsimonyProteins(exp, DuplexQuantitation,  UseOnlyCompleteSets);
                    if (IncludeNonFilteredResults)
                    {
                        writer.WriteLine(line);
                    }
                    if (pg.PassesFDR)
                    {
                        filteredwriter.WriteLine(line);
                    }
                }

                if (IncludeNonFilteredResults)
                {
                    writer.Close();
                }
                filteredwriter.Close();
            }
        }

        private void WritePeptides(Dictionary<string, ExperimentGroup> expgroups, string outputDirectory)
        {
            foreach (Peptide pep in Peptides.Values)
            {
                if (pep.ProteinGroups == null || pep.ProteinGroups.Count < 1) continue;
                pep.ProteinGroups.Sort(ProteinGroup.CompareIncreasing);
                pep.BestPG = pep.ProteinGroups[0];
            }
            // Loop over each experiment
            foreach (ExperimentGroup exp in expgroups.Values)
            {
                string experimentName = exp.Name;
                List<CsvFile> files = exp.CsvFiles;
                StreamWriter writer = null;
                if (IncludeNonFilteredResults)
                {
                    string filename = "";
                    if (string.IsNullOrEmpty(experimentName))
                    {
                        filename = Path.Combine(outputDirectory, "Parsimony_peptides.csv");
                    }
                    else
                    {
                        filename = Path.Combine(outputDirectory, string.Format("{0}_parsimony_peptides.csv", experimentName));
                    }
                    Log("Writing file {0}", filename);
                    writer = new StreamWriter(filename);
                }
                string filteredfilename = "";
                if (string.IsNullOrEmpty(experimentName))
                {
                    filteredfilename = Path.Combine(outputDirectory, "Parsimony_peptides_filtered.csv");
                }
                else
                {
                    filteredfilename = Path.Combine(outputDirectory, string.Format("{0}_parsimony_peptides_filtered.csv", experimentName));           
                }
                Log("Writing file {0}", filteredfilename);
                StreamWriter filteredwriter = new StreamWriter(filteredfilename);
                
                string header = exp.Header + ",Experiment Name,# of Sharing PGs,Best PG Name";
                if (IncludeNonFilteredResults)
                {
                    writer.WriteLine(header);
                }
                filteredwriter.WriteLine(header);

                foreach (CsvFile csvfile in files)
                {
                    string sequenceString = "Peptide";
                    //string spectrumNumberString = "Spectrum number";
                    //string pvalueString = "P-value";

                    // Open up the csvfile and read it's contents, skipping the header
                    using (CsvReader reader = new CsvReader(new StreamReader(csvfile.FilePath), true))
                    {
                        bool pdOutput = false;
                        if (reader.GetFieldHeaders().Contains("XCorr"))
                        {
                            sequenceString = "Sequence";
                            //spectrumNumberString = "RT [min]";
                            //pvalueString = "PEP";
                            pdOutput = true;
                        }

                        int deflineIndex = reader.GetFieldIndex("Defline");
                        int startRes = reader.GetFieldIndex("Start");
                        int stopRes = reader.GetFieldIndex("Stop");
                        Peptide pep = null;
                        // Read in each psm
                        while (reader.ReadNextRecord())
                        {
                            string seq = reader[sequenceString].ToUpper();
                            string leuSeq = seq.Replace('I', 'L');

                            string[] data = new string[reader.FieldCount];
                            if (Peptides.TryGetValue(leuSeq, out pep))
                            {
                                if (pep.BestPG == null || pep.BestPG.IsDecoy) continue;

                                foreach (ProteinGroup pg in pep.ProteinGroups)
                                {
                                    exp.ProteinGroups.Add(pg);
                                    int psms = 0;
                                    if (pg.PsmsPerGroup.TryGetValue(exp, out psms))
                                    {
                                        psms++;
                                        pg.PsmsPerGroup[exp] = psms;
                                    }
                                    else
                                    {
                                        pg.PsmsPerGroup.Add(exp, 1);
                                    }
                                    HashSet<Peptide> peptides = null;
                                    if (pg.UniquePeptidesPerGroup.TryGetValue(exp, out peptides))
                                    {
                                        peptides.Add(pep);
                                    }
                                    else
                                    {
                                        peptides = new HashSet<Peptide>();
                                        peptides.Add(pep);
                                        pg.UniquePeptidesPerGroup.Add(exp, peptides);
                                    }
                                }

                                // If performing quantitation
                                if (exp.UseQuant)
                                {
                                    // Only use unshared Peptides
                                    bool usePeptideInQuant = !pep.IsShared;

                                    if (usePeptideInQuant)
                                    {
                                        string mod_line = reader["Mods"];
                                        if (!string.IsNullOrEmpty(mod_line))
                                        {
                                            string[] mods = mod_line.Split(',');
                                            foreach (string mod in mods)
                                            {
                                                string mod_name = mod.Split(':')[0];
                                                Modification testmod = new Modification(mod_name);
                                                if (ModificationsToIgnore.Contains(testmod))
                                                {
                                                    usePeptideInQuant = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                  

                                    if (usePeptideInQuant)
                                    {
                                        Quantitation quant = null;
                                        double[] quantData = new double[4 * (int)exp.QuantPlex];
                                        bool keepPeptide = true;
                                        
                                        // Read in all the quant data from the csvfile
                                        int j = 0;
                                        for (int i = exp.TQStart; i <= exp.TQStop; i++)
                                        {
                                            double value = 0;
                                            if (double.TryParse(reader[i], out value))
                                            {
                                                quantData[j] = value;
                                            }
                                            else
                                            {
                                                quantData[j] = 0;
                                            }


                                            if (IgnorePeptideWithMissingData && quantData[j] == 0)
                                            {
                                                keepPeptide = false;
                                                break;
                                            }

                                            j++;
                                        }

                                        int detectedChannels = int.Parse(reader[exp.TQStop + 1]);
                                        int totalChannels = (int) exp.QuantPlex;
                                        bool isMissingChannel = detectedChannels != totalChannels;

                                        if (keepPeptide)
                                        {
                                            //pep.SetQuantData(quantData);

                                            // append/add the quant data to the best and only protein group
                                            if (pep.BestPG.Quantitation.TryGetValue(exp.Name, out quant))
                                            {
                                                quant.AddData(pep, quantData, isMissingChannel);
                                            }
                                            else
                                            {
                                                quant = new Quantitation(exp.QuantPlex, pep, quantData, isMissingChannel);
                                                pep.BestPG.Quantitation.Add(exp.Name, quant);
                                            }
                                        }
                                    }
                                }

                                StringBuilder sb = new StringBuilder();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    data[i] = reader[i];
                                    if (!pdOutput)
                                    {
                                        if (i == deflineIndex)
                                        {
                                            data[i] = pep.BestPG.RepresentativeProtein.Description;
                                        }
                                        else if (i == startRes)
                                        {
                                            data[i] =
                                                pep.BestPG.RepresentativeProtein.FindStartResidue(pep.LeucineSequence)
                                                    .ToString();
                                        }
                                        else if (i == stopRes)
                                        {
                                            data[i] =
                                                pep.BestPG.RepresentativeProtein.FindStopResidue(pep.LeucineSequence)
                                                    .ToString();
                                        }
                                    }
                                    if(data[i].Contains(','))
                                        data[i] = "\"" + data[i] + "\"";
                                    sb.Append(data[i]);
                                    sb.Append(',');
                                }
                                sb.Append(exp.Name);
                                sb.Append(',');
                                sb.Append(pep.NumberOfSharingProteinGroups);
                                sb.Append(',');
                                foreach (ProteinGroup pg2 in pep.ProteinGroups)
                                {
                                    sb.Append(pg2.Name);
                                    sb.Append('|');
                                }
                                sb.Remove(sb.Length - 1, 1);
                                if (IncludeNonFilteredResults)
                                {
                                    writer.WriteLine(sb);
                                }
                                if (pep.BestPG.PassesFDR)
                                {
                                    filteredwriter.WriteLine(sb);
                                }
                            }
                            else
                            {
                                throw new Exception("Read in a PSM that isn't mapped! Did an input CSV's contents change?");
                            }
                        }                    
                    }
                }
                if (IncludeNonFilteredResults)
                    writer.Close();
                filteredwriter.Close();
            }
        }

        /// <summary>
        /// Free memory of data structures used and tell the main form to reactivate
        /// </summary>
        private void CleanUp()
        {
            if (Peptides != null)
            {
                Peptides.Clear();
                Peptides = null;
            }
            if (ParsimonyPeptides != null)
            {
                ParsimonyPeptides.Clear();
                ParsimonyPeptides = null;
            }
            if (ParismonyPeptidesFiltered != null)
            {
                ParismonyPeptidesFiltered.Clear();
                ParismonyPeptidesFiltered = null;
            }           
            TimeSpan diff = DateTime.Now - _startTime;
            Log("Finished [{0:D2} hrs, {1:D2} mins, {2:D2} secs]", diff.Hours, diff.Minutes, diff.Seconds);
            ProgressUpdate(-1);
        }
        
        #region Callbacks

        public event EventHandler<StatusEventArgs> UpdateLog;

        protected virtual void OnUpdateLog(StatusEventArgs e)
        {
            var del = UpdateLog;
            if (del != null)
            {
                del(this, e);
            }
        }

        public void Log(string message)
        {
            OnUpdateLog(new StatusEventArgs(message));
        }

        public void Log(string format, params object[] args)
        {
            OnUpdateLog(new StatusEventArgs(string.Format(format, args)));                
        }

        public event EventHandler<ProgressEventArgs> UpdateProgress;

        protected virtual void OnUpdateProgress(ProgressEventArgs e)
        {
            var del = UpdateProgress;
            if (del != null)
            {
                del(this, e);
            }
        }

        public void ProgressUpdate(double percent)
        {
            OnUpdateProgress(new ProgressEventArgs(percent));
        }

        #endregion Callbacks
    }
}