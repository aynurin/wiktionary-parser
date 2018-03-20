using CommandLine;
using Fabu.Wiktionary.FuzzySearch;
using Fabu.Wiktionary.Graph;
using Fabu.Wiktionary.Transform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fabu.Wiktionary.Commands
{
    internal class SectionGraphCommand : BaseCommand<SectionGraphCommand.Args>
    {
        [Verb("graph", HelpText = "Extract language names")]
        public class Args : BaseArgs
        {
            [Option("onlystandard", Required = false, Default = true, HelpText = "Keep only the sections that could be matched to standard.")]
            public bool KeepOnlyStandardSections { get; set; }
            [Option("minfreq", Required = false, Default = 2, HelpText = "Minimum edge frequency in graph.")]
            public int MinimumEdgeFrequency { get; set; }
        }

        // graph --in enwiktionary-20180120-pages-articles.xml --minfreq 2
        protected override void RunCommand(Args args, Func<int, BaseArgs, bool> onProgress)
        {
            var languages = LoadLanguages(args.DumpDir);
            var sections = LoadSections(args.DumpDir);
            var savedSections = new List<SectionName>(sections.Dict);
            
            ClearSectionStats(savedSections);

            var transform = new GraphVertexSectionName(languages, sections, args.KeepOnlyStandardSections);
            var graphBuilder = new GraphBuilder(transform, savedSections);

            // TODO NOW: If Leva won't find a section in sections, user has to decide whether to add this section to the graph or not.

            var wiktionaryDump = DumpTool.LoadWikimediaDump(args.DumpDir, args.WiktionaryDumpFile);
            var analyzer = new WiktionaryAnalyzer(graphBuilder, wiktionaryDump, args.MinimumEdgeFrequency);
            if (onProgress != null)
                analyzer.PageProcessed += (sender, e) => e.Abort = onProgress(e.Index, args);
            analyzer.Compute();

            using (var file = File.CreateText(Path.Combine(args.DumpDir, DumpTool.SectionsGraph + ".json")))
                graphBuilder.Serialize(file);

            Console.WriteLine();

            PrintSkeptItemsStats(graphBuilder);
            
            CleanupSectionStats(savedSections);

            DumpTool.SaveDump(args.DumpDir, DumpTool.SectionsDictDump, savedSections);
        }

        private static void CleanupSectionStats(List<SectionName> sections)
        {
            sections.ForEach(s => s.Parents = s.Parents.CutOff(0.001));
            sections.ForEach(s => s.Children = s.Children.CutOff(0.001));
            sections.ForEach(s => s.DepthStats = s.DepthStats.CutOff(0.001));
        }

        private static void ClearSectionStats(List<SectionName> sections)
        {
            sections.RemoveAll(section => 
                section.Name == SimpleSectionsCategorizer.RootSectionName || 
                section.Name == SimpleSectionsCategorizer.LanguageSectionName);
            sections.ForEach(s => s.Parents.Clear());
            sections.ForEach(s => s.Children.Clear());
            sections.ForEach(s => s.DepthStats.Clear());
        }

        private static void PrintSkeptItemsStats(GraphBuilder graphBuilder)
        {
            Console.WriteLine("Skept items stats: ");
            Console.WriteLine("Less than 10 mentions: " + graphBuilder.NamesSkept.Count(s => s.Value < 10));
            Console.WriteLine("Less than 50 mentions: " + graphBuilder.NamesSkept.Count(s => s.Value >= 10 && s.Value < 50));
            Console.WriteLine("More than 50 mentions: " + graphBuilder.NamesSkept.Count(s => s.Value >= 50));
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
