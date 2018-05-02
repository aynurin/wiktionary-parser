using System;
using System.Collections.Generic;
using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class ContextTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();
            if(template.Arguments.TryGetArray(null, out Wikitext[] args))
            {
                if (args.Length > 0)
                {
                    result.Write("(");
                    for (var i = 0; i < args.Length; i++)
                    {
                        if (i > 0)
                            result.Write(", ");
                        result.Write("<em>", args[i].TooSmart(), "</em>");
                    }
                    result.Write(")");
                }
            }
            return result;
        }
    }
}
