using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fabu.Wiktionary.TextConverters
{
    public static class StripHtml
    {
        private class TagCoord
        {
            public TagCoord(int start, int end) { Start = start; End = end; }
            public int Start;
            public int End;
            public int Length => End - Start;
            public bool Includes(int index) => index >= Start && index <= End;
        }

        public static string Comments(string wikitext)
        {
            if (wikitext == null)
                return null;
            if (wikitext == String.Empty)
                return String.Empty;

            var ranges = new List<TagCoord>();
            int indexOfStart(int start) => wikitext.IndexOf("<!--", start + 1, StringComparison.InvariantCultureIgnoreCase);
            int indexOfEnd(int start) => wikitext.IndexOf("-->", start + 1, StringComparison.InvariantCultureIgnoreCase);
            int indexOfTagEnd(int start) => wikitext.IndexOf(">", start + 1, StringComparison.InvariantCultureIgnoreCase);
            var startIndex = -1;
            while ((startIndex = indexOfStart(startIndex)) >= 0)
            {
                var endIndex = 0;
                var lookStart = startIndex;
                endIndex = indexOfEnd(lookStart);
                if (endIndex == -1)
                    break;
                endIndex = indexOfTagEnd(endIndex);
                ranges.Add(new TagCoord(startIndex, endIndex));
            }

            if (ranges.Count == 0)
                return wikitext;
            
            var buffer = new StringBuilder(wikitext.Length);
            var lastIndex = 0;
            foreach (var range in ranges)
            {
                if (lastIndex > range.Start)
                    continue;
                buffer.Append(wikitext.Substring(lastIndex, range.Start - lastIndex));
                lastIndex = range.End + 1;
            }
            buffer.Append(wikitext.Substring(lastIndex));
            return buffer.ToString();
        }

        public static string Tables(string wikitext)
        {
            if (wikitext == null)
                return null;
            if (wikitext == String.Empty)
                return String.Empty;

            var ranges = new List<TagCoord>();
            int indexOfStart(int start) => wikitext.LastIndexOf("<table", start, StringComparison.InvariantCultureIgnoreCase);
            int indexOfEnd(int start) => wikitext.IndexOf("</table", start + 1, StringComparison.InvariantCultureIgnoreCase);
            int indexOfTagEnd(int start) => wikitext.IndexOf(">", start + 1, StringComparison.InvariantCultureIgnoreCase);
            var startIndex = wikitext.Length-1;
            while ((startIndex = indexOfStart(startIndex)) >= 0)
            {
                var endIndex = 0;
                var lookStart = startIndex;
                while (true)
                {
                    endIndex = indexOfEnd(lookStart);
                    if (endIndex == -1)
                        break;
                    if (ranges.Any(r => r.Includes(endIndex)))
                    {
                        lookStart = endIndex + 1;
                        continue;
                    }
                    break;
                }
                if (endIndex == -1)
                    break;
                endIndex = indexOfTagEnd(endIndex);
                ranges.Add(new TagCoord(startIndex, endIndex));
            }

            if (ranges.Count == 0)
                return wikitext;

            ranges.Sort((r1, r2) => r1.Start - r2.Start);
            var buffer = new StringBuilder(wikitext.Length);
            var lastIndex = 0;
            foreach(var range in ranges)
            {
                if (lastIndex > range.Start)
                    continue;
                buffer.Append(wikitext.Substring(lastIndex, range.Start - lastIndex));
                lastIndex = range.End + 1;
            }
            buffer.Append(wikitext.Substring(lastIndex));
            return buffer.ToString();
        }
    }
}
