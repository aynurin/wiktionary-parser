using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    /// <summary>
    /// <see cref="BaseFormOfTemplatesConverter"/> for a very similar one. Maybe merge?
    /// </summary>
    class TemplateNameTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            result.Write(name.OriginalName);

            return result;
        }
    }
}
