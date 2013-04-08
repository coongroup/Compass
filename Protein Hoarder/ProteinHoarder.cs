using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CSMSL.IO;
using CSMSL.Proteomics;
using CSMSL.Analysis.Identification;
using LumenWorks.Framework.IO.Csv;

namespace Protein_Hoarder
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

        private DateTime startTime;
        private int smallestPeptide = int.MaxValue;
        private int largestPeptide = 0;
       // private List<ProteinGroup> proteinGroups;
        //private List<ProteinGroup> filteredproteinGroups;
      
        public static Dictionary<Peptide, List<ProteinGroup>> Parsimony_Peptides;
        public static Dictionary<Peptide, List<ProteinGroup>> Parismony_Peptides_Filtered;
        public HashSet<Protease> Proteases;
        public HashSet<Modification> ModificationsToIgnore;

        /// <summary>
        /// Represents all the unique peptide sequences mapped to all occurances (PSMs) of that sequence.
        /// </summary>
        internal static Dictionary<string, Peptide> Peptides;

        /// <summary>
        /// The header of the input csv file. Assuming all inputs have the same headers...
        /// </summary>
        private string inputHeader = string.Empty;

        public static bool UseConservativePScore = true;
        public static PScoreCalculateionMethod PScoreCalculationMethod = PScoreCalculateionMethod.UseAllPeptides;
        public static bool UseQuant = false;
        public static bool FilterQuantInterference = false;
        public static double QuantInterferenceCutoff = 0.25;
        public static bool IncludeNonFilteredResults = false;    
        public static bool IgnorePeptideWithMissingData = false;

        public ProteinHoarder(IEnumerable<CsvFile> csvFiles,
            string fastaFile,
            string outputDirectory,
            int minPeptidesPerGroup = 1,
            int maxMissedCleavages = 3,
            double maxFDR = 1,
            bool useConservativePScore = true,
            bool useQuant = false,
            HashSet<Modification> modstoignore = null,
            bool filterquantInterference = true,
            double quantInterferenceCutoff = 0.25,
            bool includeUnfilteredResults = false,
            bool ignorePeptideWithMissingData = false)
        {
            CsvFiles = new List<CsvFile>(csvFiles);
            FastaFile = fastaFile;
            OutputDirectory = outputDirectory;
            MaxMissedCleavages = maxMissedCleavages;
            MaxFdr = maxFDR;
            MinPeptidesPerGroup = minPeptidesPerGroup;
            UseConservativePScore = useConservativePScore;
            UseQuant = useQuant;
            ModificationsToIgnore = modstoignore;
            FilterQuantInterference = filterquantInterference;
            QuantInterferenceCutoff = quantInterferenceCutoff;
            IncludeNonFilteredResults = includeUnfilteredResults;
            IgnorePeptideWithMissingData = ignorePeptideWithMissingData;
        }
        
        /// <summary>
        /// Main algorithm for mapping peptides to proteins, and grouping proteins into protein groups
        /// </summary>
        public void Herd()
        {
            // Keep track of how long the hoarding takes
            startTime = DateTime.Now;

            try
            {
                // 1) Get the unique peptides from all csv files
                Peptides = GetAllUniquePeptides(CsvFiles);

                // 2) Digest Proteins in the Fasta and compare with the Unique Peptides
                List<Protein> proteins = GetMappedProteinsFromFasta(FastaFile, Peptides, Proteases);              

                // 3) Construct the protein groups from the mapped proteins
                List<ProteinGroup> proteinGroups = GroupProteins(proteins);

                // 4) Write out the data
                Dictionary<char, ExperimentGroup> expgroups = GroupExperiments(CsvFiles, UseQuant);

                WritePeptides(expgroups, OutputDirectory);

                WriteGroups(expgroups, OutputDirectory);
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

        /// <summary>
        /// Loads all the unique peptide sequences (L / I ambiguous) from the OMSSA csv files
        /// supplied. Keeps track of all the psms for those peptides as well.
        /// </summary>
        /// <param name="csvFiles">The OMSSA .csv Files to read the PSMs from</param>
        /// <returns>A Dictionary of all unique peptide sequences with values of all the PSMs</returns>
        private Dictionary<string, Peptide> GetAllUniquePeptides(List<CsvFile> csvFiles)
        {
            Log("Reading in unique peptides sequences from all .csv files...");
            ProgressUpdate(0.0); //force the progressbar to go into marquee mode
            Dictionary<string, Peptide> peptides = new Dictionary<string, Peptide>();
            Proteases = new HashSet<Protease>();
            Peptide realPep = null;
            int psmCount = 0;

            // Loop over each input file and read its contents
            foreach (CsvFile csvfile in csvFiles)
            {
                // Keep a list of all the proteases used
                Proteases.Add(CSMSL.Proteomics.Protease.GetProtease(csvfile.Protease));

                // Counter for the number of PSMs loaded in this csvfile
                int csvPsmCount = 0;

                // Open up the csvfile and read its contents, skipping the header
                using (CsvReader reader = new CsvReader(new StreamReader(csvfile.FilePath), true))
                {
                    // Read each line of the csv
                    while (reader.ReadNextRecord())
                    {
                        // Remove leucine / isoleucine ambiguity                      
                        string leuSeq = reader["Peptide"].ToUpper().Replace('I', 'L');

                        // Read in the basic stats from the file
                        int specNum = int.Parse(reader["Spectrum number"]);
                        double pvalue = double.Parse(reader["P-value"]);

                        // Create a new peptide spectral match
                        PSM psm = new PSM(csvfile, specNum, pvalue);

                        // Add to the list of the all the unique peptides
                        if (peptides.TryGetValue(leuSeq, out realPep))  // Faster than containskey since you only try to hash once
                        {
                            realPep.PSMs.Add(psm);
                        }
                        else
                        {
                            realPep = new Peptide(leuSeq);
                            realPep.PSMs.Add(psm);

                            peptides.Add(leuSeq, realPep);
                            
                            // Check to see if the peptide was the biggest or smallest
                            if (leuSeq.Length < smallestPeptide)
                            {
                                smallestPeptide = leuSeq.Length;
                            }
                            if (leuSeq.Length > largestPeptide)
                            {
                                largestPeptide = leuSeq.Length;
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
        /// <returns>True if all the unique peptides get mapped to at least one protein, false otherwise</returns>
        private List<Protein> GetMappedProteinsFromFasta(string fastaFile, Dictionary<string, Peptide> uniquePeptides, IEnumerable<Protease> proteases)
        {
            string fastaFileniceName = Path.GetFileName(fastaFile);
            StringBuilder sb = new StringBuilder();
            foreach (Protease protease in proteases)
            {
                sb.Append(protease.Name);
                sb.Append('/');
            }
            if (sb.Length > 0) { sb.Remove(sb.Length - 1, 1); }
            Log("Performing {0} digestion on {1}...", sb, fastaFileniceName);
            Peptide pep = null;
            Peptide.MappedCount = 0;
            int forwardProteins = 0, decoyProteins = 0, forwardProteinsMapped = 0, decoyProteinsMapped = 0, fastaCounter = 0;
            long total_bytes = new FileInfo(fastaFile).Length;

            // A hashset of all proteins that have a peptide that was in the input files
            HashSet<Protein> proteins = new HashSet<Protein>();

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

                    // Digest the protein's leucine sequences (all I's are now L's) with the given proteaes, max missed clevages, limiting it to the smallest and larget peptide obsvered (speed improvement)
                    // *Note each peptide sequence (pep_seq) will be leucine sequences as well
                    foreach (string pep_seq in CSMSL.Proteomics.Protein.Digest(prot.LeucineSequence, Proteases,0, MaxMissedCleavages, false, smallestPeptide - 1, largestPeptide + 1))
                    {
                        // Is this one of the unique peptide sequences in the csv files? If not, we don't care about it
                        if (uniquePeptides.TryGetValue(pep_seq, out pep))
                        {
                            // Check to see if this protein has already been added to the list of proteins hit
                            if (proteins.Add(prot)) // returns true if the protein is new to the hashset of proteins
                            {
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

                            // Mark that this peptide was successfully mapped, this is for error checking purposes
                            pep.MarkAsMapped();
                        }
                    }

                    // Only call every 1000 proteins otherwise you are wasting a lot of time refreshing and not doing actual work
                    if (fastaCounter > 1000)
                    {
                        fastaCounter = 0;
                        ProgressUpdate((double)reader.BaseStream.Position / total_bytes);
                    }
                }
            }

            // Check to see if every peptide is matched, if not quit, wrong database or another error
            if (uniquePeptides.Count > Peptide.MappedCount)
            {                
                foreach (Peptide pep2 in uniquePeptides.Values)
                {
                    if (pep2.mapped) continue;
                    Log("[ERROR]\tPeptide {0} was not mapped", pep2);
                }
                throw new ArgumentException(string.Format("Could not mapped every peptide to {0}. You might be using the wrong database?", fastaFileniceName));
            }

            Log("{0:N0} of {1:N0} ({2:F2}%) target proteins were mapped at least once", forwardProteinsMapped, forwardProteins, 100.0 * (double)forwardProteinsMapped / (double)forwardProteins);
            Log("{0:N0} of {1:N0} ({2:F2}%) decoy proteins were mapped at least once", decoyProteinsMapped, decoyProteins, 100.0 * (double)decoyProteinsMapped / (double)decoyProteins);
            Log("Every unique peptide was successfully mapped to at least one protein");
            ProgressUpdate(0.0); // force the progress bar to go into marquee mode      
               
            // Return a list of all the proteins that were mapped at least once
            return proteins.ToList();
        }

        /// <summary>
        /// Groups proteins into groups based on the peptides in the proteins. Combines Proteins if
        /// they contain all the same peptide sequences (Indistinquishable) and removes groups that
        /// can be made up by other groups in its entirety (Subsumable). Lastly, it filters for false
        /// discovery.
        /// </summary>
        /// <param name="proteins">A list of unique proteins to group together</param>
        private List<ProteinGroup> GroupProteins(List<Protein> proteins)
        {
            Log("Grouping proteins into protein groups...");
            ProgressUpdate(0.0);

            // A list of protein groups that, at the end of this method, will have distinct protein groups.
            List<ProteinGroup> proteinGroups = new List<ProteinGroup>();
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
            Log("{0:N0} protein groups are left after combining indistinguishable groups (having the exact same set of peptides)", proteinGroups.Count);

            #endregion Indistinguishable

            // 2) Find Subsumable Proteins
            // Sort proteins from worst to best to remove the worst scoring groups first (note well, lower p-values mean better scores)
            // Case Example: P-Value, Protein, Peptides
            // 0.1  A 1 2
            // 0.05 B 1   3
            // 0.01 C   2 3
            // These are subsumable and we remove the worst scoring protein (in this case, Protein A at p-value of 0.1) first. This would leave:
            // 0.05 B 1   3
            // 0.01 C   2 3
            // Which would mean Protein Group B and C are seperate groups, but share a common peptide (3), peptides 1 and 2 would remain unshared.
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

            // Loop over each protein group
            p1 = 0;
            while (p1 < proteinGroups.Count)
            {
                // Get the peptides in the protein group
                ProteinGroup proteinGroup = proteinGroups[p1];
                HashSet<Peptide> reference_peptides = new HashSet<Peptide>(proteinGroup.Peptides);

                bool subsumable_protein_group = false;

                // Loop over each protein group again
                for (int p2 = 0; p2 < proteinGroups.Count; p2++)
                {
                    // Don't compare the same protein group to each other, move to the next protein group then
                    if (p1 == p2)
                    {
                        continue;
                    }

                    // Remove all the pepetides that are in the second protein group (p2) from the peptides in the first protein group (p1, reference_peptides);
                    reference_peptides.ExceptWith(proteinGroups[p2].Peptides);

                    // If the first protein group (p1) has no peptides left, it is subsumable (e.g. Protein A in above example, Peptides 1 and 2 are found in other groups)
                    if (reference_peptides.Count == 0)
                    {
                        subsumable_protein_group = true;
                        break;
                    }
                }

                // Remove the group since it was subsumable and has a worst p-value then the other groups.
                if (subsumable_protein_group)
                {
                    // Since this protein group is being eliminated, remove its reference from all the peptides
                    foreach (Peptide pep in proteinGroup.Peptides)
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
                Log("{0:N0} protein groups are left after removing groups with < {1:N0} peptides [parsimonious proteins]", proteinGroups.Count, MinPeptidesPerGroup);
            }

            #endregion

            // 4) Apply false discovery filtering at the protein level
            #region FDR filtering

            proteinGroups.Sort();
            // Mark each protein group that passes fdr filtering
            int count = 0;
            foreach (ProteinGroup proteinGroup in FalseDiscoveryRate<ProteinGroup, double>.Filter(proteinGroups, (double)MaxFdr / 100, true))
            {
                proteinGroup.PassesFDR = true;
                count++;
            }

            Log("{0:N0} protein groups are left after applying FDR of {1:N2}% [parsimonious proteins filtered]", count, MaxFdr);

            #endregion FDR filtering

            return proteinGroups;
        }

        /// <summary>
        /// Maps the peptide to all the protein groups that it is apart of
        /// </summary>
        /// <param name="proteinGroups">a list of Protein Groups to map too</param>
        /// <returns>a Dictionary of peptides mapped to a List of Protein Groups</returns>
        private void MappedPeptidesToProteinGroups(List<ProteinGroup> proteinGroups)
        {
            // Clear all the mappings first
            foreach (ProteinGroup proteinGroup in proteinGroups)
            {
                // Go over each peptide of that protein group
                foreach (Peptide peptide in proteinGroup.Peptides)
                {
                    //peptide.ClearProteinGroups();
                }
            }
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
            return;
        }

        private Dictionary<char, ExperimentGroup> GroupExperiments(List<CsvFile> csvFiles, bool useQuant)
        {
            Dictionary<char, ExperimentGroup> expgroups = new Dictionary<char, ExperimentGroup>();
            ExperimentGroup exp = null;
            foreach (CsvFile csvfile in csvFiles)
            {
                if (expgroups.TryGetValue(csvfile.ExperimentGroup, out exp))
                {
                    exp.CsvFiles.Add(csvfile);
                }
                else
                {
                    exp = new ExperimentGroup(csvfile.ExperimentGroup);
                    exp.CsvFiles.Add(csvfile);
                    expgroups.Add(csvfile.ExperimentGroup, exp);
                    using (CsvReader reader = new CsvReader(new StreamReader(csvfile.FilePath), true))
                    {
                        // write the header only once to the two outputs
                        string[] headers = reader.GetFieldHeaders();
                        exp.Header = string.Join(",", headers);

                        if (useQuant)
                        {
                            exp.TQStart = -1;
                            exp.TQStop = -1;

                            foreach (string header in headers)
                            {
                                if (header.StartsWith("TQ_126_"))
                                {
                                    exp.TQStart = reader.GetFieldIndex(header);
                                    break;
                                }
                                if (header.StartsWith("TQ_113_"))
                                {
                                    exp.TQStart = reader.GetFieldIndex(header);
                                    break;
                                }
                                if (header.StartsWith("TQ_114_"))
                                {
                                    exp.TQStart = reader.GetFieldIndex(header);
                                    break;
                                }
                            }

                            if (exp.TQStart < 0)
                            {
                                Log("[WARNING!] No quantification data found in {0} for experiment {1}", csvfile, exp.ExperimentalID);
                                exp.UseQuant = false;
                                break;
                            }
                            else
                            {
                                exp.UseQuant = true;
                            }

                            int totalheaders = headers.Length - 1;
                            exp.TQStop = totalheaders;

                            //exp.TQStop = exp.TQStart + 4 * (int)exp.QuantType + 1;

                            // Get the experimental Quant headers
                            StringBuilder sb = new StringBuilder();
                            for (int i = exp.TQStart; i < exp.TQStop; i++)
                            {
                                sb.Append(headers[i]);
                                sb.Append(',');
                            }
                            sb.Remove(sb.Length - 1, 1);
                            exp.QuantHeader = sb.ToString();
                        }
                    }
                }
            }
            return expgroups;
        }

        private void WriteGroups(Dictionary<char, ExperimentGroup> expgroups, string outputDirectory)
        {
            foreach (ExperimentGroup exp in expgroups.Values)
            {
                StreamWriter writer = null, filteredwriter = null;
                if (IncludeNonFilteredResults)
                {
                    string filename = Path.Combine(outputDirectory, string.Format("{0}_parsimony_proteins.csv", exp.ExperimentalID));
                    Log("Writing file " + filename);
                    writer = new StreamWriter(filename);
                }
               
                string filteredfilename = Path.Combine(outputDirectory, string.Format("{0}_parsimony_proteins_filtered.csv", exp.ExperimentalID));               
                Log("Writing file " + filteredfilename);
                filteredwriter = new StreamWriter(filteredfilename);
                
                StringBuilder header = new StringBuilder("Protein Group Name,Representative Protein Description,Total Amino Acids,Sequence Coverage (%),Numbers of Proteins,Number of PSMs,Number of Unique Peptides,# PSMs in Experiment,# Unique Seq in Experiment,P-Score,");
                if (exp.UseQuant)
                {
                    header.Append("# Quantified PSMs,# Quantified Peptides,");
                    header.Append(exp.QuantHeader);
                    header.Append(',');
                }
                header.Append("UniprotIDs,Gene Names");

                if (IncludeNonFilteredResults)
                {
                    writer.WriteLine(header);
                }
                filteredwriter.WriteLine(header);

                // Loop over each protein group
                foreach (ProteinGroup pg in exp.ProteinGroups.OrderBy(pg => pg.PScore))
                {
                    string line = pg.ToParsimonyProteins(exp);
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

        private void WritePeptides(Dictionary<char, ExperimentGroup> expgroups, string outputDirectory)
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
                char experimentID = exp.ExperimentalID;
                List<CsvFile> files = exp.CsvFiles;
                StreamWriter writer = null, filteredwriter = null;
                if (IncludeNonFilteredResults)
                {
                    string filename = Path.Combine(outputDirectory, string.Format("{0}_parsimony_peptides.csv", experimentID));
                    Log("Writing file {0}", filename);
                    writer = new StreamWriter(filename);
                }
               
                string filteredfilename = Path.Combine(outputDirectory, string.Format("{0}_parsimony_peptides_filtered.csv", experimentID));                
                Log("Writing file {0}", filteredfilename);
                filteredwriter = new StreamWriter(filteredfilename);
                
                string header = exp.Header + ",Experiment ID,# of Sharing PGs,Best PG Name";
                if (IncludeNonFilteredResults)
                {
                    writer.WriteLine(header);
                }
                filteredwriter.WriteLine(header);

                foreach (CsvFile csvfile in files)
                {
                    // Open up the csvfile and read it's contents, skipping the header
                    using (CsvReader reader = new CsvReader(new StreamReader(csvfile.FilePath), true))
                    {
                        int deflineIndex = reader.GetFieldIndex("Defline");
                        int startRes = reader.GetFieldIndex("Start");
                        int stopRes = reader.GetFieldIndex("Stop");
                        Peptide pep = null;
                        // Read in each psm
                        while (reader.ReadNextRecord())
                        {
                            string seq = reader["Peptide"].ToUpper();
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
                                    
                                    if (FilterQuantInterference)
                                    {
                                        double interference = 0;
                                        if (double.TryParse(reader["Interference"], out interference))
                                        {
                                            if (interference >= QuantInterferenceCutoff)
                                            {
                                                usePeptideInQuant = false;
                                            }
                                        }
                                    }

                                    if (usePeptideInQuant)
                                    {
                                        Quantitation quant = null;
                                        double[] quantData = new double[4 * (int)exp.QuantPlex + 1];
                                        bool keepPeptide = true;
                                        
                                        // Read in all the quant data from the csvfile
                                        int j = 0;
                                        for (int i = exp.TQStart; i < exp.TQStop; i++)
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
                                        if (keepPeptide)
                                        {
                                            // append/add the quant data to the best and only protein group
                                            if (pep.BestPG.Quantitation.TryGetValue(experimentID, out quant))
                                            {
                                                quant.AddData(quantData);
                                            }
                                            else
                                            {
                                                quant = new Quantitation(exp.QuantPlex, quantData);
                                                pep.BestPG.Quantitation.Add(experimentID, quant);
                                            }

                                            // Keep track of unique quantified peptides
                                            quant.Peptides.Add(pep);
                                        }
                                    }
                                }

                                StringBuilder sb = new StringBuilder();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    data[i] = reader[i];
                                    if (i == deflineIndex)
                                    {
                                        data[i] = pep.BestPG.RepresentativeProtein.Description;
                                    }
                                    else if (i == startRes)
                                    {
                                        data[i] = pep.BestPG.RepresentativeProtein.FindStartResidue(pep.LeucineSequence).ToString();
                                    }
                                    else if (i == stopRes)
                                    {
                                        data[i] = pep.BestPG.RepresentativeProtein.FindStopResidue(pep.LeucineSequence).ToString();
                                    }
                                    data[i] = "\"" + data[i] + "\"";
                                    sb.Append(data[i]);
                                    sb.Append(',');
                                }
                                sb.Append(experimentID);
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
            if (Parsimony_Peptides != null)
            {
                Parsimony_Peptides.Clear();
                Parsimony_Peptides = null;
            }
            if (Parismony_Peptides_Filtered != null)
            {
                Parismony_Peptides_Filtered.Clear();
                Parismony_Peptides_Filtered = null;
            }           
            TimeSpan diff = DateTime.Now - startTime;
            Log("Finished [{0:D2} hrs, {1:D2} mins, {2:D2} secs]", diff.Hours, diff.Minutes, diff.Seconds);
            ProgressUpdate(-1);
        }

        #region Callbacks

        public event EventHandler<StatusEventArgs> UpdateLog;

        protected virtual void onUpdateLog(StatusEventArgs e)
        {
            if (UpdateLog != null)
            {
                UpdateLog(this, e);
            }
        }

        public void Log(string message)
        {
            onUpdateLog(new StatusEventArgs(message));
        }

        public void Log(string format, params object[] args)
        {
            onUpdateLog(new StatusEventArgs(string.Format(format, args)));                
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

        #endregion Callbacks
    }
}