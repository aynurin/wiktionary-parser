using Fabu.Wiktionary.TextConverters.Wiki;
using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    class EllipsisTemplateConverter : TemplateConverter
    {
        protected override ConversionResult ConvertTemplate(TemplateName name, Template template, ConversionContext context)
        {
            var result = new ConversionResult();
            if (name.OriginalName == "nb...")
                result.Add("&nbsp;");
            else result.Add(" ");
            result.Add("[&hellip;]");
            if (name.OriginalName != "nb...")
                result.Add(" ");
            return result;
        }
    }
}
