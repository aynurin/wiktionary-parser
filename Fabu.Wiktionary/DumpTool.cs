using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using WikimediaProcessing;

namespace Fabu.Wiktionary
{
    static class DumpTool
    {
        public const string LanguagesDump = "languages";
        public const string SectionsDump = "sections";
        public const string SectionsDictDump = "sections_dict";
        public const string SectionsGraph = "wiktionary_graph";

        private readonly static JsonSerializer _jsonSerializer =
            JsonSerializer.Create(new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

        public static Wikimedia LoadWikimediaDump(string dir, string dumpFile)
        {
            dumpFile = Path.Combine(dir, dumpFile);
            if (!String.IsNullOrWhiteSpace(dumpFile) && File.Exists(dumpFile))
                return new Wikimedia(dumpFile);
            throw new InvalidOperationException("Does the dump file exist? " + dumpFile);
        }

        internal static void SaveDump<T>(string dumpDir, string dumpName, T data)
        {
            dumpDir = String.IsNullOrWhiteSpace(dumpDir) ? Environment.CurrentDirectory : dumpDir;
            dumpDir = Path.Combine(dumpDir, FixExtension(dumpName));
            using (var file = File.CreateText(dumpDir))
                _jsonSerializer.Serialize(file, data);
        }

        internal static T LoadDump<T>(string dumpDir, string dumpName)
        {
            dumpDir = String.IsNullOrWhiteSpace(dumpDir) ? Environment.CurrentDirectory : dumpDir;
            dumpDir = Path.Combine(dumpDir, dumpName);
            if (!File.Exists(dumpDir))
                dumpDir = FixExtension(dumpDir);
            using (var file = File.OpenText(dumpDir))
            using (var reader = new JsonTextReader(file))
                return _jsonSerializer.Deserialize<T>(reader);
        }

        private static string FixExtension(string fileName)
        {
            return Path.ChangeExtension(fileName, ".json");
        }
    }
}
