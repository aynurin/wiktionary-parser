namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class EnThirdPersonSingularOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "third-person singular simple present indicative of",
                "third-person singular simple present indicative of",
                "Third-person singular simple present indicative of"
            };
    }
    class EnArchaicThirdPersonSingularOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "(archaic) third-person singular simple present indicative of",
                "(archaic) third-person singular simple present indicative of",
                "(archaic) Third-person singular simple present indicative of"
            };
    }
    class EnPastOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "simple past and past participle of",
                "simple past and past participle of",
                "Simple past and past participle of"
            };
    }
    class PresentParticipleOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "present participle of",
                "present participle of",
                "Present participle of"
            };
    }
    class EnSimplePastOfTemplateConverter : BaseFormOfTemplatesConverter
    {
        protected override string[] GetPhrases() => new[]
            {
                "simple past of",
                "simple past of",
                "Simple past of"
            };
    }
}
