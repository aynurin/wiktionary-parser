using MwParserFromScratch.Nodes;
using System;
using System.Diagnostics;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    class HtmlTagConverter : BaseNodeConverter
    {
        private static string[] _allowedTags = new string[]
        {
            "sup"
        };
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var tag = node as HtmlTag;
            var result = new ConversionResult();
            var writeTags = Array.BinarySearch(_allowedTags, tag.Name) >= 0;
            if (writeTags)
                result.Write($"<{tag.Name}>");
            if (tag.Content != null)
                result.Node = MaybeARun(tag.Content);
            else result.Node = new PlainText(); // e.g. <br />
            if (writeTags)
                result.WriteTail($"</{tag.Name}>");
            return result;
        }
    }

    class HeadingConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var heading = node as Heading;
            var result = new ConversionResult();
            result.Write($"<h{heading.Level}>");
            result.Node = heading.Inlines.ToRun();
            result.WriteTail($"</h{heading.Level}>");
            return result;
        }
    }

    class CommentConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var result = new ConversionResult();
            result.Node = new PlainText();
            return result;
        }
    }

    class ParserTagConverter : BaseNodeConverter
    {
        public readonly static Stats<string> ConvertedParserTags = new Stats<string>();

        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var parserTag = node as ParserTag;
            if (parserTag == null)
                throw new ArgumentException("Node must be an instance of ParserTag");
            ConvertedParserTags.Add(parserTag.Name);

            var result = new ConversionResult();
            result.Node = new PlainText(parserTag.Content);

            return result;
        }

        public override string GetSubstitute(Node node)
        {
            var parserTag = (node as ParserTag).Name;
            return char.ToUpperInvariant(parserTag[0]).ToString() + parserTag.Substring(1) + "ParserTag";
        }
    }

    class TemplateConverter : BaseNodeConverter
    {
        private readonly string[] _okTemplates = new string[]
        {
            "wikipedia"
        };

        public readonly static Stats<string> ConvertedTemplates = new Stats<string>();

        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var template = node as Template;
            if (template == null)
                throw new ArgumentException("Node must be an instance of Template");
            //if (Array.BinarySearch(_okTemplates, template.Name.ToPlainText()) < 0)
            //    Debugger.Break();
            ConvertedTemplates.Add(template.Name.ToPlainText());
            return base.Convert(template.Arguments.ToRun(), context);
        }

        public override string GetSubstitute(Node node)
        {
            var template = (node as Template).Name.ToPlainText();
            return char.ToUpperInvariant(template[0]).ToString() + template.Substring(1) + "Template";
        }
    }

    class ExternalLinkConverter : BaseNodeConverter
    {
        private const bool WriteExternalLinks = true;
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var result = new ConversionResult();
            var link = node as ExternalLink;
            if (WriteExternalLinks)
            {
                result.Write($"<a href=\"{link.Target.ToPlainText()}\">");
                result.WriteTail($"</a>");
            }
            result.Node = link.Text;
            return result;
        }
    }

    class ListItemConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var result = new ConversionResult();
            var li = node as ListItem;
            var tag = li.Prefix == "*" ? "ul" : "ol";
            if (li.PreviousNode == null || li.PreviousNode.GetType() != typeof(ListItem))
                result.Write($"<{tag}>");
            result.Write("<li>");
            if (li.NextNode == null || li.NextNode.GetType() != typeof(ListItem))
                result.WriteTail($"</li></{tag}>");
            else
                result.WriteTail("</li>");
            result.Node = new Run(li.Inlines);
            return result;
        }
    }

    class ParagraphConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var result = new ConversionResult();
            var par = node as Paragraph;
            result.Write("<p>");
            result.WriteTail("</p>");
            return result;
        }
    }

    class PlainTextConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var result = new ConversionResult();
            result.Write(node.ToPlainText());
            return result;
        }
    }

    class WikiLinkConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var link = node as WikiLink;
            var result = new ConversionResult();
            result.Write($"<a href=\"{link.Target.ToPlainText()}\">");
            result.WriteTail("</a>");
            result.Node = link.Text ?? link.Target;
            return result;
        }
    }

    class FormatSwitchConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var result = new ConversionResult();
            var sw = node as FormatSwitch;
            // bug: there is no way to get the closing tag from the tree.
            if (sw.SwitchBold)
            {
                if (context.BoldSwitched)
                {
                    result.Write("</strong>");
                    context.BoldSwitched = false;
                }
                else
                {
                    result.Write("<strong>");
                    context.BoldSwitched = true;
                }
            }
            else if (sw.SwitchItalics)
            {
                if (context.ItalicsSwitched)
                {
                    result.Write("</em>");
                    context.ItalicsSwitched = false;
                }
                else
                {
                    result.Write("<em>");
                    context.ItalicsSwitched = true;
                }
            }
            return result;
        }
    }
}
