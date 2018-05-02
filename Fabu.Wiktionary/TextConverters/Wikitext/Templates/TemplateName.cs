using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class TemplateName
    {
        public string Name { get; set; }
        public string[] AllParts { get; private set; }
        public string OriginalName { get; set; }
        public string Language { get; set; }
        public bool IsHeadTemplate { get; set; }

        private static readonly Regex[] _ignoreTemplates = new[]
        {
            new Regex(@"^RQ?\:", RegexOptions.IgnoreCase),
            new Regex(@"^User\:")
        };

        private static string[] _acceleratedPosTemplates = new string[]
        {
            "adj",
            "adj",
            "adv",
            "noun",
            "proper noun",
            "verb",
            "interj",
            "plural noun",
            "PP",
            "prep phrase"
        };

        public TemplateName(string originalName)
        {
            OriginalName = originalName;
        }

        public static TemplateName Parse(string originalName, Func<string,bool> isLanguage)
        {
            var name = new TemplateName(originalName);

            var nameParts = originalName.Split('-');
            if (nameParts.Length == 1)
            {
                name.Name = originalName;
                if (name.Name == "head")
                    name.IsHeadTemplate = true;
                return name;
            }

            if (isLanguage(nameParts[0]))
            {
                name.Language = nameParts[0];

                if (Array.BinarySearch(_acceleratedPosTemplates, nameParts[1]) >= 0 && name.Language != null)
                {
                    name.Name = nameParts[1];
                    name.AllParts = nameParts;
                    name.IsHeadTemplate = true;
                    return name;
                }
            }

            name.Name = name.OriginalName;

            return name;
        }

        public string[] GetNameParts()
        {
            var nameParts = OriginalName.Split('-');
            if (nameParts.Length == 1)
                return new[] { OriginalName };

            var allNames = new List<string>(nameParts.Length);
            for (var i = 0; i < nameParts.Length; i++)
            {
                var newParts = new string[nameParts.Length];
                for (var j = 0; j < nameParts.Length; j++)
                    newParts[j] = j.ToString();
                newParts[i] = nameParts[i];
                allNames.Add(String.Join('-', newParts));
            }

            return allNames.ToArray();
        }

        // TODO: Refactor so that template converters report their names themselves
        private static Dictionary<string, string> _fullNames = new Dictionary<string, string>()
        {
            { "m", "mention" },
            { "l", "mention" },
            { "der", "derived" },
            { "bor", "derived" },
            { "borrowed", "derived" },
            { "lb", "label" },
            { "lbl", "label" },
            { "ipa", "IPA" },
            { "inh", "inherited" },
            { "...", "ellipsis" },
            { "nb...", "ellipsis" },
            { "quote-magazine", "quote-journal" },
            { "quote-news", "quote-journal" },
            { "a", "accent" },
            { "cog", "cognate" },
            { "ux", "usage" },
            { "usex", "usage" },
            { "w", "wikipedia" },
            { "def-date", "defdate" },
            { "non-gloss definition", "non-gloss" },
            { "n-g", "non-gloss" },
            { "t", "translation" },
            { "t+", "translation" },
            { "t-check", "translation" },
            { "t+check", "translation" },
            { "en-irregular plural of", "plural of" },
            { "alternative plural of", "plural of" },
            { "alt form", "alternative form of" },
            { "altform", "alternative form of" },
            { "alt form of", "alternative form of" },
            { "historical given name", "given name" },
            { "vern", "wikipedia" },
            { "syn", "synonyms" },
            { "hyper", "hypernyms" },
            { "hypo", "hyponyms" },
            { "ant", "antonyms" },
            { "af", "affix" },
            { "comparative of", "en-comparative of" },
            { "superlative of", "en-superlative of" },
            { "attributive of", "attributive form of" },
            { ";", "template-name" },
            { ",", "template-name" },
            { "IPAchar", "template-value" }
        };

        internal static bool IgnoreName(string originalName)
        {
            foreach (var rx in _ignoreTemplates)
            {
                if (rx.IsMatch(originalName))
                    return true;
            }
            return false;
        }

        internal static string FullName(string template)
        {
            if (_fullNames.TryGetValue(template, out string fullname))
                return fullname;
            return template;
        }

        public override string ToString()
        {
            return OriginalName ?? Name ?? base.ToString();
        }
    }
}
