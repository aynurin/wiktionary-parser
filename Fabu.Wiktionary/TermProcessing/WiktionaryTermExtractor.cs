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

            var termsDefined = graph.GetItems(Term.TermStatus.Defined);
            if (termsDefined.Count == 0)
                Debugger.Break();
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