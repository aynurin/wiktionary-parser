using System;
using System.Linq;
using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    class DerivedTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            context.LanguageCodes.TryGetValue(template.Arguments[2].ToString(), out string sourceLanguageName);
            
            if (!String.IsNullOrEmpty(sourceLanguageName))
            {
                result.Write(sourceLanguageName);
                result.WriteTrailingSpace();
            }

            if (template.Arguments.ContainsNotEmpty("alt"))
                result.Write("<em>", template.Arguments["alt"].Value.TooSmart(), "</em>");
            if (template.Arguments.ContainsNotEmpty(4))
                result.Write("<em>", template.Arguments[4].Value.TooSmart(), "</em>");
            else if (template.Arguments.ContainsNotEmpty(3))
                result.Write("<em>", template.Arguments[3].Value.TooSmart(), "</em>");

            Wikitext translation = null;
            if (template.Arguments.ContainsNotEmpty("t"))
                translation = template.Arguments["t"].Value;
            else if (template.Arguments.ContainsNotEmpty("gloss"))
                translation = template.Arguments["gloss"].Value;
            else if (template.Arguments.ContainsNotEmpty(5))
                translation = template.Arguments[5].Value;
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
