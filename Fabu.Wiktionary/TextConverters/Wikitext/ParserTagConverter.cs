using Fabu.Wiktionary.TextConverters.Wiki;
using MwParserFromScratch.Nodes;
using System;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    /// <summary>
    /// TODO: Tests missing for ParserTagConverter
    /// </summary>
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

        public override ConversionResult Convert(Node node, WikiConversionContext context)
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
        public override ConversionResult Convert(Node node, WikiConversionContext context)
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
        public override ConversionResult Convert(Node node, WikiConversionContext context)
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
        public override ConversionResult Convert(Node node, WikiConversionContext context)
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
}
