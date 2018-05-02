using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class ColorPanelTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            if(template.Arguments.TryGet(out Wikitext value, 1))
            {
                var color = value.ToString();
                result.Write($"<span color=\"{color}\" style=\"background-color:#{color}; display:inline-block; width:80px;\">&nbsp;</span>");
            }

            return result;
        }
    }
}
