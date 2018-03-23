using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fabu.Wiktionary.FuzzySearch;

namespace Fabu.Wiktionary.Transform
{
    /// <summary>
    /// Applies <see cref="NormalizeSectionName"/> normalization, then fixes typos in names
    /// based on the known sections dictionary. E.g. for "adjektiv" returnes "Adjective"
    /// </summary>
    internal class FixTyposSectionName : NormalizeSectionName
    {
        private readonly bool _keepOnlyStandardSections;
        private readonly IFuzzySearcher<SectionName> _knownLanguages;
        private readonly IFuzzySearcher<SectionName> _knownSections;
        private readonly SearchableSectionName _searchableSectionName = new SearchableSectionName();

        public FixTyposSectionName(IFuzzySearcher<SectionName> knownLanguages, 
            IFuzzySearcher<SectionName> knownSections, bool keepOnlyStandardSections)
        {
            _knownLanguages = knownLanguages;
            _knownSections = knownSections;
            _keepOnlyStandardSections = keepOnlyStandardSections;
        }

        /// <summary>
        /// Applies <see cref="NormalizeSectionName"/> normalization, then fixes typos in names
        /// based on the known sections dictionary. E.g. for "adjektiv" returnes "Adjective"
        /// </summary>
        public override SectionName Apply(SectionName sectionName)
        {
            if (TryGetLanguage(sectionName, out SectionName lang))
                return lang;

            var normalName = base.Apply(sectionName);

            Debug.Assert(normalName != null && !String.IsNullOrWhiteSpace(normalName.Name));

            if (TryGetStandardSectionName(normalName, out SectionName sect))
                return sect;


            if (_keepOnlyStandardSections)
                return null;

            return normalName;
        }

        /// <summary>
        /// Removes case ambiguity (e.g. if <paramref name="value"/> is all lowercase)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool TryGetStandardSectionName(SectionName value, out SectionName newValue)
        {
            if (_knownSections == null)
            {
                newValue = null;
                return false;
            }
            value = _searchableSectionName.Apply(value);
            if (_knownSections.TryFindBest(value.Name, out List<SectionName> result))
            {
                newValue = result.First();
                return true;
            }
            newValue = null;
            return false;
        }

        public bool TryGetLanguage(SectionName name, out SectionName newValue)
        {
            if (_knownLanguages == null)
            {
                newValue = null;
                return false;
            }
            if (_knownLanguages.TryFindBest(name.Name, out List<SectionName> result))
            {
                newValue = result.First();
                newValue.IsLanguage = true;
                return true;
            }
            newValue = null;
            return false;
        }
    }
}
