﻿using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class FormOfTests : TestConverterFactory
    {
        [Fact]
        public void AlternativeSpellingOf()
        {
            var creole = "{{alternative spelling of|swap|lang=en}}";
            var html = "<p><em>Alternative spelling of</em> swap.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void AlternativeSpellingOfNoCapNoDot()
        {
            var creole = "{{alternative spelling of|swap|lang=en|nocap=1|nodot=1}}";
            var html = "<p><em>spelling of</em> swap</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void AlternativeSpellingOfTwoFromClauses()
        {
            var creole = "{{alternative spelling of|kilometre|from=US|from2=Canada|lang=en}}";
            var html = "<p><em>US and Canada spelling of</em> kilometre.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void DoubletNoText()
        {
            var creole = "{{doublet|lang=en|notext=1}}";
            var html = "";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
