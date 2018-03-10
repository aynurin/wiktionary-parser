using CommandLine;
using Fabu.Wiktionary.FuzzySearch;
using Fabu.Wiktionary.Graph;
using Fabu.Wiktionary.Transform;
using System;
using System.IO;
using System.Linq;

namespace Fabu.Wiktionary.Commands
{
    internal class AllSectionsCommand : BaseCommand<AllSectionsCommand.Args>
    {
        [Verb("sectionnames", HelpText = "Extract section names")]
        public class Args : BaseArgs
        {
            [Option("langs", Required = true, HelpText = "Language names file.")]
            public string LanguagesFile { get; set; }
        }

        // sectionnames --in C:\repos\data\fabu\enwiktionary-20180120-pages-articles.xml --langs c:\repos\data\fabu\languages.tsv --out c:\repos\data\fabu\sections.tsv
        protected override void RunCommand(Args args, Func<int, BaseArgs, bool> onProgress)
        {
            var transform = new TrimSectionNames();
            var rawLanguageNames = DumpTool.LoadLanguages(args.LanguagesFile);
            var languageNames = new IgnoreCaseSearch<LanguageWeight>(rawLanguageNames, _ => _.Name, new LanguageWeightComparer());
            var wiktionaryDump = DumpTool.LoadWikimediaDump(args.WiktionaryDumpFile);
            var wiktionary = new WiktionaryMeta(languageNames, null, null);
            var graphBuilder = new GraphBuilder(transform, wiktionary);
            var analyzer = new WiktionaryAnalyzer(graphBuilder, wiktionaryDump);

            if (onProgress != null)
                analyzer.PageProcessed += (sender, e) => e.Abort = onProgress(e.Index, args);
            analyzer.GetStatistics();

            using (var file = File.CreateText(args.OutputFile))
            {
                graphBuilder.AllSections
                    .OrderBy(section => section.Title).ToList()
                    .ForEach(v => v.WriteAsTsv(file, 0));
            }
        }
    }
}
