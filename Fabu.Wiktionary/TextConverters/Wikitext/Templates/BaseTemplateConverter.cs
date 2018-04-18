using System;
using System.Linq;
using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    abstract class BaseTemplateConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var template = node as Template;
            if (template == null)
                throw new ArgumentException("Node must be an instance of Template");
            
            var name = TemplateName.Parse(template.Name.ToPlainText(), str => context.LanguageCodes.ContainsKey(str));

            return ConvertTemplate(name, template, context);
        }

        protected abstract ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context);
    }
    class TemplateConverter : BaseTemplateConverter
    {
        private static string[] _voidTemplates = new string[]
        {
            "PIE root" // https://en.wiktionary.org/wiki/Template:PIE_root
        };

        public readonly static Stats<string> ConvertedTemplates = new Stats<string>();
        public readonly static Examples TemplatesExamples = new Examples();

        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            if (Array.BinarySearch(_voidTemplates, name.Name) >= 0)
                return new ConversionResult();

            if (name.IsHeadTemplate)
                return new ConversionResult();

            var templateNames = name.GetNameParts();
            foreach (var templateName in templateNames)
                ConvertedTemplates.Add(templateName);
            TemplatesExamples.Add(name.OriginalName, context.Meta + ":" + template.ToString());

            var result = new ConversionResult();
            result.Write(template.Arguments.ToRun());
            return result;
        }

        public override string GetSubstitute(Node node)
        {
            var template = (node as Template).Name.ToPlainText();
            template = TemplateName.FullName(template);
            // convert abc-def-ghi -> AbcDefGhi
            template = String.Concat(template.Split('-').Select(i => char.ToUpperInvariant(i[0]).ToString() + i.Substring(1)));
            return template + "Template";
        }
    }
}
