using Fabu.Wiktionary.TextConverters;
using Fabu.Wiktionary.TextConverters.Wiki;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fabu.Wiktionary.Tests.TextConverters
{
    public class WikitextQuoteTemplateConvertersTests
    {
        private Dictionary<string, string> _dictionary = new Dictionary<string, string>()
        {
            { "en", "English" },
            { "de", "German" },
            { "fy", "West Frisian" },
            { "la", "Latin" },
            { "ang", "Old English" },
            { "gml", "Middle Low German" },
            { "gmh", "Middle High German" },
            { "goh", "Old High German" },
            { "gem-pro", "Proto-Germanic" },
            { "ine-pro", "Proto-Indo-European" },
            { "nl", "Dutch" },
            { "osx", "Old Saxon" }
        };

        private string Convert(string creole, bool allowLinks = false)
        {
            var converter = new WikitextProcessor(_dictionary, allowLinks);
            return converter.ConvertToStructured(creole).ToHtml();
        }

        [Fact]
        public void ShouldConvertQuoteBook1()
        {
            var creole = "{{quote-book|author=William Shakespeare|authorlink=William Shakespeare|title=[[w:First Folio|Mr. William Shakespeares Comedies, Histories, & Tragedies: Published According to the True Originall Copies]]|location=London|publisher=Printed by [[w:William Jaggard|Isaac Iaggard]], and [[w:Edward Blount|Ed[ward] Blount]]|year=c. 1601–1602|year_published=1623|page=255|pageurl=https://books.google.com/books?id=uNtBAQAAMAAJ&pg=PA255|oclc=606515358|passage='''Twelfe Night''', Or vvhat you vvill. [title of play]}}";
            var html = "<p>c. 1601–1602, William Shakespeare, <em>Mr. William Shakespeares Comedies, Histories, & Tragedies: Published According to the True Originall Copies</em>, London: Printed by Isaac Iaggard, and Ed[ward] Blount: &ldquo;<strong>Twelfe Night</strong>, Or vvhat you vvill. [title of play]&rdquo;</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertQuoteBook2()
        {
            var creole = "{{quote-book|author=Pat Shaw Iversen, transl.|chapter=Soup from a Sausage Peg|title=The Snow Queen and Other Tales|series=Signet Classic|seriesvolume=CT334|location=New York, N.Y.|publisher={{w|New American Library}}|year=1966|page=224|pageurl=http://books.google.com/books?id=2tjWAAAAMAAJ&q=%22from+your+own+nook+and+cranny%22#search_anchor|oclc=636818779|original=Fairy Tales|by={{w|Hans Christian Andersen}}|passage=It's strange to come away from home, from your own '''nook and cranny''', to go by ship – which is also a kind of '''nook and cranny''' – and then suddenly be more than a hundred miles away and stand in a foreign land!}}";
            var html = "<p>1966, Pat Shaw Iversen, transl., &ldquo;Soup from a Sausage Peg&rdquo;, in <em>The Snow Queen and Other Tales</em>, New York, N.Y.: New American Library: &ldquo;It's strange to come away from home, from your own <strong>nook and cranny</strong>, to go by ship – which is also a kind of <strong>nook and cranny</strong> – and then suddenly be more than a hundred miles away and stand in a foreign land!&rdquo;</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertQuoteBook3()
        {
            var creole = "{{quote-book|author=David George Ritchie|authorlink=David George Ritchie|quotee={{w|Andrew Seth Pringle-Pattison}}|title=Hegelianism and Personality|edition=2nd|others=1893, page 72, note 1|quoted_in=''Darwin and Hegel, with other Philosophical Studies''|location=London|publisher=S. Sonnenschein & Co.; New York, N.Y.: [[w:Macmillan Publishers (United States)|Macmillan & Co.]]|year=1893|page=72, note 1|pageurl=http://books.google.com/books?id=8yGEAHvAwiIC&q=%22An+absolute+system+cannot+afford+to+leave+any+nook+or+cranny+of+existence+unexplored%22#search_anchor|oclc=3299658|passage=An absolute system cannot afford to leave '''any nook or cranny''' of existence unexplored.}}";
            var html = "<p>1893, David George Ritchie, quoting Andrew Seth Pringle-Pattison, <em>Hegelianism and Personality</em>, quoted in <em>Darwin and Hegel, with other Philosophical Studies</em>, London: S. Sonnenschein & Co.; New York, N.Y.: Macmillan & Co.: &ldquo;An absolute system cannot afford to leave <strong>any nook or cranny</strong> of existence unexplored.&rdquo;</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertQuoteBook4()
        {
            var creole = "{{quote-book|author=John Boyle O'Reilly|authorlink=John Boyle O'Reilly|chapter=On the Trail|title=[[w:Moondyne|Moondyne: A Story from the Under-world]]|location=London|publisher=[[w:Routledge|George Routledge and Sons]]|year=1878|year_published=1879|oclc=39983928|title2=Moondyne: A Story of Convict Life in Australia|location2=London|publisher2=George Routledge & Sons, Limited, Broadway House, {{w|Ludgate Hill}}|year2=[1880s]|section2=book first|pages2=23–24|pageurl2=https://archive.org/stream/moondynestory00oreirich#page/24/mode/1up/|oclc2=83033698|passage=It was sore travelling for horse and man under the blazing sun, with no food nor water save what he pressed from the pith of the palms, and even these were growing scarce. The only life on the plains was the hard and dusty scrub. Every hour brought a more hopeless and '''grislier''' desolation.}}";
            var html = "<p>1878, John Boyle O'Reilly, &ldquo;On the Trail&rdquo;, in <em>Moondyne: A Story from the Under-world</em>, London: George Routledge and Sons: &ldquo;It was sore travelling for horse and man under the blazing sun, with no food nor water save what he pressed from the pith of the palms, and even these were growing scarce. The only life on the plains was the hard and dusty scrub. Every hour brought a more hopeless and <strong>grislier</strong> desolation.&rdquo;</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertQuoteBook5()
        {
            var creole = "{{quote-book|author=Hayden Carruth|authorlink=Hayden Carruth|chapter=Making It New|title={{w|The Hudson Review}}|location=New York, N.Y.|publisher=Hudson Review, Inc.|month=summer|year=1968|volume=XXI|issue=2|issn=2325-5935|oclc=920393805|newversion=reprinted as|chapter2=From ‘Making It New’ [Body Rags]|editor2=Howard Nelson|title2=On the Poetry of Galway Kinnell: The Wages of Dying|series2=Under Discussion|location2=Ann Arbor, Mich.|publisher2={{w|University of Michigan Press}}|year2=1987|page2=75|pageurl2=https://books.google.com/books?id=7cXV26PmJUYC&pg=PA75|isbn2=978-0-472-09376-2|passage=In his [{{w|Galway Kinnell}}'s] new book, ''Body Rags'', he has brought this style to a kind of perfection, especially in two poems about the killing of animals, \"The Porcupine\" and \"The Bear.\" These are the '''grisliest''' poems I have ever read.}}";
            var html = "<p>1968 summer, Hayden Carruth, &ldquo;Making It New&rdquo;, in <em>The Hudson Review</em>, New York, N.Y.: Hudson Review, Inc.: &ldquo;In his [Galway Kinnell's] new book, <em>Body Rags</em>, he has brought this style to a kind of perfection, especially in two poems about the killing of animals, \"The Porcupine\" and \"The Bear.\" These are the <strong>grisliest</strong> poems I have ever read.&rdquo;</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertQuoteJournal()
        {
            var creole = "{{quote-journal|title=Art VIII.—Who are the True Conservatives?|journal=The Quarterly Christian Spectator|location=New Haven, Conn.|publisher=Published for the proprietor by Hezekiah Howe [...]|month=November|year=1838|volume=X|issue=IV|page=608|pageurl=https://books.google.com/books?id=S7MAAAAAMAAJ&pg=PA608|oclc=5090641|passage=They have guarded a few minds against '''sciolism''' and pretension in philosophy, and they have given a new and more imposing air to '''sciolism''' itself, and placed in the hands of the charlatan enchantments and magical arts, which hold fast its victims with a potency never granted to an inferior spell. There is no '''sciolism''' now which is more dangerous than that which is so very careful to inform us of its entire freedom from all that is superficial, {{...}}}}";
            var html = "<p>1838 November, &ldquo;Art VIII.—Who are the True Conservatives?&rdquo;, in <em>The Quarterly Christian Spectator</em>, New Haven, Conn.: Published for the proprietor by Hezekiah Howe [...]: &ldquo;They have guarded a few minds against <strong>sciolism</strong> and pretension in philosophy, and they have given a new and more imposing air to <strong>sciolism</strong> itself, and placed in the hands of the charlatan enchantments and magical arts, which hold fast its victims with a potency never granted to an inferior spell. There is no <strong>sciolism</strong> now which is more dangerous than that which is so very careful to inform us of its entire freedom from all that is superficial, [&hellip;]&rdquo;</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertQuoteSong()
        {
            var creole = "{{quote-song|author=[[w:Madonna (entertainer)|Madonna]]; {{w|Stephen Bray}}; {{w|Patrick Leonard}}|artist=Madonna|title=Where's the Party|album=[[w:True Blue (Madonna album)|True Blue]]|year=1986|note=track 5|passage=Don't want to grow old too fast / Don't want to let the '''system''' get me down / I've got to find a way to make the good times last / And if you'll show me how, I'm ready now}}";
            var html = "<p>1986, Madonna; Stephen Bray; Patrick Leonard, &ldquo;Where's the Party&rdquo;, in <em>True Blue</em>, performed by Madonna: &ldquo;Don't want to grow old too fast / Don't want to let the <strong>system</strong> get me down / I've got to find a way to make the good times last / And if you'll show me how, I'm ready now&rdquo;</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertQuoteVideo()
        {
            var creole = "{{quote-video|actor={{w|Jay Mohr}}|role=Bob Sugar|title={{w|Jerry Maguire}}|writer={{w|Cameron Crowe}}|location=Culver City, Calif.|publisher={{w|TriStar Pictures}}; distributed by [[w:Sony Pictures Home Entertainment|Columbia TriStar Home Video]]|year=1996|year_published=1997|isbn=978-0-8001-4174-5|passage=It's also my job to take care of the '''skanks''' on the road that you bang.}}";
            var html = "<p>1996, Jay Mohr as Bob Sugar, <em>Jerry Maguire</em>, written by Cameron Crowe, Culver City, Calif.: TriStar Pictures; distributed by Columbia TriStar Home Video: &ldquo;It's also my job to take care of the <strong>skanks</strong> on the road that you bang.&rdquo;</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertQuoteWeb()
        {
            var creole = "{{quote-web|title=Why you should read the Madicken (Mardie) books|url=http://kattahj.tumblr.com/post/72567245752/i-made-a-presentation-on-madicken-because|work=Market Chipping|format=blog|date=7 January 2014|accessdate=8 March 2016|passage=Let me tell you about Madicken. (Mardie in English. Or Meg, but that’s in the American translation and that’s '''bowdlerized''' and you should never read it.)}}";
            var html = "<p>2014 January 7, &ldquo;Why you should read the Madicken (Mardie) books&rdquo;, in <em>Market Chipping</em>, retrieved 8 March 2016: &ldquo;Let me tell you about Madicken. (Mardie in English. Or Meg, but that’s in the American translation and that’s <strong>bowdlerized</strong> and you should never read it.)&rdquo;</p>";
            Assert.Equal(html, Convert(creole));
        }

        [Fact]
        public void ShouldConvertQuoteNewsgroup()
        {
            var creole = "{{quote-newsgroup|author=Peter da Silva|title=Re:Microsoft versus Digital Equipment Corporation|newsgroup=alt.folklore.computers|id=g0hq1u$2hkn$3@monolith.in.taronga.com|url=http://groups.google.com/group/alt.folklore.computers/msg/032c30495567b213|date=16 March 2008|passage={{...}} otherwise the pager needs to start doing a bunch of unnecessary '''yak shaving'''.}}";
            var html = "<p>2008 March 16, Peter da Silva, &ldquo;Re:Microsoft versus Digital Equipment Corporation&rdquo;, in <em>alt.folklore.computers</em>: &ldquo;[&hellip;] otherwise the pager needs to start doing a bunch of unnecessary <strong>yak shaving</strong>.&rdquo;</p>";
            Assert.Equal(html, Convert(creole));
        }
    }
}
