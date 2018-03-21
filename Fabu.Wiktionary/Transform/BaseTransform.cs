namespace Fabu.Wiktionary.Transform
{
    public abstract class BaseSectionNameTransform<TName>
        where TName: SectionName
    {
        public abstract TName Apply(TName sectionName);
    }

    public abstract class SectionNameTransform : BaseSectionNameTransform<SectionName>
    {
    }
}
