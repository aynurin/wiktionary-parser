using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using Wordy.ClientServer.Model;

namespace Fabu.Wiktionary.ElasticHosted
{
    public class DocumentIndexer
    {
        public DocumentIndexer(IConfigurationRoot config, string domainName, int batchCount)
        {
            Config = config.GetSection("Elastic");

            var serviceUri = Config["WordIndexHost"];

            var node = new Uri(Config["WordIndexHost"]);
            var settings = new ConnectionSettings(node);
            Client = new ElasticClient(settings);
        }

        public IConfigurationSection Config { get; }
        public ElasticClient Client { get; }

        private readonly List<object> _buffer;

        public void Write(string docid, string index, ElasticDocument document)
        {
            document.Id = docid;
            var response = Client.Index((object)document, idx => idx.Index(index)); //or specify index via settings.DefaultIndex("mytweetindex");
            if (!response.IsValid)
                throw new InvalidOperationException(response.DebugInformation);
        }
    }

    [Nest.ElasticsearchType(IdProperty = "Id", Name = "word")]
    public class ElasticDocument : IndexedDocument
    {
    }
}
