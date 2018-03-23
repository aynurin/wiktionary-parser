using System;
using System.Collections.Generic;

namespace Fabu.Wiktionary.TermProcessing
{
    public class ProcessingMode
    {
        public string[] AllowedTermModelSubSections;
        public bool AllowWiktionaryChildrenProcessing;
        public bool MayDefineTerm;

        public static ProcessingMode operator | (ProcessingMode one, ProcessingMode another)
        {
            var newMode = new ProcessingMode
            {
                AllowWiktionaryChildrenProcessing = one.AllowWiktionaryChildrenProcessing || another.AllowWiktionaryChildrenProcessing,
                MayDefineTerm = one.MayDefineTerm || another.MayDefineTerm
            };
            if (one.AllowedTermModelSubSections != null || another.AllowedTermModelSubSections != null)
            {
                var newSections = new List<string>(one.AllowedTermModelSubSections ?? new string[0]);
                if (another.AllowedTermModelSubSections != null)
                {
                    foreach (var s in another.AllowedTermModelSubSections)
                        if (!newSections.Contains(s))
                            newSections.Add(s);
                    newSections.Sort();
                }
                newMode.AllowedTermModelSubSections = newSections.ToArray();
            }
            return newMode;
        }

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