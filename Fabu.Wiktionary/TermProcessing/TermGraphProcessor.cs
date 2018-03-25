using System;
using System.Collections.Generic;
using System.Linq;
using Fabu.Wiktionary.Transform;
using WikimediaProcessing;

namespace Fabu.Wiktionary.TermProcessing
{
    public class TermGraphProcessor
    {
        private readonly SectionNameTransform _transform;

        public TermGraphProcessor(SectionNameTransform transform, SectionName[] termDefiners)
        {
            _transform = transform;
            // termDefiners ignored atm, as POSes cannot define new term each as several POSes can be within the same term.
            //foreach (var name in termDefiners)
            //{
            //    if (_supportedSections.ContainsKey(name.Name))
            //        _supportedSections[name.Name] |= ProcessingMode.CanDefineTerm;
            //    else _supportedSections.Add(name.Name, ProcessingMode.CanDefineTerm);
            //}
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
            { "Acronym", ProcessingMode.PosOrSimilar },
            { "Noun", ProcessingMode.PosOrSimilar },
            { "Adjective", ProcessingMode.PosOrSimilar },
            { "Participle", ProcessingMode.PosOrSimilar },
            { "Proper noun", ProcessingMode.PosOrSimilar },
            { "Adverb", ProcessingMode.PosOrSimilar },
            { "Pronoun", ProcessingMode.PosOrSimilar },
            { "Phrase", ProcessingMode.PosOrSimilar },
            { "Conjunction", ProcessingMode.PosOrSimilar },
            { "Proverb", ProcessingMode.PosOrSimilar },
            { "Initialism", ProcessingMode.PosOrSimilar },
            { "Preposition", ProcessingMode.PosOrSimilar },
            { "Interjection", ProcessingMode.PosOrSimilar },
            { "Prepositional phrase", ProcessingMode.PosOrSimilar }
        };

        public GraphItem CreateGraph(WikimediaPage page) => 
            CreateGraph(page.Title, page.Sections.Select(_ => new WikiSectionAccessor(_)));

        public GraphItem CreateGraph(string title, IEnumerable<ISectionAccessor> subSections)
        {
            var graph = GraphItem.CreateRoot(title);
            AddSections(graph, subSections, null);
            return graph;
        }

        public Dictionary<string, int> SkippedSections { get; set; } = new Dictionary<string, int>();

        private void AddSections(GraphItem graph, IEnumerable<ISectionAccessor> sections, Action<GraphItem> forEachItem)
        {
            foreach (var section in sections)
            {
                var nodeName = _transform.Apply(new SectionName { Name = section.GetSectionName() });
                if (nodeName == null)
                {
                    if (SkippedSections.ContainsKey(section.GetSectionName()))
                        SkippedSections[section.GetSectionName()] += 1;
                    else SkippedSections.Add(section.GetSectionName(), 1);
                    continue;
                }
                ProcessingMode sectionProcessingMode = null;
                var containsTermDefiners = false;
                if (nodeName.IsLanguage || _supportedSections.TryGetValue(nodeName.Name, out sectionProcessingMode))
                {
                    if (nodeName.IsLanguage)
                        sectionProcessingMode = ProcessingMode.Language;

                    if (sectionProcessingMode == null)
                        throw new InvalidOperationException("This shouldn't ever happen");

                    if (sectionProcessingMode.MayDefineTerm)
                        containsTermDefiners = true;

                    var item = new GraphItem(nodeName.Name, graph, graph.OwnerPageTitle, section.GetContent(), nodeName.IsLanguage,
                        sectionProcessingMode.MayDefineTerm, sectionProcessingMode.AllowedTermModelSubSections);

                    if (sectionProcessingMode.AllowWiktionaryChildrenProcessing)
                    {
                        Action<GraphItem> setContainsTermDefiners = null;
                        if (nodeName.IsLanguage)
                        {
                            if (forEachItem == null)
                            {
                                setContainsTermDefiners = (GraphItem x) => containsTermDefiners |= x.CanDefineTerm;
                            }
                            else
                            {
                                setContainsTermDefiners = (GraphItem x) =>
                                {
                                    forEachItem(x);
                                    containsTermDefiners |= x.CanDefineTerm;
                                };
                            }
                        }
                        else if (forEachItem != null)
                            setContainsTermDefiners = forEachItem;
                        AddSections(item, section.GetSubSections(), setContainsTermDefiners);
                    }

                    if (nodeName.IsLanguage && !containsTermDefiners)
                        item.CanDefineTerm = true;

                    forEachItem?.Invoke(item);

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