using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    public class BaseNodeConverter
    {
        public virtual ConversionResult Convert(Node node, ConversionContext context)
        {
            return new ConversionResult();
        }

        public virtual string GetSubstitute(Node node) => null;

        protected Node MaybeARun(Wikitext content)
        {
            if (content.Lines.Count == 1)
            {
                var first = content.Lines.FirstNode;
                if (first is Paragraph)
                    return new Run(first.Inlines);
            }
            return content;
        }
    }

    public class ConversionContext
    {
        public bool ItalicsSwitched { get; set; }
        public bool BoldSwitched { get; set; }
    }

    public class ConversionResult
    {
        private readonly List<string> _data = new List<string>();

        public string Tail { get; private set; }
        public Node Node { get; internal set; }

        public void WriteTail(string tail) => Tail = tail;
        public void Write(string data) => _data.Add(data);
        public void WriteData(TextWriter writer) => _data.ForEach(d => writer.Write(d));
    }
}
