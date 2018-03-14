using MinimumEditDistance;
using Phonix;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.FuzzySearch
{
    public class IgnoreCaseSearch<TVal> : IFuzzySearcher<TVal>
    {
        private readonly Dictionary<string, List<TVal>> _innerDict
            = new Dictionary<string, List<TVal>>(
                new IgnoreCaseStringComparer());
        private readonly IComparer<TVal> _rankingComparer;

        public IgnoreCaseSearch(IEnumerable<TVal> sourceData, Func<TVal,string> getKey, IComparer<TVal> rankingComparer)
        {
            var dict = new Dictionary<string, List<TVal>>(
                new IgnoreCaseStringComparer());
            foreach (var item in sourceData)
            {
                if (!dict.TryGetValue(getKey(item), out List<TVal> values))
                {
                    values = new List<TVal>();
                    dict.Add(getKey(item).Trim(), values);
                }
                values.Add(item);
            }
            foreach (var kvp in dict)
            {
                _innerDict.Add(kvp.Key,
                    kvp.Value.OrderBy(val => val, rankingComparer)
                    .ToList());
            }

            _rankingComparer = rankingComparer;
        }

        public List<TVal> FindAll(string term)
        {
            if (_innerDict.TryGetValue(term.Trim(), out List<TVal> values))
                return values;
            return new List<TVal>();
        }

        public List<TVal> FindBest(string term)
        {
            TVal best = default(TVal);
            bool inited = false;
            return FindAll(term)
                .TakeWhile(val =>
                {
                    if (!inited)
                    {
                        best = val;
                        inited = true;
                        return true;
                    }
                    return _rankingComparer.Compare(best, val) == 0;
                }).ToList();
        }

        public bool TryFindBest(string term, out List<TVal> result)
        {
            result = FindBest(term);
            if (result.Count == 0)
                return false;
            return true;
        }
    }

    class IgnoreCaseStringComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y) => String.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);

        public int GetHashCode(string obj) => obj.ToLowerInvariant().GetHashCode();
    }
}
