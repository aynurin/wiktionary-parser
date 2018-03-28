using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    static class TemplateArgumentsExtensions
    {
        /// <summary>
        /// This is a dirty hack to convert all templates if specific converters are not implemented.
        /// </summary>
        /// <remarks>
        /// It ignores template argument names. But for best results this shouldn't be run at all.
        /// </remarks>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Run ToRun(this TemplateArgumentCollection args)
        {
            var nodes = new List<InlineNode>();
            foreach (var art in args)
            {
                foreach (var line in art.Value.Lines)
                {
                    nodes.AddRange(line.EnumChildren().Select(n => n as InlineNode));
                }
            }
            return new Run(nodes);
        }
    }

    class DefdateTemplateConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var template = node as Template;
            var result = new ConversionResult();
            result.Write("<span class=\"defdate\">");
            result.WriteTail("</span>");
            result.Node = template.Arguments.ToRun();
            return result;
        }
    }
}
