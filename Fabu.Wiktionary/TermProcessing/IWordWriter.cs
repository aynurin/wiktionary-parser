using Fabu.Wiktionary.TextConverters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fabu.Wiktionary.TermProcessing
{
    internal interface IWordWriter
    {
        void Write(List<Term> word);
    }

    internal class FileWordWriter : IWordWriter
    {
        private readonly string _targetDir;
        private readonly ITextConverter _textConverter;

        public FileWordWriter(string targetDir, ITextConverter converter)
        {
            _targetDir = targetDir;
            _textConverter = converter;
        }
        public void Write(List<Term> word)
        {
            var buffer = new StringBuilder();

            
            for (var i = 0; i < word.Count; i++)
            {
                var term = word[i];
                var args = new ContextArguments() { PageTitle = term.Title, SectionName = null };
                buffer.Append("<hr />");
                buffer.Append("<h2>TERM " + (i + 1) + " (" + term.Language + "): " + term.Title + "</h1>");
                buffer.Append(_textConverter.ConvertToStructured(args, term.Content));
                WriteTerms(buffer, term, term.Properties, 1);
            }

            var title = word.First().Title;
            foreach (var c in Path.GetInvalidPathChars())
                title = title.Replace(c.ToString(), "");
            var folder = title;
            if (folder.Length > 2)
                folder = folder.Substring(0, 2);
            var fileName = Path.Combine(_targetDir, folder).Trim();
            Directory.CreateDirectory(fileName);
            fileName = Path.Combine(fileName, title + ".html");
            File.WriteAllText(fileName, buffer.ToString());
        }

        private void WriteTerms(StringBuilder buffer, Term term, Dictionary<string, TermProperty> properties, int level)
        {
            foreach (var kvp in properties)
            {
                var prop = kvp.Value;
                buffer.Append("<h" + (level + 2) + ">" + prop.Title + "</h" + (level + 2) + ">");
                var args = new ContextArguments() { PageTitle = term.Title, SectionName = prop.Title };
                buffer.Append(_textConverter.ConvertToStructured(args, kvp.Value.Content));
                WriteTerms(buffer, term, kvp.Value, level + 1);
            }
        }
    }
}