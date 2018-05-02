using Fabu.Wiktionary.TextConverters.Wiki.Templates;
using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class MentionTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            if (template.Arguments.TryGetOneOf(out Wikitext value, 3, 2))
                result.Write("<em>", value.TooSmart(), "</em>");

            GetTrAndGloss(template, out Node tr, out Node gloss);
            if (gloss == null && template.Arguments.ContainsNotEmpty(4))
                gloss = template.Arguments[4].Value.TooSmart();
            if (tr != null || gloss != null)
                WriteTrAndGloss(result, tr, gloss);

            return result;
        }
    }
}
