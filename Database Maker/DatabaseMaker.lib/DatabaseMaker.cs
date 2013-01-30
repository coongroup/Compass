using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CSMSL.IO;

namespace Coon.Compass.DatabaseMaker
{
    public class DatabaseMaker
    {
        private List<string> fastaFilepaths;
        private DatabaseType databaseType;
        private DecoyDatabaseMethod decoyDatabaseMethod;
        private bool excludeNTerminus;
        private bool onlyIfNTerminusIsMethionine;
        private bool blastFormatForOmssa;
        private bool mergeOutput;
        private string formatdbFilepath;
        private string outputFolder;

        public DatabaseMakerOptions Options { get; set; }

        public DatabaseMaker(DatabaseMakerOptions options)
        {
            Options = options;
        }

        public DatabaseMaker(IEnumerable<string> fastaFilepaths,
            DatabaseType databaseType,
            DecoyDatabaseMethod decoyDatabaseMethod, bool excludeNTerminus, bool onlyIfNTerminusIsMethionine,
            bool blastFormatForOmssa, bool mergeOutput, string formatdbFilepath, string outputFolder)
        {
            this.fastaFilepaths = new List<string>(fastaFilepaths);
            this.databaseType = databaseType;
            this.decoyDatabaseMethod = decoyDatabaseMethod;
            this.excludeNTerminus = excludeNTerminus;
            this.onlyIfNTerminusIsMethionine = onlyIfNTerminusIsMethionine;
            this.blastFormatForOmssa = blastFormatForOmssa;
            this.mergeOutput = mergeOutput;
            this.formatdbFilepath = formatdbFilepath;
            this.outputFolder = outputFolder;
        }

        public void CreateDatabase()
        {
            try
            {
                using (StreamWriter log = new StreamWriter(Path.Combine(outputFolder, "Database_Maker_log.txt")))
                {
                    log.WriteLine("Database Maker PARAMETERS");
                    log.WriteLine("Database Type: " + databaseType.ToString());
                    if (databaseType != DatabaseType.Target)
                    {
                        log.WriteLine("Decoy Database Method: " + decoyDatabaseMethod.ToString());
                        log.WriteLine("Exclude N-Terminus: " + excludeNTerminus.ToString());
                        if (excludeNTerminus)
                        {
                            log.WriteLine("Only If N-Terminus Is Methionine: " + onlyIfNTerminusIsMethionine.ToString());
                        }
                    }
                    log.WriteLine();

                    if (mergeOutput)
                    {
                        log.WriteLine("Merging Fasta Files:");
                    }
                    else
                    {
                        log.WriteLine("Fasta Files:");
                    }
                    foreach (string fastafile in fastaFilepaths)
                    {
                        log.WriteLine(fastafile);
                    }
                    log.WriteLine();
                }

                //string output_filename = Path.GetFileName(fastaFilepaths[0]);
                string ext = Path.GetExtension(fastaFilepaths[0]);
                string output_filename = Path.GetFileNameWithoutExtension(fastaFilepaths[0]);
                switch (databaseType)
                {
                    case DatabaseType.Target:
                        output_filename += "_TARGET" + ext;//"TARGET_" + output_filename;
                        break;
                    case DatabaseType.Decoy:
                        output_filename += "_DECOY" + ext;//"DECOY_" + output_filename;
                        break;
                    case DatabaseType.Concatenated:
                    default:
                        output_filename += "_CONCAT" + ext; //"CONCAT_" + output_filename;
                        break;
                }
                string outputPath = Path.Combine(outputFolder, output_filename);

                using (FastaWriter writer = new FastaWriter(outputPath))
                {
                    foreach (string fastaFile in fastaFilepaths)
                    {
                        using (FastaReader reader = new FastaReader(fastaFile))
                        {
                            foreach (Fasta fasta in reader.ReadNextFasta())
                            {
                                if (databaseType == DatabaseType.Target || databaseType == DatabaseType.Concatenated)
                                {
                                    writer.Write(fasta);
                                }

                                if (databaseType == DatabaseType.Decoy || databaseType == DatabaseType.Concatenated)
                                {
                                    writer.Write(fasta.ToDecoy());
                                }
                            }
                        }
                    }
                }

                if (blastFormatForOmssa)
                {
                    MakeBlastDatabase(outputFolder, output_filename, Path.GetFileNameWithoutExtension(output_filename));
                }

                // Give feedback to user.
                //MessageBox.Show("Database saved to " + outputFolder);
            }
            catch (DirectoryNotFoundException)
            {
                //MessageBox.Show("Output Folder Path does not exist.");
            }
        }

