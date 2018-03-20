using Fabu.Wiktionary.Graph;
using System;
using System.Diagnostics;
using WikimediaProcessing;


namespace Fabu.Wiktionary
{
    class WiktionaryAnalyzer
    {
        private readonly IWiktionaryPageProcessor _processor;
        private readonly Wikimedia _dump;
        private readonly dynamic _completionArgs;

        public event EventHandler<WikimediaPageProcessedEventArgs> PageProcessed;

        public WiktionaryAnalyzer(IWiktionaryPageProcessor processor, Wikimedia dump, dynamic completionArgs = null)
        {
            _processor = processor;
            _dump = dump;
            _completionArgs = completionArgs;
        }

        public IWiktionaryPageProcessor Compute()
        {
            var counter = 0;
            foreach (var article in _dump.Articles)
            {
                _processor.AddPage(article);
                counter++;

                var args = new WikimediaPageProcessedEventArgs { Index = counter, PageProcessed = article };

                PageProcessed?.Invoke(this, args);

                if(args.Abort)
                    break;
            }
            _processor.Complete(_completionArgs ?? new { });
            return _processor;
        }
    }

    internal class WikimediaPageProcessedEventArgs : EventArgs
    {
        public int Index { get; set; }
        public WikimediaPage PageProcessed { get; set; }
        public bool Abort { get; set; }
    }
}
