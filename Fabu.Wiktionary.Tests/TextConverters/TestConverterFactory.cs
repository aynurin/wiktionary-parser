using Fabu.Wiktionary.Commands;
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
            { "es", "Spanish" },
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

        protected TestConverterOutput Convert(string creole, bool allowLinks = false, string sectionName = "TEST")
        {
            var ignoredTemplates = DumpTool.LoadDump<List<string>>(BaseArgs.DefaultDumpDir, DumpTool.IgnoredTemplatesDump);
            var converterFactory = new WikitextConverterFactory(_dictionary, ignoredTemplates, allowLinks);
            var converter = converterFactory.CreateConverter(new ContextArguments("TEST", sectionName));
            var converted = converter.ConvertText(creole);
            return new TestConverterOutput(converted, converter.Context.Proninciations);
        }

        public class TestConverterOutput
        {
            public TestConverterOutput(string converted, List<Proninciation> proninciations)
            {
                Converted = converted;
                Proninciations = proninciations;
            }

            public string Converted { get; set; }
            public List<Proninciation> Proninciations { get; set; }

            public string ToHtml()
            {
                return Converted;
            }
        }
    }
}
