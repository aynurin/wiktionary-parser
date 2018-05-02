using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class SuffixTests : TestConverterFactory
    {
        [Fact]
        public void Compound()
        {
            var creole = "{{der|en|la|-}} {{compound|la|floccus|t1=a wisp|naucum|t2=a trifle|nihilum|t3=nothing|pilus|t4=a hair|nocat=1}} + {{m|en|-fication}}";
            var html = "<p>Latin <em>floccus</em> (&ldquo;a wisp&rdquo;) + <em>naucum</em> (&ldquo;a trifle&rdquo;) + <em>nihilum</em> (&ldquo;nothing&rdquo;) + <em>pilus</em> (&ldquo;a hair&rdquo;) + <em>-fication</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void CompoundSimple()
        {
            var creole = "{{compound|en|above|deck}}";
            var html = "<p><em>above</em> + <em>deck</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void CompoundAlt()
        {
            var creole = "{{compound|en|above|deck|alt2=peck}}";
            var html = "<p><em>above</em> + <em>peck</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void Blend()
        {
            var creole = "{{blend|he|תַּשְׁבֵּץ|tr1=tashbéts|t1=crossword puzzle|חֵץ|t2=arrow|tr2=chets}}";
            var html = "<p>Blend of תַּשְׁבֵּץ (<em>tashbéts</em>, &ldquo;crossword puzzle&rdquo;) + חֵץ (<em>chets</em>, &ldquo;arrow&rdquo;)</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void BlendNoCap()
        {
            var creole = "{{blend|lang=en|mouse|couch potato|nocap=1}}";
            var html = "<p>blend of mouse + couch potato</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void BlendNoText()
        {
            var creole = "{{blend|abscond|squat|perambulate|lang=en|notext=1}}";
            var html = "<p>abscond + squat + perambulate</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ConfixSimple()
        {
            var creole = "{{confix|en|neuro|genic}}";
            var html = "<p><em>neuro</em>- + -<em>genic</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void ConfixLang()
        {
            var creole = "{{confix|anti|disestablishmentarian|ism|lang=en}}";
            var html = "<p><em>anti</em>- + -<em>disestablishmentarian</em>- + -<em>ism</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void Prefix1()
        {
            var creole = "{{prefix|ab|articulation|lang=en}}";
            var html = "<p><em>ab</em>- + <em>articulation</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void Prefix2()
        {
            var creole = "{{prefix|ab|t1=away from|axial|t2=axis|lang=en}}";
            var html = "<p><em>ab</em>- (&ldquo;away from&rdquo;) + <em>axial</em> (&ldquo;axis&rdquo;)</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void Prefix3()
        {
            var creole = "{{prefix|abdomino|thoracic|t1=abdomen|t2=chest|lang=en}}";
            var html = "<p><em>abdomino</em>- (&ldquo;abdomen&rdquo;) + <em>thoracic</em> (&ldquo;chest&rdquo;)</p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void PrefixLang()
        {
            var creole = "{{prefix|en|abdomino|thoracic|ly|t1=abdomen|t2=chest}}";
            var html = "<p><em>abdomino</em>- (&ldquo;abdomen&rdquo;) + <em>thoracic</em>- (&ldquo;chest&rdquo;) + <em>ly</em></p>";
            Assert.Equal(html, Convert(creole, false).ToHtml());
        }

        [Fact]
        public void Suffix1()
        {
            var creole = "{{suffix|ru|учить|тель}}";
            var html = "<p>учить + -тель</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Suffix2()
        {
            var creole = "{{suffix|ru|учить|tr1=teach|тель|gloss2=-er}}";
            var html = "<p>учить (<em>teach</em>) + -тель (&ldquo;-er&rdquo;)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Suffix3()
        {
            var creole = "{{suffix|ru|учить|tr1=učit'|gloss1=teach|тель|tr2=tel'|gloss2=-er}}";
            var html = "<p>учить (<em>učit'</em>, &ldquo;teach&rdquo;) + -тель (<em>tel'</em>, &ldquo;-er&rdquo;)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Suffix4()
        {
            var creole = "{{suffix|de|Einheit|t1=unity|lich|gloss2=-ly|pich|tr3=pic|gloss3=-my}}";
            var html = "<p>Einheit (&ldquo;unity&rdquo;) + -lich (&ldquo;-ly&rdquo;) + -pich (<em>pic</em>, &ldquo;-my&rdquo;)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void AffixComplex()
        {
            var creole = "{{affix|en|hypo-|gloss1=under|-onym|gloss2=name; word|lit1=XxX}}";
            var html = "<p><em>hypo-</em> (&ldquo;under&rdquo;, literally &ldquo;XxX&rdquo;) + <em>-onym</em> (&ldquo;name; word&rdquo;)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void CircumfixTrGloss()
        {
            var creole = "{{circumfix|em|bold|en|lang=en|tr2=XXX|t3=YYY|gloss=ZZZ|gloss1=ASA|alt2=sss}}";
            var html = "<p><em>em-</em> (&ldquo;ASA&rdquo;) + <em>sss</em> (<em>XXX</em>) + <em>-en</em> (&ldquo;YYY&rdquo;)</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void Circumfix()
        {
            var creole = "{{circumfix|en|en|grave|en}}";
            var html = "<p><em>en-</em> + <em>grave</em> + <em>-en</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }

        [Fact]
        public void CircumfixLang()
        {
            var creole = "{{circumfix|em|bold|en|lang=en}}";
            var html = "<p><em>em-</em> + <em>bold</em> + <em>-en</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
