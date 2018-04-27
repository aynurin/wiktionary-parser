using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class SuffixTemplateConverter : BaseTemplateConverter
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
                var argNum = (i + 1);
                var arg = indexedArgs[i];
                if (template.Arguments.ContainsNotEmpty("alt" + argNum.ToString()))
                    arg = template.Arguments["alt" + argNum.ToString()];
                if (!arg.Value.IsEmpty())
                {
                    WriteSplitterAndValue(result, arg.Value, argsWritten, i, indexedArgs.Count);
                    if (GetTrAndGloss(template, argNum, out Node tr, out Node gloss))
                        WriteTrAndGloss(result, tr, gloss);
                    argsWritten++;
                }
            }

            return result;
        }

        protected virtual void WriteSplitterAndValue(ConversionResult result, Wikitext value, int argsWritten, int i, int count)
        {
            if (argsWritten > 0)
                result.Write(" + -");
            result.Write(value.TooSmart());
        }
    }

    class CompoundTemplateConverter : SuffixTemplateConverter
    {
        protected override void WriteSplitterAndValue(ConversionResult result, Wikitext value, int argsWritten, int i, int count)
        {
            if (argsWritten > 0)
                result.Write(" + ");
            result.Write("<em>", value.TooSmart(), "</em>");
        }
    }

    class ConfixTemplateConverter : SuffixTemplateConverter
    {
        protected override void WriteSplitterAndValue(ConversionResult result, Wikitext value, int argsWritten, int i, int count)
        {
            if (argsWritten > 0)
                result.Write("- + -");
            result.Write("<em>", value.TooSmart(), "</em>");
        }
    }
    
    class PrefixTemplateConverter : SuffixTemplateConverter
    {
        protected override void WriteSplitterAndValue(ConversionResult result, Wikitext value, int argsWritten, int i, int count)
        {
            if (argsWritten > 0)
                result.Write(" + ");
            result.Write("<em>", value.TooSmart(), "</em>");
            if (i + 1 < count)
            {
                result.Write("-");
            }
        }
    }
}
