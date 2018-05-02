namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class EnThirdPersonSingularOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Third-person singular simple present indicative of";
        protected override string DefaultNoCap => "third-person singular simple present indicative of";
    }
    class EnArchaicThirdPersonSingularOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "(archaic) Third-person singular simple present indicative of";
        protected override string DefaultNoCap => "(archaic) third-person singular simple present indicative of";
    }
    class EnPastOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Simple past and past participle of";
        protected override string DefaultNoCap => "simple past and past participle of";
    }
    class PresentParticipleOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Present participle of";
        protected override string DefaultNoCap => "present participle of";
    }
    class EnSimplePastOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string DefaultCap => "Simple past of";
        protected override string DefaultNoCap => "simple past of";
    }
}
