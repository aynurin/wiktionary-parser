using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class PedlinkTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            template.Arguments.TryGetOneOf(out Wikitext value, "disp", 1);
            result.Write(value.TooSmart());

            return result;
        }
    }
    class TaxlinkTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            template.Arguments.TryGetOneOf(out Wikitext value, "disp", 1);
            result.Write("<em>", value.TooSmart(), "</em>");

            return result;
        }
    }
}
