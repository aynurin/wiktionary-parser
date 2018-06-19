using Fabu.Wiktionary.ElasticHosted;
using Fabu.Wiktionary.TextConverters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Fabu.Wiktionary.TermProcessing
{
    internal interface IWordCreator
    {
        void Create(List<Term> word);
    }

    public interface IWordSaver
    {
        void Save(SavableWordDefinition wordDefinition);
    }

    internal class HtmlWordCreator : IWordCreator
    {
        private readonly ITextConverterFactory _textConverterFactory;
        private readonly IWordSaver _writerImpl;

        public HtmlWordCreator(ITextConverterFactory converterFactory, IWordSaver writerImpl)
        {
            _textConverterFactory = converterFactory;
            _writerImpl = writerImpl;
        }

        public void Create(List<Term> wordTerms)
        {
            var title = wordTerms.First().Title;
            var language = wordTerms.First().Language;
            var slugline = wordTerms.SelectMany(term => term.Properties)
                .Where(kvp => kvp.Key != Term.Etymology && kvp.Key != Term.Pronunciation)
                .FirstOrDefault().Value?.Content;
            var word = new SavableWordDefinition(language, title);
            word.SetSlugline(_textConverterFactory, title, slugline);
            word.PopulateTerms(_textConverterFactory, wordTerms);
            word.CreateHtml();
            _writerImpl.Save(word);
        }
    }

    public class SavableWordDefinition
    {
        const int TermMainHeaderLevel = 1; // hX used to format word itself. So X+1 is for POS.

        public SavableWordDefinition(string language, string title)
        {
            Language = language;
            Title = title;
        }

        public string Language { get; }
        public string Title { get; }
        public List<WordDefinitionSection> Sections { get; private set; }
        public string Slugline { get; private set; }
        public string Html { get; private set; }
        public string Audio { get; private set; }

        internal void PopulateTerms(ITextConverterFactory textConverterFactory, List<Term> wordTerms)
        {
            var sections = new List<WordDefinitionSection>();
            var addedItems = new List<TermProperty>();
            for (var wordCounter = 0; wordCounter < wordTerms.Count; wordCounter++)
            {
                var term = wordTerms[wordCounter];
                if (wordCounter > 0)
                {
                    var section = new WordDefinitionSection(wordCounter);
                    section.Type = Term.Divider;
                    section.Content = "<hr />";
                    sections.Add(section);
                }
                if (term.TryGetValue(Term.Pronunciation, out TermProperty pronunciation) && !addedItems.Contains(pronunciation))
                {
                    addedItems.Add(pronunciation);

                    var section = new WordDefinitionSection(wordCounter);
                    var converter = textConverterFactory.CreateConverter(new ContextArguments(term.Title, Term.Pronunciation));
                    section.Type = Term.Pronunciation;
                    section.Content = pronunciation.RecursiveContentAsHtml(converter, false, TermMainHeaderLevel + 1);
                    if (String.IsNullOrWhiteSpace(Audio) && converter.Context.Proninciations.Count > 0)
                        Audio = converter.Context.Proninciations.First().FileName;
                    sections.Add(section);
                }
                if (term.TryGetValue(Term.Etymology, out TermProperty etymology) && !addedItems.Contains(etymology))
                {
                    addedItems.Add(etymology);

                    var section = new WordDefinitionSection(wordCounter);
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

                    var section = new WordDefinitionSection(wordCounter);
                    var converter = textConverterFactory.CreateConverter(new ContextArguments(term.Title, prop.Key));
                    section.Type = prop.Key;
                    section.Content = prop.Value.RecursiveContentAsHtml(converter, true, TermMainHeaderLevel + 1);
                    sections.Add(section);
                }
            }
            Sections = sections;
        }

        internal void SetSlugline(ITextConverterFactory textConverterFactory, string title, string slugline)
        {
            if (String.IsNullOrWhiteSpace(slugline))
                return;
            var converter = textConverterFactory.CreateConverter(new ContextArguments(title, slugline));
            Slugline = converter.ConvertText(slugline)
                .Split(new[] { "<li>", "<p>", "<ul>", "<ol>", "<div>" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => Regex.Replace(line, "<[^>]+>", String.Empty))
                .FirstOrDefault()?.Trim();
        }

        internal void CreateHtml()
        {
            Html = String.Join("", Sections.Select(section => section.Content));
        }
    }

    public class WordDefinitionSection : IComparable
    {
        private readonly int _wordCounter;

        public WordDefinitionSection(int wordCounter)
        {
            _wordCounter = wordCounter;
        }

        public string Type { get; internal set; }
        public string Content { get; internal set; }

        public int CompareTo(object obj)
        {
            var other = obj as WordDefinitionSection;
            return Content.CompareTo(other.Content);
        }
    }

    public class AWSCloudSearchHtmlWriter : DocumentIndexer, IWordSaver
    {
        private readonly MD5 _md5 = MD5.Create();

        public AWSCloudSearchHtmlWriter(IConfigurationRoot config, string domainName) : base(config, domainName, 1000)
        { }

        private string GetDocId(string language, string title)
        {
            var inputBytes = System.Text.Encoding.ASCII.GetBytes($"{language}_{title}");
            var hash = new Guid(_md5.ComputeHash(inputBytes));
            return hash.ToString("n");
        }

        public void Save(SavableWordDefinition wordDefinition) => Write(
            GetDocId(wordDefinition.Language, wordDefinition.Title),
            "words",
            new ElasticDocument
            {
                Body = wordDefinition.Html,
                Language = wordDefinition.Language,
                Slugline = wordDefinition.Slugline ?? String.Empty,
                Word = wordDefinition.Title,
                Audio = wordDefinition.Audio
            });
    }
}