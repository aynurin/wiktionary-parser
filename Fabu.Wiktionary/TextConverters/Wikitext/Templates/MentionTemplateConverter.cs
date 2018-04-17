using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    class MentionTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();
            
            if (template.Arguments.Contains(3) && !template.Arguments[3].Value.IsEmpty())
                result.Write(template.Arguments[3].Value.TooSmart());
            else
                result.Write(template.Arguments[2].Value.TooSmart());

            Wikitext translation = null;
            if (template.Arguments.Contains("t"))
                translation = template.Arguments["t"].Value;
            else if (template.Arguments.Contains("gloss"))
                translation = template.Arguments["gloss"].Value;
            else if (template.Arguments.Contains(4))
                translation = template.Arguments[4].Value;
            if (translation != null)
            {
                result.Write(" (&ldquo;");
                result.Write(translation.TooSmart());
                result.Write("&rdquo;)");
            }

            return result;
        }
    }
}
