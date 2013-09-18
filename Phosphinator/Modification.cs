using System.Collections.Generic;

namespace Phosphinator
{
    public class Modification
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private double monoisotopicMassShift;

        public double MonoisotopicMassShift
        {
            get { return monoisotopicMassShift; }
            set { monoisotopicMassShift = value; }
        }

        private ModificationType modificationType;

        public ModificationType ModificationType
        {
            get { return modificationType; }
            set { modificationType = value; }
        }

        private IEnumerable<char> aminoAcidResidues;

        public IEnumerable<char> AminoAcidResidues
        {
            get { return aminoAcidResidues; }
            set { aminoAcidResidues = value; }
        }

        public Modification(string name, double monoisotopicMassShift,
            ModificationType modificationType, IEnumerable<char> aminoAcidResidues)
        {
            this.name = name;
            this.monoisotopicMassShift = monoisotopicMassShift;
            this.modificationType = modificationType;
            this.aminoAcidResidues = aminoAcidResidues;
        }
    }
}
