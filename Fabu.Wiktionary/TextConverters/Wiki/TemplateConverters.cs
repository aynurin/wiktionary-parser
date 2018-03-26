using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    class DefdateTemplateConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var template = node as Template;
            var result = new ConversionResult();
            result.Write("<span class=\"defdate\">");
            result.WriteTail("</span>");
            var nodes = new List<InlineNode>();
            foreach (var art in template.Arguments)
            {
                Debug.Assert(art.Name == null);
                foreach (var line in art.Value.Lines)
                {
                    nodes.AddRange(line.EnumChildren().Select(n => n as InlineNode));
                }
            }
            result.Node = new Run(nodes);
            return result;
        }
    }
}
