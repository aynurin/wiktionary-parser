using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class GivenNameTemplateConverter : BaseTemplateConverter
    {
        private readonly char[] _vowels = new[] { 'a', 'e', 'i', 'o', 'u' };
        private readonly string[] _deniedFroms = new[] { "female", "male", "surnames" };
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();
            result.Write("<em>");

            template.Arguments.TryGetOneOf(out Wikitext from, "from", 1);
            if (Array.BinarySearch(_deniedFroms, from.ToString()) >= 0)
                from = null;

            template.Arguments.TryGetArray("dim", out Wikitext[] dimValues);

            var startsWithAVowel = dimValues == null && from != null && Array.BinarySearch(_vowels, Char.ToLowerInvariant(from.ToString()[0])) >= 0;

            if (template.Arguments.TryGet(out Wikitext a, "A"))
            {
                if (a.ToString() == "a" && startsWithAVowel)
                    result.Write("an");
                else result.Write(a.TooSmart());
            }
            else
            {
                if (startsWithAVowel)
                    result.Write("An");
                else result.Write("A");
            }
            result.WriteSpaceIfNotEmpty();

            if (dimValues != null)
                result.Write("diminutive of the ");

            if (from != null)
                result.Write(from.TooSmart(), " ");


            template.Arguments.TryGetOneOf(out Wikitext gender1, "gender", 1);
            template.Arguments.TryGet(out Wikitext gender2, "or");

            if (gender1 != null && Array.BinarySearch(_deniedFroms, gender1.ToString()) >= 0)
                result.Write(gender1.TooSmart(), " ");
            if (gender2 != null)
                result.Write("or ", gender2.TooSmart(), " ");

            if (dimValues == null || dimValues.Length <= 1)
                result.Write("given name");
            else
                result.Write("given names");

            if (dimValues != null)
            {
                for (var i = 0; i < dimValues.Length; i++)
                {
                    Wikitext tr = null, alt = null;
                    if (i == 0)
                    {
                        template.Arguments.TryGet(out tr, "dimtr");
                        template.Arguments.TryGet(out alt, "dimalt");
                    }
                    else
                    {
                        template.Arguments.TryGet(out tr, i + 1, "dimtr");
                        template.Arguments.TryGet(out alt, i + 1, "dimalt");
                    }

                    if (dimValues[i] != null | alt != null)
                    {
                        if (i == 0)
                            result.Write(" ");
                        else if (i > 0)
                        {
                            if (i == dimValues.Length - 1)
                                result.Write(" or ");
                            else result.Write(", ");
                        }
                        if (alt != null)
                            result.Write(alt.TooSmart());
                        else result.Write(dimValues[i].TooSmart());
                        if (tr != null)
                            result.Write(" (", tr.TooSmart(), ")");
                    }
                }
            }

            template.Arguments.TryGetArray("eq", out Wikitext[] eqValues);

            if (eqValues != null)
            {
                result.Write(", equivalent to English");
                for (var i = 0; i < eqValues.Length; i++)
                {
                    if (i == 0)
                        result.Write(" ");
                    else if (i > 0)
                    {
                        if (i == eqValues.Length - 1)
                            result.Write(" or ");
                        else result.Write(", ");
                    }
                    result.Write(eqValues[i].TooSmart());
                }
            }

            result.Write("</em>");

            return result;
        }
    }
}
