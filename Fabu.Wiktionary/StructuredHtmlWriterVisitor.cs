using Mup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Wordy.ClientServer.Model;

namespace Wordy.Wiktionary
{
    class StructuredHtmlWriterVisitor : HtmlWriterVisitor
    {
        private HtmlTextList _result = new HtmlTextList();
        private HtmlTextSection _currentSection = null;

        private int _onStartItemsCount = 0;
        private bool _isReadingItem = false;
        private bool _ignore = false;

        protected virtual void NewItem()
        {
            // if nothing was added within this paragraph and we have something in our buffer, then add it as is
            var currentBuffer = HtmlStringBuilder.ToString();
            if (!_ignore && _result.Count == _onStartItemsCount && !String.IsNullOrWhiteSpace(currentBuffer))
            {
                if (_currentSection == null)
                    _currentSection = new HtmlTextSection();
                _currentSection.Text = currentBuffer;
                _result.Add(_currentSection);
            }

            _currentSection = null;
            HtmlStringBuilder.Clear();
            _onStartItemsCount = _result.Count;
        }

        protected override void VisitImage(string source, string alternativeText)
        {
            var parser = new WiktionaryTemplateParser(source, alternativeText);
            parser.Parse();
            if (_currentSection == null)
                _currentSection = parser.Value;
            else
                _currentSection.SubItems.Add(parser.Value);
        }

        protected override void VisitLineBreak() => NewItem();
        protected override void VisitListItemBeginning() => NewItem();
        protected override void VisitListItemEnding() => NewItem();
        protected override void VisitOrderedListBeginning() => NewItem();
        protected override void VisitOrderedListEnding() => NewItem();
        protected override void VisitParagraphBeginning() => NewItem();
        protected override void VisitParagraphEnding() => NewItem();
        protected override void VisitTableBeginning() => NewItem();
        protected override void VisitTableEnding() => NewItem();
        protected override void VisitTableRowBeginning() => NewItem();
        protected override void VisitTableRowEnding() => NewItem();
        protected override void VisitUnorderedListBeginning() => NewItem();
        protected override void VisitUnorderedListEnding() => NewItem();

        protected override void VisitPlugin(string text) { }
        protected override void VisitTableCellBeginning() { }
        protected override void VisitTableCellEnding() { }
        protected override void VisitTableHeaderCellBeginning() { }
        protected override void VisitTableHeaderCellEnding() { }
        
    }

    public class WiktionaryTemplateParser
    {
        public WiktionaryTemplateParser(string template, string content)
        {
            Template = template;
            Content = content;
        }

        public string Template { get; }
        public string Content { get; }
        public HtmlTextSection Value { get; set; }

        public void Parse()
        {
            var opts = Content == null ? new string[0] : Content.Split('|');
            if (opts.Length == 0)
                return;
            var marcro = Template.Split(':').First()?.ToLowerInvariant();
            switch (marcro)
            {
                case "a":
                    var value = Value = new TermPronunciation();
                    value.Type = PronunciationValueType.Transcription;
                    if (alternativeText != null)
                        value.Dialect = alternativeText;
                    break;
                case "ipa":
                case "enpr":
                    var value = Value?.Clone() ?? new TermPronunciation();
                    Value.Type = PronunciationValueType.Transcription;
                    Value.Value = opts[0];
                    if (opts.Length > 1)
                    {
                        var lang = opts.FirstOrDefault(s => s.StartsWith("lang="));
                        if (lang != null)
                            Value.Lang = lang.Remove(0, "lang=".Length);
                    }
                    Value.ContentType = source;
                    _perspective.Add(_currentItem);
                    break;
                //en-us-dictionary.ogg|Audio (US)|lang=en
                //en-uk-dictionary.ogg|Audio (UK)|lang=en
                case "audio":
                    if (alternativeText != null)
                    {
                        var value = Value = new TermPronunciation();
                        value.Type = PronunciationValueType.Audio;
                        value.Value = opts[0];
                        value.ContentType = Path.GetExtension(opts[0]);
                        if (opts.Length > 1)
                        {
                            value.Dialect = opts[1];
                            var lang = opts.FirstOrDefault(s => s.StartsWith("lang="));
                            if (lang != null)
                                _currentItem.Lang = lang.Remove(0, "lang=".Length);
                        }
                        _perspective.Add(value);
                    }
                    break;
                //{{audio-pron|en-us-freedom_of_speech.ogg|ipa=/ˈfɹiː.dəm.əv.ˌspiːtʃ/|lang=en|country=us|dial=Midland American English}}
                case "audio-pron":
                    if (alternativeText != null)
                    {
                        var value = new TermPronunciation();
                        value.Type = PronunciationValueType.Audio;
                        value.Value = opts[0];
                        value.ContentType = Path.GetExtension(opts[0]);
                        if (opts.Length > 1)
                        {
                            var dial = opts.FirstOrDefault(s => s.StartsWith("dial="));
                            if (dial != null)
                                value.Dialect = dial.Remove(0, "dial=".Length);

                            var lang = opts.FirstOrDefault(s => s.StartsWith("lang="));
                            if (lang != null)
                                value.Lang = lang.Remove(0, "lang=".Length);

                            var country = opts.FirstOrDefault(s => s.StartsWith("country="));
                            if (country != null)
                            {
                                if (!String.IsNullOrEmpty(value.Lang))
                                    value.Lang += "-" + country.Remove(0, "country=".Length);
                                else
                                    value.Lang = country.Remove(0, "country=".Length);
                            }

                            var ipa = opts.FirstOrDefault(s => s.StartsWith("ipa="));
                            if (ipa != null)
                            {
                                var item = value.Clone();
                                item.Type = PronunciationValueType.Transcription;
                                item.Value = ipa;
                                item.ContentType = "IPA";
                                _perspective.Add(item);
                            }
                        }
                        _perspective.Add(value);
                    }
                    break;
                case "rel-top":
                    _ignore = true;
                    _isReadingItem = false;
                    break;
                case "rel-bottom":
                    _ignore = false;
                    break;
                // TODO:
                case "ipa letters":
                case "sense":
                case "hyphenation":
                case "hyph":
                case "homophones":
                case "rhymes":
                case "homophone":
                case "ipachar":
                case "w":
                // FUN FEATURE:
                case "rfap":
                case "wikisource1911enc":
                case "projectlink":
                // IGNORE:
                case "qualifier":
                case "attention":
                case "rfp":
                case "l":
                case "m":
                case "q":
                case "r":
                case "cln":
                    _perspective.UnknownSections.Add(source);
                    // ignored
                    break;
                default:
                    throw new NotImplementedException($"Section type '{source}' is not implemented for pronunciation parsing");
            }
        }
    }
}
