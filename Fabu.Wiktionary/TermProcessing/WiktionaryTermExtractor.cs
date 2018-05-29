using Fabu.Wiktionary.TextConverters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WikimediaProcessing;

namespace Fabu.Wiktionary.TermProcessing
{
    internal class WiktionaryTermExtractor : IWiktionaryPageProcessor
    {
        private readonly PageGraphProcessor _graphProcessor;

        /// <summary>
        /// Process only this given term
        /// </summary>
        private readonly string _onlyTheTerm;
        private readonly IWordCreator _wordWriter;

        public int WordsCount { get; set; }
        public int TermsCount { get; set; }
        public List<string> EmptyResults { get; } = new List<string>();

        public WiktionaryTermExtractor(PageGraphProcessor processor, string onlyTheTerm, IWordCreator writer)
        {
            _graphProcessor = processor;
            _onlyTheTerm = onlyTheTerm;
            _wordWriter = writer;
        }

        public void AddPage(WikimediaPage page)
        {
            if (page.IsSpecialPage || page.IsRedirect || page.IsDisambiguation)
                return;

            if (!String.IsNullOrWhiteSpace(_onlyTheTerm) && page.Title != _onlyTheTerm)
                return;

            page.Text = StripHtml.Comments(page.Text);

            var graph = _graphProcessor.CreateGraph(page);

            _graphProcessor.ProcessGraph(graph);

            var termsDefined = graph.DefinedTerms.Where(term => term.Language == "English").ToList();
            if (termsDefined.Count == 0 && graph.AllItems.Any(i => i.Language == "English"))
                EmptyResults.Add(page.Title);
            // now get rid of non-English definitions, because parsing wikitext to HTML is simply impossible for all languages at the moment.

            if (termsDefined.Count > 0)
            {
                _wordWriter.Create(termsDefined);
                WordsCount += 1;
                TermsCount += termsDefined.Count;
            }
        }

        public void Complete(dynamic completionArgs)
        {
            // no completion actions needed so far
            foreach (var kvp in _graphProcessor.SkippedSections)
            {
                Console.WriteLine(kvp.Key + ": " + kvp.Value);
            }
        }
    }
}