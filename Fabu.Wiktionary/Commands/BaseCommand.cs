using System;
using System.Diagnostics;

namespace Fabu.Wiktionary.Commands
{
    internal abstract class BaseCommand<T>
        where T: BaseArgs
    {
        public int Run(T args, Func<int, BaseArgs, bool> onProgress)
        {
            RunCommand(args, onProgress);
            return 0;
        }

        protected abstract void RunCommand(T args, Func<int, BaseArgs, bool> onProgress);
    }
}
