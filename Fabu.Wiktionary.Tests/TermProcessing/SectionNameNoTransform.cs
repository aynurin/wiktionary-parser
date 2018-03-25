using Fabu.Wiktionary.Transform;

namespace Fabu.Wiktionary.Tests.TermProcessing
{
    public class SectionNameNoTransform : SectionNameTransform
    {
        public override SectionName Apply(SectionName sectionName)
        {
            if (sectionName.Name == "English" || sectionName.Name == "Old German")
                sectionName.IsLanguage = true;

            return sectionName;
        }
    }
}
