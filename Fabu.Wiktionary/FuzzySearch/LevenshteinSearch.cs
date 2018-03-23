using MinimumEditDistance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.FuzzySearch
{
    public class LevenshteinSearch<TVal> : IFuzzySearcher<TVal>
    {
        private readonly IEnumerable<TVal> _values;
        private readonly Func<TVal, string> _getKey;
        private readonly int _maxEdits;

        public IEnumerable<TVal> Values => _values;

        public LevenshteinSearch(IEnumerable<TVal> values, Func<TVal,string> getKey, int maxEdits)
        {
            _values = values;
            _getKey = getKey;
            _maxEdits = maxEdits;
        }

        public List<TVal> FindAll(string term)
        {
            var candidates = InnerSearch(term);
            return candidates.Select(kvp => kvp.Key).ToList();
        }

        private List<KeyValuePair<TVal, int>> InnerSearch(string term)
        {
            var candidates = new List<KeyValuePair<TVal, int>>();
            foreach (var known in Values)
            {
                var distance = Levenshtein.CalculateDistance(_getKey(known).ToLowerInvariant(), term.ToLowerInvariant(), 1);
                if (distance <= _maxEdits)
                    candidates.Add(new KeyValuePair<TVal, int>(known, distance));
            }
            return candidates.OrderBy(kvp => kvp.Value).ToList();
        }

        public List<TVal> FindBest(string term)
        {
            KeyValuePair<TVal, int> best = new KeyValuePair<TVal, int>();
            bool inited = false;
            return InnerSearch(term)
                .TakeWhile(kvp =>
                {
                    if (!inited)
                    {
                        best = kvp;
                        inited = true;
                        return true;
                    }
                    return best.Value == kvp.Value;
                }).Select(kvp => kvp.Key).ToList();
        }

        public bool TryFindBest(string term, out List<TVal> result)
        {
            result = FindBest(term);
            if (result.Count == 0)
                return false;
            return true;
        }
    }
}
