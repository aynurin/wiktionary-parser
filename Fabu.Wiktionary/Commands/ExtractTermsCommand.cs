using CommandLine;
using Fabu.Wiktionary.FuzzySearch;
using Fabu.Wiktionary.TermProcessing;
using Fabu.Wiktionary.Transform;
using System;
using System.Collections.Generic;

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
            var languages = LoadLanguages(args.DumpDir);
            var sections = LoadSections(args.DumpDir);

            var transform = new FixTyposSectionName(languages, sections, true);

            var wiktionaryDump = DumpTool.LoadWikimediaDump(args.DumpDir, args.WiktionaryDumpFile);
            var extractor = new WiktionaryTermExtractor(transform, args.Term);
            var analyzer = new WiktionaryAnalyzer(extractor, wiktionaryDump);
            if (onProgress != null)
                analyzer.PageProcessed += (sender, e) => e.Abort = onProgress(e.Index, args);
            analyzer.Compute();
        }

        internal static ReverseLevenshteinSearch LoadSections(string dir)
        {
            var sections = DumpTool.LoadDump<List<SectionName>>(dir, DumpTool.SectionsDictDump);
            var sectionsSearch = new ReverseLevenshteinSearch(sections);
            return sectionsSearch;
        }

        internal static IgnoreCaseSearch<SectionName> LoadLanguages(string dir)
        {
            var languageNames = DumpTool.LoadDump<List<SectionName>>(dir, DumpTool.LanguagesDump);
            var languageSearch = new IgnoreCaseSearch<SectionName>(languageNames, _ => _.Name, new SectionNameComparer());
            return languageSearch;
        }
    }
}
