using System;
using System.Collections.Generic;
using System.Text;

namespace Fabu.Wiktionary.Transform
{
    internal class TrimSectionNames : BaseTransform<string, string>
    {
        public override string Apply(string obj)
        {
            return obj?.Trim();
        }
    }
}
