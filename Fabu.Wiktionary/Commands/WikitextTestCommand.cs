using CommandLine;
using Fabu.Wiktionary.TextConverters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Fabu.Wiktionary.Commands
{
    class WikitextTestCommand : BaseCommand<WikitextTestCommand.Args>
    {
        [Verb("wikitest", HelpText = "Extract language and section names from Wiktionary dump")]
        public class Args : BaseArgs { }

        // prep --in enwiktionary-20180120-pages-articles.xml
        protected override void RunCommand(Args args, Func<int, BaseArgs, bool> onProgress)
        {
            DebugTest();
        }

        public void DebugTest()
        {
            var creole = "A domesticated [[subspecies]] (''[[Felis silvestris catus|felis melis]]'') of [[feline]] animal, commonly kept as a house [[pet]]. {{defdate|from 8th c.}}";
            var converter = new WikitextConverter();
            var result = converter.ConvertToStructured(creole);
            Console.WriteLine();
            Console.WriteLine(creole);
            Console.WriteLine();
            Console.WriteLine(result.ToHtml());
        }
        
        public void ShouldConvertEmphasis()
        {
            var creole = "A domesticated subspecies (''Felis silvestris catus'') of feline animal";
            var html = "A domesticated subspecies (<em>Felis silvestris catus</em>) of feline animal";
            var converter = new WikitextConverter();
            var result = converter.ConvertToStructured(creole);
            Debug.Assert(html == result.ToHtml());
        }
        
        public void ShouldConvertLinks()
        {
            var creole = "A domesticated [[subspecies]] ([[Felis silvestris catus]]) of [[feline]] animal, commonly kept as a house [[pet]]";
            var html = "<p>A domesticated <a href=\"subspecies\">subspecies</a> (<a href=\"Felis silvestris catus\">Felis silvestris catus</a>) of <a href=\"feline\">feline</a> animal, commonly kept as a house <a href=\"pet\">pet</a></p>";
            var converter = new WikitextConverter();
            var result = converter.ConvertToStructured(creole);
            Debug.Assert(html == result.ToHtml());
        }
        
        public void ShoudConvertMacros()
        {
            var creole = "A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet. {{defdate|from 8th c.}}";
            var html = "<p>A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet. <span class=\"defdate\">from 8th c.</span></p>";
            var converter = new WikitextConverter();
            var result = converter.ConvertToStructured(creole);
            Debug.Assert(html == result.ToHtml());
        }
        
        public void ShouldStripHtml()
        {
            var creole = "from 8<sup>th</sup>c.";
            var html = "<p>from 8thc.</p>";
            var converter = new WikitextConverter();
            var result = converter.ConvertToStructured(creole);
            Debug.Assert(html == result.ToHtml());
        }
        
        public void ShouldProcessPlainStrings()
        {
            var creole = "A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet.";
            var html = "<p>A domesticated subspecies (Felis silvestris catus) of feline animal, commonly kept as a house pet.</p>";
            var converter = new WikitextConverter();
            var result = converter.ConvertToStructured(creole);
            Debug.Assert(html == result.ToHtml());
        }
    }
}
