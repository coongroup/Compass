using System;
using System.Collections.Generic;
using System.Text;
using CSMSL.Analysis.Identification;

namespace Protein_Hoarder
{
    public class ProteinGroup : IEnumerable<Protein>, IEquatable<ProteinGroup>, IComparer<ProteinGroup>, IComparable<ProteinGroup>, IFalseDiscovery<double>
    {
        public static int GroupNumber = 1;

        public int LongestProteinLen
        {
            get
            {
                if (RepresentativeProtein == null)
                {
                    return 0;
                }
                else
                {
                    return RepresentativeProtein.Length;
                }
            }
        }

        public HashSet<string> GetUniprotIds()
        {
            HashSet<string> ids = new HashSet<string>();
            foreach (Protein protein in proteins)
            {
                if (!string.IsNullOrWhiteSpace(protein.UniprotID))
                    ids.Add(protein.UniprotID);
            }
            return ids;
        }

        public HashSet<string> GetGeneNames()
        {
            HashSet<string> ids = new HashSet<string>();
            foreach (Protein protein in proteins)
            {
                if (!string.IsNullOrWhiteSpace(protein.GeneName))
                    ids.Add(protein.GeneName);
            }
            return ids;
        }

        public double SequenceCoverage { get { return RepresentativeProtein.CalculateSequenceCoverage(Peptides); } }

        public double SequenceRedundacy { get { return RepresentativeProtein.CalculateSequenceRedundancy(Peptides); } }

        public string Description { get { return RepresentativeProtein.Description; } }

        public Protein RepresentativeProtein { get; private set; }

        public string Name { get; private set; }

        public Dictionary<char, Quantitation> Quantitation;

        public bool PassesFDR = false;

        public int Count
        {
            get
            {
                return proteins.Count;
            }
        }

        private double pScore = double.NaN;

        public double PScore
        {
            get
            {
                // Force a p-score update if one is not given already
                if (double.IsNaN(pScore))
                {
                    CalculatePScore(ProteinHoarder.PScoreCalculationMethod, ProteinHoarder.UseConservativePScore);
                }
                return pScore;
            }
        }

        public int uniquePeptides = -1;

        public int UniquePeptides
        {
            get
            {
                if (uniquePeptides < 0)
                {
                    uniquePeptides = NumberofUniquePeptides();
                }
                return uniquePeptides;
            }
        }

        public Dictionary<ExperimentGroup, int> PsmsPerGroup;
        public Dictionary<ExperimentGroup, HashSet<Peptide>> UniquePeptidesPerGroup;

        private List<Protein> proteins;

        private HashSet<Peptide> peptides;

        public HashSet<Peptide> Peptides
        {
            get
            {
                return peptides;
            }
            set
            {
                peptides = value;
                pScore = double.NaN;
                uniquePeptides = -1;
            }
        }

        public Protein this[int index]
        {
            get
            {
                return proteins[index];
            }
        }

        public ProteinGroup(Protein protein, HashSet<Peptide> peptides)
        {
            Name = "PG" + GroupNumber;
            GroupNumber++;
            Quantitation = new Dictionary<char, Quantitation>();
            proteins = new List<Protein>(5);
            Add(protein);
            Peptides = peptides;
            foreach (Peptide pep in peptides)
            {
                pep.AddProteinGroup(this);
            }
            PsmsPerGroup = new Dictionary<ExperimentGroup, int>();
            UniquePeptidesPerGroup = new Dictionary<ExperimentGroup, HashSet<Peptide>>();
        }

        // Add a protein to this protein group because you cannot tell the proteins apart
        public void Add(Protein prot)
        {
            // A protein group is a decoy group if any of its proteins are decoy
            if (prot.IsDecoy) decoy = true;

            //
            if (prot.Length >= LongestProteinLen)
            {
                RepresentativeProtein = prot;
            }

            // Add the protein to the internal list
            proteins.Add(prot);
        }

        public void UpdatePValue(PScoreCalculateionMethod method, bool useConservativeScore)
        {
            CalculatePScore(method, useConservativeScore);
        }

        private double CalculatePScore(PScoreCalculateionMethod method, bool useConservativeScore)
        {
            pScore = 1;
            foreach (Peptide pep in Peptides)
            {
                switch (method)
                {
                    // Use all peptides, whether or not they are shared between groups
                    default:
                    case PScoreCalculateionMethod.UseAllPeptides:
                        if (useConservativeScore)
                        {
                            pScore *= pep.PSMs.LowestPvalue;
                        }
                        else
                        {
                            pScore *= pep.PSMs.CumulativePValue;
                        }
                        break;

                    case PScoreCalculateionMethod.UseUnsharedPeptides:
                        if (!pep.IsShared)
                        {
                            goto case PScoreCalculateionMethod.UseAllPeptides;
                        }
                        break;
                }
            }
            return pScore;
        }

