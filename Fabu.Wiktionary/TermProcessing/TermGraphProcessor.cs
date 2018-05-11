using System;
using System.Collections.Generic;
using System.Linq;
using Fabu.Wiktionary.Transform;
using WikimediaProcessing;

namespace Fabu.Wiktionary.TermProcessing
{
    public class PageGraphProcessor
    {
        private readonly SectionNameTransform _transform;

        public PageGraphProcessor(SectionNameTransform transform)
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
            { "Article", ProcessingMode.PosOrSimilar },
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

        public PageGraphNode CreateGraph(WikimediaPage page) => 
            CreateGraph(page.Title, page.Sections.Select(_ => new WikiSectionAccessor(_)));

        public PageGraphNode CreateGraph(string title, IEnumerable<ISectionAccessor> subSections)
        {
            var graph = PageGraphNode.CreateRoot(title);
            AddSections(graph, subSections, null);
            return graph;
        }

        public Dictionary<string, int> SkippedSections { get; set; } = new Dictionary<string, int>();

        private void AddSections(PageGraphNode graph, IEnumerable<ISectionAccessor> sections, Action<PageGraphNode> forEachItem)
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

                    var item = graph.CreateChild(nodeName.Name, section.GetRawContent(), nodeName.IsLanguage,
                        sectionProcessingMode.MayDefineTerm, sectionProcessingMode.AllowedTermModelSubSections);

                    if (sectionProcessingMode.AllowWiktionaryChildrenProcessing)
                    {
                        Action<PageGraphNode> setContainsTermDefiners = null;
                        if (nodeName.IsLanguage)
                        {
                            if (forEachItem == null)
                            {
                                setContainsTermDefiners = (PageGraphNode x) => containsTermDefiners |= x.CanDefineTerm;
                            }
                            else
                            {
                                setContainsTermDefiners = (PageGraphNode x) =>
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

        public void ProcessGraph(PageGraphNode item)
        {
            if (item.CanDefineTerm) // a fixed subset of sections
                item.DefineTerm(); // creates an instance of a term
            foreach (var child in item.Children)
            {
                if (child.IsLanguage)
                    child.SetLanguage(); // updates this and dependent nodes
                else if (item.AllowsMember(child)) // subset of sections
                    item.UpdateMember(child);
                else child.UpdateTerm(); // update an existing term

                ProcessGraph(child); // repeat for the child
            }
        }
    }
}