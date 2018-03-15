using Fabu.Wiktionary.FuzzySearch;
using System.Collections.Generic;

namespace Fabu.Wiktionary.Transform
{
    /// <summary>
    /// Applies <see cref="NormalizeSectionName"/> normalization, then makes all lowercase,
    /// removes spaces and 's' in the end to make the term more generic for string lookup
    /// </summary>
    internal class SearchableSectionName : NormalizeSectionName
    {
        /// <summary>
        /// Applies <see cref="NormalizeSectionName"/> normalization, then makes all lowercase,
        /// removes spaces and 's' in the end to make the term more generic for string lookup
        /// </summary>
        public override SectionName Apply(SectionName sectionName)
        {
            var name = base.Apply(sectionName);
            return name.CloneWithName(
                name.Name
                .ToLowerInvariant()
                .Trim()
                .TrimEnd('s'));
        }
    }
}