        public override string ToString()
        {
            return string.Format("{0} (p-value = {1:G3}", Name, PScore);
        }

        private int NumberofUniquePeptides()
        {
            int count = 0;
            foreach (Peptide pep in Peptides)
            {
                count += pep.PSMs.Count;
            }
            return count;
        }

        public static string HeaderLine(OutputFileType type)
        {
            switch (type)
            {
                case OutputFileType.Parsimony_Proteins:
                    return "Protein Group Name,Decoy,Representative Protein Description,Total Amino Acids,Sequence Coverage (%),Sequence Redundacy,Numbers of Proteins,Number of Unique Proteins,Number of PSMs,Number of Unique Peptides,P-Score";
                case OutputFileType.Parsimony_Protein_Groups:
                    return "Protein Group Name,Representative Protein Description,Number of Proteins,Number of Unique Proteins,Number of PSMs,Number of Unique Peptides,P-Score\n,Protein Description,Protein Sequence,Total Amino Acids,Observed Amino Acids,Sequence Coverage (%),Sequence Redundancy (X)";
                default:
                    return String.Empty;
            }
        }

        public string ToParsimonyProteins(ExperimentGroup exp, int quantLen = 0)
        {
            StringBuilder sb = new StringBuilder(512);
            sb.AppendFormat("{0},{1},{2},{3:G4},{4},{5},{6},", Name, Description, LongestProteinLen, SequenceCoverage, Count, UniquePeptides, Peptides.Count);
            //sb.Append(Name);
            //sb.Append(',');
            //sb.Append(IsDecoy);
            //sb.Append(',');
            //sb.Append("\"" + Description + "\"");
            //sb.Append(',');
            //sb.Append(LongestProteinLen);
            //sb.Append(',');
            //sb.Append(SequenceCoverage.ToString("g4"));
            //sb.Append(',');
            //sb.Append(SequenceRedundacy.ToString("g4"));
            //sb.Append(',');
            //sb.Append(Count);
            //sb.Append(',');
            //sb.Append(Count); // Number of unique proteins... need to figure this one out at some point
            //sb.Append(',');
            //sb.Append(UniquePeptides);
            //sb.Append(',');
            //sb.Append(Peptides.Count);
            //sb.Append(',');
            if (exp != null)
            {
                int psms = 0;
                HashSet<Peptide> uniquepeps = new HashSet<Peptide>();
                UniquePeptidesPerGroup.TryGetValue(exp, out uniquepeps);
                PsmsPerGroup.TryGetValue(exp, out psms);
                sb.Append(psms);
                sb.Append(',');
                sb.Append(uniquepeps.Count);
                sb.Append(',');
            }
            sb.Append(PScore.ToString());
            if (exp.UseQuant)
            {
                Quantitation quant = null;
                if (Quantitation.TryGetValue(exp.ExperimentalID, out quant))
                {
                    sb.Append(',');
                    sb.Append(quant.PSMs);
                    sb.Append(',');
                    sb.Append(quant.Peptides.Count);
                    sb.Append(',');
                    sb.Append(quant.ToOutput());
                }
                else
                {
                    sb.Append(",0,0");
                    for (int i = 0; i < exp.QuantPlex * 4 + 1; i++)
                    {
                        sb.Append(",-");
                    }
                }
            }

            bool inId = false;
            sb.Append(",");
            HashSet<string> uniprotIDs = GetUniprotIds();
            HashSet<string> genenames = GetGeneNames();

            foreach (string uniprotID in uniprotIDs)
            {
                inId = true;
                sb.Append(uniprotID);
                sb.Append('|');
            }
            if (inId)
                sb.Remove(sb.Length - 1, 1);
            sb.Append(',');
            inId = false;
            foreach (string genename in genenames)
            {
                inId = true;
                sb.Append(genename);
                sb.Append('|');
            }
            if (inId)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public string ToOutputLine(OutputFileType type)
        {
            StringBuilder sb = new StringBuilder(512);
            switch (type)
            {
                case OutputFileType.Parsimony_Proteins:
                    sb.Append(Name);
                    sb.Append(',');
                    sb.Append(IsDecoy);
                    sb.Append(',');
                    sb.Append("\"" + Description + "\"");
                    sb.Append(',');
                    sb.Append(LongestProteinLen);
                    sb.Append(',');
                    sb.Append(SequenceCoverage.ToString("g4"));
                    sb.Append(',');
                    sb.Append(SequenceRedundacy.ToString("g4"));
                    sb.Append(',');
                    sb.Append(Count);
                    sb.Append(',');
                    sb.Append(Count); // Number of unique proteins... need to figure this one out at some point
                    sb.Append(',');
                    sb.Append(UniquePeptides);
                    sb.Append(',');
                    sb.Append(Peptides.Count);
                    sb.Append(',');
                    sb.AppendLine(PScore.ToString());
                    sb.Append(',');
                    break;

                case OutputFileType.Parsimony_Protein_Groups:
                    sb.Append(Name);
                    sb.Append(',');
                    sb.Append("\"" + Description + "\"");
                    sb.Append(',');
                    sb.Append(Count);
                    sb.Append(',');
                    sb.Append(Count);
                    sb.Append(',');
                    sb.Append(UniquePeptides);
                    sb.Append(',');
                    sb.Append(Peptides.Count);
                    sb.Append(',');
                    sb.Append(PScore);
                    sb.AppendLine();
                    sb.Append(GetProteinStrings());
                    sb.Append(GetPeptideString());
                    break;
                default:
                    break;
            }
            return sb.ToString();
        }

        private string GetPeptideString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Peptide pep in Peptides)
            {
                string defline = pep.ProteinGroups[0].Description;
                //foreach (OmssaLine psm in pep.PSMs)
                //{
                //    psm.Defline = defline;
                //    sb.Append(",,");
                //    sb.Append(psm.ToString());
                //    sb.Append(',');
                //    sb.Append(pep.ProteinGroups.Count);
                //    sb.Append(',');
                //    sb.AppendLine(pep.ProteinGroups.Count.ToString());
                //    psm.Defline = string.Empty;
                //}
            }
            return sb.ToString();
        }

