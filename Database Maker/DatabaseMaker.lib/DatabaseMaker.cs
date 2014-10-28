using System;
using System.Diagnostics;
using System.IO;
using CSMSL.IO;
using System.Text.RegularExpressions;

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
        }

        public void CreateDatabase()
        {
            try
            {
                if (Options.InputFiles == null || Options.InputFiles.Count == 0)
                {
                    throw new ArgumentNullException("Input Files");
                }

                if (!Directory.Exists(Options.OutputFolder))
                {
                    Directory.CreateDirectory(Options.OutputFolder);
                }
                string outputFolder = Options.OutputFolder;
                
                string ext = ".fasta";
                
                switch (Options.OutputType)
                {
                    case DatabaseType.Target:
                        ext = "_TARGET.fasta";
                        break;
                    case DatabaseType.Decoy:
                        ext = "_DECOY.fasta";
                        break;
                    case DatabaseType.Concatenated:
                    default:
                        ext = "_CONCAT.fasta";
                        break;
                }
                
                if (Options.GenerateLogFile)
                {
                    string logFilename = Path.GetFileNameWithoutExtension(Options.OutputFolder);
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
               
                if (Options.DoNotMergeFiles)
                {
                    foreach (string fastaFile in Options.InputFiles)
                    {
                        string baseName = Path.GetFileNameWithoutExtension(fastaFile);
                        string outputPath = Path.Combine(outputFolder, baseName + ext);

                        using (FastaWriter writer = new FastaWriter(outputPath))
                        {
                            WriteFasta(fastaFile, writer);
                        }

                        if (Options.BlastDatabase)
                        {
                            MakeBlastDatabase(outputPath, Path.GetFileNameWithoutExtension(outputPath));
                        }
                    }
                }
                else
                {
                    string baseName = Path.GetFileNameWithoutExtension(Options.InputFiles[0]);
                    string outputPath = Path.Combine(outputFolder, baseName + ext);
                    using (FastaWriter writer = new FastaWriter(outputPath))
                    {
                        foreach (string fastaFile in Options.InputFiles)
                        {
                            WriteFasta(fastaFile, writer);
                        }
                    }

                    if (Options.BlastDatabase)
                    {
                        MakeBlastDatabase(outputPath, Path.GetFileNameWithoutExtension(outputPath));
                    }
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
                log.WriteLine("Input File(s): {0}", string.Join("\n", Options.InputFiles));
                log.WriteLine("\nOutput File: {0}", Options.OutputFolder);
                log.WriteLine("\nDatabase Maker PARAMETERS");
                log.WriteLine("\nDatabase Type: {0}", Options.OutputType);
                if (Options.OutputType != DatabaseType.Target)
                {
                    log.WriteLine("\nDecoy Database Method: {0}", Options.DecoyType);
                    log.WriteLine("\nExclude N-Terminus: {0}", Options.ExcludeNTerminalResidue);
                    log.WriteLine("\nOnly If N-Terminus Is Methionine: {0}", Options.ExcludeNTerminalMethionine);
                }
                log.WriteLine("\nMerging Fasta Files: {0}", Options.DoNotMergeFiles);
                log.WriteLine("\nEnforce Standard Uniprot Headers: {0}", Options.EnforceUniprot);
                log.WriteLine("\nCreate a BLAST Database: {0}", Options.BlastDatabase);
                foreach (string fastafile in Options.InputFiles)
                {
                    log.WriteLine(fastafile);
                }
            }
        }

        public void WriteFasta(string fastaFilePath, FastaWriter writer)
        {
            bool writeTarget = (Options.OutputType & DatabaseType.Target) == DatabaseType.Target;
            bool writeDecoy = (Options.OutputType & DatabaseType.Decoy) == DatabaseType.Decoy;
            bool excludeMethionine = Options.ExcludeNTerminalMethionine && !Options.ExcludeNTerminalResidue;
            Regex uniprotRegex = new Regex(@"(.+)\|(.+)\|(.+?)\s(.+?)\sOS=(.+?)(?:\sGN=(.+?))?(?:$|PE=(\d+)\sSV=(\d+))", RegexOptions.ExplicitCapture);
    
            using (FastaReader reader = new FastaReader(fastaFilePath))
            {
                foreach (Fasta fasta in reader.ReadNextFasta())
                {
                    if (Options.EnforceUniprot)
                    {
                        Match uniprotMatch = uniprotRegex.Match(fasta.Description);

                        if (!uniprotMatch.Success)
                        {
                            InvalidHeader(fasta);
                            continue;
                        }
                    }
                    
                    if (writeTarget)
                    {
                        writer.Write(fasta);
                    }

                    if (writeDecoy)
                    {
                        writer.Write(fasta.ToDecoy(Options.DecoyPrefix, Options.DecoyType, (excludeMethionine || Options.ExcludeNTerminalResidue), Options.ExcludeNTerminalMethionine));
                    }
                }
                
            }
        }

        public const string MakeBlastDBExecutable = "makeblastdb.exe";
        public const string DefaultLogFilename = "blast_log.txt";
        
        /// <summary>
        /// Makes a Blast compatible database file
        /// </summary>
        /// <param name="inputFastaFilePath"></param>
        /// <param name="outputFileName"></param>
        public static void MakeBlastDatabase(string inputFastaFilePath, string outputFileName)
        {
            string outputFolder = Path.GetDirectoryName(inputFastaFilePath);
            Process process = new Process { StartInfo = { CreateNoWindow = true, WorkingDirectory = outputFolder, UseShellExecute = false } };

            // Pick makeblastdb.exe from Application Directory Folder
    
            process.StartInfo.FileName = Path.Combine(@"\\coongrp\GENERAL\Software\internal\COMPASS\makeblastdb.exe");
           // process.StartInfo.FileName = Path.Combine(Environment.CurrentDirectory, MakeBlastDBExecutable);

            // makeblastdb doesn't like spaces in the filenames...
            // http://stackoverflow.com/questions/15126020/why-multiple-arguments-with-spaces-are-not-interpreted-correctly-in-a-batch-scri
            process.StartInfo.Arguments = string.Format("-in \\\"\"{0}\"\\\" -dbtype prot -out {1} -max_file_sz {2} -logfile {3}", inputFastaFilePath, outputFileName, "2GB", DefaultLogFilename);

            process.Start();
            process.WaitForExit();
        }

        public void DisplayVerboseOptions(bool verboseOptions, DatabaseMakerOptions Options)
        {
            Console.WriteLine("Input File(s): {0}", string.Join("\n", Options.InputFiles));
            Console.WriteLine("\nOutput File: {0}", Options.OutputFolder);
            Console.WriteLine("\nDatabase Maker PARAMETERS");
            Console.WriteLine("\nDatabase Type: {0}", Options.OutputType);
            if (Options.OutputType != DatabaseType.Target)
            {
                Console.WriteLine("\nDecoy Database Method: {0}", Options.DecoyType);
                Console.WriteLine("\nExclude N-Terminus: {0}", Options.ExcludeNTerminalResidue);
                Console.WriteLine("\nOnly If N-Terminus Is Methionine: {0}", Options.ExcludeNTerminalMethionine);
            }
            Console.WriteLine("\nMerging Fasta Files: {0}", Options.DoNotMergeFiles);
            Console.WriteLine("\nEnforce Standard Uniprot Headers: {0}", Options.EnforceUniprot);
            Console.WriteLine("\nCreate a BLAST Database: {0}", Options.BlastDatabase);
        }

    }
}