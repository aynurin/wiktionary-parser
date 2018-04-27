using MwParserFromScratch.Nodes;
using System;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class InitialismOfTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            var displayText = template.Arguments.ContainsNotEmpty(2) ? template.Arguments[2].Value.TooSmart() : template.Arguments[1].Value.TooSmart();

            result.Write("<em>");
            if (template.Arguments.IsSet("nocap"))
                result.Write("initialism of</em> ");
            else
                result.Write("Initialism of</em> ");

            result.Write(displayText);

            if (GetTrAndGloss(template, out Node tr, out Node gloss))
                WriteTrAndGloss(result, tr, gloss);

            if (!template.Arguments.IsSet("nodot"))
                result.Write(".");

            return result;
        }
    }
}
