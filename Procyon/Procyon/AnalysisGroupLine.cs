using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Coon.Compass.Procyon
{
    public class AnalysisGroupLine : INotifyPropertyChanged
    {
        public string FileName { get; set; }
        public string HeaderName { get; set; }
        public string UniqueName { get; set; }
        public string GroupString { get; set; }
        public string FileLocation { get; set; }

        public AnalysisGroupLine(string fileLocation, string headerName, string uniqueName)
        {
            string fileName = Path.GetFileName(fileLocation);

            FileLocation = fileLocation;
            FileName = fileName;
            HeaderName = headerName;
            UniqueName = uniqueName;
            GroupString = "";
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
