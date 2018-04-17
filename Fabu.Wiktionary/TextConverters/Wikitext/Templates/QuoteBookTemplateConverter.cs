using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    class QuoteBookTemplateConverter : BaseTemplateConverter
    {
        class QuoteListItem
        {
            public string Name { get; set; }
            public bool IsEmphasized { get; set; }

            public QuoteListItem(string attrName)
            {
                Name = attrName;
            }

            public QuoteListItem(string attrName, bool isEmphasized)
            {
                Name = attrName;
                IsEmphasized = isEmphasized;
            }

            public static implicit operator QuoteListItem(string attrName) => new QuoteListItem(attrName);
        }
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var items = new List<object>();
            var attrs = new QuoteListItem[] { "year", "author", new QuoteListItem("title", true), "location", "publisher" };

            foreach (var attr in attrs)
            {
                if (template.Arguments.ContainsNotEmpty(attr.Name))
                {
                    if (items.Count > 0)
                        items.Add(", ");
                    if (attr.IsEmphasized)
                        items.Add("<em>");
                    items.Add(template.Arguments[attr.Name].Value.TooSmart());
                    if (attr.IsEmphasized)
                        items.Add("</em>");
                }
            }
            if (items.Count > 0)
                items.Add(":");
            if (template.Arguments.ContainsNotEmpty("passage"))
            {
                if (items.Count > 0)
                    items.Add(" ");
                items.Add("&ldquo;");
                items.Add(template.Arguments["passage"].Value.TooSmart());
                items.Add("&rdquo;");
            }

            var result = new ConversionResult();
            result.Write(items);
            return result;
        }
    }
}
