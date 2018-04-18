using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Fabu.Wiktionary.TextConverters.Wiki
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
            if (template.Arguments.ContainsNotEmpty("year") && template.Arguments.ContainsNotEmpty("month"))
            {
                _items.Add(template.Arguments["year"].Value.TooSmart());
                _items.Add(" ");
                _items.Add(template.Arguments["month"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("year"))
            {
                _items.Add(template.Arguments["year"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("date"))
            {
                if (DateTime.TryParseExact(template.Arguments["date"].Value.ToString(), "d MMMM yyyy", USCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out DateTime date))
                {
                    date = date.ToUniversalTime();
                    _items.Add(date.Year.ToString());
                    _items.Add(" ");
                    _items.Add(USCulture.DateTimeFormat.GetMonthName(date.Month));
                    _items.Add(" ");
                    _items.Add(date.Day.ToString());
                }
                else 
                    _items.Add(template.Arguments["date"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("author"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add(template.Arguments["author"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("quotee"))
            {
                if (_items.Count > 0)
                    _items.Add(", quoting ");
                _items.Add(template.Arguments["quotee"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("actor") && template.Arguments.ContainsNotEmpty("role"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add(template.Arguments["actor"].Value.TooSmart());
                _items.Add(" as ");
                _items.Add(template.Arguments["role"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("actor"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add(template.Arguments["actor"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("chapter") && template.Arguments.ContainsNotEmpty("title"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add("&ldquo;");
                _items.Add(template.Arguments["chapter"].Value.TooSmart());
                _items.Add("&rdquo;, in <em>");
                _items.Add(template.Arguments["title"].Value.TooSmart());
                _items.Add("</em>");
            }
            else if (template.Arguments.ContainsNotEmpty("title") && template.Arguments.ContainsNotEmpty("journal"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add("&ldquo;");
                _items.Add(template.Arguments["title"].Value.TooSmart());
                _items.Add("&rdquo;, in <em>");
                _items.Add(template.Arguments["journal"].Value.TooSmart());
                _items.Add("</em>");
            }
            else if (template.Arguments.ContainsNotEmpty("title") && template.Arguments.ContainsNotEmpty("work"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add("&ldquo;");
                _items.Add(template.Arguments["title"].Value.TooSmart());
                _items.Add("&rdquo;, in <em>");
                _items.Add(template.Arguments["work"].Value.TooSmart());
                _items.Add("</em>");
            }
            else if (template.Arguments.ContainsNotEmpty("title") && template.Arguments.ContainsNotEmpty("album"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add("&ldquo;");
                _items.Add(template.Arguments["title"].Value.TooSmart());
                _items.Add("&rdquo;, in <em>");
                _items.Add(template.Arguments["album"].Value.TooSmart());
                _items.Add("</em>");
            }
            else if (template.Arguments.ContainsNotEmpty("title") && template.Arguments.ContainsNotEmpty("writer"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add("<em>");
                _items.Add(template.Arguments["title"].Value.TooSmart());
                _items.Add("</em>, written by ");
                _items.Add(template.Arguments["writer"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("title") && template.Arguments.ContainsNotEmpty("newsgroup"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add("&ldquo;");
                _items.Add(template.Arguments["title"].Value.TooSmart());
                _items.Add("&rdquo;, in <em>");
                _items.Add(template.Arguments["newsgroup"].Value.TooSmart());
                _items.Add("</em>");
            }
            else if (template.Arguments.ContainsNotEmpty("title"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add("<em>");
                _items.Add(template.Arguments["title"].Value.TooSmart());
                _items.Add("</em>");
            }
            else if (template.Arguments.ContainsNotEmpty("journal"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add(template.Arguments["journal"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("album"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add(template.Arguments["album"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("quoted_in"))
            {
                if (_items.Count > 0)
                    _items.Add(", quoted in ");
                _items.Add(template.Arguments["quoted_in"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("artist"))
            {
                if (_items.Count > 0)
                    _items.Add(", performed by ");
                _items.Add(template.Arguments["artist"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("accessdate"))
            {
                if (_items.Count > 0)
                    _items.Add(", retrieved ");
                _items.Add(template.Arguments["accessdate"].Value.TooSmart());
            }
            if (template.Arguments.ContainsNotEmpty("location") && template.Arguments.ContainsNotEmpty("publisher"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add(template.Arguments["location"].Value.TooSmart());
                _items.Add(": ");
                _items.Add(template.Arguments["publisher"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("location"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add(template.Arguments["location"].Value.TooSmart());
            }
            else if (template.Arguments.ContainsNotEmpty("publisher"))
            {
                if (_items.Count > 0)
                    _items.Add(", ");
                _items.Add(template.Arguments["publisher"].Value.TooSmart());
            }
            if (_items.Count > 0 && template.Arguments.ContainsNotEmpty("passage"))
            {
                _items.Add(": &ldquo;");
                _items.Add(template.Arguments["passage"].Value.TooSmart());
                _items.Add("&rdquo;");
            }

            var result = new ConversionResult();
            result.Write(_items);
            return result;
        }

        private List<object> _items = new List<object>();
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
