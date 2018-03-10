using System;
using System.Collections.Generic;
using System.Text;

namespace Fabu.Wiktionary.Transform
{
    internal class ComparableSectionName : BaseTransform<string, string>
    {
        private readonly WiktionaryMeta _wiktionary;

        public ComparableSectionName(WiktionaryMeta wiktionary = null)
        {
            _wiktionary = wiktionary ?? new WiktionaryMeta(null, null, null);
        }

        public override string Apply(string value)
        {
            if (_wiktionary.TryRemoveHints(value, out string newValue))
                value = newValue;

            if (_wiktionary.TryCleanupSectionName(value, out newValue))
                value = newValue;

            return value
                .ToLowerInvariant()
                .Trim()
                .TrimEnd('s');
        }
    }
}