        private string GetProteinStrings()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Protein prot in proteins)
            {
                sb.Append(',');
                sb.Append("\"" + prot.Description + "\"");
                sb.Append(',');
                sb.Append(prot.Sequence);
                sb.Append(',');
                sb.Append(prot.Length);
                sb.Append(',');
                double seqcov = prot.CalculateSequenceCoverage(Peptides);
                sb.Append((int)(seqcov * prot.Length) / 100);
                sb.Append(',');
                sb.Append(seqcov.ToString("g4"));
                sb.Append(',');
                sb.AppendLine(prot.CalculateSequenceRedundancy(Peptides).ToString("g4"));
            }
            return sb.ToString();
        }

        public IEnumerator<Protein> GetEnumerator()
        {
            return proteins.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return proteins.GetEnumerator();
        }

        public override int GetHashCode()
        {
            int hCode = 1;
            for (int i = 0; i < Count; i++)
            {
                hCode ^= proteins[i].GetHashCode();
            }
            return hCode;
        }

        public bool Equals(ProteinGroup other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;
            if (Count != other.Count) return false;
            foreach (Protein prot in proteins)
            {
                if (!other.proteins.Contains(prot)) return false;
            }
            return true;
        }

        public int Compare(ProteinGroup pg1, ProteinGroup pg2)
        {
            return CompareIncreasing(pg1, pg2);
        }

        public static int CompareDecreasing(ProteinGroup pg1, ProteinGroup pg2)
        {
            return Compare(pg1, pg2, -1);
        }

        public static int CompareIncreasing(ProteinGroup pg1, ProteinGroup pg2)
        {
            return Compare(pg1, pg2, 1);
        }

        public static int Compare(ProteinGroup pg1, ProteinGroup pg2, int direction)
        {
            direction = Math.Sign(direction);
            if (pg1 == null)
            {
                // If both x and y are null they are equal, otherwise y is greater
                return (pg2 == null) ? 0 : -direction;
            }
            else
            {
                // If y is null x is greater, otherwise compare their pscores
                return (pg2 == null) ? direction : direction * pg1.PScore.CompareTo(pg2.PScore);
            }
        }

        public int CompareTo(ProteinGroup other)
        {
            return Compare(this, other);
        }

        private bool decoy = false;

        public bool IsDecoy
        {
            get { return decoy; }
        }

        public double FDRScoreMetric
        {
            get { return PScore; }
        }
    }
}