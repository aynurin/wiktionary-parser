using MwParserFromScratch.Nodes;
using System;

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
            if (writeTags)
                result.WriteTail($"</{tag.Name}>");
            return result;
        }
    }

    class TemplateConverter : BaseNodeConverter
    {
        private readonly string[] _okTemplates = new string[]
        {
            //"wikipedia"
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
