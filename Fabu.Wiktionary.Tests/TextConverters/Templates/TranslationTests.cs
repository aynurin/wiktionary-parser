﻿using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class TranslationTests : TestConverterFactory
    {
        [Fact]
        public void T()
        {
            var creole = "Catalan: {{t|ca|abelià}}, {{t|ca|commutatiu}}";
            var html = "<p>Catalan: abelià, commutatiu</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void TSkippingGenderNumber()
        {
            var creole = "Czech: {{t|cs|Abelova|f}}, {{t|cs|abelovská|f}}, {{t|cs|komutativní}}";
            var html = "<p>Czech: Abelova, abelovská, komutativní</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void TPlus()
        {
            var creole = "French: {{t+|fr|abélien|m}}, {{t+|fr|abélienne|f}}, {{t+|fr|commutatif|m}}, {{t+|fr|commutative|f}}";
            var html = "<p>French: abélien, abélienne, commutatif, commutative</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void TCheck()
        {
            var creole = "Maori: {{t-check|mi|me}}, {{t-check|mi|a}}";
            var html = "<p>Maori: me (?), a (?)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void TPlusCheck()
        {
            var creole = "Breton: {{t+check|br|marc'had-mat}}";
            var html = "<p>Breton: marc'had-mat (?)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
