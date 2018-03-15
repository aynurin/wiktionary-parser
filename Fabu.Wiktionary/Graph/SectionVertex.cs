using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using WikimediaProcessing;

namespace Fabu.Wiktionary.Graph
{
    [Serializable]
    internal class SectionVertex
    {
        private static SectionVertex _root = new SectionVertex
        {
            ID = SimpleSectionsCategorizer.RootSectionName,
            Title = SimpleSectionsCategorizer.RootSectionName,
            OriginalSection = new SectionName { Name = SimpleSectionsCategorizer.RootSectionName }
        };
        private static SectionVertex _lang = new SectionVertex
        {
            ID = SimpleSectionsCategorizer.LanguageSectionName,
            Title = SimpleSectionsCategorizer.LanguageSectionName,
            OriginalSection = new SectionName { Name = SimpleSectionsCategorizer.LanguageSectionName }
        };
        public static SectionVertex Root => _root;
        public static SectionVertex Lang => _lang;

        internal static SectionVertex From(WikiSection section, string parentTitle, string sampleRef) 
            => new SectionVertex
        {
            ID = String.Join('-', parentTitle, section.SectionName),
            Title = section.SectionName,
            Count = 0,
            Samples = new List<string>() { sampleRef }
        };

        [XmlAttribute("id")]
        [JsonProperty("id")]
        public string ID { get; set; }
        [XmlAttribute("title")]
        [JsonProperty("title")]
        public string Title { get; set; }
        [XmlAttribute("count")]
        [JsonProperty("count")]
        public int Count { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public List<string> Samples { get; set; } = new List<string>();
        [XmlIgnore]
        [JsonIgnore]
        public int Depth => _depthStats.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).First();
        [XmlIgnore]
        [JsonIgnore]
        public Dictionary<int, int> DepthStats { get => _depthStats; }
        [XmlIgnore]
        [JsonIgnore]
        public SectionName OriginalSection { get; internal set; }

        private readonly Dictionary<int, int> _depthStats = new Dictionary<int, int>();
        public void AddDepth(int depth)
        {
            if (_depthStats.ContainsKey(depth))
                _depthStats[depth] += 1;
            else
                _depthStats.Add(depth, 1);
        }

        public override bool Equals(object obj)
        {
            var other = obj as SectionVertex;
            if (other == null)
                return false;
            return ID.Equals(other.ID);
        }

        public override int GetHashCode() => ID.GetHashCode();

        public override string ToString() => ID;

        public void WriteAsTsv(TextWriter writer, int padding = -1)
        {
            if (padding == -1) padding = Depth;
            writer.Write(new String('\t', padding));
            writer.Write(Title);
            writer.Write('\t');
            writer.Write(Count);
            writer.WriteLine();
        }
    }
}