using Fabu.Wiktionary.TextConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Fabu.Wiktionary.TermProcessing
{
    internal interface IWordWriter
    {
        void Write(List<Term> word);
    }

    internal class GoogleSearchAPIWordWriter : IWordWriter
    {
        private readonly ITextConverterFactory _textConverterFactory;

        public GoogleSearchAPIWordWriter(ITextConverterFactory converterFactory)
        {
            _textConverterFactory = converterFactory;
        }

        public void Write(List<Term> wordTerms)
        {
            var title = wordTerms.First().Title;
            var word = new MobileDeviceWordDefinition(title);
            word.PopulateTerms(_textConverterFactory, wordTerms);
            var json = word.ToJson();
            IndexWord(json);
        }

        private void IndexWord(string json)
        {
            var html = ToHtml(json);
            Debugger.Break();
        }

        private string ToHtml(string json)
        {
            var items = JsonConvert.DeserializeObject(json) as JArray;
            var result = String.Join("", items.Select(token => token["Content"].ToString()));
            return result;
        }

        private class MobileDeviceWordDefinition
        {
            const int TermMainHeaderLevel = 1; // hX used to format word itself. So X+1 is for POS.

            public MobileDeviceWordDefinition(string title)
            {
                Title = title;
            }

            public string Title { get; }
            public List<WordSection> Sections { get; private set; }

            internal void PopulateTerms(ITextConverterFactory textConverterFactory, List<Term> wordTerms)
            {
                var sections = new List<WordSection>();
                var addedItems = new List<TermProperty>();
                for (var wordCounter = 0; wordCounter < wordTerms.Count; wordCounter++)
                {
                    var term = wordTerms[wordCounter];
                    if (wordCounter > 0)
                    {
                        var section = new WordSection(wordCounter);
                        section.Type = Term.Divider;
                        section.Content = "<hr />";
                        sections.Add(section);
                    }
                    if (term.TryGetValue(Term.Pronunciation, out TermProperty pronunciation) && !addedItems.Contains(pronunciation))
                    {
                        addedItems.Add(pronunciation);

                        var section = new WordSection(wordCounter);
                        var converter = textConverterFactory.CreateConverter(new ContextArguments(term.Title, Term.Pronunciation));
                        section.Type = Term.Pronunciation;
                        section.Content = pronunciation.RecursiveContentAsHtml(converter, false, TermMainHeaderLevel + 1);
                        sections.Add(section);
                    }
                    if (term.TryGetValue(Term.Etymology, out TermProperty etymology) && !addedItems.Contains(etymology))
                    {
                        addedItems.Add(etymology);

                        var section = new WordSection(wordCounter);
                        var converter = textConverterFactory.CreateConverter(new ContextArguments(term.Title, Term.Etymology));
                        section.Type = Term.Etymology;
                        section.Content = etymology.RecursiveContentAsHtml(converter, false, TermMainHeaderLevel + 1);
                        sections.Add(section);
                    }
                    foreach (var prop in term)
                    {
                        if (addedItems.Contains(prop.Value) || prop.Key == Term.Pronunciation || prop.Key == Term.Etymology)
                            continue;
                        addedItems.Add(prop.Value);

                        var section = new WordSection(wordCounter);
                        var converter = textConverterFactory.CreateConverter(new ContextArguments(term.Title, prop.Key));
                        section.Type = prop.Key;
                        section.Content = prop.Value.RecursiveContentAsHtml(converter, true, TermMainHeaderLevel + 1);
                        sections.Add(section);
                    }
                }
                Sections = sections;
            }

            internal string ToJson()
            {
                return JsonConvert.SerializeObject(Sections, Formatting.Indented);
            }
        }

        private class WordSection : IComparable
        {
            private int wordCounter;

            public WordSection(int wordCounter)
            {
                this.wordCounter = wordCounter;
            }

            public string Type { get; internal set; }
            public string Content { get; internal set; }

            public int CompareTo(object obj)
            {
                var other = obj as WordSection;
                return Content.CompareTo(other.Content);
            }
        }
    }
}