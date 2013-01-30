using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace Coon.Compass.DatabaseMaker
{
    public class DatabaseMakerOptions
    {     
        [Option('o', "output", MetaValue="FILE", Required = true, HelpText = "The base name of the FILE to write the fasta file to")]
        public string OutputFastaFile { get; set; }

        [Option('u', "uniprot", DefaultValue = true, HelpText = "Use standard Uniprot heading in database")]
        public bool EnforceUniprot { get; set; }

        [Option('p', "prefix", MetaValue="STRING", Required = false, DefaultValue = "DECOY_", HelpText = "Decoy prefix to add to the front of the protein description")]
        public string DecoyPrefix { get; set; }

        [Option('d', "decoy", MetaValue = "TYPE", Required = false, DefaultValue = DecoyDatabaseMethod.Reverse, HelpText = "How to generate a decoy database (Options: reverse, shuffle, random)")]
        public DecoyDatabaseMethod DecoyType { get; set; }

        [Option('t', "type", MetaValue = "TYPE", Required = false, DefaultValue = DatabaseType.Concatenated, HelpText = "Type of file to generate (concat, target, decoy)")]
        public DatabaseType OutputType { get; set; }

        [Option('b', null, HelpText = "Make a BLAST database")]
        public bool MakeBlastDatabase { get; set; }

        [Option('m', null, HelpText = "Exclude N-Terminal if Methionine (requires -x)")]
        public bool ExcludeNTerminalMethionine { get; set; }

        [Option('x', null, HelpText = "Exclude N-Terminal Amino Acid Residue (requires reversed or shuffled)")]
        public bool ExcludeNTerminalResidue { get; set; }

        [ValueList(typeof(List<string>), MaximumElements = -1)]
        public IList<string> InputFiles { get; set; }

        [Option('v', null, DefaultValue = true, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }
               
        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var text = HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current), false, false);  
            text.AddPreOptionsLine("Usage: dbmaker [Input Files]... [OPTIONS] -o <Output Base Name>");      
            text.MaximumDisplayWidth = 80;
            return text;
            //var help = new HelpText
            //{
            //    Heading = new HeadingInfo("<>", "<>"),
            //    Copyright = new CopyrightInfo("<>", 2013),
            //    AdditionalNewLineAfterOption = false,
            //    AddDashesToOption = true
            //};

            //help.AddPreOptionsLine("<>");
            //help.MaximumDisplayWidth = 60;
            //help.AddPreOptionsLine("Usage: app -pSomeone");
            //help.AddOptions(this);
       
            ////if (this.LastPostParsingState.Errors.Count > 0)
            ////{
            ////    var errors = help.RenderParsingErrorsText(this, 2);
            ////    if (!string.IsNullOrEmpty(errors))
            ////    {
            ////        help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
            ////        help.AddPreOptionsLine(errors);
            ////    }
            ////}
            //return help;
        }
        

    }
}
