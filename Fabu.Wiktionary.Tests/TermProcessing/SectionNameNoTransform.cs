using Fabu.Wiktionary.Transform;

namespace Fabu.Wiktionary.Tests.TermProcessing
{
    public class SectionNameNoTransform : SectionNameTransform
    {
        public override SectionName Apply(SectionName sectionName)
        {
            if (sectionName.Name == "English")
                sectionName.IsLanguage = true;

            return sectionName;
        }
    }
}
