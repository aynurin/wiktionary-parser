using MwParserFromScratch.Nodes;
using System;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    /// <summary>
    /// This template is discontinued.
    /// </summary>
    class AudioTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            Wikitext fileName = null, text = null;
            string language = null;

            if (template.Arguments.ContainsNotEmpty(2))
                text = (template.Arguments[2].Value);
            if (template.Arguments.ContainsNotEmpty(1))
                fileName = (template.Arguments[1].Value);
            if (template.Arguments.ContainsNotEmpty("lang"))
            {
                if (!context.LanguageCodes.TryGetValue(template.Arguments["lang"].ToString(), out language))
                    language = null;
            }

            if (context.Arguments.SectionName == "Pronunciation")
                context.AddPronunciation(language, fileName.ToString(), text);

            if (template.Name.ToString() == "audio-IPA" && text != null)
            {
                result.Write("IPA: ");
                result.Write(text.TooSmart());
            }

            return result;
        }
    }
}
