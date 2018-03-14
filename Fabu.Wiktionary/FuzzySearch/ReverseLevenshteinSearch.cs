using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.FuzzySearch
{
    public class ReverseLevenshteinSearch : IFuzzySearcher<SectionName>
    {
        private readonly Dictionary<string, List<SectionName>> _hashset = new Dictionary<string, List<SectionName>>();

        public List<SectionName> Dict { get; }

        public ReverseLevenshteinSearch(List<SectionName> sectionsDict)
        {
            foreach (var sect in sectionsDict)
            {
                foreach (var spelling in sect.Spellings)
                {
                    var spellingLower = spelling.ToLower();
                    if (!_hashset.TryGetValue(spellingLower, out List<SectionName> items))
                    {
                        items = new List<SectionName>();
                        _hashset.Add(spellingLower, items);
                    }
                    items.Add(sect);
                }
            }

            Dict = sectionsDict;
        }

        private List<SectionName> InnerSearch(string term)
        {
            if (_hashset.TryGetValue(term.ToLower(), out List<SectionName> values))
                return values;
            return new List<SectionName>();
        }

        public List<SectionName> FindAll(string term) => InnerSearch(term);

        public List<SectionName> FindBest(string term) => InnerSearch(term);

        public bool TryFindBest(string term, out List<SectionName> result)
        {
            result = FindBest(term);
            if (result.Count == 0)
                return false;
            return true;
        }
    }
}
