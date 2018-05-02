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

            template.Arguments.TryGetOneOf(out Wikitext value, 2, 1);

            if (value == null || template.Arguments.IsSet("notext"))
                return result;

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
                result.Write(DefaultNoCap);
            }
            else
            {

                if (template.Arguments.IsSet("nocap"))
                    result.Write(DefaultNoCap);
                else
                    result.Write(DefaultCap);
            }
            result.Write("</em>");

            result.WriteSpaceIfNotEmpty();
            result.Write(value.TooSmart());

            template.Arguments.TryGetOneOf(out Wikitext gloss, 3);
            template.Arguments.TryGetOneOf(out Wikitext tr, "tr");

            WriteTrAndGloss(result, tr, gloss);

            if (!template.Arguments.IsSet("nodot"))
                result.Write(".");

            return result;
        }

        protected abstract string DefaultCap { get; }
        protected abstract string DefaultNoCap { get; }
    }

    class FormOfTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            if (template.Arguments.TryGet(out Wikitext definition, 1))
            {
                result.Write(definition.TooSmart());
            }
            if (template.Arguments.TryGetOneOf(out Wikitext value, 3, 2))
            {
                if (result.Count > 0)
                    result.Write(" of ");
                result.Write("<em>", value.TooSmart(), "</em>");
            }
            template.Arguments.TryGetOneOf(out Wikitext gloss, "gloss", 4, "t");
            template.Arguments.TryGetOneOf(out Wikitext tr, "tr");
            WriteTrAndGloss(result, tr, gloss);

            return result;
        }
    }

    class AlternativeSpellingOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Alternative spelling of";
        protected override string DefaultNoCap => "spelling of";
    }

    class ObsoleteSpellingOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Obsolete spelling of";
        protected override string DefaultNoCap => "obsolete spelling of";
    }

    class StandardSpellingOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Spelling of";
        protected override string DefaultNoCap => "spelling of";
    }

    class AlternativeCaseFormOfTemplateConverter : AlternativeFormOfTemplateConverter
    {
    }

    class AlternativeFormOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Alternative of";
        protected override string DefaultNoCap => "alternative of";
    }

    class ObsoleteFormOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Obsolete form of";
        protected override string DefaultNoCap => "obsolete form of";
    }

    class ArchaicFormOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Archaic form of";
        protected override string DefaultNoCap => "archaic form of";
    }

    class ArchaicSpellingOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Archaic spelling of";
        protected override string DefaultNoCap => "archaic spelling of";
    }

    class DatedFormOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Dated form of";
        protected override string DefaultNoCap => "dated form of";
    }

    class LateFormOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Late form of";
        protected override string DefaultNoCap => "late form of";
    }

    class MisspellingOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Misspelling of";
        protected override string DefaultNoCap => "misspelling of";
    }
    class AttributiveFormOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Attributive form of";
        protected override string DefaultNoCap => "attributive form of";
    }
    class DoubletTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Doublet of";
        protected override string DefaultNoCap => "doublet of";
    }
    class SynonymOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Synonym of";
        protected override string DefaultNoCap => "synonym of";
    }
}
