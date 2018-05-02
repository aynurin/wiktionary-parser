using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class SynonymsTemplateConverter : BaseTemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();

            if(template.Arguments.TryGetArray(null, out Wikitext[] words))
            {
                WriteCap(result, template, DefaultCap, DefaultNoCap);

                for (var i = 1; i < words.Length; i++)
                {
                    template.Arguments.TryGetOneOfAt(out Wikitext alt, i, "alt");
                    template.Arguments.TryGetOneOfAt(out Wikitext tr, i, "tr");
                    template.Arguments.TryGetOneOfAt(out Wikitext q, i, "q");

                    if (i > 1)
                        result.Write(", ");

                    result.Write((alt ?? words[i]).TooSmart());

                    if (tr != null)
                        result.Write(" (", tr.TooSmart(), ")");
                    if (q != null)
                        result.Write(" (<em>", q.TooSmart(), "</em>)");
                }
                
                WriteDot(result, template);
            }
            
            return result;
        }

        protected virtual string DefaultCap => "Synonyms: ";
        protected virtual string DefaultNoCap => "synonyms: ";
    }

    class HyponymsTemplateConverter : SynonymsTemplateConverter
    {
        protected override string DefaultCap => "Hyponyms: ";
        protected override string DefaultNoCap => "hyponyms: ";
    }

    class AntonymsTemplateConverter : SynonymsTemplateConverter
    {
        protected override string DefaultCap => "Antonyms: ";
        protected override string DefaultNoCap => "antonyms: ";
    }

    class HypernymsTemplateConverter : SynonymsTemplateConverter
    {
        protected override string DefaultCap => "Hypernyms: ";
        protected override string DefaultNoCap => "hypernyms: ";
    }
}
