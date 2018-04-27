using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    abstract class BaseCircaTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            var value = template.Arguments[1].Value.TooSmart();

            result.Write(GetPrefix());

            result.Write(value);

            return result;
        }

        protected abstract string GetPrefix();
    }

    class CircaTemplateConverter : BaseCircaTemplateConverter
    {
        protected override string GetPrefix() => "<em>c.</em> ";
    }

    class AnteTemplateConverter : BaseCircaTemplateConverter
    {
        protected override string GetPrefix() => "<em>a.</em> ";
    }

    class PostTemplateConverter : BaseCircaTemplateConverter
    {
        protected override string GetPrefix() => "<em>p.</em> ";
    }

    class ISBNTemplateConverter : BaseCircaTemplateConverter
    {
        protected override string GetPrefix() => "ISBN ";
    }
}
