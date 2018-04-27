using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    /// <summary>
    /// Suffix template works very strange in Wiktionary, sometimes some parts are italic, sometimes quoted, sometimes other parts are italic and quoted.
    /// I couldn't induct any logic, so I will make all parts of the word normal, all transcriptions italic, and all glosses - quoted.
    /// </summary>
    class HomophonesTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            var homophones = new List<object>();

            for (var i = 1; i <= template.Arguments.Count; i++)
            {
                if (template.Arguments.ContainsNotEmpty(i))
                    homophones.Add(template.Arguments[i].Value.TooSmart());
            }

            if (homophones.Count == 1)
                result.Write("Homophone: ");
            else if (homophones.Count > 1)
                result.Write("Homophones: ");

            for (var i = 0; i < homophones.Count; i++)
            {
                if (i > 0)
                    result.Write(", ");
                result.Write(homophones[i]);
            }

            return result;
        }
    }
}
