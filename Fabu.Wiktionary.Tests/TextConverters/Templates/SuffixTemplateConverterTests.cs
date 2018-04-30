using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class SuffixTemplateConverterTests : TestConverterFactory
    {
        [Fact]
        public void ShouldConvertCompound()
        {
            var creole = "{{der|en|la|-}} {{compound|la|floccus|t1=a wisp|naucum|t2=a trifle|nihilum|t3=nothing|pilus|t4=a hair|nocat=1}} + {{m|en|-fication}}";
            var html = "<p>Latin <em>floccus</em> (&ldquo;a wisp&rdquo;) + <em>naucum</em> (&ldquo;a trifle&rdquo;) + <em>nihilum</em> (&ldquo;nothing&rdquo;) + <em>pilus</em> (&ldquo;a hair&rdquo;) + -fication</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertCompoundSimple()
        {
            var creole = "{{compound|en|above|deck}}";
            var html = "<p><em>above</em> + <em>deck</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertCompoundAlt()
        {
            var creole = "{{compound|en|above|deck|alt2=peck}}";
            var html = "<p><em>above</em> + <em>peck</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertBlend()
        {
            var creole = "{{blend|he|תַּשְׁבֵּץ|tr1=tashbéts|t1=crossword puzzle|חֵץ|t2=arrow|tr2=chets}}";
            var html = "<p>Blend of תַּשְׁבֵּץ (<em>tashbéts</em>, &ldquo;crossword puzzle&rdquo;) + חֵץ (<em>chets</em>, &ldquo;arrow&rdquo;)</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertBlendNoCap()
        {
            var creole = "{{blend|lang=en|mouse|couch potato|nocap=1}}";
            var html = "<p>blend of mouse + couch potato</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertBlendNoText()
        {
            var creole = "{{blend|abscond|squat|perambulate|lang=en|notext=1}}";
            var html = "<p>abscond + squat + perambulate</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertConfixSimple()
        {
            var creole = "{{confix|en|neuro|genic}}";
            var html = "<p><em>neuro</em>- + -<em>genic</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertConfixLang()
        {
            var creole = "{{confix|anti|disestablishmentarian|ism|lang=en}}";
            var html = "<p><em>anti</em>- + -<em>disestablishmentarian</em>- + -<em>ism</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertPrefix1()
        {
            var creole = "{{prefix|ab|articulation|lang=en}}";
            var html = "<p><em>ab</em>- + <em>articulation</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertPrefix2()
        {
            var creole = "{{prefix|ab|t1=away from|axial|t2=axis|lang=en}}";
            var html = "<p><em>ab</em>- (&ldquo;away from&rdquo;) + <em>axial</em> (&ldquo;axis&rdquo;)</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertPrefix3()
        {
            var creole = "{{prefix|abdomino|thoracic|t1=abdomen|t2=chest|lang=en}}";
            var html = "<p><em>abdomino</em>- (&ldquo;abdomen&rdquo;) + <em>thoracic</em> (&ldquo;chest&rdquo;)</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertPrefixLang()
        {
            var creole = "{{prefix|en|abdomino|thoracic|ly|t1=abdomen|t2=chest}}";
            var html = "<p><em>abdomino</em>- (&ldquo;abdomen&rdquo;) + <em>thoracic</em>- (&ldquo;chest&rdquo;) + <em>ly</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ShouldConvertSuffix1()
        {
            var creole = "{{suffix|ru|учить|тель}}";
            var html = "<p>учить + -тель</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertSuffix2()
        {
            var creole = "{{suffix|ru|учить|tr1=teach|тель|gloss2=-er}}";
            var html = "<p>учить (<em>teach</em>) + -тель (&ldquo;-er&rdquo;)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertSuffix3()
        {
            var creole = "{{suffix|ru|учить|tr1=učit'|gloss1=teach|тель|tr2=tel'|gloss2=-er}}";
            var html = "<p>учить (<em>učit'</em>, &ldquo;teach&rdquo;) + -тель (<em>tel'</em>, &ldquo;-er&rdquo;)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void ShouldConvertSuffix4()
        {
            var creole = "{{suffix|de|Einheit|t1=unity|lich|gloss2=-ly|pich|tr3=pic|gloss3=-my}}";
            var html = "<p>Einheit (&ldquo;unity&rdquo;) + -lich (&ldquo;-ly&rdquo;) + -pich (<em>pic</em>, &ldquo;-my&rdquo;)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
