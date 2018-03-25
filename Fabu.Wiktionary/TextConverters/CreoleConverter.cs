using Mup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fabu.Wiktionary.TextConverters
{
    public class CreoleConverter
    {
        public FormattedString ConvertToStructured(string creole)
        {
            CreoleParser parser = new CreoleParser();
            IParseTree parseTree = parser.Parse(creole);
            FormattedStringWriterVisitor htmlWriterVisitor = new FormattedStringWriterVisitor();
            parseTree.Accept(htmlWriterVisitor);
            return htmlWriterVisitor.Result;
        }
    }

    internal class FormattedStringWriterVisitor : HtmlWriterVisitor
    {
        // todo: implement all blocks
        public FormattedString Result { get; private set; } = new FormattedString();
    }

    /// <summary>
    /// Contains a formatted string that is easy to render as rich text label or as HTML on mobile devices, or serialize as JSON for storage
    /// </summary>
    public class FormattedString
    {
        public string ToHtml()
        {
            return String.Empty;
        }
    }
}
