using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CSMSL.IO;
using System.Text.RegularExpressions;
using System.Windows;



namespace Coon.Compass.DatabaseMaker
{
    public class DatabaseMaker
    {        
        public DatabaseMakerOptions Options { get; set; }

        public DatabaseMaker(DatabaseMakerOptions options)
        {
            Options = options;
        }

     

        // Create an event that can be handled by other code
        // It takes as an argument <FastaEvent> or any other class
        // that inherits from EventArgs.

        public event EventHandler<FastaEvent> OnInvalidHeader;

        // This private method is called when an event should be 
        // thrown. It first does context-switching, i.e. setting
        // a public variable (OnInvalidHeader) to a local variable
        // to prevent race conditions. Then it checks to see if 
        // there are any event handlers registered with it (the null
        // check). If no one is listening to this event, it does nothing.
        // If there are one or more listeners to this event, it "raises"
        // the event and those are handled by their respective methods.

        private void InvalidHeader(Fasta fasta)
        {
            var handler = OnInvalidHeader;
            if (handler != null)
            {
                handler(this, new FastaEvent(fasta));
            }
            // BAD below
            //if (OnInvalidHeader != null)
            //{
            //    OnInvalidHeader(this, new FastaEvent(fasta));
            //}
        }

        public void CreateDatabase()
        {
            try
            {
                // Validate Options are kosher

                if (Options.InputFiles == null ||Options.InputFiles.Count == 0)
                {
                    throw new ArgumentNullException("Input Files");
                }

                string ext = Path.GetExtension(Options.OutputFastaFile);
                if (string.IsNullOrEmpty(ext))
                {
                    ext = ".fasta";
                }
                string output_filename = Path.GetFullPath(Options.OutputFastaFile);

                if (Options.DoNotAppendDatabaseType)
                {
                    output_filename = output_filename + ext;
                    if (Options.InputFiles.Contains(output_filename))
                    {
                        throw new ArgumentException("Output file path cannot be the same as an input file.");
                    }
                }
                else
                {
                    switch (Options.OutputType)
                    {
                        case DatabaseType.Target:
                            output_filename += "_TARGET" + ext;
                            break;
                        case DatabaseType.Decoy:
                            output_filename += "_DECOY" + ext;
                            break;
                        case DatabaseType.Concatenated:
                        default:
                            output_filename += "_CONCAT" + ext;
                            break;
                    }
                }

                string logFilename = Path.GetFileNameWithoutExtension(Options.OutputFastaFile);
                string outputFolder = Path.GetDirectoryName(Options.OutputFastaFile);
                if (!Directory.Exists(outputFolder))
                {
                    outputFolder = Directory.GetCurrentDirectory();
                }

                if (Options.GenerateLogFile)
                {
                    switch (Options.OutputType)
                    {
                        case DatabaseType.Target:
                            logFilename += "_TARGET.log";
                            break;
                        case DatabaseType.Decoy:
                            logFilename += "_DECOY.log";
                            break;
                        case DatabaseType.Concatenated:
                        default:
                            logFilename += "_CONCAT.log";
                            break;
                    }
                    GenerateLog(outputFolder, logFilename);
                }

                string outputPath = Path.Combine(outputFolder, output_filename);

                if (Options.DoNotMergeFiles)
                {
                    foreach (string fastaFile in Options.InputFiles)
                    {
                        using (FastaWriter writer = new FastaWriter(outputPath))
                        {
                            WriteFasta(fastaFile, writer);
                        }
                    }
                }
                else
                {
                    using (FastaWriter writer = new FastaWriter(outputPath))
                    {
                        foreach (string fastaFile in Options.InputFiles)
                        {
                            WriteFasta(fastaFile, writer);
                        }
                    }
                }

                if (Options.BlastDatabase)
                {
                    MakeBlastDatabase(outputFolder, output_filename, Path.GetFileNameWithoutExtension(output_filename));
                }
            }

            catch (DirectoryNotFoundException)
            {
            }
        }
    
