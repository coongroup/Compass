using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Coon;

namespace Lotor
{
    public class PTM : Modification, IEquatable<PTM>, INotifyPropertyChanged
    {
        public bool IsFixed { get; set; }

        public double Mass { get { return base.Mass.MonoisotopicMass; } }

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

        private bool isProteinCTerm = false;
        public bool IsProteinCTerm
        {
            get
            {
                return isProteinCTerm;
            }
            set
            {
                isProteinCTerm = value;
                ToggleModificationSite(ModificationSite.ProtC, value);
                OnPropertyChanged("ModifiableSites");
            }
        }

        private bool isProteinNTerm = false;
        public bool IsProteinNTerm
        {
            get
            {
                return isProteinNTerm;
            }
            set
            {
                isProteinNTerm = value;
                ToggleModificationSite(ModificationSite.Nprot, value);
                OnPropertyChanged("ModifiableSites");
            }
        } 

        public PTM(string name, Mass mass, ModificationSite modSites, bool isFixed = true)
            : base(name, mass, modSites)
        {
            IsFixed = isFixed;
        }

        public PTM(Modification mod, bool isFixed = true)
            : base(mod.Name, mod.Mass, mod.ModifiableSites)
        {
            IsFixed = isFixed;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() + IsFixed.GetHashCode();
        }

        public bool Equals(PTM other)
        {
            return this.Name.Equals(other.Name) && IsFixed == other.IsFixed;
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
