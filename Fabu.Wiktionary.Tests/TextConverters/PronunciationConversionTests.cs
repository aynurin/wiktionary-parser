using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class PronunciationConversionTests : TestConverterFactory
    {
        [Fact]
        public void ShouldConvertPronunciation()
        {
            var creole = @"
* {{a|RP}} {{IPA|/ˈdɪkʃ(ə)n(ə)ɹi/|lang=en}}
* {{a|GenAm|Canada}} {{enPR|dĭk'shə-nĕr-ē}}, {{IPA|/ˈdɪkʃənɛɹi/|lang=en}}
* {{audio|en-us-dictionary.ogg|Audio (US)|lang=en}}
* {{audio|en-uk-dictionary.ogg|Audio (UK)|lang=en}}
* {{hyphenation|dic|tion|ary|lang=en}}
";
            var html = "" +
"<ul>" +
"<li><p>(Received Pronunciation) IPA: /ˈdɪkʃ(ə)n(ə)ɹi/</p></li>" +
"<li><p>(General American, Canada) enPR: dĭk'shə-nĕr-ē, IPA: /ˈdɪkʃənɛɹi/</p></li>" +
"</ul>" +
"";
            var formatted = Convert(creole, sectionName: "Pronunciation");
            Assert.Equal(html, formatted.ToHtml());
            Assert.Collection(formatted.Proninciations,
                item =>
                {
                    Assert.Equal("Audio (US)", item.Label);
                    Assert.Equal("English", item.Language);
                    Assert.Equal("en-us-dictionary.ogg", item.FileName);
                },
                item =>
                {
                    Assert.Equal("Audio (UK)", item.Label);
                    Assert.Equal("English", item.Language);
                    Assert.Equal("en-uk-dictionary.ogg", item.FileName);
                });
        }
    }
}
