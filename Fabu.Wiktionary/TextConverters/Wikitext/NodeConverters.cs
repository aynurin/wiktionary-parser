using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
                result.Write(MaybeARun(tag.Content));
            if (writeTags)
                result.Write($"</{tag.Name}>");
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
            result.Write(heading.Inlines.ToRun());
            result.Write($"</h{heading.Level}>");
            return result;
        }
    }

    class CommentConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            return new ConversionResult();
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
                result.Write($"<a href=\"{link.Target.ToPlainText()}\">");
            if (link.Text != null)
                result.Write(link.Text);
            else if (WriteExternalLinks)
                result.Write(link.Target);
            if (WriteExternalLinks)
                result.Write($"</a>");
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

            List<InlineNode> inlines = GetInlines(li.Inlines);

            if (inlines.Count > 0)
            {
                result.Write("<li>");
                result.Write(new Run(inlines));
                result.Write("</li>");
            }

            if (li.NextNode == null || li.NextNode.GetType() != typeof(ListItem))
                result.Write($"</{tag}>");

            return result;
        }

        private static List<InlineNode> GetInlines(IEnumerable<InlineNode> rawinlines)
        {
            var inlines = new List<InlineNode>(rawinlines);
            // the first node can be the space that goes after list item specifier (*_blabla), which we should avoid
            if (inlines[0].GetType() == typeof(PlainText) && String.IsNullOrWhiteSpace(inlines[0].ToString()))
                inlines.RemoveAt(0);
            var lastIndex = inlines.Count-1;
            if (lastIndex > 0 && inlines[lastIndex].GetType() == typeof(PlainText) && String.IsNullOrWhiteSpace(inlines[lastIndex].ToString()))
                inlines.RemoveAt(lastIndex);
            return inlines;
        }
    }

    class ParagraphConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var result = new ConversionResult();
            if (node.ToString().Trim().Length > 0)
            {
                result.Write("<p>");
                result.Write(node);
                result.Write("</p>");
            }
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
            if (context.AllowLinks)
                result.Write($"<a href=\"{link.Target.ToPlainText()}\">");
            result.Write(link.Text ?? link.Target);
            if (context.AllowLinks)
                result.Write("</a>");
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
