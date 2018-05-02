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
        private readonly TermGraphProcessor _graphProcessor;
        private readonly ITextConverter _textConverter;

        /// <summary>
        /// Process only this given term
        /// </summary>
        private readonly string _onlyTheTerm;
        private readonly IWordWriter _wordWriter;

        public int WordsCount { get; set; }
        public int TermsCount { get; set; }
        public List<string> EmptyResults { get; } = new List<string>();

        public WiktionaryTermExtractor(TermGraphProcessor processor, ITextConverter contentConverter, string onlyTheTerm, IWordWriter writer)
        {
            _graphProcessor = processor;
            _textConverter = contentConverter;
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

            foreach (var item in graph.AllItems.Where(i => i.Status == Term.TermStatus.Defined && i.IsEmpty))
                item.Status = Term.TermStatus.Empty;

            var termsDefined = graph.GetItems(Term.TermStatus.Defined);
            if (termsDefined.Count == 0 && graph.AllItems.Any(i => i.Language == "English"))
                EmptyResults.Add(page.Title);
            // now get rid of non-English definitions, because parsing wikitext to HTML is simply impossible for all languages at the moment.
            termsDefined = termsDefined.Where(term => term.Language == "English").ToList();
            if (termsDefined.Count > 0)
            {
                ConvertContent(page.Title, null, termsDefined);
                var dictword = new DictionaryWord(page.Title, termsDefined);
                _wordWriter.Write(dictword);
                WordsCount += 1;
                TermsCount += dictword.Terms.Count;
            }
        }

        private void ConvertContent(string pageTitle, string sectionName, IEnumerable<Term> terms)
        {
            if (terms == null || _textConverter == null)
                return;

            foreach (var term in terms)
            {
                var args = new ContextArguments() { PageTitle = pageTitle, SectionName = sectionName };
                term.ConvertContent(args, _textConverter);
                if (sectionName == null && term.Title != pageTitle)
                    sectionName = term.Title;
                ConvertContent(pageTitle, sectionName, term.Properties.Values);
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