using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Coon.Compass.ProteinHoarder
{
    public class Protein : IEquatable<Protein>
    {
        public static Regex _uniprotID = new Regex(@"\|([\w-]+)\|", RegexOptions.Compiled);
        public static Regex _genenameID = new Regex(@"GN=([\w-]+)", RegexOptions.Compiled);

        public static Regex UniProtRegex = new Regex(@"\|([\w-]+)\|.+GN=([\w-]+)", RegexOptions.Compiled);
        public static Regex SGDRegex = new Regex(@"(?:DECOY_)?([\w-]+) ([\w-]+)", RegexOptions.Compiled);

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
            }
        }

        public string GeneName { get; private set; }

        public string ProteinID { get; private set; }

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

            Match m = null;

            if (ProteinHoarder.AnnotationType == AnnotationType.SGD)
            {
                m = SGDRegex.Match(description);
            } else if (ProteinHoarder.AnnotationType == AnnotationType.UniProt)
            {
                m = UniProtRegex.Match(description);
            }
            if (m != null && m.Success)
            {
                ProteinID = m.Groups[1].Value;
                GeneName = m.Groups[2].Value;
            }
            else
            {
                ProteinID = string.Empty;
                GeneName = string.Empty;
            }

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