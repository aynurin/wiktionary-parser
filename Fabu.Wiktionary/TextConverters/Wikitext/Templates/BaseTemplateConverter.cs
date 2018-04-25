using System;
using System.Diagnostics;
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
            "bottom"
            , "col-bottom"
            , "col-top"
            , "der-bottom"
            , "der-bottom3"
            , "der-bottom4"
            , "der-bottom5"
            , "der-mid"
            , "der-mid3"
            , "der-mid4"
            , "der-mid5"
            , "der-top"
            , "der-top3"
            , "der-top4"
            , "der-top5"
            , "hyphenation"
            , "mid2"
            , "mid3"
            , "mid4"
            , "mid5"
            , "PIE root" // https://en.wiktionary.org/wiki/Template:PIE_root
            , "rel-bottom"
            , "rel-bottom3"
            , "rel-bottom4"
            , "rel-bottom5"
            , "rel-mid"
            , "rel-mid3"
            , "rel-mid4"
            , "rel-mid5"
            , "rel-top"
            , "rel-top3"
            , "rel-top4"
            , "rel-top5"
            , "rfquotek"
            , "rhymes"
            , "top2"
            , "top3"
            , "top4"
            , "top5"
        };

        public readonly static Stats<string> ConvertedTemplates = new Stats<string>();
        public readonly static Examples TemplatesExamples = new Examples();

        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            if (Array.BinarySearch(_voidTemplates, name.Name) >= 0)
                return new ConversionResult();

            if (name.IsHeadTemplate)
                return new ConversionResult();

            //if (name.Language != null && name.Language != "en")
            //{
            //    // TODO: zh-l, zh-m, ltc-l, och-l
            //    if (name.Language == "zh" || name.Language == "ar" || name.Language == "ko" || name.Language == "grc" || name.Language == "ja" || name.Language == "ltc" || name.Language == "och")
            //        return new ConversionResult();
            //    Debugger.Break();
            //    return new ConversionResult();
            //}

            var templateNames = name.GetNameParts();
            foreach (var templateName in templateNames)
                ConvertedTemplates.Add(templateName);
            TemplatesExamples.Add(name.OriginalName, context.Arguments.PageTitle + ":" + template.ToString());

            var result = new ConversionResult();
            result.Write(template.Arguments.ToRun());
            return result;
        }

        public override string GetSubstitute(Node node)
        {
            var template = (node as Template).Name.ToPlainText();
            template = TemplateName.FullName(template);
            // convert abc-def-ghi -> AbcDefGhi
            template = String.Concat(template.Split('-', ' ').Select(i => i == "" ? "" : (char.ToUpperInvariant(i[0]).ToString() + i.Substring(1))));
            if (String.IsNullOrWhiteSpace(template))
                return null;
            return template + "Template";
        }
    }
}
