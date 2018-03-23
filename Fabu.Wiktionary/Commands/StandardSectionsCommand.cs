using CommandLine;
using Fabu.Wiktionary.FuzzySearch;
using Fabu.Wiktionary.Graph;
using Fabu.Wiktionary.Transform;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.Commands
{
    internal class StandardSectionsCommand : BaseCommand<StandardSectionsCommand.Args>
    {
        [Verb("sectionsdict", HelpText = "Extract section names")]
        public class Args : BaseArgs
        {
        }

        private readonly string[] _additionalStandardSections = new string[]
        {
            // Wiktionary recommendation is to use Homographs and Homophones instead, that's why it doesn't make it into statistics. 
            // But if it's not here then it easily makes it into Holonyms's misspellings, which I definitely don't want to.
            "Homonyms"
        };

        // sectionsdict
        protected override void RunCommand(Args args, Func<int, BaseArgs, bool> onProgress)
        {
            var searchableSectionName = new SearchableSectionName();

            var allSections = DumpTool.LoadDump<List<SectionName>>(args.DumpDir, DumpTool.SectionsDump)
                // so that when a new standard section is created, it is created from the most frequent term
                .OrderByDescending(v => v.Weight)
                //.Select(s => new Tuple<string,SectionName> (searchableSectionName.Apply(s).Na, s))
                .ToList();

            var standardSections = allSections
                .GroupBy(_ => searchableSectionName.Apply(_).Name,
                        (key, group) => new SectionName
                        {
                            Name = group.OrderByDescending(_ => _.Weight).First().Name,
                            Weight = group.Sum(_ => _.Weight)
                        })
                .OrderByDescending(_ => _.Weight)
                .TakeWhile(_ => _.Weight > 100)
                .ToList();
            var additionalStandard = allSections.Where(s => Array.BinarySearch(_additionalStandardSections, s.Name) >= 0 && !standardSections.Any(_ => _.Name == s.Name));
            standardSections.AddRange(additionalStandard);

            var reducedSectionsList = ReduceTyposFuzzySearch(allSections, standardSections, searchableSectionName)
                .OrderByDescending(_ => _.Weight)
                .TakeWhile(_ => _.Weight > 30)
                .ToList();

            DumpTool.SaveDump(args.DumpDir, "sections_dict", reducedSectionsList);
        }

        private static List<SectionName> ReduceTyposFuzzySearch(
            List<SectionName> allSections, List<SectionName> standardSections,
            SectionNameTransform searchableSectionName)
        {
            var mappingResult = new Dictionary<string, SectionName>();

            var searchImpl = new LevenshteinSearch<SectionName>(standardSections, 
                _ => searchableSectionName.Apply(_).Name, 2);

            foreach (var section in allSections)
            {
                var searchableName = searchableSectionName.Apply(section);
                var candidate = searchImpl.FindBest(searchableName.Name)
                    .OrderByDescending(s => s.Weight)
                    .FirstOrDefault();
                // if none found, add unknowns to the search and to the result so that they can 
                // attach new unknowns to themselves using Levenshtein
                if (candidate == null)
                {
                    standardSections.Add(section);
                    candidate = section;
                }

                var candidateName = searchableSectionName.Apply(candidate);
                if (!mappingResult.TryGetValue(candidateName.Name, out SectionName sections))
                {
                    sections = new SectionName { Name = candidate.Name };
                    mappingResult.Add(candidateName.Name, sections);
                }
                sections.AddSpelling(searchableName.Name, section.Weight);
            }
            return mappingResult.Values.ToList();
        }
    }
}
