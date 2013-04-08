using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using CSMSL.IO;

namespace Coon.Compass.DatabaseMaker
{
    public class DatabaseMakerOptions
    {
        //[Option('e', "extension", MetaValue = "STRING", Required = false, DefaultValue = ".fasta", HelpText = "Output file extension")]
        //public string FileExtension { get; set; }

        [Option("do-not-append", DefaultValue = false, HelpText = "Appends the output file name with the database type (CONCAT_, TARGET_, DECOY_)")]
        public bool DoNotAppendDatabaseType { get; set; }

        [Option("do-not-merge", DefaultValue = false, HelpText = "Do not merge files")]
        public bool DoNotMergeFiles { get; set; }

        [Option('l', null, DefaultValue = null, HelpText = "Generate a .log file.")]
        public bool GenerateLogFile { get; set; }

        [Option('o', "output", MetaValue="FILE", Required = true, HelpText = "The base name of the FILE to write the fasta file to")]
        public string OutputFastaFile { get; set; }

        [Option('u', "uniprot", DefaultValue = null, HelpText = "Use standard Uniprot heading in database")]
        public bool EnforceUniprot { get; set; }

        [Option('p', "prefix", MetaValue="STRING", Required = false, DefaultValue = "DECOY_", HelpText = "Decoy prefix to add to the front of the protein description")]
        public string DecoyPrefix { get; set; }

        [Option('d', "decoy", MetaValue = "TYPE", Required = false, DefaultValue = DecoyDatabaseMethod.Reverse, HelpText = "How to generate a decoy database (Options: reverse, shuffle, random)")]
        public DecoyDatabaseMethod DecoyType { get; set; }

        [Option('t', "type", MetaValue = "TYPE", Required = false, DefaultValue = DatabaseType.Concatenated, HelpText = "Type of file to generate (concat, target, decoy)")]
        public DatabaseType OutputType { get; set; }

        [Option('b', null, HelpText = "Make a BLAST database")]
        public bool BlastDatabase { get; set; }

        [Option('m',"exclude-if-meth", DefaultValue = false, HelpText = "Exclude N-Terminal if Methionine")]
        public bool ExcludeNTerminalMethionine { get; set; }

        [Option('n', null, HelpText = "Exclude N-Terminal Amino Acid Residue (requires reversed or shuffled)")]
        public bool ExcludeNTerminalResidue { get; set; }

        [ValueList(typeof(List<string>), MaximumElements = -1)]
        public IList<string> InputFiles { get; set; }

        [Option('v', null, DefaultValue = false, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
                
        public bool help { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var text = HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current), false, false);  
            text.AddPreOptionsLine("Usage: dbmaker [Input Files]... [OPTIONS] -o <Output Base Name>");      
            text.MaximumDisplayWidth = 80;
            return text;
        }
        
        
    }
}
