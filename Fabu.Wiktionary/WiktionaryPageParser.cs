using Mup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WikimediaProcessing;
using Wordy.ClientServer.Model;

namespace Wordy.Wiktionary
{
    internal class WiktionaryPageParser
    {
        public WiktionaryPageParser(WikimediaPage page)
        {
            Page = page;
        }

        public WikimediaPage Page { get; }
        public WiktionaryPerspective Perspective { get; } = new WiktionaryPerspective();

        private Regex _ethymology = new Regex("Ethymology(?:\\s+(\\d+))?", RegexOptions.Compiled);

        public enum ProcessResult
        {
            Unknown,
            Success,
            IsSpecialPage
        }

        internal ProcessResult Process()
        {
            if (Page.IsSpecialPage)
                return ProcessResult.IsSpecialPage;
            if (Page.IsRedirect)
                // TODO: Follow the redirect to add a synonym
                return ProcessResult.IsSpecialPage;

            var english = Page.Sections.FirstOrDefault(s => "English" == s.SectionName);
            var ethymology = english.SubSections.FirstOrDefault(s => _ethymology.IsMatch(s.SectionName));

            var count = 0;

            if (TryParseEthymology(ethymology))
                count++;

            if (TryParsePos(ethymology.SubSections) || TryParsePos(english.SubSections))
                count++;

            if (TryParsePronunciation(ethymology.SubSections) || TryParsePronunciation(english.SubSections))
                count++;

            if (count > 1)
                return ProcessResult.Success;

            return ProcessResult.Unknown;
        }

        private bool TryParseEthymology(WikiSection ethymology)
        {
            var parser = new CreoleParser();
            var visitor = new StructuredHtmlWriterVisitor();

            parser.Parse(ethymology.Content).Accept(visitor);
            
            if (result.Count == 0)
            {
                Console.WriteLine($"Empty result for article '{page.Title}'");
                if (!result.UnknownSections.Any(_unknownOk.Contains))
                    BreakDebugger();
            }
        }

        private bool TryParsePos(ICollection<WikiSection> subSections)
        {
            throw new NotImplementedException();
        }

        private bool TryParsePronunciation(ICollection<WikiSection> subSections)
        {
            throw new NotImplementedException();
        }
    }
}