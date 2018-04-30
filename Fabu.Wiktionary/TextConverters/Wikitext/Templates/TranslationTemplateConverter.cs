using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class TranslationTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            template.Arguments.TryGetArray(null, out Wikitext[] args);
            template.Arguments.TryGet(out Wikitext tr, "tr");
            template.Arguments.TryGet(out Wikitext alt, "alt");

            result.Write((alt ?? args[1]).TooSmart());

            if (name.OriginalName.EndsWith("check"))
                result.Write(" (?)");

            return result;
        }
    }
}
