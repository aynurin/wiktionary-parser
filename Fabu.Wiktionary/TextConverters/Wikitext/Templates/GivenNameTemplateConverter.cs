using MwParserFromScratch.Nodes;
using System;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class GivenNameTemplateConverter : BaseTemplateConverter
    {
        private readonly char[] _vowels = new[] { 'a', 'e', 'i', 'o', 'u' };
        private readonly string[] _deniedFroms = new[] { "female", "male", "surnames" };
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();
            var keywords = GetKeywords();

            result.Write("<em>");

            template.Arguments.TryGet(out Wikitext firstArg, 1);
            template.Arguments.TryGet(out Wikitext from, "from");
            template.Arguments.TryGet(out Wikitext gender1, "gender");

            if (firstArg != null && Array.BinarySearch(_deniedFroms, firstArg.ToString()) >= 0)
            {
                gender1 = firstArg;
                firstArg = null;
            }
            if (from != null && Array.BinarySearch(_deniedFroms, from.ToString()) >= 0)
            {
                from = firstArg;
                firstArg = null;
            }

            template.Arguments.TryGetArray("dim", out Wikitext[] dimValues);

            var openingString = dimValues != null ? "diminutive"
                : from != null ? from.ToString()
                : firstArg != null ? firstArg.ToString()
                : keywords[0];

            var startsWithAVowel = Array.BinarySearch(_vowels, Char.ToLowerInvariant(openingString[0])) >= 0;

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

            if (firstArg != null)
                result.Write(firstArg.TooSmart(), " ");


            template.Arguments.TryGet(out Wikitext gender2, "or");

            if (gender1 != null && Array.BinarySearch(_deniedFroms, gender1.ToString()) >= 0)
                result.Write(gender1.TooSmart(), " ");
            if (gender2 != null)
                result.Write("or ", gender2.TooSmart(), " ");

            if (dimValues == null || dimValues.Length <= 1)
                result.Write(keywords[0]);
            else
                result.Write(keywords[1]);

            if (name.OriginalName == "historical given name")
                result.Write(" of historical usage");

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

            if (name.OriginalName == "historical given name" && template.Arguments.TryGet(out Wikitext bore, 2))
                result.Write(", notably borne by ", bore.TooSmart());

            WriteDot(result, template);

            result.Write("</em>");

            return result;
        }

        protected virtual string[] GetKeywords() => new string[] { "given name", "given names" };
    }

    class SurnameTemplateConverter : GivenNameTemplateConverter
    {
        protected override string[] GetKeywords() => new string[] { "surname", "surnames" };
    }
}
