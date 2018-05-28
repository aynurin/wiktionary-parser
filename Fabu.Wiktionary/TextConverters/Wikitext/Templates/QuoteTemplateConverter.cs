using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class QuoteTemplateConverter : BaseTemplateConverter
    {
        private static readonly CultureInfo USCulture = CultureInfo.CreateSpecificCulture("en-US");

        protected class QuoteListItem
        {
            public string Name { get; set; }
            public bool IsEmphasized { get; set; }
            public bool IsQuoted { get; set; }
            public bool CommaBefore { get; set; }
            public bool CommaAfter { get; set; }
            public string Format { get; set; }

            private QuoteListItem() { }

            internal static QuoteListItem Emphasis(string attrName) => new QuoteListItem
            {
                Name = attrName,
                IsEmphasized = true
            };

            public static implicit operator QuoteListItem(string attrName) => new QuoteListItem
            {
                Name = attrName
            };

            internal static QuoteListItem Quoted(string attrName) => new QuoteListItem
            {
                Name = attrName,
                IsQuoted = true
            };
        }

        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var items = new List<object>();
            if (template.Arguments.ContainsNotEmpty("year") && template.Arguments.ContainsNotEmpty("month"))
            {
                items.Add(template.Arguments["year"].Value.TooSmart());
                items.Add(" ");
                items.Add(template.Arguments["month"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("year"))
            {
                items.Add(template.Arguments["year"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("date"))
            {
                if (DateTime.TryParseExact(template.Arguments["date"].Value.ToString(), "d MMMM yyyy", USCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out DateTime date))
                {
                    date = date.ToUniversalTime();
                    items.Add(date.Year.ToString());
                    items.Add(" ");
                    items.Add(USCulture.DateTimeFormat.GetMonthName(date.Month));
                    items.Add(" ");
                    items.Add(date.Day.ToString());
                }
                else 
                    items.Add(template.Arguments["date"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("author"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add(template.Arguments["author"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("quotee"))
            {
                if (items.Count > 0)
                    items.Add(", quoting ");
                items.Add(template.Arguments["quotee"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("actor") && template.Arguments.ContainsNotEmpty("role"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add(template.Arguments["actor"].Value.TooSmart());
                items.Add(" as ");
                items.Add(template.Arguments["role"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("actor"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add(template.Arguments["actor"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("chapter") && template.Arguments.ContainsNotEmpty("title"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                if (Regex.IsMatch(template.Arguments["chapter"].Value.ToString().Trim(), @"^\d+$"))
                {
                    items.Add("chapter ");
                    items.Add(template.Arguments["chapter"].Value.TooSmart());
                }
                else
                {
                    items.Add("&ldquo;");
                    items.Add(template.Arguments["chapter"].Value.TooSmart());
                    items.Add("&rdquo;");
                }
                items.Add(", in <em>");
                items.Add(template.Arguments["title"].Value.TooSmart());
                items.Add("</em>");
            }
            else if (template.Arguments.ContainsNotEmpty("title") && template.Arguments.ContainsNotEmpty("journal"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add("&ldquo;");
                items.Add(template.Arguments["title"].Value.TooSmart());
                items.Add("&rdquo;, in <em>");
                items.Add(template.Arguments["journal"].Value.TooSmart());
                items.Add("</em>");
            }
            else if (template.Arguments.ContainsNotEmpty("title") && template.Arguments.ContainsNotEmpty("work"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add("&ldquo;");
                items.Add(template.Arguments["title"].Value.TooSmart());
                items.Add("&rdquo;, in <em>");
                items.Add(template.Arguments["work"].Value.TooSmart());
                items.Add("</em>");
            }
            else if (template.Arguments.ContainsNotEmpty("title") && template.Arguments.ContainsNotEmpty("album"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add("&ldquo;");
                items.Add(template.Arguments["title"].Value.TooSmart());
                items.Add("&rdquo;, in <em>");
                items.Add(template.Arguments["album"].Value.TooSmart());
                items.Add("</em>");
            }
            else if (template.Arguments.ContainsNotEmpty("title") && template.Arguments.ContainsNotEmpty("writer"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add("<em>");
                items.Add(template.Arguments["title"].Value.TooSmart());
                items.Add("</em>, written by ");
                items.Add(template.Arguments["writer"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("title") && template.Arguments.ContainsNotEmpty("newsgroup"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add("&ldquo;");
                items.Add(template.Arguments["title"].Value.TooSmart());
                items.Add("&rdquo;, in <em>");
                items.Add(template.Arguments["newsgroup"].Value.TooSmart());
                items.Add("</em>");
            }
            else if (template.Arguments.ContainsNotEmpty("title"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add("<em>");
                items.Add(template.Arguments["title"].Value.TooSmart());
                items.Add("</em>");
            }
            else if (template.Arguments.ContainsNotEmpty("journal"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add(template.Arguments["journal"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("album"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add(template.Arguments["album"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("quoted_in"))
            {
                if (items.Count > 0)
                    items.Add(", quoted in ");
                items.Add(template.Arguments["quoted_in"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("artist"))
            {
                if (items.Count > 0)
                    items.Add(", performed by ");
                items.Add(template.Arguments["artist"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("accessdate"))
            {
                if (items.Count > 0)
                    items.Add(", retrieved ");
                items.Add(template.Arguments["accessdate"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("location") && template.Arguments.ContainsNotEmpty("publisher"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add(template.Arguments["location"].Value.TooSmart());
                items.Add(": ");
                items.Add(template.Arguments["publisher"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("location"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add(template.Arguments["location"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("publisher"))
            {
                if (items.Count > 0)
                    items.Add(", ");
                items.Add(template.Arguments["publisher"].Value.TooSmart());
            }
            if (items.Count > 0 && template.Arguments.ContainsNotEmpty("passage"))
            {
                items.Add(": &ldquo;");
                items.Add(template.Arguments["passage"].Value.TooSmart());
                items.Add("&rdquo;");
            }
            else if (name.OriginalName == "quote" && template.Arguments.TryGet(out Wikitext value, 1))
            {
                if (context.LanguageCodes.ContainsKey(value.ToString()))
                {
                    if (template.Arguments.TryGet(out value, 2))
                        items.Add(value.TooSmart());
                }
                else
                {
                    items.Add(value.TooSmart());
                }
            }

            var result = new ConversionResult();
            result.Write(items);
            return result;
        }
    }
    class QuoteBookTemplateConverter : QuoteTemplateConverter
    {
    }
    class QuoteJournalTemplateConverter : QuoteTemplateConverter
    {
    }
    class QuoteSongTemplateConverter : QuoteTemplateConverter
    {
    }
    class QuoteVideoTemplateConverter : QuoteTemplateConverter
    {
    }
    class QuoteWebTemplateConverter : QuoteTemplateConverter
    {
    }
    class QuoteNewsgroupTemplateConverter : QuoteTemplateConverter
    {
    }
    class QuoteTextTemplateConverter : QuoteTemplateConverter
    {
    }
}
