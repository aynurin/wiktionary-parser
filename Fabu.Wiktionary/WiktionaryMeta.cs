using Fabu.Wiktionary;
using Fabu.Wiktionary.FuzzySearch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fabu.Wiktionary
{
    internal class WiktionaryMeta
    {
        public WiktionaryMeta(IFuzzySearcher<LanguageWeight> languageNames,
            IFuzzySearcher<SectionWeight> knownSections,
            IFuzzySearcher<SectionType> sectionTypes)
        {
            _languageNames = languageNames;
            _knownSections = knownSections;
            _sectionTypes = sectionTypes;
        }

        #region Manual classification
        //private readonly string[] LOJBAN_GRAM_TERMS = new string[] {
        //    "cmavo",
        //    "gismu",
        //    "rafsi",
        //    "lujvo",
        //    "brivla",
        //    "cmevla"
        //};

        //private readonly string[] PARTS_OF_SPEECH = new string[] {
        //    "adjectival noun", "adjective", "adjective suffix", "adverb", "article",
        //    "character", "conjunction", "determiner", "determiner and pronoun",
        //    "idiom", "initialism", "interjection", "letter", "noun", "numeral",
        //    "phrase", "postposition", "prefix", "preposition", "preverb",
        //    "pronoun", "proper noun", "proverb",
        //    "syllable", "symbol", "verb", "verb suffix"
        //};

        //private readonly string[] LINKS = new string[] {
        //    "external links",
        //    "external sources",
        //    "further notes",
        //    "further reading",
        //    "note",
        //    "notes",
        //    "references",
        //    "see also"
        //};

        //private readonly string[] NOTES = new string[] {
        //    "note on morphophonemics", "usage notes"
        //};

        //private readonly string[] SECTIONS = new string[] {
        //    "definitions", "etymology", "examples", "pronunciation", "related"
        //};
        //private readonly string[] RELATIONS = new string[] {
        //    "abbreviations", "alternative forms", "alternative terms",
        //    "alternate spelling", "anagrams", "antonyms",
        //    "coordinate terms", "derived terms", "related words", "descendents",
        //    "holonyms", "homophones", "hypernyms", "hyponyms",
        //    "meronyms", "more usual form", "related terms", "synonyms",
        //    "translations", "troponyms"
        //};

        //private readonly string[] UNWANTED_LIST = new string[] {
        //    "anagrams", "compounds", "external links", "further reading", "references",
        //    "see also", "statistics", "usage notes"
        //};
        #endregion

        public const string ROOT_NODE_NAME = "PAGE";

        public const string LOJBAN_GRAM = "LOJBAN";
        public const string PART_OF_SPEECH = "PART_OF_SPEECH";
        public const string LINK = "LINK";
        public const string NOTE = "NOTE";
        public const string SECTION = "SECTION";
        public const string RELATION = "RELATION";
        public const string LANGUAGE_NAME = "LANGUAGE_NAME";

        private readonly IFuzzySearcher<LanguageWeight> _languageNames;
        private readonly IFuzzySearcher<SectionWeight> _knownSections;
        private readonly IFuzzySearcher<SectionType> _sectionTypes;

        private static readonly Regex CleanupSectionName = new Regex(@"^[\W_]*(.+?)[\W\d_]*$", RegexOptions.Compiled);

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

        private static readonly Regex SectionNameHint = new Regex(@"<!--[^>]*>", RegexOptions.Compiled);

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

        private static readonly Regex Parentheses = new Regex(@"\s*\([^\)]*\)\s*", RegexOptions.Compiled);

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

        public bool TryGetLanguage(string name, out string newValue)
        {
            if (_languageNames == null)
            {
                newValue = null;
                return false;
            }
            if (_languageNames.TryFindBest(name, out List<LanguageWeight> result))
            {
                newValue = result.First().Name;
                return true;
            }
            newValue = null;
            return false;
        }

        /// <summary>
        /// Removes case ambiguity (e.g. if <paramref name="value"/> is all lowercase)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool TryGetStandardSectionName(string value, out string newValue)
        {
            if (_knownSections == null)
            {
                newValue = null;
                return false;
            }
            if (_knownSections.TryFindBest(value, out List<SectionWeight> result))
            {
                newValue = result.First().Name;
                return true;
            }
            newValue = null;
            return false;
        }

        /// <summary>
        /// Produces a section class based on section name.
        /// </summary>
        /// <remarks>
        /// E.g. returns "LANGUAGE_NAME" for "English"
        /// </remarks>
        /// <param name="value"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool TryGetSectionClass(string value, out string newValue)
        {
            if (_sectionTypes == null)
            {
                newValue = null;
                return false;
            }
            if (_sectionTypes.TryFindBest(value, out List<SectionType> sectionTypes))
            {
                if (sectionTypes.Any(s => s.Type == PART_OF_SPEECH))
                {
                    newValue = PART_OF_SPEECH;
                    return true;
                }
                else if (sectionTypes.Any(s => s.Type == LOJBAN_GRAM))
                {
                    newValue = LOJBAN_GRAM;
                    return true;
                }
            }
            if (_languageNames.TryFindBest(value, out List<LanguageWeight> result))
            {
                newValue = LANGUAGE_NAME;
                return true;
            }
            newValue = null;
            return false;
        }
    }
}
