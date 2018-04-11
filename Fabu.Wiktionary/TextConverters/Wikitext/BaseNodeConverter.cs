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
            var result = new ConversionResult();
            result.Write(node);
            return result;
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

    public class ConversionResult : List<object>
    {
        public void Write(string data) => Add(data);
        public void Write(Node node) => Add(node);
    }
}
