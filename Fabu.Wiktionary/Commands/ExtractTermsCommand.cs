using CommandLine;
using Fabu.Wiktionary.FuzzySearch;
using Fabu.Wiktionary.TermProcessing;
using Fabu.Wiktionary.Transform;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.Commands
{
    class ExtractTermsCommand : BaseCommand<ExtractTermsCommand.Args>
    {
        [Verb("extract", HelpText = "Extract language names")]
        public class Args : BaseArgs
        {
            [Option("term", Required = false, HelpText = "A single term to process.")]
            public string Term { get; set; }
        }

        // extract --in enwiktionary-20180120-pages-articles.xml
        protected override void RunCommand(Args args, Func<int, BaseArgs, bool> onProgress)
        {
            var sections = DumpTool.LoadDump<List<SectionName>>(args.DumpDir, DumpTool.SectionsDictDump);
            var sectionsSearch = new ReverseLevenshteinSearch(sections);
            var languageNames = DumpTool.LoadDump<List<SectionName>>(args.DumpDir, DumpTool.LanguagesDump);
            var languageSearch = new IgnoreCaseSearch<SectionName>(languageNames, _ => _.Name, new SectionNameComparer());

            var termDefiners = sections.OrderByDescending(s => s.Weight).Take(5).ToArray();

            var transform = new FixTyposSectionName(languageSearch, sectionsSearch, true);

            var wiktionaryDump = DumpTool.LoadWikimediaDump(args.DumpDir, args.WiktionaryDumpFile);
            var processor = new TermGraphProcessor(transform, termDefiners);
            var extractor = new WiktionaryTermExtractor(processor, args.Term);
            var analyzer = new WiktionaryAnalyzer(extractor, wiktionaryDump);
            if (onProgress != null)
                analyzer.PageProcessed += (sender, e) => e.Abort = onProgress(e.Index, args);
            analyzer.Compute();
            Console.WriteLine($"Terms defined: {extractor.DefinedTerms.Count}");
        }
    }
}
