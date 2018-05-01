using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters.Templates
{
    public class GivenNameTests : TestConverterFactory
    {
        [Fact]
        public void Female()
        {
            var creole = "{{given name|female|from=surnames}}";
            var html = "<p><em>A female given name.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void Male()
        {
            var creole = "{{given name|male}}";
            var html = "<p><em>A male given name.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void FemaleMale()
        {
            var creole = "{{given name|female|or=male}}";
            var html = "<p><em>A female or male given name.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void Hebrew()
        {
            var creole = "{{given name|male|from=Hebrew|lang=de}}";
            var html = "<p><em>A Hebrew male given name.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void EnglishMaleFemale()
        {
            var creole = "{{given name|female|or=male|from=English}}";
            var html = "<p><em>An English female or male given name.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void EnglishNoGender()
        {
            var creole = "{{given name|English|A=a}}";
            var html = "<p><em>an English given name.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void DimFemale()
        {
            var creole = "{{given name|female|dim=Svetlana}}";
            var html = "<p><em>A diminutive of the female given name Svetlana.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void DimFemaleEnglish()
        {
            var creole = "{{given name|female|dim=Svetlana|from=English}}";
            var html = "<p><em>A diminutive of the English female given name Svetlana.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void DimTrFemale()
        {
            var creole = "{{given name|female|dim=Svetlana|dimtr=Svetlanah}}";
            var html = "<p><em>A diminutive of the female given name Svetlana (Svetlanah).</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void DimTr23Female()
        {
            var creole = "{{given name|female|dim=Svetlana|dim2=Svetulya|dimtr2=Svetulet|dim3=Pipa|dimalt3=Pipulya|nodot=1}}";
            var html = "<p><em>A diminutive of the female given names Svetlana, Svetulya (Svetulet) or Pipulya</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void FemaleEq()
        {
            var creole = "{{given name|female|eq=Sweet|dot=,}}";
            var html = "<p><em>A female given name, equivalent to English Sweet,</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void DimFemaleEq2()
        {
            var creole = "{{given name|female|dim=Svetlana|eq=Sweet|eq2=Sweeta}}";
            var html = "<p><em>A diminutive of the female given name Svetlana, equivalent to English Sweet or Sweeta.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void FemaleEq3()
        {
            var creole = "{{given name|female|eq3=Svetlana|eq=Sweet|eq2=Sweeta}}";
            var html = "<p><em>A female given name, equivalent to English Sweet, Sweeta or Svetlana.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void Historical()
        {
            var creole = "{{historical given name|male|{{w|Saint Abundius}}, an early Christian bishop|lang=en}}";
            var html = "<p><em>A male given name of historical usage, notably borne by Saint Abundius, an early Christian bishop.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void Surname()
        {
            var creole = "{{surname}}";
            var html = "<p><em>A surname.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void SurnamePatronymic()
        {
            var creole = "{{surname|patronymic}}";
            var html = "<p><em>A patronymic surname.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void SurnameAn()
        {
            var creole = "{{surname|A=An|occupational}}";
            var html = "<p><em>An occupational surname.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void SurnameComma()
        {
            var creole = "{{surname|from=Latin|dot=,}} a form of [[Silvanus]].";
            var html = "<p><em>A Latin surname,</em> a form of Silvanus.</p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void SurnameFinnish()
        {
            var creole = "{{surname|lang=fi|from=Finnish}}";
            var html = "<p><em>A Finnish surname.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
        [Fact]
        public void SurnamePatronymicFinnish()
        {
            var creole = "{{surname|patronymic|lang=fi|from=Finnish}}";
            var html = "<p><em>A Finnish patronymic surname.</em></p>";
            Assert.Equal(html, Convert(creole).ToHtml());
        }
    }
}
