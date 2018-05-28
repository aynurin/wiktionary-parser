using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class DefdateTemplateConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, WikiConversionContext context)
        {
            var template = node as Template;
            var result = new ConversionResult();
            result.Write("(");
            result.Write(template.Arguments.ToRun());
            result.Write(")");
            return result;
        }
    }
}
