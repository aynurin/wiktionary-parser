using System;
using System.Collections.Generic;
using System.Text;

namespace Fabu.Wiktionary.Transform
{
    internal class TrimSectionNames : SectionNameTransform
    {
        public override SectionName Apply(SectionName sectionName)
        {
            return sectionName.CloneWithName(sectionName.Name.Trim());
        }
    }
}
