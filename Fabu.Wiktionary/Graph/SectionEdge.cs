using QuickGraph;
using System;
using System.Xml.Serialization;

namespace Fabu.Wiktionary.Graph
{
    [Serializable]
    internal class SectionEdge : IEdge<SectionVertex>
    {
        public SectionVertex Source { get; set; }
        public SectionVertex Target { get; set; }
        [XmlAttribute("weight")]
        public int Weight { get; set; }
        [XmlAttribute("id")]
        public string ID { get; set; }
    }
}