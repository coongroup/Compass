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
           // Arguments.Add("-nt 1");
        }

        public void Add(File file, string outputfileName, string databaseName, string argumentLine)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#### " + Path.GetFileNameWithoutExtension(file.ToString()));
            sb.AppendLine("transfer_input_files = /home/Groups/Condor/usermods.xml,/home/Groups/Condor/mods.xml,");
            sb.Append("/home/Groups/Condor/Databases/" + Path.GetFileNameWithoutExtension(databaseName.ToString()) + ".pin,");
            sb.Append("/home/Groups/Condor/Databases/" + Path.GetFileNameWithoutExtension(databaseName.ToString()) + ".phr,");
            sb.Append("/home/Groups/Condor/Databases/" + Path.GetFileNameWithoutExtension(databaseName.ToString()) + ".psq,");
            sb.Append(Path.GetFileName(file.ToString()));
        }

        public void Add(File file, string modFile, string outputfileNam, string databaseName, string argumentLine)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#### " + Path.GetFileNameWithoutExtension(file.ToString()));

        }

        public override string ToString()
        {
            string requirementLine = "requirements = " + string.Join(" && ", Requirements);

            string argLine = string.Join(" ", Arguments);
            return requirementLine;
        }

        public string BuildRequirementLine(List<string> requirements)
        {
             string requirementLine = "requirements = " + string.Join(" && ", requirements);
             return requirementLine;
        }

        public string SubmitFileHeader(string executable, string requirements)
        {

            executable = "executable goes here";
            requirements = "requirements go here";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("######################################");
            sb.AppendLine("# Condor Submit File by Coondornator #");
            sb.AppendLine("# Derek Bailey, Coon Chemistry Group #");
            sb.AppendLine("######################################");
            sb.AppendLine();
            sb.AppendLine("universe = vanilla");
            sb.AppendLine(executable);
            sb.AppendLine("error = omssa.$(cluster).$(process).err");
            sb.AppendLine("output = /dev/null");
            sb.AppendLine("log = /home/Groups/Condor/Logs/omssacl.log");
            sb.AppendLine("match_list_length = 5");
            sb.AppendLine(requirements);
            sb.AppendLine("should_transfer_files = YES");
            sb.AppendLine("when_to_transfer_output = ON_EXIT_OR_EVICT");
            sb.AppendLine("notification = Error");
            sb.AppendLine();
            sb.AppendLine("######################################");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
