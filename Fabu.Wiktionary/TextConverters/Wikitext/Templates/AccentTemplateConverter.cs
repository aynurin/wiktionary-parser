using System;
using System.Collections.Generic;
using MwParserFromScratch.Nodes;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class AccentTemplateConverter : BaseNodeConverter
    {
        public override ConversionResult Convert(Node node, WikiConversionContext context)
        {
            var template = node as Template;
            var result = new ConversionResult();
            if (template.Arguments.Count > 0)
                result.Write("(");
            for (var i = 1; i <= template.Arguments.Count; i++)
            {
                if (i > 1)
                    result.Write(", ");
                result.Write(GetAccent(template.Arguments[i].ToString()));
            }
            if (template.Arguments.Count > 0)
                result.Write(")");
            return result;
        }

        // TODO: Get accents from wiktionary
        private string GetAccent(string key)
        {
            if (labels.TryGetValue(key, out string val))
                return val;
            return key;
        }

        private Dictionary<string, string> labels = new Dictionary<string, string>()
        {
{ "AAVE", "African American Vernacular English" },

{ "æ-tensing", "æ-tensing" },
{ "ae-tensing", "æ-tensing" },

{ "Anglicised", "Anglicised" },
{ "Anglicized", "Anglicised" },

{ "Aran", "Aran" },

{ "ar-Cairene", "Cairene" },
{ "Cairene", "Cairene" },

{ "Arvanite", "Arvanite" },

{ "Ashkenazi Hebrew", "Ashkenazi Hebrew" },
{ "Ashkenazi", "Ashkenazi Hebrew" },

{ "Australia", "General Australian" },
{ "AU", "General Australian" },
{ "AuE", "General Australian" },
{ "Aus", "General Australian" },
{ "AusE", "General Australian" },
{ "GenAus", "General Australian" },
{ "General Australian", "General Australian" },

// B

{ "BE-nl", "Belgium" },
{ "BE", "Belgium" },

{ "Beijing", "Beijing" },

{ "Bosnia", "Bosnia" },
{ "Bosnian", "Bosnia" },

{ "Boston", "Boston" },
{ "Bos", "Boston" },

{ "Brazil", "Brazil" },
{ "BP", "Brazil" },
{ "BR", "Brazil" },
{ "Brazilian Portuguese", "Brazil" },

// C

{ "Canada", "Canada" },
{ "CA", "Canada" },
{ "Canadian", "Canada" },

{ "Canadian Shift", "Canadian Vowel Shift" },
{ "Canadian shift", "Canadian Vowel Shift" },
{ "Canadian Vowel Shift", "Canadian Vowel Shift" },
{ "Canadian vowel shift", "Canadian Vowel Shift" },

{ "Carioca", "Carioca" },
{ "RJ", "Carioca" },

{ "Castilian", "Castilian" },
{ "Spain", "Castilian" },

{ "Central Catalan", "Central" },

{ "central Germany", "central Germany" },
{ "central German", "central Germany" },
{ "Central German", "central Germany" },
{ "Central Germany", "central Germany" },

{ "central Italy", "central Italy" },
{ "central Italian", "central Italy" },
{ "Central Italian", "central Italy" },
{ "Central Italy", "central Italy" },

{ "Central Scotland", "Central Scotland" },

{ "ceceo", "''ceceo'' merger" },

{ "Classical", "Classical" },

{ "Classical Sanskrit", "Classical" },

{ "Cois Fharraige", "Cois Fharraige" },
{ "CF", "Cois Fharraige" },

{ "Connacht", "Connacht" },

{ "Cork", "Cork" },

{ "cot-caught", "''cot''–''caught'' merger" },
{ "caught-cot", "''cot''–''caught'' merger" },

{ "Croatia", "Croatia" },
{ "Croatian", "Croatia" },

{ "cy-N", "North Wales" },
{ "cy-g", "North Wales" },

{ "cy-S", "South Wales" },
{ "cy-h", "South Wales" },

// D

{ "Dari", "Dari" },

{ "Delhi", "Delhi Hindi" },

{ "distinción", "''z''-''s'' distinction" },
{ "distincion", "''z''-''s'' distinction" },


// E

{ "Ecclesiastical", "Ecclesiastical" },

{ "Egyptological", "modern Egyptological" },
{ "modern Egyptological", "modern Egyptological" },

{ "Estuary English", "Estuary English" },

// F

{ "father-bother", "''father''-''bother'' merger" },

{ "FS", "FS" },

{ "FV", "French Flanders" },

// G

{ "GenAm", "General American" },
{ "GA", "General American" },

{ "Geordie", "Geordie" },

{ "Gheg", "Gheg" },

{ "Givi", "Givi" },

{ "Glenties", "The Glenties" },

{ "grc-byz1", "Byzantine" },

{ "grc-byz2","Constantinopolitan"
},

{ "grc-cla","Attic"
},

{ "grc-koi1","Egyptian"
},

{ "grc-koi2","Koine"
},

// H

{ "hbo", "Biblical Hebrew" },
{ "Biblical Hebrew", "Biblical Hebrew" },

{ "Hong Kong", "Hong Kong" },
{ "HK", "Hong Kong" },

{ "horse-hoarse", "without the ''horse''–''hoarse'' merger" },

{ "hy-E", "Eastern Armenian" },

{ "hy-IR", "Eastern Armenian - Iran" },

{ "hy-W", "Western Armenian" },

{ "hy-Y", "Eastern Armenian - Yerevan" },

// I

{ "IL", "Modern Israeli Hebrew" },
{ "Israeli Hebrew", "Modern Israeli Hebrew" },
{ "Modern Hebrew", "Modern Israeli Hebrew" },
{ "Modern Israeli", "Modern Israeli Hebrew" },
{ "Modern Israeli Hebrew", "Modern Israeli Hebrew" },
{ "Modern/Israeli Hebrew", "Modern Israeli Hebrew" },

{ "InE", "Indian English" },

{ "IR", "Iranian Persian" },

{ "Ireland", "Ireland" },
{ "HE", "Ireland" },
{ "IE", "Ireland" },

{ "Italian Hebrew", "Italian Hebrew" },

// J

{ "Johor-Selangor", "Johor-Selangor" },

// K

{ "Kabul, Peshawar", "Kabul, Peshawar" },

{ "Kandahar", "Kandahar" },
{ "ps-Kandahar", "Kandahar" },

{ "Kerry", "Kerry" },

{ "ker-ham",
"Hamadani"
},

{ "ker-mah",
"Mahallati"
},

{ "ker-von",
"Vonishuni"
},

{ "ker-del",
"Delijani"
},

{ "ker-kas",
"Kashani"
},

{ "ker-kes",
"Kese'i"
},

{ "ker-mey",
"Meyme'i"
},

{ "ker-abz",
"Abuzeydabadi"
},

{ "ker-aby",
"Abyanehi"
},

{ "ker-far",
"Farizandi"
},

{ "ker-jow",
"Jowshaqani"
},

{ "ker-qoh",
"Qohrudi"
},

{ "ker-yar",
"Yarandi"
},

{ "ker-tar",
"Tari"
},

{ "ker-sed",
"Sedehi"
},


{ "ker-ard",
"Ardestani"
},

{ "ker-zef",
"Zefre'i"
},

{ "ker-isf",
"Isfahani"
},

{ "ker-kaf",
"Kafroni"
},

{ "ker-var",
"Varzenei"
},

{ "ker-nyq",
"Nayini"
},

{ "ker-vaf",
"Vafsi"
},

{ "ker-atn",
"Ashtiani language"
},

{ "ker-kfm",
"Khunsari"
},

{ "ker-ntz",
"Natanzi"
},

{ "ker-soj",
"Soi"
},

{ "ker-gzi",
"Gazi"
},

{ "ker-ana",
"Anaraki"
},

{ "ker-krm",
"Kermani"
},

{ "ker-yzd",
"Yazdi"
},

// L

{ "LAm", "Latin American" },

{ "Late Egyptian", "reconstructed Late Egyptian" },

{ "Latinate", "Latinate" },

{ "lleísmo", "''ll''-''y'' distinction" },

// M

{ "Mary-marry-merry", "''Mary''–''marry''–''merry'' merger" },
{ "Mmmm", "''Mary''–''marry''–''merry'' merger" },

{ "Mayo", "Mayo" },

{ "Medio-Late Egyptian", "reconstructed Medio-Late Egyptian" },

{ "Middle Egyptian", "reconstructed Middle Egyptian" },

{ "Midwestern US", "Midwestern US" },
{ "Midwest US", "Midwestern US" },
{ "Midwest US English", "Midwestern US" },
{ "Midwestern US English", "Midwestern US" },

{ "Mizrahi Hebrew", "Mizrahi Hebrew" },
{ "Mizrahi", "Mizrahi Hebrew" },
{ "Mizrakhi", "Mizrahi Hebrew" },
{ "Mizrachi", "Mizrahi Hebrew" },
{ "Mizrakhi Hebrew", "Mizrahi Hebrew" },
{ "Mizrachi Hebrew", "Mizrahi Hebrew" },

{ "MLE", "MLE" },

{ "Montenegro", "Montenegro" },
{ "Montenegrin", "Montenegro" },

{ "Munster", "Munster" },

// N

{ "Netherlands", "Netherlands" },
{ "NL", "Netherlands" },

{ "New Latin", "New Latin" },

{ "New York", "NYC" },
{ "NY", "NYC" },
{ "NYC", "NYC" },

{ "New Zealand", "General New Zealand" },
{ "NZ", "General New Zealand" },
{ "GNZ", "General New Zealand" },
{ "General New Zealand", "General New Zealand" },

{ "non-Mary-marry-merry", "''Mary''–''marry''–''merry'' distinction" },
{ "nMmmm", "''Mary''–''marry''–''merry'' distinction" },

{ "non-rhotic", "non-rhotic" },
{ "nonrhotic", "non-rhotic" },

{ "non-weak vowel", "weak vowel distinction" },

{ "northern and central Germany", "northern Germany, central Germany" },
{ "north and central German", "northern Germany, central Germany" },
{ "North and Central German", "northern Germany, central Germany" },
{ "north and central Germany", "northern Germany, central Germany" },
{ "North and Central Germany", "northern Germany, central Germany" },
{ "northern and central German", "northern Germany, central Germany" },
{ "Northern and Central German", "northern Germany, central Germany" },
{ "Northern and Central Germany", "northern Germany, central Germany" },

{ "Northern England", "Northern England" },
{ "North England", "Northern England" },

{ "northern Germany", "northern Germany" },
{ "north German", "northern Germany" },
{ "North German", "northern Germany" },
{ "north Germany", "northern Germany" },
{ "North Germany", "northern Germany" },
{ "northern German", "northern Germany" },
{ "Northern German", "northern Germany" },
{ "Northern Germany", "northern Germany" },

{ "Northern Scotland", "Northern Scotland" },

// O

{ "Old Egyptian", "reconstructed Old Egyptian" },

{ "Osaka", "Osaka" },

// P

{ "Palestinian Hebrew", "Palestinian Hebrew" },

{ "pin-pen", "''pin''–''pen'' merger" },
{ "pen-pin", "''pin''–''pen'' merger" },

{ "Philippine", "Philippine" },
{ "Philippines", "Philippine" },

{ "Portugal", "Portugal" },
{ "EP", "Portugal" },
{ "PT", "Portugal" },

{ "ps-Kabul", "Kabuli" },

// Q

{ "Quanzhou", "Quanzhou" },

{ "Quetta", "Quetta" },

// R

{ "r-dissimilation", "''r''-dissimilation" },

{ "rhotic", "rhotic" },

{ "Riau-Lingga", "Riau-Lingga" },

{ "Rioplatense", "Rioplatense" },

{ "RP", "Received Pronunciation" },

// S

{ "São Paulo", "São Paulo" },

{ "Scotland", "Scotland" },

{ "Sephardi Hebrew", "Sephardi Hebrew" },
{ "Sephardi", "Sephardi Hebrew" },

{ "Serbia", "Serbia" },
{ "Serbian", "Serbia" },

{ "seseo", "''seseo'' merger" },

{ "Sistani", "Sistani" },

{ "South Africa", "General South African" },
{ "SAE", "General South African" },
{ "GSAE", "General South African" },
{ "GenSAE", "General South African" },
{ "General South African", "General South African" },

{ "Southern American English", "Southern American English" },
{ "Southern US", "Southern American English" },
{ "Southern US English", "Southern American English" },
{ "Southern U.S. English", "Southern American English" },

{ "South Brazil", "South Brazil" },

{ "southern Germany", "southern Germany" },
{ "south German", "southern Germany" },
{ "South German", "southern Germany" },
{ "south Germany", "southern Germany" },
{ "South Germany", "southern Germany" },
{ "southern German", "southern Germany" },
{ "Southern German", "southern Germany" },
{ "Southern Germany", "southern Germany" },

{ "Southern Scotland", "Southern Scotland" },

{ "St. Louis", "St. Louis (Missouri)" },
{ "STL", "St. Louis (Missouri)" },

{ "standard", "standard" },
{ "Standard", "standard" },

{ "Swedish", "Swedish" },

{ "Syrian Hebrew", "Syrian Hebrew" },

// T

{ "t-glottalization", "''t''-glottalization" },
{ "t-glottaling", "''t''-glottalization" },

{ "Tajik", "Tajik" },
{ "Tajiki", "Tajik" },

{ "Tehrani", "Tehrani" },

{ "Tiberian Hebrew", "Tiberian Hebrew" },
{ "Tiberian", "Tiberian Hebrew" },

{ "Tosk", "Tosk" },

// U

{ "UK", "UK" },
{ "British", "UK" },
{ "U.K.", "UK" },

{ "Ulaanbaatar", "Ulaanbaatar" },
{ "UlaanBaatar", "Ulaanbaatar" },

{ "Ulster", "Ulster" },
{ "Donegal", "Ulster" },

{ "US", "US" },
{ "U.S.", "US" },

// V

{ "Valencia", "Valencian" },

{ "Vedic Sanskrit", "Vedic" },
{ "Vedic", "Vedic" },
{ "Rigvedic", "Vedic" },

{ "Vulgar", "Vulgar" },

// W

{ "Wales", "Wales" },
{ "Welsh", "Wales" },

{ "Wardak", "Wardak" },

{ "Waterford", "Waterford" },

{ "Wazirwola", "Wazirwola" },

{ "weak vowel", "weak vowel merger" },

{ "wine/whine", "without the ''wine–whine'' merger" },

// X

{ "Xiamen", "Xiamen" },

// Y

{ "yeísmo", "''ll''-''y'' neutralization" },

{ "Yemenite Hebrew", "Yemenite Hebrew" },

{ "YIVO", "YIVO" },

{ "yod-coalescence", "yod-coalescence" },

// Z

{ "Zhangzhou", "Zhangzhou" }
        };
    }
}
