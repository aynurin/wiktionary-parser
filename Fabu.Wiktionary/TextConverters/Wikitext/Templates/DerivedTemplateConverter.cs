using System;
using System.Linq;
using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
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

            if (template.Arguments.TryGetOneOf(out Wikitext value, "alt", 4, 3))
            {
                if (value.ToString() != "-")
                    result.Write("<em>", value.TooSmart(), "</em>");
            }

            GetTrAndGloss(template, out Node tr, out Node gloss);
            if (gloss == null && template.Arguments.ContainsNotEmpty(5))
                gloss = template.Arguments[5].Value.TooSmart();
            if (tr != null || gloss != null)
                WriteTrAndGloss(result, tr, gloss);

            return result;
        }
    }
}
