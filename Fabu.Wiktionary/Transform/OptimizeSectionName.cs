
namespace Fabu.Wiktionary.Transform
{
    internal class OptimizeSectionName : BaseTransform<string, string>
    {
        private readonly WiktionaryMeta _wiktionary;

        public OptimizeSectionName(WiktionaryMeta wiktionary = null)
        {
            _wiktionary = wiktionary ?? new WiktionaryMeta(null, null, null);
        }

        public override string Apply(string value)
        {
            if (_wiktionary.TryRemoveHints(value, out string newValue))
                value = newValue;

            if (_wiktionary.TryRemoveParentheses(value, out newValue))
                value = newValue;

            if (_wiktionary.TryCleanupSectionName(value, out newValue))
                value = newValue;

            if (_wiktionary.TryGetLanguage(value, out newValue))
                return newValue;

            if (_wiktionary.TryGetStandardSectionName(value, out newValue))
                return newValue;

            return value;
        }
    }
}
