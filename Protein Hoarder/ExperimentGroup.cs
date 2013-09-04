using System.Collections.Generic;

namespace Compass.ProteinHoarder
{
    public class ExperimentGroup
    {
        public List<CsvFile> CsvFiles { get; set; }

        public char ExperimentalID { get; set; }

        public HashSet<ProteinGroup> ProteinGroups { get; set; }

        public int TQStart { get; set; }

        public int TQStop { get; set; }

        public bool UseQuant { get; set; }

        public int QuantPlex
        {
            get
            {
                int length = TQStop - TQStart - 1;                
                int plex = length / 4;
                return plex;
            }
        }

        public string QuantHeader { get; set; }

        public string Header { get; set; }

        public ExperimentGroup(char id)
        {
            ExperimentalID = id;
            CsvFiles = new List<CsvFile>();
            ProteinGroups = new HashSet<ProteinGroup>();
        }
    }
}