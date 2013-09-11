using System.Collections.Generic;

namespace Coon.Compass.FdrOptimizer
{
    public class Modification2
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

        private bool userModification;

        public bool UserModification
        {
            get { return userModification; }
            set { userModification = value; }
        }

        public Modification2(string name, double monoisotopicMassShift,
            ModificationType modificationType, IEnumerable<char> aminoAcidResidues,
            bool userModification)
        {
            this.name = name;
            this.monoisotopicMassShift = monoisotopicMassShift;
            this.modificationType = modificationType;
            this.aminoAcidResidues = aminoAcidResidues;
            this.userModification = userModification;
        }

        public override string ToString()
        {
            return name + (userModification ? "*" : null);
        }
    }
}
