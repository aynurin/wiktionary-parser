using System;
using System.Collections.Generic;
using System.Text;

namespace Fabu.Wiktionary.FuzzySearch
{
    public interface IFuzzySearcher<TVal>
    {
        List<TVal> FindBest(string term);
        bool TryFindBest(string term, out List<TVal> result);
        List<TVal> FindAll(string term);
    }
}
