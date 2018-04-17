using Fabu.Wiktionary.TextConverters.Wiki;
using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    class EllipsisTemplateConverter : TemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();
            result.Add("[&hellip;]");
            return result;
        }
    }
}
