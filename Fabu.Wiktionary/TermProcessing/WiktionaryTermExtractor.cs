using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fabu.Wiktionary.Transform;
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

        public WiktionaryTermExtractor(SectionNameTransform transform, string onlyTheTerm)
        {
            _graphProcessor = new TermGraphProcessor(transform);
            _onlyTheTerm = onlyTheTerm;
        }

        public void AddPage(WikimediaPage page)
        {
            if (page.IsSpecialPage || page.IsRedirect || page.IsDisambiguation)
                return;

            if (page.Title == "cat")
                Debugger.Break();

            if (!String.IsNullOrWhiteSpace(_onlyTheTerm) && page.Title != _onlyTheTerm)
                return;

            var graph = _graphProcessor.CreateGraph(page);

            _graphProcessor.ProcessGraph(graph);
        }

        public void Complete(dynamic completionArgs)
        {
            // no completion actions needed so far
        }
    }

    public class TermGraphProcessor
    {
        private readonly SectionNameTransform _transform;

        public TermGraphProcessor(SectionNameTransform transform)
        {
            _transform = transform;
        }

        private readonly Dictionary<string, ProcessingMode> _supportedSections = new Dictionary<string, ProcessingMode>
        {
            { "Etymology", ProcessingMode.CanDefineTerm }, // means any   
            { "Pronunciation", ProcessingMode.CanDefineTerm }, // means any

            { "Synonyms", ProcessingMode.ChildSection },
            { "Quotations", ProcessingMode.ChildSection },
            { "Idiom", ProcessingMode.ChildSection },
            { "Definitions", ProcessingMode.ChildSection },
            { "Usage notes", ProcessingMode.ChildSection },
            { "Abbreviation", ProcessingMode.ChildSection },
            { "Description", ProcessingMode.ChildSection },

            { "Verb", ProcessingMode.PosOrSimilar },
            { "Noun", ProcessingMode.PosOrSimilar },
            { "Adjective", ProcessingMode.PosOrSimilar },
            { "Participle", ProcessingMode.PosOrSimilar },
            { "Proper noun", ProcessingMode.PosOrSimilar },
            { "Adverb", ProcessingMode.PosOrSimilar },
            { "Pronoun", ProcessingMode.PosOrSimilar },
            { "Phrase", ProcessingMode.PosOrSimilar },
            { "Conjunction", ProcessingMode.PosOrSimilar },
            { "Proverb", ProcessingMode.PosOrSimilar }
        };

        public GraphItem CreateGraph(WikimediaPage page)
        {
            var graph = GraphItem.CreateRoot(page.Title);
            AddSections(graph, page.Sections);
            return graph;
        }

        private void AddSections(GraphItem graph, ICollection<WikiSection> sections)
        {
            foreach (var section in sections)
            {
                var nodeName = _transform.Apply(new SectionName { Name = section.SectionName });
                ProcessingMode sectionProcessingMode = null;
                if (nodeName.IsLanguage || _supportedSections.TryGetValue(nodeName.Name, out sectionProcessingMode))
                {
                    if (nodeName.IsLanguage)
                        sectionProcessingMode = ProcessingMode.Language;

                    if (sectionProcessingMode == null)
                        throw new InvalidOperationException("This shouldn't ever happen");

                    var item = new GraphItem(nodeName.Name, graph, graph.OwnerPageTitle, section.Content, nodeName.IsLanguage,
                        sectionProcessingMode.MayDefineTerm, sectionProcessingMode.AllowedTermModelSubSections);

                    if (sectionProcessingMode.AllowWiktionaryChildrenProcessing)
                        AddSections(item, section.SubSections);

                    graph.AddChild(item);
                }
            }
        }

        public void ProcessGraph(GraphItem item)
        {
            if (item.CanDefineTerm)
                item.DefineTerm();
            foreach (var child in item.Children)
            {
                if (child.IsLanguage)
                    child.SetLanguage();
                else if (item.AllowsMember(child))
                    item.UpdateMember(child);
                else child.UpdateTerm();

                ProcessGraph(child);
            }
        }
    }
}