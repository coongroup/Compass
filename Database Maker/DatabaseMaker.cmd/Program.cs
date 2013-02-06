using CommandLine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using CSMSL.IO;

namespace Coon.Compass.DatabaseMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new DatabaseMakerOptions();
            //var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));     
            //if (!parser.ParseArguments(args, options))
            //    Environment.Exit(1);

            //var databaseMaker = new DatabaseMaker(options);
            ////databaseMaker.CreateDatabase();
            //Console.WriteLine("Success!");
            //Environment.Exit(0);        

            var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
                Environment.Exit(1);
            {
                if (options.Verbose)
                {
                
                  //string outputFolder = Path.GetDirectoryName(options.OutputFastaFile);
                  //  if (string.IsNullOrEmpty(outputFolder))
                  //  {
                  //      outputFolder = Path.GetDirectoryName(options.InputFiles[0]);
                  //  }
                  //  string output_filename = Path.GetFileNameWithoutExtension(options.OutputFastaFile);

                  //  string outputPath = Path.Combine(outputFolder, output_filename);

                    //Console.WriteLine(outputFolder);
                    //Console.WriteLine(outputPath);

                    Console.WriteLine("");
                    Console.WriteLine("Your input files are: ");
                    Console.WriteLine(string.Join("\n", options.InputFiles));
                    Console.WriteLine("");
                    Console.WriteLine("Your Database File Type: ");
                    Console.WriteLine(options.OutputType);
                    Console.WriteLine("");
                    Console.WriteLine("Type of decoy database generated: ");
                    Console.WriteLine(options.DecoyType);
                    Console.WriteLine("");
                    Console.WriteLine("Decoy prefix: ");
                    Console.WriteLine(options.DecoyPrefix);
                    Console.WriteLine("");
                    //Console.WriteLine("Output file path: ");
                    ////Console.WriteLine(outputPath);
                    //Console.WriteLine("");

            

                }
                //else
                //{
                //    Console.WriteLine("working ...");
                //}

                else
                     {
                    Console.WriteLine(options.GetUsage());
                     return;
                    }
                if (options.ExcludeNTerminalResidue)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Exclude N-Terminal Amino Acid Residue");

                    if (options.ExcludeNTerminalMethionine)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Exclude N-Terminal Acid Residue if Methionine");
                    }
                }
                if (options.BlastDatabase)
                {
                    Console.WriteLine("");
                    Console.WriteLine("A BLAST database will be built");
                }
                if (options.EnforceUniprot)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Standard Uniprot headers will be used");


                    var databaseMaker = new DatabaseMaker(options);
                    databaseMaker.CreateDatabase();
                    Console.WriteLine("Success!");
                    Environment.Exit(0);
                }
            }
        }
    
    }
}
