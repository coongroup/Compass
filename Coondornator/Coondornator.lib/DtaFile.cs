using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL.IO;

namespace Compass.Coondornator
{
    public class DtaFile : File
    {
        private string parameterLine;

        public string ParameterLine
        {
            get { return parameterLine; }
            set
            {
                parameterLine = value;
                OnPropertyChanged("ParameterLine");
            }
        }

        public UserModFile UserModFile { get; set; }

        public List<DtaFile> SplitedDtaFiles; 

        public string GetTransferLine(DatabaseFile database)
        {
            List<string> parts = new List<string>();
            parts.Add(NameWithExtension);
            parts.Add(Coondornator.DefaultOmssaModsXML);
            if (UserModFile == null)
            {
                parts.Add(Coondornator.DefaultOmssaUserModsXML);
            }
            else
            {
                parts.Add(UserModFile.NameWithExtension);
            }
            parts.Add(Path.ChangeExtension(database.FilePath, "pin"));
            parts.Add(Path.ChangeExtension(database.FilePath, "phr"));
            parts.Add(Path.ChangeExtension(database.FilePath, "psq"));
            return string.Join(",", parts);
        }

        public string GetArgumentLine(DatabaseFile database, bool appendDBName = false)
        {
            List<string> parts = new List<string>();
            parts.Add("-fx " + NameWithExtension);
            if (appendDBName)
            {
                parts.Add("-oc " + Path.ChangeExtension(Name + "_" + database.Name, "csv")); ;
            }
            else
            {
                parts.Add("-oc " + Path.ChangeExtension(Name, "csv"));
            }
            parts.Add("-d " + database.Name);
            parts.Add(ParameterLine);
            return string.Join(" ", parts);
        }

        public DtaFile(string filePath)
            : base(filePath) { }

        public IEnumerable<DtaFile> Split(int numberOfSpectra = 5000)
        {
            SplitedDtaFiles = new List<DtaFile>();

            int fileCount = 1;
            
            string tempPath = Path.GetTempPath();
            string tempName = Path.Combine(tempPath, Name + "_" + fileCount + ".txt");
            StreamWriter writer = new StreamWriter(tempName);
            DtaFile dtaFile = null;
            int dtaCount = 0;
            using (StreamReader reader = new StreamReader(FilePath))
            {
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.StartsWith("<dta"))
                    {
                        dtaCount++;
                        if (dtaCount > numberOfSpectra)
                        {
                            dtaCount = 0;
                            writer.Close();
                            dtaFile = new DtaFile(tempName) { ParameterLine = ParameterLine, UserModFile = UserModFile };
                            SplitedDtaFiles.Add(dtaFile);
                            yield return dtaFile;

                            fileCount++;
                            tempName = Path.Combine(tempPath, Name + "_" + fileCount + ".txt");
                            writer = new StreamWriter(tempName);
                        }
                    }

                    writer.WriteLine(line);
                }
            }
            writer.Close();
            dtaFile = new DtaFile(tempName) { ParameterLine = ParameterLine, UserModFile = UserModFile };
            SplitedDtaFiles.Add(dtaFile);
            yield return dtaFile;
        }

    }
}
