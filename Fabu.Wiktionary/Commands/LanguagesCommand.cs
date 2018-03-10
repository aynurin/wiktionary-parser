using CommandLine;
using Fabu.Wiktionary.Graph;
using Fabu.Wiktionary.Transform;
using System;
using System.IO;

namespace Fabu.Wiktionary.Commands
{
    internal class LanguagesCommand : BaseCommand<LanguagesCommand.Args>
    {
        [Verb("lang", HelpText = "Extract language names")]
        public class Args : BaseArgs { }

        // lang --in C:\repos\data\fabu\enwiktionary-20180120-pages-articles.xml --out c:\repos\data\fabu\languages.tsv
        protected override void RunCommand(Args args, Func<int, BaseArgs, bool> onProgress)
        {
            var transform = new TrimSectionNames();
            var wiktionaryDump = DumpTool.LoadWikimediaDump(args.WiktionaryDumpFile);
            var wiktionary = new WiktionaryMeta(null, null, null);
            var graphBuilder = new GraphBuilder(transform, wiktionary);
            var analyzer = new WiktionaryAnalyzer(graphBuilder, wiktionaryDump);
            if (onProgress != null)
                analyzer.PageProcessed += (sender, e) => e.Abort = onProgress(e.Index, args);
            analyzer.GetStatistics();
            using (var file = File.CreateText(args.OutputFile))
                graphBuilder.LanguageNames.ForEach(v => v.WriteAsTsv(file, 0));
        }
    }
}
