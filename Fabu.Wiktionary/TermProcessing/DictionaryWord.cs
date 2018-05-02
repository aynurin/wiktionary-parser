using System.Collections.Generic;

namespace Fabu.Wiktionary.TermProcessing
{
    internal class DictionaryWord
    {
        private readonly string _title;
        private readonly List<Term> _terms;

        public DictionaryWord(string title, List<Term> termsDefined)
        {
            _title = title;
            _terms = termsDefined;
        }

        public List<Term> Terms => _terms;

        public string Title => _title;
    }
}