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

        public UserModFile UserModFile;
        public List<DtaFile> DtaFiles;
        public List<DatabaseFile> DatabaseFiles;

        public CondorSubmitFile(IEnumerable<DtaFile> dtaFiles, UserModFile userModFile, IEnumerable<DatabaseFile> dbFiles)
            : base(Path.GetTempFileName())
        {
            Requirements = new List<string> {"(Arch == \"x86_64\")", "(TARGET.Name =!= LastMatchName1)", "(OpSys == \"LINUX\")", "(Disk > 500000)"};

            UserModFile = userModFile;
            DatabaseFiles = new List<DatabaseFile>(dbFiles);
            DtaFiles = new List<DtaFile>(dtaFiles);
            foreach (DtaFile dta in DtaFiles)
            {
                dta.UserModFile = userModFile;
            }
        }

        public void WriteToDisk()
        {
            int numberOfDatbases = DatabaseFiles.Count;

            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                writer.WriteLine(SubmitFileHeader(Coondornator.OmssaFilePath, string.Join(" && ", Requirements)));

                foreach (DtaFile dtaFile in DtaFiles)
                {
                    writer.WriteLine();
                    writer.WriteLine("######################################");
                    writer.WriteLine("### " + dtaFile.NameWithExtension);
                    writer.WriteLine();
                    foreach (DatabaseFile dataBaseFile in DatabaseFiles)
                    {
                        writer.WriteLine("transfer_input_files = " + dtaFile.GetTransferLine(dataBaseFile));
                        writer.WriteLine("arguments = " + dtaFile.GetArgumentLine(dataBaseFile, numberOfDatbases > 1));
                        writer.WriteLine("queue");
                        writer.WriteLine();
                    }
                    writer.WriteLine("######################################");
                }
            }
        }
        
        private string SubmitFileHeader(string executable = Coondornator.OmssaFilePath, string requirements = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("######################################");
            sb.AppendLine("# Condor Submit File by Coondornator #");
            sb.AppendLine("# Derek Bailey, Coon Chemistry Group #");
            sb.AppendLine("######################################");
            sb.AppendLine();
            sb.AppendLine("universe = vanilla");
            sb.AppendLine("executable = " + executable);
            sb.AppendLine("error = omssa.$(cluster).$(process).err");
            sb.AppendLine("output = /dev/null");
            sb.AppendLine("log = /home/Groups/Condor/Logs/omssacl.log");
            sb.AppendLine("match_list_length = 5");
            sb.AppendLine("requirements = "+requirements);
            sb.AppendLine("should_transfer_files = YES");
            sb.AppendLine("when_to_transfer_output = ON_EXIT_OR_EVICT");
            sb.AppendLine("notification = Error");
            return sb.ToString();
        }
    }
}
