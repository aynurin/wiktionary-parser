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

            WriteStart(result, template);

            var tmpArgs = template.Arguments.Where(a => a.Name == null);
            if (!template.Arguments.Contains("lang"))
                tmpArgs = tmpArgs.Skip(1);
            var indexedArgs = tmpArgs.ToList();
            for (var i = 0; i < indexedArgs.Count; i++)
            {
                var argNum = (i + 1);
                var arg = indexedArgs[i].Value;
                if (template.Arguments.TryGet(out Wikitext alt, argNum, "alt"))
                    arg = alt;
                if (!arg.IsEmpty())
                {
                    WriteSplitterAndValue(result, arg, argsWritten, i, indexedArgs.Count);
                    if (GetTrAndGloss(template, argNum, out Node tr, out Node gloss))
                        WriteTrAndGloss(result, tr, gloss);
                    argsWritten++;
                }
            }
            WriteEnd(result, template, argsWritten);

            return result;
        }

        protected virtual void WriteStart(ConversionResult result, Template template)
        {
            // no heading is necessary in most cases
        }

        protected virtual void WriteEnd(ConversionResult result, Template template, int argsWritten)
        {
            // no heading is necessary in most cases
        }

        protected virtual void WriteSplitterAndValue(ConversionResult result, Wikitext value, int argsWritten, int i, int count)
        {
            if (argsWritten > 0)
                result.Write(" + -");
            result.Write(value.TooSmart());
        }
    }

    class BlendTemplateConverter : SuffixTemplateConverter
    {
        protected override void WriteStart(ConversionResult result, Template template)
        {
            if (!template.Arguments.IsSet("notext"))
            {
                if (template.Arguments.IsSet("nocap"))
                    result.Write("blend of ");
                else
                    result.Write("Blend of ");
            }
        }

        protected override void WriteSplitterAndValue(ConversionResult result, Wikitext value, int argsWritten, int i, int count)
        {
            if (argsWritten > 0)
                result.Write(" + ");
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
