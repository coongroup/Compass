using System;
using System.ComponentModel;
using System.IO;
using CSMSL.Proteomics;

namespace Protein_Hoarder
{
    public class CsvFile : INotifyPropertyChanged, IEquatable<CsvFile>
    {
        private string filePath;

        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
                OnPropertyChanged(FilePath);
                OnPropertyChanged(Name);
            }
        }

        private string name = String.Empty;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = Path.GetFileName(FilePath);
                }
                return name;
            }
        }

        private char expGroup = 'A';

        public char ExperimentGroup
        {
            get
            {
                return expGroup;
            }
            set
            {
                expGroup = value;
                OnPropertyChanged("ExperimentGroup");
            }
        }

        private string _protease;

        public string Protease
        {
            get
            {
                return _protease;
            }
            set
            {
                _protease = value;
                OnPropertyChanged("Protease");
            }
        }

        public CsvFile(string fileName)
        {
            FilePath = fileName;
            Protease = CSMSL.Proteomics.Protease.Trypsin.Name;
        }

        public override string ToString()
        {
            return Name;
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

        public bool Equals(CsvFile other)
        {
            return FilePath.Equals(other.FilePath);
        }
    }
}