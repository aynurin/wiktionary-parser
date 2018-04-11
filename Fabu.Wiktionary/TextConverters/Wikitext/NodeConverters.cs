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

    class ParserTagConverter : BaseNodeConverter
    {
        private static string[] _voidTags = new string[]
        {
            "ref" // <ref> does not render to a text, it's a reference to another resource
            ,"references" // renders a set of all <ref> references which we don't want to use at the moment
        };
        private static string[] _contentTags = new string[]
        {
            "math" // TODO: Implement math formulae rendering?.. https://www.mediawiki.org/wiki/Extension:Math
            ,"poem" // TODO: Test what happens. https://www.mediawiki.org/wiki/Extension:Poem
        };
        private static string[] _okIfEmptyTags = new string[]
        {
            "section" // I don't get the section meaning. Let's just ignore it if it's a self-closing tag.
        };

        public readonly static Stats<string> ConvertedParserTags = new Stats<string>();

        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var parserTag = node as ParserTag;
            if (parserTag == null)
                throw new ArgumentException("Node must be an instance of ParserTag");

            if (Array.BinarySearch(_voidTags, parserTag.Name) >= 0)
                return new ConversionResult();

            if (Array.BinarySearch(_contentTags, parserTag.Name) >= 0)
            {
                var result = new ConversionResult();
                result.Write(parserTag.Content);
                return result;
            }

            if (Array.BinarySearch(_okIfEmptyTags, parserTag.Name) >= 0 && parserTag.Content == null)
            {
                return new ConversionResult();
            }

            ConvertedParserTags.Add(parserTag.Name);

            return new ConversionResult();
        }

        public override string GetSubstitute(Node node)
        {
            var parserTag = (node as ParserTag).Name;
            return char.ToUpperInvariant(parserTag[0]).ToString() + parserTag.Substring(1) + "ParserTag";
        }
    }

    class GalleryParserTagConverter : BaseNodeConverter
    {
        // https://phabricator.wikimedia.org/diffusion/EHIE/browse/master/img/
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var parserTag = node as ParserTag;
            if (parserTag == null)
                throw new ArgumentException("Node must be an instance of ParserTag");
            var result = new ConversionResult();
            var galleryItemStrings = parserTag.Content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in galleryItemStrings)
            {
                var parts = item.Split('|');
                var fileName = parts[0];
                var title = "";
                if (parts.Length > 1)
                    title = parts[1];
                result.Write($"<img src=\"wiktfile://{Uri.EscapeUriString(fileName)}\" title=\"{Uri.EscapeDataString(title)}\" />");
            }
            return result;
        }
    }

    class HieroParserTagConverter : BaseNodeConverter
    {
        // https://phabricator.wikimedia.org/diffusion/EHIE/browse/master/img/
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var parserTag = node as ParserTag;
            if (parserTag == null)
                throw new ArgumentException("Node must be an instance of ParserTag");
            var result = new ConversionResult();
            result.Write($"<img src=\"hiero://{Uri.EscapeUriString(parserTag.Content)}\" />");
            return result;
        }
    }

    class NowikiParserTagConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var parserTag = node as ParserTag;
            if (parserTag == null)
                throw new ArgumentException("Node must be an instance of ParserTag");
            var result = new ConversionResult();
            if (parserTag.Content != null)
                result.Write(Uri.EscapeDataString(parserTag.Content));
            return result;
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
                result.Write($"<a href=\"{link.Target.ToPlainText()}\">");
            result.Write(link.Text);
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
            result.Write("<li>");
            result.Write(new Run(li.Inlines));
            if (li.NextNode == null || li.NextNode.GetType() != typeof(ListItem))
                result.Write($"</li></{tag}>");
            else
                result.Write("</li>");
            return result;
        }
    }

    class ParagraphConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, ConversionContext context)
        {
            var result = new ConversionResult();
            result.Write("<p>");
            result.Write(node);
            result.Write("</p>");
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
            result.Write(link.Text ?? link.Target);
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
