using CommandLine;
using Fabu.Wiktionary.FuzzySearch;
using Fabu.Wiktionary.Graph;
using Fabu.Wiktionary.Transform;
using System;
using System.IO;

namespace Fabu.Wiktionary.Commands
{
    internal class SectionGraphCommand : BaseCommand<SectionGraphCommand.Args>
    {
        [Verb("sectionsgraph", HelpText = "Extract language names")]
        public class Args : BaseArgs
        {
            [Option("langs", Required = true, HelpText = "Language names file.")]
            public string LanguagesFile { get; set; }
            [Option("types", Required = true, HelpText = "Section names file.")]
            public string SectionTypesFile { get; set; }
            [Option("weights", Required = true, HelpText = "Section names file.")]
            public string SectionWeightsFile { get; set; }
        }

        // sectionsgraph --in C:\repos\data\fabu\enwiktionary-20180120-pages-articles.xml --langs c:\repos\data\fabu\languagenames.tsv --types c:\repos\data\fabu\types.tsv --weights c:\repos\data\fabu\weights.tsv --out c:\repos\data\fabu\wiktionary_graph.json
        protected override void RunCommand(Args args, Func<int, BaseArgs, bool> onProgress)
        {
            var languageNames = DumpTool.LoadLanguages(args.LanguagesFile);
            var languageSearch = new IgnoreCaseSearch<LanguageWeight>(languageNames, _ => _.Name, new LanguageWeightComparer());
            var sectionWeights = DumpTool.LoadSectionWeights(args.SectionWeightsFile);
            var sectionsSearch = new LevenshteinSearch<SectionWeight>(sectionWeights, _ => _.Name, 3);
            var sectionTypes = DumpTool.LoadSectionTypes(args.SectionTypesFile);
            var sectionTypesSearch = new IgnoreCaseSearch<SectionType>(sectionTypes, _ => _.Name, new SectionTypeComparer());

            var wiktionaryMeta = new WiktionaryMeta(languageSearch, sectionsSearch, sectionTypesSearch);
            var transform = new SectionClass(wiktionaryMeta);
            var graphBuilder = new GraphBuilder(transform, wiktionaryMeta);
            var wiktionaryDump = DumpTool.LoadWikimediaDump(args.WiktionaryDumpFile);
            var analyzer = new WiktionaryAnalyzer(graphBuilder, wiktionaryDump);
            if (onProgress != null)
                analyzer.PageProcessed += (sender, e) => e.Abort = onProgress(e.Index, args);
            analyzer.GetStatistics();
            using (var file = File.CreateText(args.OutputFile))
                graphBuilder.Serialize(file);
        }
    }
}
