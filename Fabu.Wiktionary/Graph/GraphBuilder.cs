using Fabu.Wiktionary.FuzzySearch;
using Fabu.Wiktionary.Transform;
using QuickGraph.Algorithms.Search;
using QuickGraph.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using WikimediaProcessing;

namespace Fabu.Wiktionary.Graph
{
    internal partial class GraphBuilder
    {
        private readonly SectionsGraph _graph = new SectionsGraph();
        private readonly SectionNameTransform _sectionNameToVertexNameTransform;
        private readonly Dictionary<string, SectionVertex> _addedVertices = new Dictionary<string, SectionVertex>();
        private readonly Dictionary<string, int> _namesSkept = new Dictionary<string, int>();
        private readonly Dictionary<string, SectionName> _nameTransformCache = new Dictionary<string, SectionName>();

        public GraphBuilder(SectionNameTransform sectionNameToVertexNameTransform)
        {
            _graph.AddVertex(SectionVertex.Root);

            _sectionNameToVertexNameTransform = sectionNameToVertexNameTransform;
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
                var vertexName = CachedNameTransform(section);
                if (vertexName == null)
                {
                    AddSkeptSection(section);
                    continue;
                }

                vertexName.OriginalSection.Parents.Add(parentNode.OriginalSection?.Name ?? parentNode.ID);
                vertexName.OriginalSection.DepthStats.Add(depth);
                parentNode.OriginalSection?.Children.Add(vertexName.Name);

                var vertexExists = _addedVertices.TryGetValue(vertexName.Name, out SectionVertex vertex);
                if (vertexExists)
                    vertex.Count += 1;
                else
                {
                    vertex = new SectionVertex
                    {
                        ID = vertexName.Name,
                        Title = vertexName.Name,
                        Samples = new List<string> { sampleRef },
                        Count = 1,
                        OriginalSection = vertexName.OriginalSection
                    };
                }
                vertex.AddDepth(depth);
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
        
        private SectionName CachedNameTransform(WikiSection section)
        {
            if (!_nameTransformCache.TryGetValue(section.SectionName, out SectionName transformed))
            {
                transformed = _sectionNameToVertexNameTransform.Apply(new SectionName { Name = section.SectionName } );
                _nameTransformCache.Add(section.SectionName, transformed);
            }
            return transformed;
        }

        private void AddSkeptSection(WikiSection section)
        {
            if (_namesSkept.ContainsKey(section.SectionName))
                _namesSkept[section.SectionName] += 1;
            else _namesSkept.Add(section.SectionName, 1);
        }

        internal void RemoveEdges(int minimumEdgeFrequency)
        {
            _graph.RemoveEdgeIf(edge => edge.Weight < minimumEdgeFrequency);

            //var connectedComponents = new StronglyConnectedComponentsAlgorithm<SectionVertex, SectionEdge>(_graph);
            //connectedComponents.Compute();

            //_graph.RemoveVertexIf(vertex =>
            //{
            //    var connections = connectedComponents.Components[vertex];
            //    return connections == 0;
            //});
        }

        internal List<SectionVertex> LanguageNames => 
            BreadthFirstSearch(v => v.Depth == 1, v => v.Depth > 1);

        internal List<SectionVertex> AllSections =>
            BreadthFirstSearch(v => v.Depth > 1);

        public Dictionary<string, int> NamesSkept => _namesSkept;

        internal List<SectionVertex> FindOne(string name) =>
            BreadthFirstSearch(v => v.Title == name);

        internal List<SectionVertex> BreadthFirstSearch(Func<SectionVertex, bool> predicate, Func<SectionVertex, bool> stop = null)
        {
            var search = new BreadthFirstSearchAlgorithm<SectionVertex, SectionEdge>(_graph);

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
    }
}