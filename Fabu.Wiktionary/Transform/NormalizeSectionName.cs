
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Fabu.Wiktionary.Transform
{
    /// <summary>
    /// Removes unnecessary symbols from section names, 
    /// e.g. "Etymology 23 (something) &lt!-- something else --&gt;" becomes "Etymology"
    /// </summary>
    internal abstract class NormalizeSectionName : SectionNameTransform
    {
        /// <summary>
        /// Removes unnecessary symbols from section names, 
        /// e.g. "Etymology 23 (something) &lt!-- something else --&gt;" becomes "Etymology"
        /// </summary>
        public override SectionName Apply(SectionName sectionName)
        {
            var value = sectionName.Name;

            if (TryRemoveHints(value, out string newValue))
                value = newValue;

            if (TryRemoveParentheses(value, out newValue))
                value = newValue;

            if (TryCleanupSectionName(value, out newValue))
                value = newValue;

            if (value != sectionName.Name)
                return sectionName.CloneWithName(value);
            return sectionName;
        }

        private readonly Regex CleanupSectionName = new Regex(@"^[\W_]*(.+?)[\W\d_]*$", RegexOptions.Compiled);

        /// <summary>
        /// Removes all none-alphanumeric characters from the beginning of the string, and all non-alpha characters from the end
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryCleanupSectionName(string value, out string newValue)
        {
            var cleanup = CleanupSectionName.Match(value);
            if (cleanup.Success)
            {
                newValue = cleanup.Groups[1].Value.Trim();
                return true;
            }

            Debug.Assert(false);

            newValue = null;
            return false;
        }

        private readonly Regex SectionNameHint = new Regex(@"<!--[^>]*>", RegexOptions.Compiled);

        /// <summary>
        /// For names like "Etymology&lt;!-- Something links here--&gt;" returns "Etymology"
        /// </summary>
        /// <returns></returns>
        public bool TryRemoveHints(string value, out string newValue)
        {
            newValue = SectionNameHint.Replace(value, String.Empty);
            var changed = value != newValue;
            if (!changed)
                newValue = null;
            return changed;
        }

        private readonly Regex Parentheses = new Regex(@"\s*\([^\)]*\)\s*", RegexOptions.Compiled);

        /// <summary>
        /// For names like "Declension (masc)" returns "Declension"
        /// </summary>
        /// <returns></returns>
        public bool TryRemoveParentheses(string value, out string newValue)
        {
            newValue = Parentheses.Replace(value, " ").Trim();
            if (String.IsNullOrEmpty(newValue))
            {
                newValue = null;
                return false;
            }
            var changed = value != newValue;
            if (!changed)
                newValue = null;
            return changed;
        }
    }
}
