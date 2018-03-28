using System.Collections.Generic;
using System.Linq;
using WikimediaProcessing;

namespace Fabu.Wiktionary.TermProcessing
{
    public interface ISectionAccessor
    {
        string GetSectionName();

        string GetRawContent();

        IEnumerable<ISectionAccessor> GetSubSections();
    }

    internal class WikiSectionAccessor : ISectionAccessor
    {
        private readonly WikiSection _section;

        public WikiSectionAccessor(WikiSection section)
        {
            _section = section;
        }

        public string GetRawContent() => _section.Content;

        public string GetSectionName() => _section.SectionName;

        public IEnumerable<ISectionAccessor> GetSubSections() => _section.SubSections.Select(_ => new WikiSectionAccessor(_));
    }
}