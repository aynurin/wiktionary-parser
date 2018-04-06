using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    public class BaseNodeConverter
    {
        public readonly static Stats<string> ConvertedNodes = new Stats<string>();

        private readonly Type[] _okNodes = new Type[]
        {
            typeof(Wikitext),
            typeof(Run),
            typeof(TemplateArgument)
        };

        public virtual ConversionResult Convert(Node node, ConversionContext context)
        {
            if (!_okNodes.Any(type => type == node.GetType()))
            {
                Debugger.Break();
                ConvertedNodes.Add(node.GetType().Name);
            }
            return new ConversionResult();
        }

        public virtual string GetSubstitute(Node node) => null;

        protected Node MaybeARun(Wikitext content)
        {
            if (content.Lines.Count == 1)
            {
                var first = content.Lines.FirstNode;
                if (first is Paragraph)
                    return new Run(first.EnumChildren().Select(n => n as InlineNode));
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
