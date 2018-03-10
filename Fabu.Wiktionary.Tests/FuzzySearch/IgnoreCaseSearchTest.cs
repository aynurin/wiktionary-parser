using Fabu.Wiktionary.FuzzySearch;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fabu.Wiktionary.Tests.FuzzySearch
{
    public class IgnoreCaseSearchTest
    {
        private List<string> GetSearchData()
        {
            return new List<string>
            {
                "Etymology",
                "Adverb",
                "Proverb"
            };
        }
        private class InvariantStringComparer : StringComparer
        {
            public override int Compare(string x, string y)
            {
                return x.CompareTo(y);
            }

            public override bool Equals(string x, string y)
            {
                return x.Equals(y);
            }

            public override int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }

        [Fact]
        public void FindOne()
        {
            var search = new IgnoreCaseSearch<string>(GetSearchData(), _ => _, new InvariantStringComparer());
            var results = search.FindAll("etymology");
            Assert.Single(results);
            Assert.Contains("Etymology", results);
            Assert.DoesNotContain("Adverb", results);
            Assert.DoesNotContain("Proverb", results);
        }

        [Fact]
        public void FindMatching()
        {
            var search = new IgnoreCaseSearch<string>(GetSearchData(), _ => _, new InvariantStringComparer());
            var results = search.FindAll("Etymology");
            Assert.Single(results);
            Assert.Contains("Etymology", results);
            Assert.DoesNotContain("Adverb", results);
            Assert.DoesNotContain("Proverb", results);
        }

        [Fact]
        public void FindAllUpper()
        {
            var search = new IgnoreCaseSearch<string>(GetSearchData(), _ => _, new InvariantStringComparer());
            var results = search.FindAll("ADVERB");
            Assert.Single(results);
            Assert.DoesNotContain("Etymology", results);
            Assert.Contains("Adverb", results);
            Assert.DoesNotContain("Proverb", results);
        }
    }
}
