using WikimediaProcessing;

namespace Fabu.Wiktionary
{
    public interface IWiktionaryPageProcessor
    {
        void AddPage(WikimediaPage page);
        void Complete(dynamic completionArgs);
    }
}