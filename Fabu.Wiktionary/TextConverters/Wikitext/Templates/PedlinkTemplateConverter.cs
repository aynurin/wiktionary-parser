using System;
using System.Collections.Generic;
using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class PedlinkTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            if (template.Arguments.ContainsNotEmpty("disp"))
                result.Write(template.Arguments["disp"].Value.TooSmart());
            else
                result.Write(template.Arguments[1].Value.TooSmart());

            return result;
        }
    }
}
