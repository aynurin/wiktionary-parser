using System;
using System.Collections.Generic;
using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki
{

    class TemplateConverter : BaseNodeConverter
    {
        private readonly string[] _okTemplates = new string[]
        {
            "wikipedia"
        };

        public readonly static Stats<string> ConvertedTemplates = new Stats<string>();

        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var template = node as Template;
            if (template == null)
                throw new ArgumentException("Node must be an instance of Template");
            //if (Array.BinarySearch(_okTemplates, template.Name.ToPlainText()) < 0)
            //    Debugger.Break();
            var templateNames = GetNames(template.Name.ToPlainText());
            foreach (var templateName in templateNames)
                ConvertedTemplates.Add(templateName);
            return base.Convert(template.Arguments.ToRun(), context);
        }

        private string[] GetNames(string v)
        {
            var nameParts = v.Split('-');
            if (nameParts.Length == 1)
                return new[] { v };
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

        public override string GetSubstitute(Node node)
        {
            var template = (node as Template).Name.ToPlainText();
            return char.ToUpperInvariant(template[0]).ToString() + template.Substring(1) + "Template";
        }
    }

    class DefdateTemplateConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var template = node as Template;
            var result = new ConversionResult();
            result.Write("<span class=\"defdate\">");
            result.Write(template.Arguments.ToRun());
            result.Write("</span>");
            return result;
        }
    }
}
