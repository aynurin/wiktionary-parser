using Fabu.Wiktionary.FuzzySearch;
using Fabu.Wiktionary.Transform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using WikimediaProcessing;

namespace Fabu.Wiktionary
{
    static class DumpTool
    {
        public static Wikimedia LoadWikimediaDump(string dumpFile)
        {
            if (!String.IsNullOrWhiteSpace(dumpFile) && File.Exists(dumpFile))
                return new Wikimedia(dumpFile);
            throw new InvalidOperationException("Does the dump file exist? " + dumpFile);
        }

        public static List<LanguageWeight> LoadLanguages(string tsvFile)
        {
            return LoadTsv(tsvFile, line =>
                line.Length == 2 ?
                    new LanguageWeight(line[0], Double.Parse(line[1])) :
                    throw new InvalidOperationException("Expected two columns"));
        }

        public static List<SectionWeight> LoadSectionWeights(string tsvFile)
        {
            return LoadTsv(tsvFile, line =>
                line.Length == 2 ?
                    new SectionWeight(line[0], Double.Parse(line[1])) :
                    throw new InvalidOperationException("Expected two columns"));
        }

        public static List<SectionType> LoadSectionTypes(string tsvFile)
        {
            return LoadTsv(tsvFile, line =>
                line.Length == 2 ?
                    new SectionType(line[0], line[1]) :
                    throw new InvalidOperationException("Expected two columns"));
        }

        private static List<TVal> LoadTsv<TVal>(string tsvFile, Func<string[],TVal> converter)
        {
            var values = new List<TVal>(1000);
            using (var reader = File.OpenText(tsvFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var vals = line.Split('\t');
                    values.Add(converter(vals));
                }
            }
            return values;
        }
    }

    public class SectionWeight
    {
        public SectionWeight(string name, double weight)
        {
            Name = name;
            Weight = weight;
        }

        public string Name { get; set; }
        public double Weight { get; set; }
    }

    public class SectionWeightComparer : IComparer<SectionWeight>
    {
        public int Compare(SectionWeight x, SectionWeight y) => x.Weight.CompareTo(y.Weight);
    }

    public class LanguageWeight
    {
        public LanguageWeight(string name, double weight)
        {
            Name = name;
            Weight = weight;
        }

        public string Name { get; set; }
        public double Weight { get; set; }
    }

    public class LanguageWeightComparer : IComparer<LanguageWeight>
    {
        public int Compare(LanguageWeight x, LanguageWeight y) => x.Weight.CompareTo(y.Weight);
    }

    public class SectionType
    {
        public SectionType(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class SectionTypeComparer : IComparer<SectionType>
    {
        public int Compare(SectionType x, SectionType y) => x.Type.CompareTo(y.Type);
    }
}
