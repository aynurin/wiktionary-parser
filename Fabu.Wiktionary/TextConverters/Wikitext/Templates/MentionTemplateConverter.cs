using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    class MentionTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();
            
            if (template.Arguments.ContainsNotEmpty(3))
                result.Write(template.Arguments[3].Value.TooSmart());
            else if (template.Arguments.ContainsNotEmpty(2))
                result.Write(template.Arguments[2].Value.TooSmart());

            if (template.Arguments.ContainsNotEmpty("tr"))
                result.Write("(<em>", template.Arguments["tr"].Value.TooSmart(), "</em>)");

            Wikitext translation = null;
            if (template.Arguments.Contains("t"))
                translation = template.Arguments["t"].Value;
            else if (template.Arguments.Contains("gloss"))
                translation = template.Arguments["gloss"].Value;
            else if (template.Arguments.Contains(4))
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
