using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using CSMSL.Analysis.Identification;
using System.Collections;

namespace Coon.Compass.ProteinHoarder
{
    public class ProteinGroup : IEnumerable<Protein>, IEquatable<ProteinGroup>, IComparer<ProteinGroup>, IComparable<ProteinGroup>, IFalseDiscovery<double>
    {
        public static int GroupNumber = 1;

        public int LongestProteinLen
        {
            get
            {
                return RepresentativeProtein == null ? 0 : RepresentativeProtein.Length;
            }
        }

        public double SequenceCoverage
        {
            get
            {                
                return RepresentativeProtein.CalculateSequenceCoverage(Peptides);
            }
        }

        public double SequenceRedundacy { get { return RepresentativeProtein.CalculateSequenceRedundancy(Peptides); } }

        public string Description { get { return RepresentativeProtein.Description; } }

        public Protein RepresentativeProtein { get; private set; }

        public string Name { get; private set; }

        public Dictionary<string, Quantitation> Quantitation;

        public bool PassesFDR = false;

        public int Count
        {
            get
            {
                return _proteins.Count;
            }
        }

        private double _pScore = double.NaN;

        public double PScore
        {
            get
            {
                // Force a p-score update if one is not given already
                if (double.IsNaN(_pScore))
                {
                    CalculatePScore(ProteinHoarder.PScoreCalculationMethod, ProteinHoarder.UseConservativePScore);
                }
                return _pScore;
            }
        }

        private int _uniquePeptides = -1;

        public int UniquePeptides
        {
            get
            {
                if (_uniquePeptides < 0)
                {
                    _uniquePeptides = NumberofUniquePeptides();
                }
                return _uniquePeptides;
            }
        }

        public Dictionary<ExperimentGroup, int> PsmsPerGroup;
        public Dictionary<ExperimentGroup, HashSet<Peptide>> UniquePeptidesPerGroup;

        private readonly List<Protein> _proteins;

        private HashSet<Peptide> _peptides;
        public HashSet<Peptide> Peptides
        {
            get
            {
                return _peptides;
            }
            set
            {
                _peptides = value;
                _pScore = double.NaN;
                _uniquePeptides = -1;
            }
        }

        public Protein this[int index]
        {
            get
            {
                return _proteins[index];
            }
        }

        public HashSet<string> ProteinIDs { get; private set; }

        public HashSet<string> GeneNames { get; private set; }

        public ProteinGroup(Protein protein, HashSet<Peptide> peptides)
        {
            Name = "PG" + GroupNumber;
            GroupNumber++;
            Quantitation = new Dictionary<string, Quantitation>();
            _proteins = new List<Protein>(5);
            ProteinIDs = new HashSet<string>();
            GeneNames = new HashSet<string>();
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
            if (prot.IsDecoy) _decoy = true;

            //
            if (prot.Length >= LongestProteinLen)
            {
                RepresentativeProtein = prot;
            }

            _hCode ^= prot.GetHashCode();

            // Add the protein ids
            if(!string.IsNullOrEmpty(prot.ProteinID))
                ProteinIDs.Add(prot.ProteinID);

            // Add the gene names
            if (!string.IsNullOrEmpty(prot.GeneName))
                GeneNames.Add(prot.GeneName);

            // Add the protein to the internal list
            _proteins.Add(prot);
        }
        
        public void UpdatePValue(PScoreCalculateionMethod method, bool useConservativeScore)
        {
            _pScore = CalculatePScore(method, useConservativeScore);
        }

