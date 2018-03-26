using Fabu.Wiktionary.TextConverters.Wiki;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using System;
using System.IO;
using System.Text;

namespace Fabu.Wiktionary.TextConverters
{
    public class WikitextConverter
    {
        private static readonly ConverterFactory _converterFactory = new ConverterFactory();

        public FormattedString ConvertToStructured(string wikitext)
        {
            var parser = new WikitextParser();
            var ast = parser.Parse(wikitext);

            PrintAst(ast, 0);

            var buffer = new StringBuilder();
            using (var writer = new StringWriter(buffer))
                BuildAst(ast, writer, new ConversionContext());

            return new FormattedString(buffer.ToString());
        }

        private void BuildAst(Node node, TextWriter writer, ConversionContext context)
        {
            var converter = _converterFactory.GetConverter(node);
            var result = converter.Convert(node, context);
            result.WriteData(writer);
            node = result.Node ?? node;
            foreach (var child in node.EnumChildren())
                BuildAst(child, writer, context);
            if (!String.IsNullOrEmpty(result.Tail))
                writer.Write(result.Tail);
        }

        private static string Escapse(string expr)
        {
            return expr.Replace("\r", "\\r").Replace("\n", "\\n");
        }

        private static void PrintAst(Node node, int level)
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

    /// <summary>
    /// Contains a formatted string that is easy to render as rich text label or as HTML on mobile devices, or serialize as JSON for storage
    /// </summary>
    public class FormattedString
    {
        private readonly string _data;

        public FormattedString(string data)
        {
            _data = data;
        }

        public string ToHtml()
        {
            return _data;
            ;
        }
    }
}
