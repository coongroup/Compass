using System.Collections.Generic;
using System.IO;

namespace Phosphinator
{
    public class ProteinDictionary : Dictionary<string, string>
    {
        public ProteinDictionary() : base() { }

        public ProteinDictionary(string fastaFilepath)
            : this()
        {
            using(StreamReader fasta = new StreamReader(fastaFilepath))
            {
                string description = null;
                string sequence = null;

                while(true)
                {
                    string line = fasta.ReadLine();

                    if(line.StartsWith(">") || fasta.Peek() == -1)
                    {
                        if(description != null)
                        {
                            Add(description, sequence);
                        }

                        if(fasta.Peek() == -1)
                        {
                            break;
                        }

                        description = line.Substring(1);
                        sequence = null;
                    }
                    else
                    {
                        sequence += line;
                    }
                }
            }
        }
    }
}
