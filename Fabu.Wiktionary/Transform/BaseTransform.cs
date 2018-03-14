namespace Fabu.Wiktionary.Transform
{
    internal abstract class BaseSectionNameTransform<TName>
        where TName: SectionName
    {
        public abstract TName Apply(TName sectionName);
    }

    internal abstract class SectionNameTransform : BaseSectionNameTransform<SectionName>
    {
    }
}
