using CommandLine;
using DBSCAN;
using Fabu.Wiktionary.FuzzySearch;
using HdbscanSharp.Distance;
using HdbscanSharp.Runner;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.Commands
{
    /// <summary>
    /// The results of this research is that I couldn't get any clusters that would introuce any human-recognizable value.
    /// I still have a problem understanding Wiktionary page structure and getting an idea of how to interpret its sections.
    /// What I can do now is to avoid deeper nodes, or to avoid leafs of deeper nodes to see a clear and understandable graph.
    /// 
    /// My goal is to create a single structured way of represending Wiktionary content.
    /// In order to do that I need to have a programmable way to understand the structure of Wiktionary page. There is a 
    /// recommendation, but if a page doesn't follow the recommended structure, it doesn't mean this is page does not have 
    /// interesting content.
    /// 
    /// In order to resolve this, after normalizing section names I have a way to address and identify section names. 
    /// The next thing is to work out a way to put sections of any page into a universal object structure.
    /// 
    /// One way is to manually tag section names for whether it is a POS or a reference, or etc. This work cannot be repeated, 
    /// so I won't try it first.
    /// 
    /// I tried clustering based on the childred frequency, parents frequency, both children and parents frequency, based on 
    /// raw or normalized and rounded numbers. All these attempts created no meaningful clusters so I cannot build a clean 
    /// repeatable graph of sections.
    /// 
    /// Try throwing away all connections after the 2nd level of depth.
    /// </summary>
    [Obsolete("Attempted clustering strategies do not bring any goodness.")]
    internal class SectionsClusteringCommand : BaseCommand<SectionsClusteringCommand.Args>
    {
        [Verb("cluster", HelpText = "Extract section names")]
        public class Args : BaseArgs
        {
        }

        // cluster
        protected override void RunCommand(Args args, Func<int, BaseArgs, bool> onProgress)
        {
            var languages = LoadLanguages(args.DumpDir);
            var sections = DumpTool.LoadDump<List<SectionName>>(args.DumpDir, DumpTool.SectionsDictDump);
            sections.Add(new SectionName { Name = SimpleSectionsCategorizer.LanguageSectionName });

            var algo1 = new HDbScanClusterAlgorithm();
            algo1.AnnalyzeSections(sections, languages);

            //var algo2 = new DbScanClusterAlgorithm();
            //algo2.AnnalyzeSections(sections, languages);

            foreach (var section in sections)
            {
                Console.WriteLine(section.Name 
                    + "\t" + String.Join('\t', section.Parents.Select(p => $"{p.Key}:{p.Value:F2}")));
            }
        }

        private static IgnoreCaseSearch<SectionName> LoadLanguages(string dir)
        {
            var languageNames = DumpTool.LoadDump<List<SectionName>>(dir, DumpTool.LanguagesDump);
            var languageSearch = new IgnoreCaseSearch<SectionName>(languageNames, _ => _.Name, new SectionNameComparer());
            return languageSearch;
        }
    }

    class DbScanClusterAlgorithm : ClusterAlgorithm
    {
        public override void AnnalyzeSections(List<SectionName> sections, IFuzzySearcher<SectionName> languages)
        {
            Console.Write("Running DBSCAN...");
            var clusters = DBSCAN.DBSCAN.CalculateClusters(
                GetData(sections, languages),
                epsilon: 1.0,
                minimumPointsPerCluster: 4);
            Console.WriteLine("Done.");
        }

        private IList<IPointData> GetData(List<SectionName> sections, IFuzzySearcher<SectionName> languages)
        {
            // dunno how to convert a 417 point vector to 2-point IPointData :(
            throw new NotImplementedException();
        }
    }

    class HDbScanClusterAlgorithm : ClusterAlgorithm
    {
        public override void AnnalyzeSections(List<SectionName> sections, IFuzzySearcher<SectionName> languages)
        {
            var index = 0;
            var vectorSpace = new Dictionary<string, int>(
                sections.Select(s => new KeyValuePair<string, int>(s.Name, index++)));

            var vectorized = sections.Select(s => SectionToVector(s, vectorSpace, languages)).ToList();

            Optimize(vectorized, vectorSpace);

            Console.Write("Running HDBSCAN...");
            var result = HdbscanRunner.Run(new HdbscanParameters
            {
                DataSet = vectorized.Select(v => v.ToArray()).ToArray(),
                MinPoints = 2,
                MinClusterSize = 5,
                DistanceFunction = new CosineSimilarity() // See HdbscanSharp/Distance folder for more distance function
            });
            Console.WriteLine("Done.");

            var clusters = new List<SectionName>[result.Labels.Max() + 1];
            for (var idx = 0; idx < result.Labels.Length; idx++)
            {
                if (clusters[result.Labels[idx]] == null)
                    clusters[result.Labels[idx]] = new List<SectionName>();
                var cluster = clusters[result.Labels[idx]];
                cluster.Add(sections.Single(s => s.Name == vectorSpace.Single(kvp => kvp.Value == idx).Key));
            }
            Console.WriteLine(clusters.Count());
        }

        private void Optimize(List<List<double>> vectorized, Dictionary<string, int> space)
        {
            var indToRemove = new List<int>();
            foreach (var ind in space.Values)
            {
                bool removeThis = true;
                foreach (var vect in vectorized)
                {
                    if (vect[ind] != 0)
                    {
                        removeThis = false;
                        break;
                    }
                }
                if (removeThis)
                    indToRemove.Add(ind);
            }
            foreach (var ind in indToRemove)
            {
                foreach (var vector in vectorized)
                {
                    vector.RemoveAt(ind);
                }
            }
        }

        private List<double> SectionToVector(SectionName section, Dictionary<string, int> vectorSpace, IFuzzySearcher<SectionName> languages)
        {
            var numOfSections = vectorSpace.Count;
            var vectorSize = numOfSections * 2; // one for parents and one for children
            var vector = new List<double>(new double[vectorSize * 2]);
            var order = 0;
            var total = section.Parents.Sum(p => p.Value);
            foreach (var item in section.Parents)
                vector[(numOfSections * order) + vectorSpace[LangIfLanguage(item.Key, languages)]] = Math.Round(item.Value / total, 1);
            order++;
            //total = section.Children.Sum(p => p.Value);
            //foreach (var item in section.Children)
            //    vector[(numOfSections * order) + vectorSpace[LangIfLanguage(item.Key, languages)]] = Math.Round(item.Value / total, 1);
            return vector;
        }

        private string LangIfLanguage(string name, IFuzzySearcher<SectionName> languages)
        {
            if (languages.TryFindBest(name, out List<SectionName> result))
                return SimpleSectionsCategorizer.LanguageSectionName;
            return name;
        }
    }

    abstract class ClusterAlgorithm
    {
        public abstract void AnnalyzeSections(List<SectionName> sections, IFuzzySearcher<SectionName> languages);
    }
}
