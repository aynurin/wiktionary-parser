using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    class TemplateName
    {
        public string Name { get; set; }
        public string[] AllParts { get; private set; }
        public string OriginalName { get; set; }
        public string Language { get; set; }
        public bool IsPOSTemplate { get; set; }

        private static string[] _acceleratedPosTemplates = new string[]
        {
            "adj",
            "adj",
            "adv",
            "noun",
            "verb"
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
                return name;
            }

            if (isLanguage(nameParts[0]))
                name.Language = nameParts[0];

            if (Array.BinarySearch(_acceleratedPosTemplates, nameParts[1]) >= 0 && name.Language != null)
            {
                name.Name = nameParts[1];
                name.AllParts = nameParts;
                name.IsPOSTemplate = true;
                return name;
            }

            Debugger.Break();

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
            { "lb", "label" },
            { "lbl", "label" },
            { "ipa", "IPA" },
            { "inh", "inherited" }
        };

        internal static string FullName(string template)
        {
            if (_fullNames.TryGetValue(template, out string fullname))
                return fullname;
            return template;
        }
    }
}
