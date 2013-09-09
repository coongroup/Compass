using System;
using System.ComponentModel;
using CSMSL.Chemistry;
using CSMSL.Proteomics;

namespace Coon.Compass.Lotor
{
    public class PTM : IMass, IEquatable<PTM>, INotifyPropertyChanged
    {
        public bool IsFixed { get; set; }
       
        public bool quanitfy = false;
        public bool Quantify
        {
            get
            {
                return quanitfy;
            }
            set
            {
                quanitfy = value;
                OnPropertyChanged("Quantify");
            }
        }

        public ModificationSites ModificationSites { get; set; }
        
        public bool IsProteinCTerm
        {
            get
            {
                return (ModificationSites & ModificationSites.ProtC) == ModificationSites.ProtC;
            }
            set
            {
                if (value)
                    ModificationSites |= ModificationSites.ProtC;
                else
                    ModificationSites ^= ModificationSites.ProtC;
                OnPropertyChanged("ModifiableSites");
            }
        }

        public bool IsProteinNTerm
        {
            get
            {
                return (ModificationSites & ModificationSites.NProt) == ModificationSites.NProt;
            }
            set
            {
                if (value)
                    ModificationSites |= ModificationSites.NProt;
                else
                    ModificationSites ^= ModificationSites.NProt;
                OnPropertyChanged("ModifiableSites");
            }
        }

        public string Name { get; set; }

        public double MonoisotopicMass
        {
            get; private set;
        }

        public PTM(string name, double monoMass, ModificationSites modSites, bool isFixed = true)
        {
            ModificationSites = modSites;
            Name = name;
            MonoisotopicMass = monoMass;
            IsFixed = isFixed;
        }
     
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() + IsFixed.GetHashCode();
        }

        public bool Equals(PTM other)
        {
            return Name.Equals(other.Name) && IsFixed == other.IsFixed;
        }

        public override string ToString()
        {
            return Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    

}
}
