using Fabu.Wiktionary.Transform;
using QuickGraph.Algorithms.ConnectedComponents;
using QuickGraph.Algorithms.Search;
using QuickGraph.Serialization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using WikimediaProcessing;

namespace Fabu.Wiktionary.Graph
{
    internal partial class GraphBuilder
    {
        private readonly SectionsGraph _graph = new SectionsGraph();
        private readonly BaseTransform<string, string> _sectionTransform;
        private readonly WiktionaryMeta _wiktionary;
        private readonly Dictionary<string, SectionVertex> _addedVertices = new Dictionary<string, SectionVertex>();

        public GraphBuilder(BaseTransform<string,string> sectionTransform, WiktionaryMeta wiktionary)
        {
            _graph.AddVertex(SectionVertex.Root);

            _sectionTransform = sectionTransform;
            
            _wiktionary = wiktionary;
        }

        internal void AddPage(WikimediaPage page)
        {
            if (page.IsSpecialPage || page.IsRedirect || page.IsDisambiguation)
                return;
            SectionVertex.Root.Count += 1;
            SectionVertex.Root.AddDepth(0);
            AddSectionsToGraph(SectionVertex.Root, page.Sections, page.Title);
        }

        private void AddSectionsToGraph(SectionVertex parentNode, ICollection<WikiSection> childSections, 
            string sampleRef, int depth = 1)
        {
            foreach (var section in childSections)
            {
                var sectionName = _sectionTransform.Apply(section.SectionName);
                var vertexExists = _addedVertices.TryGetValue(sectionName, out SectionVertex vertex);
                if (vertexExists)
                {
                    vertex.AddDepth(depth);
                    vertex.Count += 1;
                }
                else 
                {
                    vertex = new SectionVertex
                    {
                        ID = sectionName,//String.Join('-', parentNode.Title, sectionName),
                        Title = sectionName,
                        Samples = new List<string> { sampleRef },
                        Count = 1
                    };
                    vertex.AddDepth(depth);
                }
                // Is there a similar section already connected to this parent?
                if (_graph.TryGetEdge(parentNode, vertex, out SectionEdge existingEdge))
                {
                    existingEdge.Weight += 1;
                    existingEdge.Target.Samples.Add(sampleRef);
                    existingEdge.Target.Count += 1;
                }
                else
                {
                    var newEdge = new SectionEdge
                    {
                        ID = String.Join(">>", parentNode.ID, vertex.ID),
                        Source = parentNode,
                        Target = vertex,
                        Weight = 1
                    };
                    if (!vertexExists)
                    {
                        _graph.AddVertex(vertex);
                        _addedVertices.Add(vertex.ID, vertex);
                    }
                    _graph.AddEdge(newEdge);
                }
                AddSectionsToGraph(vertex, section.SubSections, sampleRef, depth + 1);
            }
        }

        internal void Serialize(TextWriter output)
        {
            using (var xmlWriter = new XmlTextWriter(output))
            {
                try
                {
                    _graph.SerializeToGraphML<SectionVertex, SectionEdge, SectionsGraph>(
                        xmlWriter,
                        v => v.ID,
                        e => e.ID);
                }
                catch (ArgumentException ex)
                {
                    // strange error when finalizing the xml, but the xml is valid.
                    if (ex.Source != "System.Private.Xml")
                        throw;
                }
                finally
                {
                    xmlWriter.Flush();
                }
            }
        }

        internal List<SectionVertex> LanguageNames => 
            BreadthFirstSearch(
                v => {
                    var isLanguage = v.Depth == 1 && !_wiktionary.TryGetStandardSectionName(v.Title, out string _);
                    if (v.ID == "Etymology")
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                    return isLanguage;
                },
                v => v.Depth > 1);

        internal List<SectionVertex> AllSections =>
            BreadthFirstSearch(v => v.Depth > 1 && !_wiktionary.TryGetLanguage(v.Title, out string _));

        internal List<SectionVertex> FindOne(string name) =>
            BreadthFirstSearch(v => v.Title == name);

        internal List<SectionVertex> BreadthFirstSearch(Func<SectionVertex, bool> predicate, Func<SectionVertex, bool> stop = null)
        {
            var search = new BreadthFirstSearchAlgorithm<SectionVertex, SectionEdge>(_graph);
            var connectedComponents = new StronglyConnectedComponentsAlgorithm<SectionVertex, SectionEdge>(_graph);
            connectedComponents.Compute();

            var result = new List<SectionVertex>(11000);
            search.DiscoverVertex += (vertex) =>
            {
                if (predicate(vertex))
                    result.Add(vertex);
                if (stop != null && stop(vertex))
                    search.Abort();
            };
            search.Compute(SectionVertex.Root);
            result.TrimExcess();
            return result;
        }
    }
}