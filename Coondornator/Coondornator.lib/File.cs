using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compass.Coondornator
{
    public abstract class File 
    {
        public string FilePath { get; set; }
       
        public File(string filePath)
        {
            FilePath = filePath;            
        }
    }
}
