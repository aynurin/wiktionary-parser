using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class IPATemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            result.Write("IPA: ");

            bool writeComma = false;

            for (var i = 1; i <= template.Arguments.Count; i++)
            {
                if (template.Arguments[i] != null)
                {
                    if (writeComma)
                        result.Write(", ");
                    result.Write(template.Arguments[i].ToString());
                    writeComma = true;
                }
            }

            return result;
        }
    }
}