        public static string MAKEBLASTDB_FILENAME = "makeblastdb.exe";
        public static string LOG_FILENAME = "blast_log.txt";

        /**
         * Create BLAST Database files for OMMSA by use makeblastdb.exe\
         **/
        public static void MakeBlastDatabase(string outputfolder, string infile, string outfile)
        {
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = outputfolder;

            // Pick makeblastdb.exe from Application Directory Folder
            process.StartInfo.FileName = Path.GetDirectoryName(System.Environment.CurrentDirectory) + @"\" + MAKEBLASTDB_FILENAME;
            process.StartInfo.Arguments = string.Format("-in {0} -out {1} -max_file_sz {2} -logfile {3}", infile, outfile, "2GB", LOG_FILENAME);
            process.Start();
            process.WaitForExit();
        }

        private const int RESIDUES_PER_LINE = 80;

        private static void WriteSequence(StreamWriter output, string sequence)
        {
            while(sequence.Length > 0)
            {
                if(sequence.Length >= RESIDUES_PER_LINE)
                {
                    output.WriteLine(sequence.Substring(0, RESIDUES_PER_LINE));
                    sequence = sequence.Remove(0, RESIDUES_PER_LINE);
                }
                else
                {
                    output.WriteLine(sequence.Substring(0));
                    sequence = sequence.Remove(0);
                }
            }
        }

        private static readonly Random RANDOM = new Random();

        private static readonly List<char> AMINO_ACIDS = new List<char>(new char[] { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' });

        private static string GenerateDecoySequence(string sequence, DecoyDatabaseMethod decoyDatabaseMethod, bool excludeNTerminus, bool onlyIfNTerminusIsMethionine)
        {
            string decoy_sequence = null;

            switch(decoyDatabaseMethod)
            {
                case DecoyDatabaseMethod.Reverse:
                    if(excludeNTerminus && (!onlyIfNTerminusIsMethionine || sequence[0] == 'M'))
                    {
                        char[] temp = sequence.ToCharArray();
                        Array.Reverse(temp, 1, temp.Length - 1);
                        decoy_sequence = new string(temp);
                    }
                    else
                    {
                        char[] temp = sequence.ToCharArray();
                        Array.Reverse(temp);
                        decoy_sequence = new string(temp);
                    }
                    break;
                case DecoyDatabaseMethod.Shuffle:
                    if(excludeNTerminus && (!onlyIfNTerminusIsMethionine || sequence[0] == 'M'))
                    {
                        char[] temp = sequence.ToCharArray();
                        Shuffle(temp, 1, temp.Length - 1);
                        decoy_sequence = new string(temp);
                    }
                    else
                    {
                        char[] temp = sequence.ToCharArray();
                        Shuffle(temp);
                        decoy_sequence = new string(temp);
                    }
                    break;
                case DecoyDatabaseMethod.Random:
                    decoy_sequence = string.Empty;
                    if(excludeNTerminus && (!onlyIfNTerminusIsMethionine || sequence[0] == 'M'))
                    {
                        decoy_sequence += sequence[0];
                    }
                    while(decoy_sequence.Length < sequence.Length)
                    {
                        decoy_sequence += AMINO_ACIDS[RANDOM.Next(AMINO_ACIDS.Count)];
                    }
                    break;
            }

            return decoy_sequence;
        }

        private static void Shuffle(char[] array)
        {
            Shuffle(array, 0, array.Length);
        }

        private static void Shuffle(char[] array, int startIndex, int Length)
        {
            for(int i = startIndex + Length; i > startIndex + 1; i--)
            {
                int k = RANDOM.Next(i - startIndex) + startIndex;
                char temp = array[k];
                array[k] = array[i - 1];
                array[i - 1] = temp;
            }
        }
    }
}