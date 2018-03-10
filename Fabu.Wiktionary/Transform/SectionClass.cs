
using System;
using System.Diagnostics;

namespace Fabu.Wiktionary.Transform
{
    internal class SectionClass : BaseTransform<string, string>
    {
        private readonly WiktionaryMeta _wiktionary;
        private readonly BaseTransform<string, string> _previous;

        public SectionClass(WiktionaryMeta wiktionary)
        {
            _wiktionary = wiktionary;
            _previous = new OptimizeSectionName(wiktionary);
        }

        public override string Apply(string value)
        {
            value = _previous.Apply(value);

            if (_wiktionary.TryGetSectionClass(value, out string newValue))
                value = newValue;

            Debug.Assert(!String.IsNullOrWhiteSpace(value));

            return value;
        }
    }
}
