using System;
using System.Collections.Generic;
using System.Text;

namespace Fabu.Wiktionary.TextConverters.Wkt
{
    public class WktConverter : ITextConverter
    {
        public FormattedString ConvertToStructured(string wikitext)
        {
            var scanner = new StringScanner(wikitext);
            return null;
        }
    }

    internal class StringScanner
    {
        private string _data;

        public StringScanner(string data)
        {
            this._data = data;
        }
    }
}
