using MwParserFromScratch.Nodes;
using System;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
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
                if (!context.LanguageCodes.TryGetValue(template.Arguments["lang"].Value.ToString(), out language))
                    language = template.Arguments["lang"].Value.ToString();
            }

            if (context.Arguments.SectionName == "Pronunciation")
                context.AddPronunciation(language, fileName.ToString(), text.ToString());

            if (template.Name.ToString() == "audio-IPA" && text != null)
            {
                result.Write("IPA: ");
                result.Write(text.TooSmart());
            }

            return result;
        }
    }
}
