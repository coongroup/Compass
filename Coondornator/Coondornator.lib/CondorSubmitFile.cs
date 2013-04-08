using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Compass.Coondornator
{
    public class CondorSubmitFile : File
    {
        public List<string> Requirements;
        public List<string> Arguments;

        public CondorSubmitFile()
            : base(Path.GetTempFileName())
        {
            Requirements = new List<string>();
            Arguments.Add("-nt 1");
        }

        public void Add(File file, string outputfileName, string databaseName, string argumentLine)
        {
            
        }

        public override string ToString()
        {
            string requirementLine = "requirements = " + string.Join(" && ", Requirements);

            string argLine = string.Join(" ", Arguments);
            return requirementLine;
        }
    }
}
