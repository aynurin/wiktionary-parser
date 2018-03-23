using System;
using System.Collections.Generic;
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

        public Dictionary<string, int> SkippedSections { get; set; } = new Dictionary<string, int>();

        private void AddSections(GraphItem graph, ICollection<WikiSection> sections)
        {
            foreach (var section in sections)
            {
                var nodeName = _transform.Apply(new SectionName { Name = section.SectionName });
                if (nodeName == null)
                {
                    Console.WriteLine($"Skipping section {section.SectionName}");
                    if (SkippedSections.ContainsKey(section.SectionName))
                        SkippedSections[section.SectionName] += 1;
                    else SkippedSections.Add(section.SectionName, 1);
                    continue;
                }
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