using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    abstract class BaseFormOfTemplatesConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            var phrases = GetPhrases();
            
            result.Write("<em>");
            if (template.Arguments.TryGetArray("from", out Wikitext[] items))
            {
                for (var i = 0; i < items.Length; i++)
                {
                    if (i > 0)
                    {
                        if (i < items.Length - 1)
                            result.Write(", ");
                        else
                            result.Write(" and ");
                    }
                    result.Write(items[i].TooSmart());
                }
                result.WriteSpaceIfNotEmpty();
                result.Write(phrases[0]);
            }
            else
            {

                if (template.Arguments.IsSet("nocap"))
                    result.Write(phrases[1]);
                else
                    result.Write(phrases[2]);
            }
            result.Write("</em> ");

            template.Arguments.TryGetOneOf(out Wikitext value, 2, 1);
            template.Arguments.TryGetOneOf(out Wikitext gloss, 3);
            template.Arguments.TryGetOneOf(out Wikitext tr, "tr");

            result.Write(value.TooSmart());

            WriteTrAndGloss(result, tr, gloss);

            if (!template.Arguments.IsSet("nodot"))
                result.Write(".");

            return result;
        }

        protected abstract string[] GetPhrases();
    }

    class AlternativeSpellingOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "spelling of",
                "alternative spelling of",
                "Alternative spelling of"
            };
    }

    class StandardSpellingOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "spelling of",
                "standard spelling of",
                "Standard spelling of"
            };
    }

    class AlternativeCaseFormOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "case form of",
                "alternative case form of",
                "Alternative case form of"
            };
    }

    class AlternativeFormOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "form of",
                "alternative form of",
                "Alternative form of"
            };
    }

    class ObsoleteFormOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "obsolete form of",
                "obsolete form of",
                "Obsolete form of"
            };
    }

    class ArchaicFormOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "archaic form of",
                "archaic form of",
                "Archaic form of"
            };
    }

    class DatedFormOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "dated form of",
                "dated form of",
                "Dated form of"
            };
    }

    class LateFormOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "late form of",
                "late form of",
                "Late form of"
            };
    }

    class MisspellingOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "misspelling of",
                "misspelling of",
                "Misspelling of"
            };
    }
}
