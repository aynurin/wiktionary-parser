using MwParserFromScratch.Nodes;
using System;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    /// <summary>
    /// This template is discontinued.
    /// </summary>
    class EtylTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            context.LanguageCodes.TryGetValue(template.Arguments[1].ToString(), out string sourceLanguageName);
            if (!String.IsNullOrEmpty(sourceLanguageName))
                result.Write(sourceLanguageName);

            return result;
        }
    }
}
