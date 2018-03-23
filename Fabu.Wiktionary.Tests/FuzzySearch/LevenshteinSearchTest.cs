using Fabu.Wiktionary.FuzzySearch;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fabu.Wiktionary.Tests.FuzzySearch
{
    public class LevenshteinSearchTest
    {
        private List<Tuple<string, double>> GetSearchData()
        {
            return new List<Tuple<string, double>>
            {
                new Tuple<string,double>("Etymology", 300),
                new Tuple<string,double>("Adverb", 200),
                new Tuple<string,double>("Proverb", 100)
            };
        }

        private IFuzzySearcher<Tuple<string,double>> GetSearcher(string[] dict = null)
        {
            var data = GetSearchData();
            if (dict != null)
                data = dict.Select(s => new Tuple<string, double>(s, 100)).ToList();
            return new LevenshteinSearch<Tuple<string, double>>(
                data, 
                _ => _.Item1, 
                3);
        }

        [Fact]
        public void FindOne()
        {
            var search = GetSearcher();
            var results = search.FindAll("etymology");
            Assert.Single(results);
            Assert.Contains("Etymology", results.Select(_ => _.Item1));
            Assert.DoesNotContain("Adverb", results.Select(_ => _.Item1));
            Assert.DoesNotContain("Proverb", results.Select(_ => _.Item1));
        }

        [Fact]
        public void FindMatching()
        {
            var search = GetSearcher();
            var results = search.FindAll("Etymology");
            Assert.Single(results);
            Assert.Contains("Etymology", results.Select(_ => _.Item1));
            Assert.DoesNotContain("Adverb", results.Select(_ => _.Item1));
            Assert.DoesNotContain("Proverb", results.Select(_ => _.Item1));
        }

        [Fact]
        public void FindAllUpperAdverb()
        {
            var search = GetSearcher();
            var results = search.FindAll("PROVERB");
            Assert.True(results.First().Item1 == "Proverb");
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void FindAllUpperProverb()
        {
            var search = GetSearcher();
            var results = search.FindAll("ADVERB");
            Assert.True(results.First().Item1 == "Adverb");
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void Find1()
        {
            var search = GetSearcher();
            var results = search.FindAll("Eymology");
            Assert.Single(results);
            Assert.Contains("Etymology", results.Select(_ => _.Item1));
            Assert.DoesNotContain("Adverb", results.Select(_ => _.Item1));
            Assert.DoesNotContain("Proverb", results.Select(_ => _.Item1));
        }

        [Fact]
        public void Find2()
        {
            var search = GetSearcher();
            var results = search.FindAll("Etymology 10");
            Assert.Single(results);
            Assert.Contains("Etymology", results.Select(_ => _.Item1));
            Assert.DoesNotContain("Adverb", results.Select(_ => _.Item1));
            Assert.DoesNotContain("Proverb", results.Select(_ => _.Item1));
        }

        [Fact]
        public void Find3()
        {
            var search = GetSearcher();
            var results = search.FindAll("Extymlogy");
            Assert.Single(results);
            Assert.Contains("Etymology", results.Select(_ => _.Item1));
            Assert.DoesNotContain("Adverb", results.Select(_ => _.Item1));
            Assert.DoesNotContain("Proverb", results.Select(_ => _.Item1));
        }

        [Fact]
        public void TooFarAway()
        {
            var search = GetSearcher();
            var results = search.FindAll("Extimlgy");
            Assert.Empty(results);
        }

        [Fact]
        public void ShouldChooseFirstInRow()
        {
            // Leva should choose first in row
            var search = GetSearcher(new string[] { "Verb", "Adverb", "Proverb" });
            var results = search.FindBest("Averb");
            Assert.True(results.First().Item1 == "Verb");

            // Leva should choose first in row
            search = GetSearcher(new string[] { "Adverb", "Verb", "Proverb" });
            results = search.FindBest("Averb");
            Assert.True(results.First().Item1 == "Adverb");
        }
    }
}
