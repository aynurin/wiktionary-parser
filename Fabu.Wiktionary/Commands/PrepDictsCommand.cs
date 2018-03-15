using CommandLine;
using Fabu.Wiktionary.Graph;
using Fabu.Wiktionary.Transform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fabu.Wiktionary.Commands
{
    internal class PrepDictsCommand : BaseCommand<PrepDictsCommand.Args>
    {
        [Verb("prep", HelpText = "Extract language and section names from Wiktionary dump")]
        public class Args : BaseArgs { }

        // prep --in enwiktionary-20180120-pages-articles.xml
        protected override void RunCommand(Args args, Func<int, BaseArgs, bool> onProgress)
        {
            var wiktionaryDump = DumpTool.LoadWikimediaDump(args.DumpDir, args.WiktionaryDumpFile);
            var graphBuilder = new GraphBuilder(new TrimSectionNames(), new List<SectionName>());
            var analyzer = new WiktionaryAnalyzer(graphBuilder, wiktionaryDump);
            if (onProgress != null)
                analyzer.PageProcessed += (sender, e) => e.Abort = onProgress(e.Index, args);
            analyzer.Compute();
            
            DumpTool.SaveDump(args.DumpDir, DumpTool.LanguagesDump, 
                graphBuilder.LanguageNames.Select(sv => new SectionName { Name = sv.Title, Weight = sv.Count }));

            var sections = graphBuilder.AllSections
                    .OrderBy(section => section.Title)
                    .Select(sv => new SectionName
                    {
                        Name = sv.Title,
                        Weight = sv.Count
                    }).ToList();
            DumpTool.SaveDump(args.DumpDir, DumpTool.SectionsDump, sections);
        }
    }
}
