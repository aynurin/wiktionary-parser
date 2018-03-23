using Fabu.Wiktionary.TermProcessing;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fabu.Wiktionary.Tests.TermProcessing
{
    public class ProcessingModeTests
    {
        [Fact]
        public void ShouldCreateProperCompound()
        {
            var one = new ProcessingMode()
            {
                AllowedTermModelSubSections = new[] { "a", "c" },
                AllowWiktionaryChildrenProcessing = false,
                MayDefineTerm = true
            };

            var another = new ProcessingMode()
            {
                AllowedTermModelSubSections = new[] { "a", "b" },
                AllowWiktionaryChildrenProcessing = false,
                MayDefineTerm = false
            };

            var third = one | another;

            Assert.NotEmpty(third.AllowedTermModelSubSections);
            Assert.Equal(3, third.AllowedTermModelSubSections.Length);
            Assert.True(third.MayDefineTerm);
            Assert.False(third.AllowWiktionaryChildrenProcessing);
        }

        [Fact]
        public void ShouldPlayWellWhenOneArrayIsNull()
        {
            var one = new ProcessingMode()
            {
                AllowedTermModelSubSections = new[] { "a", "c" },
                AllowWiktionaryChildrenProcessing = false,
                MayDefineTerm = true
            };

            var another = new ProcessingMode()
            {
                AllowedTermModelSubSections = null,
                AllowWiktionaryChildrenProcessing = false,
                MayDefineTerm = false
            };

            var third = one | another;

            Assert.NotEmpty(third.AllowedTermModelSubSections);
            Assert.Equal(2, third.AllowedTermModelSubSections.Length);
            Assert.True(third.MayDefineTerm);
            Assert.False(third.AllowWiktionaryChildrenProcessing);
            Assert.Collection(third.AllowedTermModelSubSections, 
                x => Assert.Equal("a", x),
                x => Assert.Equal("c", x));
        }
    }
}
