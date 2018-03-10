using QuickGraph;
using System;

namespace Fabu.Wiktionary.Graph
{
    internal partial class GraphBuilder
    {
        [Serializable]
        class SectionsGraph : AdjacencyGraph<SectionVertex, SectionEdge> { }
    }
}