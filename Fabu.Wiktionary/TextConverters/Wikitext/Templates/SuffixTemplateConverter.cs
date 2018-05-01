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
                    {
                        template.Arguments.TryGet(out Wikitext lit, argNum, "lit");
                        if (tr != null || gloss != null || lit != null)
                        {
                            result.WriteSpaceIfNotEmpty();
                            result.Write("(");
                        }

                        if (tr != null)
                            WriteEmphasised(result, tr);
                        if (tr != null && gloss != null)
                            result.Write(", ");
                        if (gloss != null)
                            WriteQuoted(result, gloss);
                        if (lit != null && gloss != null)
                            result.Write(", ");
                        if (lit != null)
                        {
                            result.Write("literally ");
                            WriteQuoted(result, lit.TooSmart());
                        }

                        if (tr != null || gloss != null || lit != null)
                            result.Write(")");
                    }
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

    class AffixTemplateConverter : CompoundTemplateConverter
    {
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

    class CircumfixTemplateConverter : SuffixTemplateConverter
    {
        protected override void WriteSplitterAndValue(ConversionResult result, Wikitext value, int argsWritten, int i, int count)
        {
            if (argsWritten > 0)
                result.Write(" + ");

            result.Write("<em>");
            if (i == 2)
                result.Write("-");
            result.Write(value.TooSmart());
            if (i == 0)
                result.Write("-");
            result.Write("</em>");
        }
    }
}
