using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WikimediaProcessing;

namespace Fabu.Wiktionary.TermProcessing
{
    internal class WiktionaryTermExtractor : IWiktionaryPageProcessor
    {
        private readonly TermGraphProcessor _graphProcessor;
        /// <summary>
        /// Process only this given term
        /// </summary>
        private readonly string _onlyTheTerm;

        public List<Term> DefinedTerms { get; } = new List<Term>();
        public List<string> EmptyResults { get; } = new List<string>();

        public WiktionaryTermExtractor(TermGraphProcessor processor, string onlyTheTerm)
        {
            _graphProcessor = processor;
            _onlyTheTerm = onlyTheTerm;
        }

        public void AddPage(WikimediaPage page)
        {
            if (page.IsSpecialPage || page.IsRedirect || page.IsDisambiguation)
                return;

            if (!String.IsNullOrWhiteSpace(_onlyTheTerm) && page.Title != _onlyTheTerm)
                return;

            var graph = _graphProcessor.CreateGraph(page);

            _graphProcessor.ProcessGraph(graph);

            foreach (var item in graph.AllItems.Where(i => i.Status == Term.TermStatus.Defined && i.IsEmpty))
                item.Status = Term.TermStatus.Empty;

            var termsDefined = graph.GetItems(Term.TermStatus.Defined);
            if (termsDefined.Count == 0 && graph.AllItems.Any(i => i.Language == "English"))
            {
                Console.WriteLine($"Empty result for https://en.wiktionary.org/wiki/{page.Title}");
                EmptyResults.Add(page.Title);
            }
            DefinedTerms.AddRange(termsDefined);

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