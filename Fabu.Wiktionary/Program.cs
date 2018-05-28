using CommandLine;
using Fabu.Wiktionary.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;

namespace Fabu.Wiktionary
{
    public static class Program
    {
        private static readonly Stopwatch _runTimer = new Stopwatch();
        private static long _lastFlush = 0;

        static int Main(string[] args)
        {
            _runTimer.Start();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json");
            var config = builder.Build();

            return Parser.Default.ParseArguments<
                PrepDictsCommand.Args,
                StandardSectionsCommand.Args,
                ExtractTermsCommand.Args,
                CreoleCommand.Args,
                SectionGraphCommand.Args>(args)
              .MapResult(
                    (PrepDictsCommand.Args opts) => new PrepDictsCommand().Run(config, opts, Progress),
                    (StandardSectionsCommand.Args opts) => new StandardSectionsCommand().Run(config, opts, Progress),
                    (ExtractTermsCommand.Args opts) => new ExtractTermsCommand().Run(config, opts, Progress),
                    (CreoleCommand.Args opts) => new CreoleCommand().Run(config, opts, Progress),
                    (SectionGraphCommand.Args opts) => new SectionGraphCommand().Run(config, opts, Progress),
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