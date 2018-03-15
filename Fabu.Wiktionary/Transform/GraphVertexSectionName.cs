
using Fabu.Wiktionary.FuzzySearch;
using Fabu.Wiktionary.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fabu.Wiktionary.Transform
{
    /// <summary>
    /// Applies <see cref="FixTyposSectionName"/> normalization, then searches the name 
    /// in known languages and sections and replaces with section classes where specified
    /// </summary>
    internal class GraphVertexSectionName : FixTyposSectionName
    {
        private readonly IFuzzySearcher<SectionName> _knownLanguages;
        private readonly bool _keepOnlyMatchedNames;
        private readonly Dictionary<string, SectionName> _categorySections = new Dictionary<string, SectionName>();

        public GraphVertexSectionName(
            IFuzzySearcher<SectionName> knownLanguages,
            IFuzzySearcher<SectionName> knownSections, 
            bool keepOnlyStandardSections)
            : base(knownLanguages, knownSections, keepOnlyStandardSections)
        {
            _knownLanguages = knownLanguages;
            _keepOnlyMatchedNames = keepOnlyStandardSections;
        }

        /// <summary>
        /// Applies <see cref="FixTyposSectionName"/> normalization, then searches the name 
        /// in known languages and sections and replaces with section classes where specified
        /// </summary>
        public override SectionName Apply(SectionName sectionName)
        {
            var normalName = base.Apply(sectionName);
            
            if (_keepOnlyMatchedNames && normalName == null)
                return null;

            Debug.Assert(normalName != null && !String.IsNullOrWhiteSpace(normalName.Name));

            if (TryGetSectionClass(normalName, out SectionName newValue))
                normalName = newValue;

            Debug.Assert(normalName != null && !String.IsNullOrWhiteSpace(normalName.Name));

            return normalName;
        }

        /// <summary>
        /// Produces a section class based on section name.
        /// </summary>
        /// <remarks>
        /// E.g. returns "LANGUAGE_NAME" for "English"
        /// </remarks>
        public bool TryGetSectionClass(SectionName section, out SectionName newValue)
        {
            if (section.Category != null)
            {
                if (!_categorySections.TryGetValue(section.Category, out newValue))
                {
                    newValue = section.CloneWithName(section.Category);
                    _categorySections.Add(section.Category, newValue);
                }
                return true;
            }
            if (_knownLanguages != null && _knownLanguages.TryFindBest(section.Name, out List<SectionName> result))
            {
                newValue = SectionVertex.Lang.OriginalSection;
                return true;
            }
            newValue = null;
            return false;
        }
    }
}
