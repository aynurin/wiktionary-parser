using Amazon.CloudSearchDomain;
using Amazon.CloudSearchDomain.Model;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Fabu.Wiktionary.AWS.CloudSearch
{
    public class DocumentIndexer : IDisposable
    {
        public DocumentIndexer(IConfigurationRoot config, string domainName, int batchCount)
        {
            _buffer = new List<dynamic>(batchCount);

            Config = config;

            var serviceUri = config.GetSection("WordIndex")["DocumentEndpoint"];
            var awsProfileName = config.GetSection("AWS")["Profile"];

            var sharedCredentialsFile = new SharedCredentialsFile();
            if (!sharedCredentialsFile.TryGetProfile(awsProfileName, out CredentialProfile profile))
                throw new InvalidOperationException($"Could not load AWS Credentials '{awsProfileName}' from a shared file");
            var credentials = profile.GetAWSCredentials(sharedCredentialsFile);

            CloudSearchDomain = new AmazonCloudSearchDomainClient(serviceUri, credentials);
        }

        public IConfigurationRoot Config { get; }
        public AWSOptions AwsConfig { get; }
        public IAmazonCloudSearchDomain CloudSearchDomain { get; }

        private readonly List<dynamic> _buffer;
        private readonly MD5 _md5 = MD5.Create();

        public void Dispose()
        {
            FlushBuffer();
        }

        public void Write(string title, string body, string language)
        {
            if (_buffer.Capacity == _buffer.Count)
            {
                FlushBuffer();
            }
            else
            {
                _buffer.Add(new { type = "add", id = GetDocId(language, title), fields = new { body, doctype = "word", language, word = title } });
            }
        }

        public string GetDocId(string language, string title)
        {
            var inputBytes = System.Text.Encoding.ASCII.GetBytes($"{language}_{title}");
            var hash = new Guid(_md5.ComputeHash(inputBytes));
            return hash.ToString("n");
        }

        public void FlushBuffer()
        {
            if (_buffer.Count == 0)
                return;

            var serializer = JsonSerializer.Create();
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                serializer.Serialize(writer, _buffer);
                _buffer.Clear();
                writer.Flush();
                stream.Position = 0;
                var request = new UploadDocumentsRequest
                {
                    ContentType = "application/json",
                    Documents = stream
                };
                var awaiter = CloudSearchDomain.UploadDocumentsAsync(request);
                awaiter.Wait();
            }
        }
    }
}
