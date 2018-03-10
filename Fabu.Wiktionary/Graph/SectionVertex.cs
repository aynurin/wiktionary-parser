﻿using System;
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
            ID = WiktionaryMeta.ROOT_NODE_NAME,
            Title = "PageRoot"
        };
        public static SectionVertex Root => _root;

        internal static SectionVertex From(WikiSection section, string parentTitle, string sampleRef) 
            => new SectionVertex
        {
            ID = String.Join('-', parentTitle, section.SectionName),
            Title = section.SectionName,
            Count = 0,
            Samples = new List<string>() { sampleRef }
        };

        [XmlAttribute("id")]
        public string ID { get; set; }
        [XmlAttribute("title")]
        public string Title { get; set; }
        [XmlAttribute("count")]
        public int Count { get; set; }
        [XmlIgnore]
        public List<string> Samples { get; set; }
        [XmlIgnore]
        public int Depth => _depthStats.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).First();

        private readonly Dictionary<int, int> _depthStats = new Dictionary<int, int>();
        public void AddDepth(int depth)
        {
            if (_depthStats.ContainsKey(depth))
            {
                _depthStats[depth] += 1;
            }
            else
            {
                _depthStats.Add(depth, 1);
            }
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