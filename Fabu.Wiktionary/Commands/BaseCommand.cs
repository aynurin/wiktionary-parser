using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;

namespace Fabu.Wiktionary.Commands
{
    internal abstract class BaseCommand<T>
        where T: BaseArgs
    {
        protected IConfigurationRoot Config { get; private set; }

        public int Run(IConfigurationRoot config, T args, Func<int, BaseArgs, bool> onProgress)
        {
            Config = config;
            RunCommand(args, onProgress);
            return 0;
        }

        protected abstract void RunCommand(T args, Func<int, BaseArgs, bool> onProgress);
    }
}