        /// <summary>
        /// Generates a log file detailing the parameters used
        /// </summary>
        /// <param name="logFileName">The name of the log file to write</param>
        private void GenerateLog(string outputDirectory, string logFileName = "DatabaseMaker.log")
        {
            using (StreamWriter log = new StreamWriter(Path.Combine(outputDirectory, logFileName)))
            {
                log.WriteLine("Database Maker PARAMETERS");
                log.WriteLine("Database Type: {0}", Options.OutputType);
                if (Options.OutputType != DatabaseType.Target)
                {
                    log.WriteLine("Decoy Database Method: {0}", Options.DecoyType);
                    log.WriteLine("Exclude N-Terminus: " + Options.ExcludeNTerminalResidue);
                    log.WriteLine("Only If N-Terminus Is Methionine: " + Options.ExcludeNTerminalMethionine);
                }
                log.WriteLine("Merge Fasta Files: {0}", Options.DoNotMergeFiles);
                foreach (string fastafile in Options.InputFiles)
                {
                    log.WriteLine(fastafile);
                }
            }
        }

        public void WriteFasta(string fasta_file, FastaWriter Writer)
        {
            bool MakeDecoy = false;
            
            if (Options.OutputType == DatabaseType.Target || Options.OutputType == DatabaseType.Concatenated)
            {
                MakeDecoy = false;
            }
            else if (Options.OutputType == DatabaseType.Decoy || Options.OutputType == DatabaseType.Concatenated)
            {
                MakeDecoy = true;
            }

            using (FastaReader reader = new FastaReader(fasta_file))
            {
                int mismatch = 0;
                foreach (Fasta fasta in reader.ReadNextFasta())
                {
                    Regex uniprotRegex = new Regex(@"(.+)\|(.+)\|(.+?)\s(.+?)\sOS=(.+?)(?:\sGN=(.+?))?(?:$|PE=(\d+)\sSV=(\d+))", RegexOptions.ExplicitCapture);
                    Match UniprotMatch = uniprotRegex.Match(fasta.Description);
                    string HeaderFile = "InvalidUniprotheaders.txt";
                    string headerFolder = Path.GetDirectoryName(Options.InputFiles[0]);

                    if (Options.EnforceUniprot && !UniprotMatch.Success)
                    {
                        using (StreamWriter log = new StreamWriter(Path.Combine(headerFolder, HeaderFile), true))
                        {
                            log.WriteLine("Invalid Header:");
                            log.WriteLine();
                            log.WriteLine(fasta.Description);
                            log.WriteLine();
                            log.WriteLine("At line " + mismatch + ", in file '" + fasta_file + "'");
                            log.WriteLine();
                            InvalidHeader(fasta);
                        }
                    }

                    if (UniprotMatch.Success)
                    {
                        
                        bool excludeMethionine = false;
                        if (Options.ExcludeNTerminalMethionine && !Options.ExcludeNTerminalResidue)
                        {
                            excludeMethionine = true;
                        }

                        if (MakeDecoy)
                        {
                            Writer.Write(fasta.ToDecoy(Options.DecoyPrefix, Options.DecoyType, (excludeMethionine || Options.ExcludeNTerminalResidue), Options.ExcludeNTerminalMethionine));
                        }
                        
                        else
                        {
                            Writer.Write(fasta);
                        }

                    } mismatch = reader.LineNumber;

                }
                
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
            process.StartInfo.UseShellExecute = false;

            // Pick makeblastdb.exe from Application Directory Folder
            string filename = Path.Combine(Environment.CurrentDirectory, MAKEBLASTDB_FILENAME);
            process.StartInfo.FileName = filename;// Path.GetDirectoryName(System.Environment.CurrentDirectory) + @"\" + MAKEBLASTDB_FILENAME;

            process.StartInfo.Arguments = string.Format("-in {0} -out {1} -max_file_sz {2} -logfile {3}", infile, outfile, "2GB", LOG_FILENAME);
            process.Start();
            process.WaitForExit();
        }
        public void DisplayVerboseOptions(bool verboseOptions, DatabaseMakerOptions Options)
        {
            Console.WriteLine("Input File(s): {0}", string.Join("\n", Options.InputFiles));
            Console.WriteLine("\nOutput File: {0}", Options.OutputFastaFile);
            Console.WriteLine("\nDatabase Maker PARAMETERS");
            Console.WriteLine("\nDatabase Type: {0}", Options.OutputType);
            if (Options.OutputType != DatabaseType.Target)
            {
                Console.WriteLine("\nDecoy Database Method: {0}", Options.DecoyType);
                Console.WriteLine("\nExclude N-Terminus: " + Options.ExcludeNTerminalResidue);
                Console.WriteLine("\nOnly If N-Terminus Is Methionine: " + Options.ExcludeNTerminalMethionine);
            }
            Console.WriteLine("\nMerging Fasta Files: " + Options.DoNotMergeFiles);
            Console.WriteLine("\nEnforce Standard Uniprot Headers: " + Options.EnforceUniprot);
        }

    }
}