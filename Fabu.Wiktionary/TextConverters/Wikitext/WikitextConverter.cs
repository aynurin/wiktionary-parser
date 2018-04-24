using Fabu.Wiktionary.TextConverters.Wiki;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    public class WikitextProcessor : ITextConverter
    {
        private readonly ConverterFactory _converterFactory = new ConverterFactory();
        private Dictionary<string, string> _lagnuageCodes;
        private readonly bool _allowLinks;

        public WikitextProcessor(Dictionary<string, string> lagnuageCodes, bool allowLinks)
        {
            _lagnuageCodes = lagnuageCodes;
            _allowLinks = allowLinks;
        }

        public string PageTitle { get; set; }

        public FormattedString ConvertToStructured(ContextArguments args, string wikitext)
        {
            if (wikitext == null)
                return null;

            wikitext = StripHtml.Tables(wikitext);

            var parser = new WikitextParser();
            var ast = parser.Parse(wikitext.TrimEnd());
            var context = new ConversionContext(args, _lagnuageCodes, _allowLinks);

            //PrintAst(ast, 0);

            var buffer = new StringBuilder();
            using (var writer = new StringWriter(buffer))
                BuildAst(ast, writer, context);

            var result = buffer.ToString();

            result = Cleanup(result);

            return new FormattedString(result, context.Proninciations);
        }

        /// <summary>
        /// Cleanup unnecessary HTML formatting
        /// </summary>
        /// <remarks>
        /// This method is a way to avoid a week or more of coding to implement a more proper handling of difficult cases. 
        /// For example, when converter produces empty tags, such as &lt;ul&gt;&lt;li&gt;&lt;/li&gt;&lt;/ul&gt;, this method will 
        /// remove this pointless markup.
        /// The challenge is that when writing a document on the go it is difficult to predict whether a tag will have a value, when writing
        /// that tag, and even more it is difficult to predict if inner HTML tree will have any test data associated.
        /// </remarks>
        /// <param name="result"></param>
        /// <returns></returns>
        private string Cleanup(string result)
        {
            var newResult = result;
            do
            {
                result = newResult;
                newResult = Regex.Replace(result, @"<[^/>]+>\s*</[^>]+>", String.Empty);
            }
            while (result != newResult);
            return newResult;
        }

        private void BuildAst(Node node, TextWriter writer, ConversionContext context)
        {
            var converter = _converterFactory.GetConverter(node);
            var result = converter.Convert(node, context);
            foreach (var item in result)
            {
                if (item is Node nodeItem)
                {
                    foreach (var child in nodeItem.EnumChildren())
                        BuildAst(child, writer, context);
                }
                else
                    writer.Write(item);
            }
        }

        private string Escapse(string expr)
        {
            return expr.Replace("\r", "\\r").Replace("\n", "\\n");
        }

        private void PrintAst(Node node, int level)
        {
            var indension = new string('.', level);
            var printText = node.ToString();
            if (printText.Length > 84)
                printText = printText.Substring(0, 80) + ".." + (printText.Length - 80);
            var name = node.GetType().Name;
            if (name == "FormatSwitch")
            {
                var fs = node as FormatSwitch;
                if (fs.SwitchBold)
                    name += ".Bold";
                else if (fs.SwitchItalics)
                    name += ".Italics";
                else
                    name += ".None";
            }
            Console.WriteLine("{0,-20} {1}", indension + name,
                Escapse(printText));
            foreach (var child in node.EnumChildren())
                PrintAst(child, level + 1);
        }
    }
}
