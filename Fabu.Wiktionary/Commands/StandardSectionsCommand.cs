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
    internal class StandardSectionsCommand : BaseCommand<StandardSectionsCommand.Args>
    {
        [Verb("standardsections", HelpText = "Extract section names")]
        public class Args : BaseArgs
        {
            [Option("sections", Required = true, HelpText = "Section names file.")]
            public string AllSectionsFile { get; set; }
            [Option("weights", Required = false, HelpText = "Section names file.")]
            public string SectionWeightsFile { get; set; }
            [Option("brief", Required = false, HelpText = "Section names file.")]
            public bool BriefOutput { get; set; }
        }

        // standardsections --in C:\repos\data\fabu\enwiktionary-20180120-pages-articles.xml --sections c:\repos\data\fabu\sections.tsv --weights c:\repos\data\fabu\weights.tsv --out c:\repos\data\fabu\matches.tsv
        // standardsections --in C:\repos\data\fabu\enwiktionary-20180120-pages-articles.xml --sections c:\repos\data\fabu\sections.tsv --out c:\repos\data\fabu\matches.tsv
        protected override void RunCommand(Args args, Func<int, BaseArgs, bool> onProgress)
        {
            var allSections = DumpTool.LoadSectionWeights(args.AllSectionsFile)
                // so that when a new standard section is created, it is created from the most frequent term
                .OrderByDescending(v => v.Weight) 
                .ToList();

            var transform = new OptimizeSectionName();
            var standardSections = new List<SectionWeight>();
            var autoGenerateStandardSections = false;

            if(!String.IsNullOrEmpty(args.SectionWeightsFile))
            {
                standardSections = DumpTool.LoadSectionWeights(args.SectionWeightsFile);
            }
            else
            {
                autoGenerateStandardSections = true;
                Console.WriteLine("Standard setions not provided, will auto-generate? (Ctrl+C to abort)");
                Console.ReadLine();
                foreach (var section in allSections)
                {
                    section.Name = transform.Apply(section.Name);
                }
                var sectionAnalyzer = new SectionStatsAnalyzer(
                    allSections.Select(s => new SectionVertex { Title = s.Name, Count = (int)s.Weight }));
                standardSections = sectionAnalyzer.GetMostPopularSections()
                    .OrderByDescending(v => v.Count)
                    .Select(v => new SectionWeight(v.Title, v.Count))
                    .ToList();
            }



            var mappingResult = new Dictionary<string, List<SectionWeight>>();
            var unknown = new List<SectionWeight>();

            var searchImpl = new LevenshteinSearch<SectionWeight>(standardSections, _ => _.Name, 2);
            foreach (var section in allSections)
            {
                var candidates = searchImpl.FindBest(section.Name);
                // if none found, add unknowns to the search and to the result so that they can 
                // attach new unknowns to themselves using Levenshtein
                if (candidates.Count == 0)
                {
                    if (autoGenerateStandardSections)
                    {
                        standardSections.Add(section);
                        candidates.Add(section);
                    }
                    else
                    {
                        unknown.Add(section);
                    }
                }
                foreach (var candidate in candidates)
                {
                    if (!mappingResult.TryGetValue(candidate.Name, out List<SectionWeight> sections))
                    {
                        sections = new List<SectionWeight>();
                        mappingResult.Add(candidate.Name, sections);
                    }
                    sections.Add(section);
                }
            }
            if (unknown.Count > 0)
                mappingResult.Add("UNKNOWN", unknown);

            using (var file = File.CreateText(args.OutputFile))
            {
                foreach (var item in mappingResult)
                {
                    if (args.BriefOutput)
                    {
                        var mentions = allSections.Find(v => v.Name == item.Key).Weight + 
                            item.Value.Sum(w => w.Weight);
                        if (mentions > 5)
                        {
                            file.Write(item.Key);
                            file.Write('\t');
                            file.Write(mentions);
                            file.WriteLine();
                        }
                    }
                    else
                    {
                        file.Write(item.Key);
                        foreach (var value in item.Value)
                        {
                            file.Write('\t');
                            file.Write(value.Name);
                            file.Write('\t');
                            file.Write(value.Weight);
                            file.WriteLine();
                        }
                    }
                }
                file.Flush();
            }
        }
    }
}
