using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    /// <summary>
    /// <see cref="BaseFormOfTemplatesConverter"/> for a very similar one. Maybe merge?
    /// </summary>
    class AbbreviationOfTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            result.Write("<em>");
            WriteCap(result, template, DefaultCap, DefaultNoCap);
            result.Write("</em> ");

            template.Arguments.TryGetOneOf(out Wikitext value, 2, 1);
            result.Write(value.TooSmart());

            template.Arguments.TryGetOneOf(out Wikitext tr, "tr");
            template.Arguments.TryGetOneOf(out Wikitext gloss, "gloss", "t", 3);
            WriteTrAndGloss(result, tr?.TooSmart(), gloss?.TooSmart());

            WriteDot(result, template);

            return result;
        }

        protected virtual string DefaultCap => "Abbreviation of";
        protected virtual string DefaultNoCap => "abbreviation of";
    }

    class AcronymOfTemplateConverter : AbbreviationOfTemplateConverter
    {
        protected override string DefaultCap => "Acronym of";
        protected override string DefaultNoCap => "acronym of";
    }

    class ContractionOfTemplateConverter : AbbreviationOfTemplateConverter
    {
        protected override string DefaultCap => "Contraction of";
        protected override string DefaultNoCap => "contraction of";
    }

    class InitialismOfTemplateConverter : AbbreviationOfTemplateConverter
    {
        protected override string DefaultCap => "Initialism of";
        protected override string DefaultNoCap => "initialism of";
    }

    class EnComparativeOfTemplateConverter : AbbreviationOfTemplateConverter
    {
        protected override string DefaultCap => "Comparative form of";
        protected override string DefaultNoCap => "comparative form of";
    }

    class EnSuperlativeOfTemplateConverter : AbbreviationOfTemplateConverter
    {
        protected override string DefaultCap => "Superlative form of";
        protected override string DefaultNoCap => "superlative form of";
    }
}