        private double CalculatePScore(PScoreCalculateionMethod method, bool useConservativeScore)
        {
            double score = 1;
            foreach (Peptide pep in Peptides)
            {
                switch (method)
                {
                    // Use all peptides, whether or not they are shared between groups
                    default:
                    case PScoreCalculateionMethod.UseAllPeptides:
                        if (useConservativeScore)
                        {
                            score *= pep.PSMs.LowestPvalue;
                        }
                        else
                        {
                            score *= pep.PSMs.CumulativePValue;
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
            return score;
        }

        public override string ToString()
        {
            return string.Format("{0} (p-value = {1:G3}", Name, PScore);
        }

        private int NumberofUniquePeptides()
        {
            return Peptides.Sum(pep => pep.PSMs.Count);
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

        public bool TryGetLog2Ratio(ExperimentGroup exp, out double log2, bool useOnlyCompleteSets = false)
        {
            log2 = 0;
            Quantitation quant;
            if (Quantitation.TryGetValue(exp.Name, out quant))
            {
                double[] data;
                log2 = quant.GetLog2Ratio(useOnlyCompleteSets, out data);
                return true;
            }           
            return false;
        }

        public string ToParsimonyProteins(ExperimentGroup exp, bool duplexQuant = false, bool useOnlyCompleteSets = false)
        {
            StringBuilder sb = new StringBuilder(512);
            sb.AppendFormat("{0},{1},{2},{3:G4},{4:G4},{5:G4},{6},{7},{8},{9},", Name, Description, LongestProteinLen, SequenceCoverage, RepresentativeProtein.CalculateSequenceCoverage(Peptides.Where(pep => !pep.IsShared)), SequenceRedundacy, Count, UniquePeptides, Peptides.Count, Peptides.Count(pep => pep.IsShared));
            if (exp != null)
            {
                int psms;
                HashSet<Peptide> uniquepeps;
                UniquePeptidesPerGroup.TryGetValue(exp, out uniquepeps);
                PsmsPerGroup.TryGetValue(exp, out psms);
                sb.Append(psms);
                sb.Append(',');
                if (uniquepeps != null) sb.Append(uniquepeps.Count);
                sb.Append(',');
            }
            sb.Append(PScore.ToString(CultureInfo.InvariantCulture));
            if (exp != null && exp.UseQuant)
            {
                Quantitation quant;
                if (Quantitation.TryGetValue(exp.Name, out quant))
                {
                    sb.Append(',');
                    sb.Append(quant.PSMs);
                    sb.Append(',');
                    sb.Append(quant.UniquePeptides.Count);
                    sb.Append(',');
                    sb.Append(quant.UniquePeptides.Count(pep => pep.IsShared));
                    sb.Append(',');
                    sb.Append(quant.ToOutput(duplexQuant, exp.MeidanLog2Ratio, useOnlyCompleteSets));
                }
                else
                {
                    sb.Append(",0,0");
                    for (int i = 0; i < exp.QuantPlex * 4; i++)
                    {
                        sb.Append(",-");
                    }

                    if (duplexQuant)
                    {
                        sb.Append(",N/A,N/A");
                    }
                }
            }

            if (ProteinHoarder.AnnotationType != AnnotationType.None)
            {
                sb.Append(',');
                sb.Append(ProteinIdsString());
                sb.Append(',');
                sb.Append(GeneNamesString());
            }

            return sb.ToString();
        }

        public string ProteinIdsString()
        {
            StringBuilder sb = new StringBuilder();
            bool inId = false;
            foreach (string proteinID in ProteinIDs)
            {
                inId = true;
                sb.Append(proteinID);
                sb.Append('|');
            }
            if (inId)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public string GeneNamesString()
        {
            StringBuilder sb = new StringBuilder();
            bool inId = false;
            foreach (string genename in GeneNames)
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
                    sb.AppendLine(PScore.ToString(CultureInfo.InvariantCulture));
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
            }
            return sb.ToString();
        }

        private string GetPeptideString()
        {
            StringBuilder sb = new StringBuilder();
            return sb.ToString();
        }

        private string GetProteinStrings()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Protein prot in _proteins)
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
            return _proteins.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _proteins.GetEnumerator();
        }

        private int _hCode = 1;

        public override int GetHashCode()
        {
            return _hCode;         
        }

        public bool Equals(ProteinGroup other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Count != other.Count) return false;
            return _proteins.All(prot => other._proteins.Contains(prot));
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
            // If y is null x is greater, otherwise compare their pscores
            return (pg2 == null) ? direction : direction * pg1.PScore.CompareTo(pg2.PScore);
        }

        public int CompareTo(ProteinGroup other)
        {
            return Compare(this, other);
        }

        private bool _decoy;

        public bool IsDecoy
        {
            get { return _decoy; }
        }

        public double FdrScoreMetric
        {
            get { return PScore; }
        }
    }
}