using Fabu.Wiktionary.TermProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fabu.Wiktionary.Tests.TermProcessing
{
    public class TermGraphProcessorTest
    {
        public TermGraphProcessor GraphProcessor() => new TermGraphProcessor(new SectionNameNoTransform(), new SectionName[0]);

        [Fact]
        public void TryOptionOne()
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

            GraphProcessor().ProcessGraph(graph);

            var items = graph.AllItems.Where(i => i.Status == Term.TermStatus.Defined).ToArray();

            Assert.Equal(2, items.Length);
            Assert.Collection(items,
                item =>
                {
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Noun");
                    Assert.Contains(item.Properties.Keys, k => k == "Verb");
                    Assert.Contains(item.Properties["Noun"].Properties.Keys, k => k == "Synonyms");
                },
                item =>
                {
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Adjective");
                    Assert.Contains(item.Properties.Keys, k => k == "Adverb");
                    Assert.Contains(item.Properties["Adjective"].Properties.Keys, k => k == "Quotations");
                });
        }

        [Fact]
        public void TryOptionTwo()
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

            GraphProcessor().ProcessGraph(graph);

            var items = graph.AllItems.Where(i => i.Status == Term.TermStatus.Defined).ToArray();

            Assert.Equal(2, items.Length);
            Assert.Collection(items,
                item =>
                {
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Noun");
                    Assert.Contains(item.Properties.Keys, k => k == "Verb");
                    Assert.Contains(item.Properties["Noun"].Properties.Keys, k => k == "Synonyms");
                },
                item =>
                {
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Adjective");
                    Assert.Contains(item.Properties.Keys, k => k == "Adverb");
                    Assert.Contains(item.Properties["Adjective"].Properties.Keys, k => k == "Quotations");
                });
        }

        [Fact]
        public void TryOptionThree()
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

            GraphProcessor().ProcessGraph(graph);

            var items = graph.AllItems.Where(i => i.Status == Term.TermStatus.Defined).ToArray();

            Assert.Equal(2, items.Length);
            Assert.Collection(items,
                item =>
                {
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Noun");
                    Assert.Contains(item.Properties.Keys, k => k == "Verb");
                    Assert.Contains(item.Properties["Noun"].Properties.Keys, k => k == "Synonyms");
                },
                item =>
                {
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Adjective");
                    Assert.Contains(item.Properties.Keys, k => k == "Adverb");
                    Assert.Contains(item.Properties["Adjective"].Properties.Keys, k => k == "Quotations");
                });
        }

        [Fact]
        public void TryOptionFour()
        {
            var graph = TestGraphItem.TestRoot("OptionOne")
                .Langage("English",
                    language => language
                        .Definer("Pronunciation")
                        .Definer("Etymology")
                        .Pos("Noun", noun => noun.Other("Synonyms"))
                        .Pos("Verb"));

            GraphProcessor().ProcessGraph(graph);

            var items = graph.AllItems.Where(i => i.Status == Term.TermStatus.Defined).ToArray();

            Assert.Single(items);
            Assert.Collection(items,
                item =>
                {
                    Assert.Equal(4, item.Properties.Count);
                    Assert.Contains(item.Properties.Keys, k => k == "Etymology");
                    Assert.Contains(item.Properties.Keys, k => k == "Pronunciation");
                    Assert.Contains(item.Properties.Keys, k => k == "Noun");
                    Assert.Contains(item.Properties.Keys, k => k == "Verb");
                    Assert.Contains(item.Properties["Noun"].Properties.Keys, k => k == "Synonyms");
                });
        }
    }

    internal class TestGraphItem : GraphItem
    {
        public static TestGraphItem TestRoot(string pageTitle) => new TestGraphItem("PAGE", null, pageTitle, null, false, false, null, new List<Term>());
        
        private TestGraphItem(string title,
            GraphItem parent, string pageTitle, string sectionContent,
            bool isLanguage, bool canDefineTerm, string[] allowedMembers, List<Term> termStore)
            : base(title, parent, pageTitle, sectionContent, isLanguage, canDefineTerm, allowedMembers, termStore) { }

        public TestGraphItem Definer(string title, params Action<TestGraphItem>[] childCreators)
        {
            var newNode = new TestGraphItem(title, this, OwnerPageTitle, null, false, true, null, _createdTerms);
            AddChild(newNode);
            foreach(var childCreator in childCreators)
                childCreator(newNode);
            return this;
        }

        public TestGraphItem Pos(string title, params Action<TestGraphItem>[] childCreators)
        {
            var newNode = new TestGraphItem(title, this, OwnerPageTitle, null, false, false, new[] { "Quotations", "Synonyms", "Usage notes" }, _createdTerms);
            AddChild(newNode);
            foreach (var childCreator in childCreators)
                childCreator(newNode);
            return this;
        }

        public TestGraphItem Other(string title, params Action<TestGraphItem>[] childCreators)
        {
            var newNode = new TestGraphItem(title, this, OwnerPageTitle, null, false, false, null, _createdTerms);
            AddChild(newNode);
            foreach (var childCreator in childCreators)
                childCreator(newNode);
            return this;
        }

        public TestGraphItem Langage(string title, params Action<TestGraphItem>[] childCreators)
        {
            var newNode = new TestGraphItem(title, this, OwnerPageTitle, null, true, false, null, _createdTerms);
            AddChild(newNode);
            foreach (var childCreator in childCreators)
                childCreator(newNode);
            return this;
        }
    }
}
