using System;
using System.Collections.Generic;
using System.Text;

namespace Fabu.Wiktionary.Transform
{
    internal abstract class BaseTransform<TSrc,TOut>
    {
        public abstract TOut Apply(TSrc value);
    }
}
