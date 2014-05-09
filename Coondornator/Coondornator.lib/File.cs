using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compass.Coondornator
{
    public abstract class File : INotifyPropertyChanged, IEqualityComparer<File>
    {
        public string Name { get { return Path.GetFileNameWithoutExtension(FilePath); } }
        public string NameWithExtension { get { return Path.GetFileName(FilePath); } }
        public string FilePath { get; set; }
       
        protected File(string filePath)
        {
            FilePath = filePath;            
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public bool Equals(File x, File y)
        {
            return x.FilePath.Equals(y.FilePath);
        }

        public int GetHashCode(File obj)
        {
            return FilePath.GetHashCode();
        }
    }
}
