using CommandLine;
using System;

namespace Coon.Compass.DatabaseMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new DatabaseMakerOptions();
            var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));     
            if (!parser.ParseArguments(args, options))
                Environment.Exit(1);

            var databaseMaker = new DatabaseMaker(options);
            //databaseMaker.CreateDatabase();
            Console.WriteLine("Success!");
            Environment.Exit(0);
        }
    }
}
