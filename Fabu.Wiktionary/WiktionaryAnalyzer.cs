using Fabu.Wiktionary.Graph;
using System;
using System.Diagnostics;
using WikimediaProcessing;


namespace Fabu.Wiktionary
{
    class WiktionaryAnalyzer
    {
        private readonly GraphBuilder _graphBuilder;
        private readonly Wikimedia _dump;
        private readonly int _minimumEdgeFrequency;

        public event EventHandler<WikimediaPageProcessedEventArgs> PageProcessed;

        public WiktionaryAnalyzer(GraphBuilder builder, Wikimedia dump, int minimumAllowedEdgeFrequency = 1)
        {
            _graphBuilder = builder;
            _dump = dump;
            _minimumEdgeFrequency = minimumAllowedEdgeFrequency;
        }

        public GraphBuilder Compute()
        {
            var counter = 0;
            foreach (var article in _dump.Articles)
            {
                _graphBuilder.AddPage(article);
                counter++;

                var args = new WikimediaPageProcessedEventArgs { Index = counter, PageProcessed = article };

                PageProcessed?.Invoke(this, args);

                if(args.Abort)
                    break;
            }
            _graphBuilder.RemoveEdges(_minimumEdgeFrequency);
            return _graphBuilder;
        }
    }

    internal class WikimediaPageProcessedEventArgs : EventArgs
    {
        public int Index { get; set; }
        public WikimediaPage PageProcessed { get; set; }
        public bool Abort { get; set; }
    }
}
