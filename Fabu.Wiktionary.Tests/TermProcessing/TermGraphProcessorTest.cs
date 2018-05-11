using Fabu.Wiktionary.TermProcessing;
using Fabu.Wiktionary.TextConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fabu.Wiktionary.Tests.TermProcessing
{
    public class TermGraphProcessorTest
    {
        public PageGraphProcessor GraphProcessor() => new PageGraphProcessor(new SectionNameNoTransform());

        [Fact]
        public void EtymologyDefinesTerms()
        {
            var graph = TestGraphItem.TestRoot("OptionOne")
                .Langage("English",
                    language => language.Definer("Pronunciation",
                        pronun =>
                            pronun.Definer("Etymology", ety1 =>
                                    ety1.Pos("Noun", noun => noun.Other("Synonyms"))
                                        .Pos("Verb"))
                                .Definer("Etymology", ety2 =>
                                    ety2.Pos("Adjective", adj => adj.Other("Quotations"))
                                        .Pos("Adverb"))));

            GraphProcessor().ProcessGraph(graph.InnerItem);

            var items = graph.InnerItem.AllItems.Where(i => i.Status == Term.TermStatus.Defined).ToArray();

            Assert.Equal(2, items.Length);
            Assert.Collection(items,
                item =>
                {
                    Assert.Equal("OptionOne", item.Title);
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Equal("English", item.Language);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Noun");
                    Assert.Contains(item.Properties.Keys, k => k == "Verb");
                    Assert.Contains(item.Properties["Noun"].Properties.Keys, k => k == "Synonyms");
                },
                item =>
                {
                    Assert.Equal("OptionOne", item.Title);
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Equal("English", item.Language);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Adjective");
                    Assert.Contains(item.Properties.Keys, k => k == "Adverb");
                    Assert.Contains(item.Properties["Adjective"].Properties.Keys, k => k == "Quotations");
                });
        }

        [Fact]
        public void PronunciationDefinesTerms()
        {
            var graph = TestGraphItem.TestRoot("OptionOne")
                .Langage("English",
                    language => language
                        .Definer("Pronunciation", pronun1 =>
                            pronun1.Definer("Etymology", ety1 =>
                                ety1.Pos("Noun", noun => noun.Other("Synonyms"))
                                    .Pos("Verb")))
                        .Definer("Pronunciation", pronun2 =>
                            pronun2.Definer("Etymology", ety1 =>
                                ety1.Pos("Adjective", adj => adj.Other("Quotations"))
                                    .Pos("Adverb"))));

            GraphProcessor().ProcessGraph(graph.InnerItem);

            var items = graph.InnerItem.AllItems.Where(i => i.Status == Term.TermStatus.Defined).ToArray();

            Assert.Equal(2, items.Length);
            Assert.Collection(items,
                item =>
                {
                    Assert.Equal("OptionOne", item.Title);
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Equal("English", item.Language);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Noun");
                    Assert.Contains(item.Properties.Keys, k => k == "Verb");
                    Assert.Contains(item.Properties["Noun"].Properties.Keys, k => k == "Synonyms");
                },
                item =>
                {
                    Assert.Equal("OptionOne", item.Title);
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Equal("English", item.Language);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Adjective");
                    Assert.Contains(item.Properties.Keys, k => k == "Adverb");
                    Assert.Contains(item.Properties["Adjective"].Properties.Keys, k => k == "Quotations");
                });
        }

        [Fact]
        public void SeparatePronunciationIsInheritedByEtymologyDefinedTerms()
        {
            var graph = TestGraphItem.TestRoot("OptionOne")
                .Langage("English",
                    language => language
                        .Definer("Pronunciation")
                        .Definer("Etymology", ety1 =>
                                ety1.Pos("Noun", noun => noun.Other("Synonyms"))
                                    .Pos("Verb"))
                        .Definer("Etymology", ety1 =>
                                ety1.Pos("Adjective", adj => adj.Other("Quotations"))
                                    .Pos("Adverb")));

            GraphProcessor().ProcessGraph(graph.InnerItem);

            var items = graph.InnerItem.AllItems.Where(i => i.Status == Term.TermStatus.Defined).ToArray();

            Assert.Equal(2, items.Length);
            Assert.Collection(items,
                item =>
                {
                    Assert.Equal("OptionOne", item.Title);
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Equal("English", item.Language);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Noun");
                    Assert.Contains(item.Properties.Keys, k => k == "Verb");
                    Assert.Contains(item.Properties["Noun"].Properties.Keys, k => k == "Synonyms");
                },
                item =>
                {
                    Assert.Equal("OptionOne", item.Title);
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Equal("English", item.Language);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Adjective");
                    Assert.Contains(item.Properties.Keys, k => k == "Adverb");
                    Assert.Contains(item.Properties["Adjective"].Properties.Keys, k => k == "Quotations");
                });
        }

        [Fact]
        public void PlainListDefinesOneTerm()
        {
            var graph = TestGraphItem.TestRoot("OptionOne")
                .Langage("English",
                    language => language
                        .Definer("Pronunciation")
                        .Definer("Etymology")
                        .Pos("Noun", noun => noun.Other("Synonyms"))
                        .Pos("Verb"));

            GraphProcessor().ProcessGraph(graph.InnerItem);

            var items = graph.InnerItem.AllItems.Where(i => i.Status == Term.TermStatus.Defined).ToArray();

            Assert.Single(items);
            Assert.Collection(items,
                item =>
                {
                    Assert.Equal("OptionOne", item.Title);
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Equal("English", item.Language);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Noun");
                    Assert.Contains(item.Properties.Keys, k => k == "Verb");
                    Assert.Contains(item.Properties["Noun"].Properties.Keys, k => k == "Synonyms");
                });
        }


        [Fact]
        public void LanguageDefinesIfNoOtherDefiners()
        {
            var rawGraph = TestGraphItem.TestRoot("OptionOne")
                .Langage("English",
                    language => language
                        .Pos("Noun", noun => noun.Other("Synonyms"))
                        .Pos("Verb"))
                .Langage("Old German",
                    language => language
                        .Pos("Pronunciation")
                        .Pos("Etymology"));

            var proc = GraphProcessor();
            var graph = proc.CreateGraph(rawGraph.GetSectionName(), rawGraph.GetSubSections());
            proc.ProcessGraph(graph);

            var items = graph.AllItems.Where(i => i.Status == Term.TermStatus.Defined).ToArray();

            Assert.Equal(2, items.Length);
            Assert.Collection(items,
                item =>
                {
                    Assert.Equal(2, item.Properties.Count);
                    Assert.Equal("English", item.Language);
                    Assert.Contains(item.Properties.Keys, k => k == "Noun");
                    Assert.Contains(item.Properties.Keys, k => k == "Verb");
                    Assert.Contains(item.Properties["Noun"].Properties.Keys, k => k == "Synonyms");
                },
                item =>
                {
                    Assert.Equal(2, item.Properties.Count);
                    Assert.Equal("Old German", item.Language);
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                });
        }

        [Fact]
        public void PosInAPosGoesUp()
        {
            var rawGraph = TestGraphItem.TestRoot("OptionOne")
                .Langage("English",
                    language => language
                        .Pos("Noun", noun => noun
                            .Other("Synonyms"))
                        .Pos("Verb", verb => verb
                            .Other("Synonyms")
                            .Pos("Acronym")));

            var proc = GraphProcessor();
            var graph = proc.CreateGraph(rawGraph.GetSectionName(), rawGraph.GetSubSections());
            proc.ProcessGraph(graph);

            var items = graph.AllItems.Where(i => i.Status == Term.TermStatus.Defined).ToArray();

            Assert.Single(items);
            Assert.Collection(items,
                item =>
                {
                    Assert.Equal(3, item.Properties.Count);
                    Assert.Equal("English", item.Language);
                    Assert.Contains(item.Properties.Keys, k => k == "Noun");
                    Assert.Contains(item.Properties.Keys, k => k == "Verb");
                    Assert.Contains(item.Properties.Keys, k => k == "Acronym");
                    Assert.Contains(item.Properties["Noun"].Properties.Keys, k => k == "Synonyms");
                    Assert.Contains(item.Properties["Verb"].Properties.Keys, k => k == "Synonyms");
                });
        }
    }

    internal class TestGraphItem : ISectionAccessor
    {
        private readonly PageGraphNode _innerItem;
        private readonly List<TestGraphItem> _children = new List<TestGraphItem>();

        public PageGraphNode InnerItem => _innerItem;

        public static TestGraphItem TestRoot(string pageTitle) => new TestGraphItem(PageGraphNode.CreateRoot(pageTitle));

        private TestGraphItem(PageGraphNode item)
        {
            _innerItem = item;
        }

        public TestGraphItem CreateChild(string title, string sectionContent,
            bool isLanguage, bool canDefineTerm, string[] allowedMembers)
        {
            var item = new TestGraphItem(_innerItem.CreateChild(title, sectionContent, isLanguage, canDefineTerm, allowedMembers));
            _innerItem.AddChild(item.InnerItem);
            _children.Add(item);
            return item;
        }

        public TestGraphItem Definer(string title, params Action<TestGraphItem>[] childCreators)
        {
            var newNode = CreateChild(title, null, false, true, null);
            foreach(var childCreator in childCreators)
                childCreator(newNode);
            return this;
        }

        public TestGraphItem Pos(string title, params Action<TestGraphItem>[] childCreators)
        {
            var newNode = CreateChild(title, null, false, false, new[] { "Quotations", "Synonyms", "Usage notes" });
            foreach (var childCreator in childCreators)
                childCreator(newNode);
            return this;
        }

        public TestGraphItem Other(string title, params Action<TestGraphItem>[] childCreators)
        {
            var newNode = CreateChild(title, null, false, false, null);
            foreach (var childCreator in childCreators)
                childCreator(newNode);
            return this;
        }

        public TestGraphItem Langage(string title, params Action<TestGraphItem>[] childCreators)
        {
            var newNode = CreateChild(title, null, true, false, null);
            foreach (var childCreator in childCreators)
                childCreator(newNode);
            return this;
        }

        public string GetSectionName() => _innerItem.ItemTitle;

        public string GetRawContent() => null;

        public IEnumerable<ISectionAccessor> GetSubSections() => _children;
    }
}
