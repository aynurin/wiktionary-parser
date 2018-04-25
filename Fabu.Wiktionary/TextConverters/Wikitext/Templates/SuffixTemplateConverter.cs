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
                    Node tr = null;
                    if (template.Arguments.ContainsNotEmpty("tr" + argNum))
                        tr = template.Arguments["tr" + argNum].Value.TooSmart();
                    Node gloss = null;
                    if (template.Arguments.ContainsNotEmpty("gloss" + argNum))
                        gloss = template.Arguments["gloss" + argNum].Value.TooSmart();
                    else if (template.Arguments.ContainsNotEmpty("t" + argNum))
                        gloss = template.Arguments["t" + argNum].Value.TooSmart();

                    if (argsWritten > 0)
                    {
                        result.Write(" + -");
                    }
                    result.Write(value);
                    if (tr != null || gloss != null)
                    {
                        result.Write(" ");
                        result.Write("(");
                        if (tr != null)
                        {
                            result.Write("<em>", tr, "</em>");
                            if (gloss != null)
                                result.Write(", ");
                        }
                        if (gloss != null)
                        {
                            result.Write("&ldquo;", gloss, "&rdquo;");
                        }
                        result.Write(")");
                    }
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
                    Node tr = null;
                    if (template.Arguments.ContainsNotEmpty("tr" + argNum))
                        tr = template.Arguments["tr" + argNum].Value.TooSmart();
                    Node gloss = null;
                    if (template.Arguments.ContainsNotEmpty("gloss" + argNum))
                        gloss = template.Arguments["gloss" + argNum].Value.TooSmart();
                    else if (template.Arguments.ContainsNotEmpty("t" + argNum))
                        gloss = template.Arguments["t" + argNum].Value.TooSmart();

                    if (argsWritten > 0)
                    {
                        result.Write(" + ");
                    }
                    result.Write("<em>", value, "</em>");
                    if (i + 1 < indexedArgs.Count)
                    {
                        result.Write("-");
                    }
                    if (tr != null || gloss != null)
                    {
                        result.Write(" ");
                        result.Write("(");
                        if (tr != null)
                        {
                            result.Write("<em>", tr, "</em>");
                            if (gloss != null)
                                result.Write(", ");
                        }
                        if (gloss != null)
                        {
                            result.Write("&ldquo;", gloss, "&rdquo;");
                        }
                        result.Write(")");
                    }
                    argsWritten++;
                }
            }

            return result;
        }
    }
}
