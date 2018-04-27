using MwParserFromScratch.Nodes;
using System;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class CognateTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            context.LanguageCodes.TryGetValue(template.Arguments[1].ToString(), out string sourceLanguageName);
            if (!String.IsNullOrEmpty(sourceLanguageName))
                result.Write(sourceLanguageName);

            Wikitext value = null;
            if (template.Arguments.ContainsNotEmpty("alt"))
                value = (template.Arguments["alt"].Value);
            if (template.Arguments.ContainsNotEmpty(3))
                value = (template.Arguments[3].Value);
            if (template.Arguments.ContainsNotEmpty(2))
                value = (template.Arguments[2].Value);

            if (value != null && value.ToString() != "-")
            {
                result.Write(" <em>");
                result.Write(value.TooSmart());
                result.Write("</em>");
            }

            Wikitext translation = null;
            if (template.Arguments.ContainsNotEmpty("t"))
                translation = template.Arguments["t"].Value;
            else if (template.Arguments.ContainsNotEmpty("gloss"))
                translation = template.Arguments["gloss"].Value;
            else if (template.Arguments.ContainsNotEmpty(4))
                translation = template.Arguments[4].Value;
            if (translation != null)
            {
                result.WriteSpaceIfNotEmpty();
                result.Write("(&ldquo;");
                result.Write(translation.TooSmart());
                result.Write("&rdquo;)");
            }

            return result;
        }
    }
}
