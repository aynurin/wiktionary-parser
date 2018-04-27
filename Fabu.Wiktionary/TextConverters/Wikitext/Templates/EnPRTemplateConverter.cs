using MwParserFromScratch.Nodes;
using System;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class EnPRTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            if (template.Arguments.ContainsNotEmpty(1))
            {
                result.Write("enPR: ");
                result.Write(template.Arguments[1].Value.TooSmart());
            }

            return result;
        }
    }
}
