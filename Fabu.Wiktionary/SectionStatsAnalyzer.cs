using Fabu.Wiktionary.Graph;
using Fabu.Wiktionary.Transform;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary
{
    internal class SectionStatsAnalyzer
    {
        private readonly IEnumerable<SectionVertex> _sections;
        private readonly BaseTransform<string,string> _comparableSectionName;

        public SectionStatsAnalyzer(IEnumerable<SectionVertex> sections, WiktionaryMeta wiktionary = null)
        {
            _sections = sections;
            _comparableSectionName = new ComparableSectionName(wiktionary);
        }

        internal List<SectionVertex> GetMostPopularSections()
        {
            var clean = _sections
                .GroupBy(section => _comparableSectionName.Apply(section.Title))
                .Select(group => new SectionVertex
                {
                    Title = group.OrderByDescending(section => section.Count).First().Title,
                    Count = group.Sum(section => section.Count),
                    ID = group.OrderByDescending(section => section.Count).First().Title
                })
                .OrderByDescending(v => v.Count)
                .TakeWhile(v => v.Count > 30)
                .ToList();
            return clean
                .OrderByDescending(v => v.Count)
                .TakeWhile(v => v.Count > 30)
                .ToList();
        }
    }
}