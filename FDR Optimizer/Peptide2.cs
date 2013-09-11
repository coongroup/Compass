using System;
using System.Collections.Generic;
using System.Text;
using CSMSL.Proteomics;

namespace Coon.Compass.FdrOptimizer
{
    public class Peptide2
    {
        private List<AminoAcid> aminoAcidSequence;

        public List<AminoAcid> AminoAcidSequence
        {
            get { return aminoAcidSequence; }
        }

        public string Sequence { get; set; }
       
        //private double mass = 18.0105646942;  // H2O

        public double Mass { get; set; }      

        private static readonly AminoAcidDictionary AMINO_ACIDS = new AminoAcidDictionary();

        private static ModificationDictionary MODIFICATIONS;

        public static void SetModifications(ModificationDictionary modifications)
        {
            MODIFICATIONS = modifications;
        }

        public Peptide2(string sequence, IEnumerable<Modification2> fixedModifications, string dynamicModifications)
        {
            aminoAcidSequence = new List<AminoAcid>(sequence.Length);
            StringBuilder sequence_sb = new StringBuilder(sequence.Length);
            Sequence = sequence.ToUpper();
            //Mass = Coon.AminoAcidPolymer.GetMonoisotopicMass(Sequence);
            double mass = AminoAcidPolymer.GetMass(Sequence);
            //foreach(char c in sequence.ToUpper())
            //{
            //    if(AMINO_ACIDS.ContainsKey(c))
            //    {
            //        aminoAcidSequence.Add(AMINO_ACIDS[c]);
            //        sequence_sb.Append(c);
            //        mass += AMINO_ACIDS[c].Mass;
            //    }
            //}
            //this.sequence = sequence_sb.ToString();

            // fixed modifications
            foreach(Modification2 modification in fixedModifications)
            {
                switch(modification.ModificationType)
                {
                    case ModificationType.AminoAcidResidue:
                        foreach(char amino_acid_residue in modification.AminoAcidResidues)
                        {
                            foreach(char c in Sequence)
                            {
                                if(c == amino_acid_residue)
                                {
                                    mass += modification.MonoisotopicMassShift;
                                }
                            }
                        }
                        break;
                    case ModificationType.ProteinNTerminus:
                        throw new NotSupportedException("Fixed modifications of protein N-terminii are not currently supported.");
                    case ModificationType.ProteinNTerminusAminoAcidResidue:
                        throw new NotSupportedException("Fixed modifications of protein N-terminii at specific amino acid residues are not currently supported.");
                    case ModificationType.ProteinCTerminus:
                        throw new NotSupportedException("Fixed modifications of protein C-terminii are not currently supported.");
                    case ModificationType.ProteinCTerminusAminoAcidResidue:
                        throw new NotSupportedException("Fixed modifications of protein C-terminii at specific amino acid residues are not currently supported.");
                    case ModificationType.PeptideNTerminus:
                        mass += modification.MonoisotopicMassShift;
                        break;
                    case ModificationType.PeptideNTerminusAminoAcidResidue:
                        throw new NotSupportedException("Fixed modifications of peptide N-terminii at specific amino acid residues are not currently supported.");
                    case ModificationType.PeptideCTerminus:
                        mass += modification.MonoisotopicMassShift;
                        break;
                    case ModificationType.PeptideCTerminusAminoAcidResidue:
                        foreach (char amino_acid_residue in modification.AminoAcidResidues)
                        {
                            if (sequence.ToUpper()[sequence.Length - 1] == amino_acid_residue)
                            {
                                mass += modification.MonoisotopicMassShift;
                            }
                        }
                        break;
                        //throw new NotSupportedException("Fixed modifications of peptide C-terminii at specific amino acid residues are not currently supported.");
                }
            }

            // dynamic modifications
            string[] dynamic_modifications = dynamicModifications.Replace("\"", string.Empty).Split(',');
            foreach(string dynamic_modification in dynamic_modifications)
            {
                int index = dynamic_modification.IndexOf(':');
                if(index >= 0)
                {
                    string modification_name = dynamic_modification.Substring(0, index);
                    if(dynamic_modification.Substring(2).StartsWith("substitution for"))
                    {
                        char c = dynamic_modification[0];
                        if(AMINO_ACIDS.ContainsKey(c))
                        {
                            mass += AMINO_ACIDS[c].Mass;
                        }
                        else
                        {
                            throw new Exception("Unknown amino acid '" + c + "'");
                        }
                    }
                    else
                    {
                        if(!MODIFICATIONS.ContainsKey(modification_name))
                        {
                            throw new Exception("Mass of modification \"" + modification_name + "\" not found");
                        }
                        else
                        {
                            mass += MODIFICATIONS[modification_name].MonoisotopicMassShift;
                        }
                    }
                }
                else if(dynamic_modification.Contains("mutation"))
                {
                    char c1 = dynamic_modification[0];
                    char c2 = dynamic_modification[5];
                    if(AMINO_ACIDS.ContainsKey(c1) && AMINO_ACIDS.ContainsKey(c2))
                    {
                        mass += AMINO_ACIDS[c2].Mass - AMINO_ACIDS[c1].Mass;
                    }
                }
            }
            Mass = mass;
        }

        public override string ToString()
        {
            return Sequence;
        }
    }
}
