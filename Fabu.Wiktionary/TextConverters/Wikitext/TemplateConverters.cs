using System;
using System.Collections.Generic;
using System.Diagnostics;
using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    class TemplateName
    {
        public string Name { get; set; }
        public string OriginalName { get; set; }

        public TemplateName(string originalName)
        {
            OriginalName = originalName;
        }

        public static TemplateName Parse(string originalName)
        {
            var name = new TemplateName(originalName);

            var nameParts = originalName.Split('-');
            if (nameParts.Length == 1)
                name.Name = originalName;
            else
                System.Diagnostics.Debugger.Break();

            return name;
        }

        public string[] GetNameParts()
        {
            var nameParts = OriginalName.Split('-');
            if (nameParts.Length == 1)
                return new[] { OriginalName };

            var allNames = new List<string>(nameParts.Length);
            for (var i = 0; i < nameParts.Length; i++)
            {
                var newParts = new string[nameParts.Length];
                for (var j = 0; j < nameParts.Length; j++)
                    newParts[j] = j.ToString();
                newParts[i] = nameParts[i];
                allNames.Add(String.Join('-', newParts));
            }

            return allNames.ToArray();
        }

        private static Dictionary<string, string> _fullNames = new Dictionary<string, string>()
        {
            { "m", "mention" },
            { "l", "mention" },
            { "der", "derived" }
        };

        internal static string FullName(string template)
        {
            if (_fullNames.TryGetValue(template, out string fullname))
                return fullname;
            return template;
        }
    }

    abstract class BaseTemplateConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var template = node as Template;
            if (template == null)
                throw new ArgumentException("Node must be an instance of Template");

            var name = TemplateName.Parse(template.Name.ToPlainText());

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

            var templateNames = name.GetNameParts();
            foreach (var templateName in templateNames)
                ConvertedTemplates.Add(templateName);
            TemplatesExamples.Add(name.OriginalName, context.Meta + ":" + template.ToString());

            return base.Convert(template.Arguments.ToRun(), context);
        }

        public override string GetSubstitute(Node node)
        {
            var template = (node as Template).Name.ToPlainText();
            template = TemplateName.FullName(template);
            return char.ToUpperInvariant(template[0]).ToString() + template.Substring(1) + "Template";
        }
    }

    class DerivedTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            context.LanguageCodes.TryGetValue(template.Arguments[2].ToString(), out string sourceLanguageName);
            
            if (!String.IsNullOrEmpty(sourceLanguageName))
            {
                result.Write(sourceLanguageName + " ");
            }
            if (template.Arguments.ContainsNotEmpty("alt"))
                result.Write(template.Arguments["alt"].Value.TooSmart());
            if (template.Arguments.ContainsNotEmpty(4))
                result.Write(template.Arguments[4].Value.TooSmart());
            else
                result.Write(template.Arguments[3].Value.TooSmart());

            Wikitext translation = null;
            if (template.Arguments.Contains("t"))
                translation = template.Arguments["t"].Value;
            else if (template.Arguments.Contains("gloss"))
                translation = template.Arguments["gloss"].Value;
            else if (template.Arguments.Contains(5))
                translation = template.Arguments[5].Value;
            if (translation != null)
            {
                result.Write(" (&ldquo;");
                result.Write(translation.TooSmart());
                result.Write("&rdquo;)");
            }

            return result;
        }
    }

    class MentionTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();
            
            if (template.Arguments.Contains(3) && !template.Arguments[3].Value.IsEmpty())
                result.Write(template.Arguments[3].Value.TooSmart());
            else
                result.Write(template.Arguments[2].Value.TooSmart());

            Wikitext translation = null;
            if (template.Arguments.Contains("t"))
                translation = template.Arguments["t"].Value;
            else if (template.Arguments.Contains("gloss"))
                translation = template.Arguments["gloss"].Value;
            else if (template.Arguments.Contains(4))
                translation = template.Arguments[4].Value;
            if (translation != null)
            {
                result.Write(" (&ldquo;");
                result.Write(translation.TooSmart());
                result.Write("&rdquo;)");
            }

            return result;
        }
    }

    //class DefdateTemplateConverter : BaseNodeConverter
    //{
    //    public override ConversionResult Convert(Node node, ConversionContext context)
    //    {
    //        var template = node as Template;
    //        var result = new ConversionResult();
    //        result.Write("<span class=\"defdate\">");
    //        result.Write(template.Arguments.ToRun());
    //        result.Write("</span>");
    //        return result;
    //    }
    //}
}
