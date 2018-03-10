using Fabu.Wiktionary;
using System.Collections.Generic;

namespace Fabu.Wiktionary.Graph
{
    internal partial class GraphBuilder
    {
        public class Options
        {
            public Dictionary<string, int> Languages { get; set; }
            public Dictionary<string, string> Sections { get; set; }
            public bool OptimizeSectionNames { get; set; }
        }
    }
}