using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Phosphinator
{
    public class Peptide
    {
        private List<AminoAcid> aminoAcidSequence;

        public List<AminoAcid> AminoAcidSequence
        {
            get { return aminoAcidSequence; }
        }

        public string Sequence
        {
            get
            {
                StringBuilder sequence_sb = new StringBuilder(aminoAcidSequence.Count);

                for(int i = 0; i < aminoAcidSequence.Count; i++)
                {
                    if(dynamicModifications.ContainsKey(i + 1) && dynamicModifications[i + 1].Contains("phosphorylation"))
                    {
                        sequence_sb.Append(char.ToLower(aminoAcidSequence[i].Abbreviation));
                    }
                    else
                    {
                        sequence_sb.Append(aminoAcidSequence[i].Abbreviation);
                    }
                }

                return sequence_sb.ToString();
            }
        }

        private static readonly AminoAcidDictionary AMINO_ACIDS = new AminoAcidDictionary();

        public static ModificationDictionary MODIFICATIONS;

        public static void SetModifications(ModificationDictionary modifications)
        {
            MODIFICATIONS = modifications;
        }

        private Dictionary<int, string> dynamicModifications;

        public Dictionary<int, string> DynamicModifications
        {
            get { return dynamicModifications; }
            set { dynamicModifications = value; }
        }

        private double mass = 18.0105646942;  // H2O

        public double Mass
        {
            get { return mass; }
        }

        private static readonly IonCapDictionary ION_CAPS = new IonCapDictionary();

        private double[] massShifts;

        private double[] cumulativeNTerminalMass;
        private double[] cumulativeCTerminalMass;

        public Peptide(string sequence, IEnumerable<Modification> fixedModifications, string dynamicModifications)
        {
            aminoAcidSequence = new List<AminoAcid>(sequence.Length);
            foreach(char c in sequence.ToUpper())
            {
                if(AMINO_ACIDS.ContainsKey(c))
                {
                    aminoAcidSequence.Add(AMINO_ACIDS[c]);
                    mass += AMINO_ACIDS[c].Mass;
                }
            }

            massShifts = new double[aminoAcidSequence.Count];

            // fixed modifications
            foreach(Modification modification in fixedModifications)
            {
                switch(modification.ModificationType)
                {
                    case ModificationType.AminoAcidResidue:
                        foreach(char amino_acid_residue in modification.AminoAcidResidues)
                        {
                            string upper_sequence = sequence.ToUpper();
                            for(int i = 0; i < upper_sequence.Length; i++)
                            {
                                if(upper_sequence[i] == amino_acid_residue)
                                {
                                    mass += modification.MonoisotopicMassShift;
                                    massShifts[i] += modification.MonoisotopicMassShift;
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
                        massShifts[0] += modification.MonoisotopicMassShift;
                        break;
                    case ModificationType.PeptideNTerminusAminoAcidResidue:
                        throw new NotSupportedException("Fixed modifications of peptide N-terminii at specific amino acid residues are not currently supported.");
                    case ModificationType.PeptideCTerminus:
                        mass += modification.MonoisotopicMassShift;
                        massShifts[aminoAcidSequence.Count - 1] += modification.MonoisotopicMassShift;
                        break;
                    case ModificationType.PeptideCTerminusAminoAcidResidue:
                        throw new NotSupportedException("Fixed modifications of peptide C-terminii at specific amino acid residues are not currently supported.");
                }
            }

            // dynamic modifications
            string[] dynamic_modifications = dynamicModifications.Trim('"').Split(new string[] { " ,", " ;"}, StringSplitOptions.RemoveEmptyEntries);
            this.dynamicModifications = new Dictionary<int, string>(dynamic_modifications.Length);
            foreach(string dynamic_modification in dynamic_modifications)
            {
                int index = dynamic_modification.IndexOf(':');
                string modification_name = dynamic_modification.Substring(0, index);
                if(!MODIFICATIONS.ContainsKey(modification_name))
                {
                    throw new Exception("Mass of modification \"" + modification_name + "\" not found");
                }
                int residue_number = int.Parse(dynamic_modification.Substring(index + 1));
                if(!this.dynamicModifications.ContainsKey(residue_number))
                {
                    this.dynamicModifications.Add(residue_number, dynamic_modification);
                }

                mass += MODIFICATIONS[modification_name].MonoisotopicMassShift;
                massShifts[residue_number - 1] += MODIFICATIONS[modification_name].MonoisotopicMassShift;
            }

            InitializeCumulativeMassArrays();
        }

        public override string ToString()
        {
            return Sequence;
        }

        private void InitializeCumulativeMassArrays()
        {
            cumulativeNTerminalMass = new double[aminoAcidSequence.Count + 1];
            cumulativeNTerminalMass[0] = 0.0;
            for(int r = 1; r <= aminoAcidSequence.Count; r++)
            {
                cumulativeNTerminalMass[r] = cumulativeNTerminalMass[r - 1] + aminoAcidSequence[r - 1].Mass + massShifts[r - 1];
            }

            cumulativeCTerminalMass = new double[aminoAcidSequence.Count + 1];
            cumulativeCTerminalMass[0] = 0.0;
            for(int r = 1; r <= aminoAcidSequence.Count; r++)
            {
                cumulativeCTerminalMass[r] = cumulativeCTerminalMass[r - 1] + aminoAcidSequence[aminoAcidSequence.Count - r].Mass + massShifts[aminoAcidSequence.Count - r];
            }
        }

        public Fragment CalculateFragment(FragmentType fragmentIonType, int fragmentNumber)
        {
            switch(fragmentIonType)
            {
                case FragmentType.b:
                case FragmentType.c:
                    return new Fragment(fragmentIonType, fragmentNumber, cumulativeNTerminalMass[fragmentNumber] + ION_CAPS[fragmentIonType].Mass);
                case FragmentType.y:
                case FragmentType.zdot:
                    return new Fragment(fragmentIonType, fragmentNumber, cumulativeCTerminalMass[fragmentNumber] + ION_CAPS[fragmentIonType].Mass);
                default:
                    return null;
            }
        }

        public FragmentDictionary CalculateFragments(IEnumerable<FragmentType> fragmentTypes)
        {
            FragmentDictionary fragments = new FragmentDictionary();

            for(int r = 1; r < aminoAcidSequence.Count; r++)
            {
                foreach(FragmentType fragment_type in fragmentTypes)
                {
                    if(!(fragment_type == FragmentType.c && r < aminoAcidSequence.Count && aminoAcidSequence[r].Abbreviation == 'P') &&
                       !(fragment_type == FragmentType.zdot && aminoAcidSequence.Count - r < aminoAcidSequence.Count && aminoAcidSequence[aminoAcidSequence.Count - r].Abbreviation == 'P'))
                    {
                        Fragment fragment = CalculateFragment(fragment_type, r);
                        fragments.Add(fragment.ToString(), fragment);
                    }
                }
            }

            return fragments;
        }
    }
}
