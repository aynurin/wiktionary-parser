using CommandLine;
using Fabu.Wiktionary.FuzzySearch;
using Fabu.Wiktionary.TermProcessing;
using Fabu.Wiktionary.TextConverters.Wiki;
using Fabu.Wiktionary.TextConverters.Wiki.Templates;
using Fabu.Wiktionary.Transform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fabu.Wiktionary.Commands
{
    class CreoleCommand : BaseCommand<CreoleCommand.Args>
    {
        [Verb("creole", HelpText = "Extract language names")]
        public class Args : BaseArgs
        {
            [Option("text", Required = false, HelpText = "A single term to process.")]
            public string Text { get; set; }
        }

        // extract --in enwiktionary-20180120-pages-articles.xml
        protected override void RunCommand(Args args, Func<int, BaseArgs, bool> onProgress)
        {
            var languageCodes = DumpTool.LoadLanguageCodes(args.DumpDir);
            var ignoredTemplates = DumpTool.LoadDump<List<string>>(args.DumpDir, DumpTool.IgnoredTemplatesDump);
            var factory = new WikitextConverterFactory(languageCodes, ignoredTemplates, false);
            var textConverter = factory.CreateConverter(new TextConverters.ContextArguments(null, null));
            var output = textConverter.ConvertText(args.Text);
            Console.WriteLine(output);
        }
    }
}
