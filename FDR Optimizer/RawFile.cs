using System.Collections.Generic;

namespace Coon.Compass.FdrOptimizer
{
    public class RawFile
    {
        public string FilePath { get; set; }

        public List<string> CsvFiles { get; set; }

        public RawFile(string filePath)
        {
            FilePath = filePath;
            CsvFiles = new List<string>();
        }
    }
}
