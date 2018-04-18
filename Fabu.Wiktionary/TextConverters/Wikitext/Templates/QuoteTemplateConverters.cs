using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    class QuoteBookTemplateConverter : QuoteTemplateConverter
    {
        //protected override IEnumerable<QuoteListItem> GetAttrs() => new QuoteListItem[] {
        //    "year",
        //    "author",
        //    QuoteListItem.Emphasis("title"),
        //    "location",
        //    "publisher"
        //};
    }
    class QuoteJournalTemplateConverter : QuoteTemplateConverter
    {
        //protected override IEnumerable<QuoteListItem> GetAttrs() => new QuoteListItem[] {
        //    "year",
        //    "month",
        //    QuoteListItem.Quoted("title"),
        //    QuoteListItem.Emphasis("journal"),
        //    "author",
        //    "location",
        //    "publisher"
        //};
    }
    class QuoteSongTemplateConverter : QuoteTemplateConverter
    {
        //protected override IEnumerable<QuoteListItem> GetAttrs() => new QuoteListItem[] {
        //    "year",
        //    "author",
        //    QuoteListItem.Quoted("title"),
        //    QuoteListItem.Emphasis("album")
        //};
    }
    class QuoteVideoTemplateConverter : QuoteTemplateConverter
    {
        //protected override IEnumerable<QuoteListItem> GetAttrs() => new QuoteListItem[] {
        //    "year",
        //    "actor",
        //    QuoteListItem.Quoted("title"),
        //    QuoteListItem.Emphasis("album")
        //};
    }
    class QuoteWebTemplateConverter : QuoteTemplateConverter
    {
        //protected override IEnumerable<QuoteListItem> GetAttrs() => new QuoteListItem[] {
        //    "year",
        //    "actor",
        //    QuoteListItem.Quoted("title"),
        //    QuoteListItem.Emphasis("album")
        //};
    }
    class QuoteNewsgroupTemplateConverter : QuoteTemplateConverter
    {
        //protected override IEnumerable<QuoteListItem> GetAttrs() => new QuoteListItem[] {
        //    "year",
        //    "actor",
        //    QuoteListItem.Quoted("title"),
        //    QuoteListItem.Emphasis("album")
        //};
    }
}
