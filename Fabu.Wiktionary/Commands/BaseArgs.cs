using CommandLine;

namespace Fabu.Wiktionary.Commands
{
    internal class BaseArgs
    {
        [Option("in", Required = true, HelpText = "Wiktionary dump to be processed.")]
        public string WiktionaryDumpFile { get; set; }
        [Option("out", Required = true, HelpText = "Output file.")]
        public string OutputFile { get; set; }
        [Option("limit", Required = false, HelpText = "Limit number of processed pages.", Default = -1)]
        public int LimitPages { get; internal set; }

        //[Option('r', "read", Required = true, HelpText = "Input files to be processed.")]
        //public IEnumerable<string> InputFiles { get; set; }

        //// Omitting long name, defaults to name of property, ie "--verbose"
        //[Option(Default = false, HelpText = "Prints all messages to standard output.")]
        //public bool Verbose { get; set; }

        //[Option("stdin", Default = false, HelpText = "Read from stdin")]
        //public bool stdin { get; set; }

        //[Value(0, MetaName = "offset", HelpText = "File offset.")]
        //public long? Offset { get; set; }
    }
}
