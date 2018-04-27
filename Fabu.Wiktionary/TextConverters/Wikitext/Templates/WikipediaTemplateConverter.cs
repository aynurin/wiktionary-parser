using MwParserFromScratch.Nodes;
using System;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class WikipediaTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            if (template.Arguments.ContainsNotEmpty(2))
                result.Write(template.Arguments[2].Value.TooSmart());
            else if (template.Arguments.ContainsNotEmpty(1))
                result.Write(template.Arguments[1].Value.TooSmart());

            return result;
        }
    }
}
