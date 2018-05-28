using System.Collections.Generic;

namespace Fabu.Wiktionary.TextConverters
{
    public interface ITextConverterFactory
    {
        ITextConverter CreateConverter(ContextArguments context);
    }

    public interface ITextConverter
    {
        string ConvertText(string wikitext);
        ConversionContext Context { get; }
    }

    public class ContextArguments
    {
        public ContextArguments(string title, string sectionName)
        {
            PageTitle = title;
            SectionName = sectionName;
        }

        public string PageTitle { get; set; }
        public string SectionName { get; set; }
    }

    public class ConversionContext
    {
        public ContextArguments Arguments { get; private set; }
        public Dictionary<string, string> LanguageCodes { get; set; }
        public bool AllowLinks { get; set; }

        public List<Proninciation> Proninciations { get; private set; } = new List<Proninciation>();
        public string LastResult { get; internal set; }

        public ConversionContext(ContextArguments args, Dictionary<string, string> languageCodes, bool allowLinks)
        {
            Arguments = args;
            LanguageCodes = languageCodes;
            AllowLinks = allowLinks;
        }

        internal void AddPronunciation(string language, string fileName, string label)
        {
            Proninciations.Add(new Proninciation(language, fileName, label));
        }
    }

    public class Proninciation
    {
        public string Language { get; }
        public string FileName { get; }
        public string Label { get; }

        public Proninciation(string language, string fileName, string label)
        {
            Language = language;
            FileName = fileName;
            Label = label;
        }
    }
}
