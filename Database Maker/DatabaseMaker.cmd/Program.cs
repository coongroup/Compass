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

            var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));            
            if (parser.ParseArguments(args, options))              
            {
                var databaseMaker = new DatabaseMaker(options);
                
                if (options.Verbose)
                {
                    databaseMaker.DisplayVerboseOptions(options.Verbose, options);
                }

                databaseMaker.OnInvalidHeader += databaseMaker_OnInvalidHeader;

                try
                {
                    databaseMaker.CreateDatabase();
                }
                catch (ArgumentNullException e2)
                {
                    Console.WriteLine("\nError: No input files specified");
                }

                catch (ArgumentException e3)
                {
                    Console.WriteLine("\nError: " + e3.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
                finally
                {
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            Environment.Exit(0);
        }

        static void databaseMaker_OnInvalidHeader(object sender, FastaEvent e)
        {
            Fasta fasta = e.Fasta;
            Console.WriteLine("\nInvalid Header!");
            Console.WriteLine(fasta.Description);
        }
    }
}
