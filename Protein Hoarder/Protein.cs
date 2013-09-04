using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Compass.ProteinHoarder
{
    public class Protein : IEquatable<Protein>
    {
        public static Regex _uniprotID = new Regex(@"\|([\w-]+)\|", RegexOptions.Compiled);
        public static Regex _genenameID = new Regex(@"GN=([\w-]+)", RegexOptions.Compiled);

        private string description;

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value.Replace(',', ';');
                IsDecoy = description.StartsWith("DECOY");
                Match m = _uniprotID.Match(description);
                if (m.Success)
                {
                    UniprotID = m.Groups[1].Value;
                }
                else
                {
                    UniprotID = "";
                }
                m = _genenameID.Match(description);
                if (m.Success)
                {
                    GeneName = m.Groups[1].Value;
                }
                else
                {
                    GeneName = "";
                }                
            }
        }
        public string GeneName;

        public string UniprotID;

        private string sequence;

        public string Sequence
        {
            get
            {
                return sequence;
            }
            set
            {
                sequence = value;
                leucineSequence = sequence.Replace('I', 'L');
            }
        }

        private string leucineSequence;

        public string LeucineSequence
        {
            get
            {
                return leucineSequence;
            }
        }

        public int Length
        {
            get { return Sequence.Length; }
        }

        public bool IsDecoy { get; private set; }

        public HashSet<Peptide> Peptides;

        public Protein(string description, string sequence)
        {
            Description = description;
            Sequence = sequence;
            Peptides = new HashSet<Peptide>();
        }

        public void AddPeptide(Peptide pep)
        {
            Peptides.Add(pep);
        }

        public double CalculateSequenceCoverage(HashSet<Peptide> peptides)
        {
            BitArray bit_array = new BitArray(Length, false);

            foreach (Peptide pep in peptides)
            {
                int start_index = 0;
                while (true)
                {
                    int index = LeucineSequence.IndexOf(pep.LeucineSequence, start_index);
                    start_index = index + 1;
                    if (index < 0)
                    {
                        break;
                    }

                    for (int aa = index; aa < index + pep.Length; aa++)
                    {
                        bit_array[aa] = true;
                    }
                }
            }

            int observedAminoAcids = 0;
            foreach (bool bit in bit_array)
            {
                if (bit)
                {
                    observedAminoAcids++;
                }
            }

            return (double)observedAminoAcids / bit_array.Length * 100.0;
        }

        public double CalculateSequenceRedundancy(HashSet<Peptide> peptides)
        {
            int[] amino_acid_redundancy = new int[Sequence.Length];

            foreach (Peptide pep in peptides)
            {
                int start_index = 0;
                while (true)
                {
                    int index = LeucineSequence.IndexOf(pep.LeucineSequence, start_index);
                    start_index = index + 1;
                    if (index < 0)
                    {
                        break;
                    }

                    for (int aa = index; aa < index + pep.Length; aa++)
                    {
                        amino_acid_redundancy[aa]++;
                    }
                }
            }

            int redundancy = 0;
            foreach (int count in amino_acid_redundancy)
            {
                redundancy += count;
            }

            return (double)redundancy / amino_acid_redundancy.Length;
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode();
        }

        public bool Equals(Protein other)
        {
            return Sequence.Equals(other.Sequence);
        }

        public bool TryGetUniprotID(out string uniprotID)
        {
            Match m = _uniprotID.Match(this.Description);
            uniprotID = (m.Success) ? m.Groups[1].Value : string.Empty;
            return m.Success;
        }

        public bool TryGetGeneName(out string genename)
        {
            Match m = _genenameID.Match(this.Description);
            genename = (m.Success) ? m.Groups[1].Value : string.Empty;
            return m.Success;
        }

        public override string ToString()
        {
            return Description;
        }

        public int FindStartResidue(string sequence)
        {
            return LeucineSequence.IndexOf(sequence) + 1;
        }

        public int FindStopResidue(string sequence)
        {
            return LeucineSequence.IndexOf(sequence) + sequence.Length;
        }
    }
}