using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    class SuffixTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();
            var argsWritten = 0;
            
            var indexedArgs = template.Arguments.Where(a => a.Name == null).Skip(1).ToList();
            for (var i = 0; i < indexedArgs.Count; i++)
            {
                var arg = indexedArgs[i];
                if (!arg.Value.IsEmpty())
                {
                    var value = arg.Value.TooSmart();
                    var argNum = (i + 1).ToString();

                    if (argsWritten > 0)
                    {
                        result.Write(" + -");
                    }
                    result.Write(value);
                    if (GetTrAndGloss(template, argNum, out Node tr, out Node gloss))
                        WriteTrAndGloss(result, tr, gloss);
                    argsWritten++;
                }
            }

            return result;
        }
    }

    class PrefixTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();
            var argsWritten = 0;

            var tmpArgs = template.Arguments.Where(a => a.Name == null);
            if (!template.Arguments.Contains("lang"))
                tmpArgs = tmpArgs.Skip(1);
            var indexedArgs = tmpArgs.ToList();
            for (var i = 0; i < indexedArgs.Count; i++)
            {
                var arg = indexedArgs[i];
                if (!arg.Value.IsEmpty())
                {
                    var value = arg.Value.TooSmart();
                    var argNum = (i + 1).ToString();

                    if (argsWritten > 0)
                    {
                        result.Write(" + ");
                    }
                    result.Write("<em>", value, "</em>");
                    if (i + 1 < indexedArgs.Count)
                    {
                        result.Write("-");
                    }
                    if (GetTrAndGloss(template, argNum, out Node tr, out Node gloss))
                        WriteTrAndGloss(result, tr, gloss);
                    argsWritten++;
                }
            }

            return result;
        }
    }
}
