using System;
using System.Collections.Generic;
using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class LabelTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            context.LanguageCodes.TryGetValue(template.Arguments[1].ToString(), out string sourceLanguageName);

            bool skipComma = true;
            bool anyArgsWritten = false;

            if (template.Arguments.Count > 0)
                result.Write("(");

            //if (!String.IsNullOrEmpty(sourceLanguageName))
            //{
            //    result.Write(sourceLanguageName);
            //    skipComma = false;
            //    anyArgsWritten = true;
            //}

            for (var i = 2; i <= template.Arguments.Count; i++)
            {
                if (!template.Arguments.ContainsNotEmpty(i))
                    continue;

                var value = template.Arguments[i].ToString();
                _controlLabels.TryGetValue(value, out LabelControl control);
                if (control == null)
                    control = LabelControl.Default();
                if (!skipComma && !control.omit_preComma)
                    result.Write(",");
                skipComma = control.omit_postComma;
                var actualValue = control.display ?? value;
                if (!control.omit_preSpace && !String.IsNullOrWhiteSpace(actualValue) && anyArgsWritten)
                    result.Write(" ");

                result.Write(actualValue);
                if (!String.IsNullOrWhiteSpace(actualValue))
                    anyArgsWritten = true;
            }

            if (template.Arguments.Count > 0)
                result.Write(")");

            return result;
        }

        class LabelControl
        {
            internal string display;
            internal bool omit_preComma;
            internal bool omit_postComma;
            internal bool omit_preSpace;
            internal string [] pos_categories;

            internal static LabelControl Default()
            {
                return new LabelControl()
                {
                    display = null,
                    omit_postComma = false,
                    omit_preComma = false,
                    omit_preSpace = false
                };
            }
        }

        private Dictionary<string, LabelControl> _controlLabels = new Dictionary<string, LabelControl>()
        {
            //  Helper labels

            { "_", new LabelControl() {
                display = "",
                omit_preComma = true,
                omit_postComma = true,
            } },

            { "also", new LabelControl() {
                omit_postComma = true,
            } },

            { "and", new LabelControl() {
                omit_preComma = true,
                omit_postComma = true,
            } },

            { "&", new LabelControl() {
                display = "and",
                omit_preComma = true,
                omit_postComma = true,
            } },
            //aliases['&'] = 'and'

            { "or", new LabelControl() {
                omit_preComma = true,
                omit_postComma = true,
            } },

            { ";", new LabelControl() {
                omit_preComma = true,
                omit_postComma = true,
                omit_preSpace = true,
            } },

            // combine with 'except in', 'outside'? or retain for entries like "wnuczę"?
            { "except", new LabelControl() {
                omit_preComma = true,
                omit_postComma = true,
            } },

            { "outside", new LabelControl() {
                omit_preComma = true,
                omit_postComma = true,
            } },

            { "except in", new LabelControl() {
                display = "outside",
                omit_preComma = true,
                omit_postComma = true,
            } },
            //aliases['except in'] = 'outside'

            // Qualifier labels

            { "chiefly", new LabelControl() {
                omit_postComma = true,
            } },

            { "mainly", new LabelControl() {
                omit_postComma = true,
            } },

            { "mostly", new LabelControl() {
                omit_postComma = true,
            } },

            { "primarily", new LabelControl() {
                omit_postComma = true,
            } },
            //aliases['mainly'] = 'chiefly'
            //aliases['mostly'] = 'chiefly'
            //aliases['primarily'] = 'chiefly'

            { "especially", new LabelControl() {
                omit_postComma = true,
            } },

            { "particularly", new LabelControl() {
                omit_postComma = true,
            } },

            { "excluding", new LabelControl() {
                omit_postComma = true,
            } },

            { "extremely", new LabelControl() {
                omit_postComma = true,
            } },

            { "frequently", new LabelControl() {
                omit_postComma = true,
            } },

            { "humorously", new LabelControl() { omit_postComma = true,
                // should be "terms with X senses", leaving "X terms" to the term-context temp?
                pos_categories = new [] { "jocular terms" },
            } },

            { "including", new LabelControl() {
                omit_postComma = true,
            } },

            { "many", new LabelControl() {
                omit_postComma = true,
            } },// -- e.g. "many dialects"

            { "markedly", new LabelControl() {
                omit_postComma = true,
            } },

            { "mildly", new LabelControl() {
                omit_postComma = true,
            } },

            { "now", new LabelControl() {
                omit_postComma = true,
            } },

            { "nowadays", new LabelControl() {
                omit_postComma = true,
            } },
            //aliases['nowadays'] = 'now'

            { "of", new LabelControl() {
                omit_postComma = true,
            } },

            { "of a", new LabelControl() {
                omit_postComma = true,
            } },

            { "of an", new LabelControl() {
                omit_postComma = true,
            } },

            { "often", new LabelControl() {
                omit_postComma = true,
            } },

            { "originally", new LabelControl() {
                omit_postComma = true,
            } },

            { "possibly", new LabelControl() {
                omit_postComma = true,
            } },

            { "perhaps", new LabelControl() {
                omit_postComma = true,
            } },

            { "rarely", new LabelControl() {
                omit_postComma = true,
            } },

            { "slightly", new LabelControl() {
                omit_postComma = true,
            } },

            { "sometimes", new LabelControl() {
                omit_postComma = true,
            } },

            { "somewhat", new LabelControl() {
                omit_postComma = true,
            } },

            { "strongly", new LabelControl() {
                omit_postComma = true,
            } },

            { "typically", new LabelControl() {
                omit_postComma = true,
            } },

            { "usually", new LabelControl() {
                omit_postComma = true,
            } },

            { "very", new LabelControl() {
                omit_postComma = true,
            } }
        };
    }
}
