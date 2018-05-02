using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class TemplateNameTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            result.Write(name.OriginalName);

            return result;
        }
    }

    class TemplateValueTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            if(template.Arguments.TryGetArray(null, out Wikitext[] args))
            {
                for (var i = 0; i < args.Length; i++)
                {
                    if (i > 0)
                        result.Add(" ");
                    result.Add(args[i].TooSmart());
                }
            }

            return result;
        }
    }
}
