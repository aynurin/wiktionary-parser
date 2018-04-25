using Fabu.Wiktionary.TextConverters;
using Fabu.Wiktionary.TextConverters.Wiki;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class TestConverterFactory
    {
        private Dictionary<string, string> _dictionary = new Dictionary<string, string>()
        {
            { "en", "English" },
            { "de", "German" },
            { "fy", "West Frisian" },
            { "la", "Latin" },
            { "ang", "Old English" },
            { "gml", "Middle Low German" },
            { "gmh", "Middle High German" },
            { "goh", "Old High German" },
            { "gem", "Germanic" },
            { "gem-pro", "Proto-Germanic" },
            { "ine-pro", "Proto-Indo-European" },
            { "nl", "Dutch" },
            { "osx", "Old Saxon" },
            { "da", "Danish" },
            { "sv", "Swedish" },
            { "no", "Norwegian" },
            { "onw", "Nubian" }
        };

        protected FormattedString Convert(string creole, bool allowLinks = false, string sectionName = "TEST")
        {
            var converter = new WikitextProcessor(_dictionary, allowLinks);
            var formatted = converter.ConvertToStructured(new ContextArguments() { PageTitle = "TEST", SectionName = sectionName }, creole);
            return formatted;
        }
    }
}
