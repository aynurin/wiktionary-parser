using Fabu.Wiktionary.TextConverters;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class StripHtmlTest
    {
        [Fact]
        public void TablesShouldProcessNulls()
        {
            Assert.Null(StripHtml.Tables(null));
        }

        [Fact]
        public void TablesShouldProcessEmptyStrings()
        {
            Assert.Equal("", StripHtml.Tables(""));
        }

        [Fact]
        public void TablesShouldProcessEmptyTables()
        {
            Assert.Equal("", StripHtml.Tables("<table></table>"));
        }

        [Fact]
        public void TablesShouldProcessEmbeddedTables()
        {
            Assert.Equal("abcvw", StripHtml.Tables("abc<table><tr><td>def<table><tr>ghi<td></td></tr></table>jkl</td></tr><tr><td>mno<table><tr><td>pqr</td></tr></table>stu</td></tr></table>vw"));
        }

        [Fact]
        public void TablesShouldProcessSeriesOfTables()
        {
            Assert.Equal("abcmnovw", StripHtml.Tables("abc<table><tr><td>def</td></tr><tr><td>jkl</td></tr></table>mno<table><tr><td>pqr</td></tr><tr><td>stu</td></tr></table>vw"));
        }

        [Fact]
        public void CommentsShouldProcessNulls()
        {
            Assert.Null(StripHtml.Comments(null));
        }

        [Fact]
        public void CommentsShouldProcessEmptyStrings()
        {
            Assert.Equal("", StripHtml.Comments(""));
        }

        [Fact]
        public void CommentsShouldProcessEmptyComments()
        {
            Assert.Equal("", StripHtml.Comments("<!---->"));
        }

        [Fact]
        public void CommentsShouldProcessEmbeddedComments()
        {
            Assert.Equal("abcjkl</td></tr><tr><td>mnostu</td></tr>-->vw", StripHtml.Comments("abc<!--<tr><td>def<!--<tr>ghi<td></td></tr>-->jkl</td></tr><tr><td>mno<!--<tr><td>pqr</td></tr>-->stu</td></tr>-->vw"));
        }

        [Fact]
        public void CommentsShouldProcessSeriesOfComments()
        {
            Assert.Equal("abcmnovw", StripHtml.Comments("abc<!--<tr><td>def</td></tr><tr><td>jkl</td></tr>-->mno<!--<tr><td>pqr</td></tr><tr><td>stu</td></tr>-->vw"));
        }
    }
}
