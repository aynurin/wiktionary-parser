using CommandLine;
using Fabu.Wiktionary.Commands;
using System;
using System.Diagnostics;

namespace Fabu.Wiktionary
{
    public static class Program
    {
        private static readonly Stopwatch _runTimer = new Stopwatch();
        private static long _lastFlush = 0;

        static int Main(string[] args)
        {
            _runTimer.Start();
            return Parser.Default.ParseArguments<
                LanguagesCommand.Args,
                AllSectionsCommand.Args,
                StandardSectionsCommand.Args,
                SectionGraphCommand.Args>(args)
              .MapResult(
                    (LanguagesCommand.Args opts) => new LanguagesCommand().Run(opts, Progress),
                    (AllSectionsCommand.Args opts) => new AllSectionsCommand().Run(opts, Progress),
                    (StandardSectionsCommand.Args opts) => new StandardSectionsCommand().Run(opts, Progress),
                    (SectionGraphCommand.Args opts) => new SectionGraphCommand().Run(opts, Progress),
                    errs => 1);
        }

        /// <summary>
        /// Shows processed pages and returns false if the processing needs to stop
        /// </summary>
        /// <remarks>
        /// This is a mess, but this mess better be here than in the Analyzer
        /// </remarks>
        /// <param name="pagesProcessed">Number of pages processed</param>
        /// <param name="commandArgs">Arguments for the current command</param>
        /// <returns><code>true</code> if the processing needs to stop, otherwise <code>false</code></returns>
        private static bool Progress(int pagesProcessed, BaseArgs commandArgs)
        {
            if (_lastFlush + 1000000 < _runTimer.ElapsedTicks)
            {
                var speed = pagesProcessed / _runTimer.Elapsed.TotalSeconds;
                Console.Write($"{pagesProcessed} articles processed at {speed:F2}\r");
                _lastFlush = _runTimer.ElapsedTicks;
            }

            if (commandArgs.LimitPages >= 0 && pagesProcessed >= commandArgs.LimitPages)
            {
                var speed = pagesProcessed / _runTimer.Elapsed.TotalSeconds;
                Console.Write($"{pagesProcessed} articles processed at {speed:F2}\r");
                return true;
            }

            return false;
        }
    }
}