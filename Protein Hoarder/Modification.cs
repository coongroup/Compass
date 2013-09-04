using System;
using System.ComponentModel;

namespace Compass.ProteinHoarder
{
    public class Modification : INotifyPropertyChanged, IEquatable<Modification>
    {
        public bool ignore = false;

        public bool IgnoreMod
        {
            get
            {
                return ignore;
            }
            set
            {
                ignore = value;
                OnPropertyChanged("IgnoreMod");
            }
        }

        public string name = string.Empty;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public Modification(string name, bool ignore = false)
        {
            Name = name;
            IgnoreMod = ignore;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool Equals(Modification other)
        {
            return Name.Equals(other.Name);
        }
    }
}