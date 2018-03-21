using System;

namespace Fabu.Wiktionary.TermProcessing
{
    internal class ProcessingMode
    {
        public string[] AllowedTermModelSubSections;
        public bool AllowWiktionaryChildrenProcessing;
        public bool MayDefineTerm;

        public readonly static ProcessingMode CanDefineTerm = new ProcessingMode
        {
            AllowedTermModelSubSections = null,
            AllowWiktionaryChildrenProcessing = true,
            MayDefineTerm = true
        };
        public readonly static ProcessingMode ChildSection = new ProcessingMode
        {
            AllowedTermModelSubSections = null,
            AllowWiktionaryChildrenProcessing = false,
            MayDefineTerm = false
        };
        public readonly static ProcessingMode PosOrSimilar = new ProcessingMode
        {
            AllowedTermModelSubSections = new [] { "Quotations", "Synonyms", "Usage notes" },
            AllowWiktionaryChildrenProcessing = true,
            MayDefineTerm = false
        };
        public readonly static ProcessingMode Language = new ProcessingMode
        {
            AllowedTermModelSubSections = null,
            AllowWiktionaryChildrenProcessing = true,
            MayDefineTerm = false
        };

        /// <summary>
        /// Whether this mode allows adding the <paramref name="name"/> as its child.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal bool AllowNesting(string name)
        {
            return AllowedTermModelSubSections != null && Array.BinarySearch(AllowedTermModelSubSections, name) >= 0;
        }
    }
}