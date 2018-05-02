using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fabu.Wiktionary.TermProcessing
{
    internal interface IWordWriter
    {
        void Write(DictionaryWord word);
    }

    internal class FileWordWriter : IWordWriter
    {
        private readonly string _targetDir;

        public FileWordWriter(string targetDir)
        {
            _targetDir = targetDir;
        }
        public void Write(DictionaryWord word)
        {
            var buffer = new StringBuilder();

            buffer.Append("<h1>" + word.Title + "</h1>");
            for (var i = 0; i < word.Terms.Count; i++)
            {
                var term = word.Terms[i];
                buffer.Append("<hr />");
                buffer.Append("<h2>TERM " + (i + 1) + " (" + term.Language + "): " + term.Title + "</h1>");
                buffer.Append(term.Content);
                WriteTerms(buffer, term.Properties, 1);
            }

            var title = word.Title;
            foreach (var c in Path.GetInvalidPathChars())
                title = title.Replace(c.ToString(), "");
            var folder = title;
            if (folder.Length > 2)
                folder = folder.Substring(0, 2);
            var fileName = Path.Combine(_targetDir, folder).Trim();
            Directory.CreateDirectory(fileName);
            fileName = Path.Combine(fileName, word.Title + ".html");
            File.WriteAllText(fileName, buffer.ToString());
        }

        private void WriteTerms(StringBuilder buffer, Dictionary<string, Term> properties, int level)
        {
            foreach (var kvp in properties)
            {
                buffer.Append("<h" + (level + 2) + ">" + kvp.Value.Title + "</h" + (level + 2) + ">");
                buffer.Append(kvp.Value.Content);
                WriteTerms(buffer, kvp.Value.Properties, level + 1);
            }
        }
    }
}