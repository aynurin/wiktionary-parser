using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fabu.Wiktionary.FuzzySearch;
using Fabu.Wiktionary.Transform;
using WikimediaProcessing;

namespace Fabu.Wiktionary.Commands
{
    internal class WiktionaryTermExtractor : IWiktionaryPageProcessor
    {
        private readonly SectionNameTransform _transform;
        /// <summary>
        /// Process only this given term
        /// </summary>
        private readonly string _onlyTheTerm;
        
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

        public WiktionaryTermExtractor(SectionNameTransform transform, string onlyTheTerm)
        {
            _transform = transform;
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

            var graph = CreateGraph(page);

            ProcessGraph(graph);
        }

        public GraphItem CreateGraph(WikimediaPage page)
        {
            var graph = GraphItem.CreateRoot(page);
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
                    Debug.Assert(sectionProcessingMode != null);
                    var item = new GraphItem(nodeName.Name, graph, graph.OwnerPage, section, nodeName.IsLanguage, 
                        sectionProcessingMode.MayDefineTerm, sectionProcessingMode.AllowedTermModelSubSections);
                    if (sectionProcessingMode.AllowWiktionaryChildrenProcessing)
                        AddSections(item, section.SubSections);
                    graph.AddChild(item);
                }
            }
        }

        public void ProcessGraph(GraphItem item)
        {
            foreach (var child in item.Children)
            {
                bool hasDefinedATerm = false;
                if (child.IsLanguage)
                {
                    child.SetLanguage();
                }
                else if (child.CanDefineTerm)
                {
                    child.DefineTerm();
                    hasDefinedATerm = true;
                }
                else if (item.AllowsMember(child))
                        item.UpdateMember(child);
                else child.UpdateTerm();

                ProcessGraph(child);

                if (hasDefinedATerm)
                {
                    var term = child.CreateTerm();
                    Console.WriteLine(term);
                }
            }
        }

        public void Complete(dynamic completionArgs)
        {
            // no completion actions needed so far
        }
    }

    internal class ProcessingMode
    {
        public string[] AllowedTermModelSubSections;
        public bool AllowWiktionaryChildrenProcessing;
        public bool MayDefineTerm;

        public readonly static ProcessingMode CanDefineTerm = new ProcessingMode
        {
            AllowedTermModelSubSections = null,
            AllowWiktionaryChildrenProcessing = true,
            MayDefineTerm = true
        };
        public readonly static ProcessingMode ChildSection = new ProcessingMode
        {
            AllowedTermModelSubSections = null,
            AllowWiktionaryChildrenProcessing = false,
            MayDefineTerm = false
        };
        public readonly static ProcessingMode PosOrSimilar = new ProcessingMode
        {
            AllowedTermModelSubSections = new string[] { "Quotations", "Synonyms", "Usage notes" },
            AllowWiktionaryChildrenProcessing = true,
            MayDefineTerm = false
        };
        public readonly static ProcessingMode Language = new ProcessingMode
        {
            AllowedTermModelSubSections = null,
            AllowWiktionaryChildrenProcessing = true,
            MayDefineTerm = false
        };

        /// <summary>
        /// Whether this mode allows adding the <paramref name="name"/> as its child.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal bool AllowNesting(string name)
        {
            return AllowedTermModelSubSections != null && Array.BinarySearch(AllowedTermModelSubSections, name) >= 0;
        }
    }
}