using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class PluralOfTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();
            
            result.Write("<em>");
            if (template.Arguments.IsSet("nocap"))
                result.Write("plural of");
            else
                result.Write("Plural of");
            result.Write("</em> ");

            template.Arguments.TryGet(out Wikitext value, 1);

            result.Write(value.TooSmart());

            if (name.OriginalName == "alternative plural of" && template.Arguments.TryGet(out Wikitext alternative, 2))
                result.Write(" (<em>alternative of</em> ", alternative.TooSmart(), ")");

            if (!template.Arguments.IsSet("nodot"))
                result.Write(".");

            return result;
        }
    }
}
