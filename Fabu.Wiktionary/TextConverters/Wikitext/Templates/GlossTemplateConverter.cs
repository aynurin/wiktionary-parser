using MwParserFromScratch.Nodes;
using System;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class GlossTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            if (template.Arguments.ContainsNotEmpty(1))
                result.Write("(", template.Arguments[1].Value.TooSmart(), ")");

            return result;
        }
    }
    class NonGlossTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            if (template.Arguments.ContainsNotEmpty(1))
                result.Write("<em>", template.Arguments[1].Value.TooSmart(), "</em>");

            return result;
        }
    }

    class SenseTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            if (template.Arguments.ContainsNotEmpty(1))
                result.Write("(<em>", template.Arguments[1].Value.TooSmart(), "</em>):");

            return result;
        }
    }

    class QualifierTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            if (template.Arguments.ContainsNotEmpty(1))
                result.Write("(<em>", template.Arguments[1].Value.TooSmart(), "</em>)");

            return result;
        }
    }
}
