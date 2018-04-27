using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class UsageTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();
            if (template.Arguments.ContainsNotEmpty(2))
                result.Add(template.Arguments[2].Value.TooSmart());
            return result;
        }
    }
}
