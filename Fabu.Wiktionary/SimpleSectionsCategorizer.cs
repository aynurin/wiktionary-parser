using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary
{
    public class SimpleSectionsCategorizer
    {
        public List<SectionName> CategorizeSections(List<SectionName> sections)
        {
            sections.ForEach(CleanupSectionParents);

            // identify groups avoiding vague clusters
            sections.ForEach(section =>
            {
                if (section.Category == null && section.Parents.Count < 5)
                    section.Category = ClusterNamePrefix + String.Join('_', section.Parents.Keys.Take(5));
            });

            // remove small groups
            var smallGroups = sections
                .GroupBy(s => s.Category)
                .Where(g => g.Count() < 5)
                .Select(g => g.Key)
                .ToArray();
            var smallGroupSections = sections.Where(s => smallGroups.Contains(s.Category));
            foreach (var section in smallGroupSections)
                section.Category = null;

            return sections;
        }

        public void CleanupSectionParents(SectionName section)
        {
            double full = section.Parents.Sum(p => p.Value);
            // normalize section parents
            section.Parents = new Stats<string>(
                section.Parents
                .Select(kvp => new KeyValuePair<string, double>(kvp.Key, kvp.Value / full))
                .Where(kvp => kvp.Value > 0.01)
                .OrderByDescending(kvp => kvp.Value));
        }

        #region Hardcoded sections categories

        public const string ClusterNamePrefix = "CLUSTER_";
        public const string LojbanCluster = ClusterNamePrefix + "LOJBAN";
        public const string PosCluster = ClusterNamePrefix + "POS";
        public const string LinksCluster = ClusterNamePrefix + "LINKS";
        public const string NotesCluster = ClusterNamePrefix + "NOTES";
        public const string MainCluster = ClusterNamePrefix + "MAIN";
        public const string RelationsCluster = ClusterNamePrefix + "REL";
        public const string LanguagesCluster = ClusterNamePrefix + "LANG";

        public const string LanguageSectionName = "LANG";
        public const string RootSectionName = "PAGE";
        public const string OtherSectionName = "X-OTHER";

        private readonly Dictionary<string, string> _staticSectionCategories = new Dictionary<string, string>()
        {
            { "cmavo", LojbanCluster },
            { "gismu", LojbanCluster },
            { "rafsi", LojbanCluster },
            { "lujvo", LojbanCluster },
            { "brivla", LojbanCluster },
            { "cmevla", LojbanCluster },
            {"adjectival noun", PosCluster },
            {"adjective", PosCluster },
            {"adjective suffix", PosCluster },
            {"adverb", PosCluster },
            {"article", PosCluster },
            {"character", PosCluster },
            {"conjunction", PosCluster },
            {"determiner", PosCluster },
            {"determiner and pronoun", PosCluster },
            {"idiom", PosCluster },
            {"initialism", PosCluster },
            {"interjection", PosCluster },
            {"letter", PosCluster },
            {"noun", PosCluster },
            {"numeral", PosCluster },
            {"phrase", PosCluster },
            {"postposition", PosCluster },
            {"prefix", PosCluster },
            {"preposition", PosCluster },
            {"preverb", PosCluster },
            {"pronoun", PosCluster },
            {"proper noun", PosCluster },
            {"proverb", PosCluster },
            {"syllable", PosCluster },
            {"symbol", PosCluster },
            {"verb", PosCluster },
            {"verb suffix", PosCluster },
            {"external links", LinksCluster },
            {"external sources", LinksCluster },
            {"further notes", LinksCluster },
            {"further reading", LinksCluster },
            {"note", LinksCluster },
            {"notes", LinksCluster },
            {"references", LinksCluster },
            {"see also", LinksCluster },
            {"note on morphophonemics", NotesCluster },
            {"usage notes", NotesCluster },
            {"definitions", MainCluster },
            {"etymology", MainCluster },
            {"examples", MainCluster },
            {"pronunciation", MainCluster },
            {"related", MainCluster },
            {"abbreviations", RelationsCluster },
            {"alternative forms", RelationsCluster },
            {"alternative terms", RelationsCluster },
            {"alternate spelling", RelationsCluster },
            {"anagrams", RelationsCluster },
            {"antonyms", RelationsCluster },
            {"coordinate terms", RelationsCluster },
            {"derived terms", RelationsCluster },
            {"related words", RelationsCluster },
            {"descendents", RelationsCluster },
            {"holonyms", RelationsCluster },
            {"homophones", RelationsCluster },
            {"hypernyms", RelationsCluster },
            {"hyponyms", RelationsCluster },
            {"meronyms", RelationsCluster },
            {"more usual form", RelationsCluster },
            {"related terms", RelationsCluster },
            {"synonyms", RelationsCluster },
            {"translations", RelationsCluster },
            {"troponyms", RelationsCluster }
        };

        private string GetStaticSectionCategory(string sectionName)
        {
            if (_staticSectionCategories.TryGetValue(sectionName.ToLowerInvariant(), out string category))
                return category;
            return null;
        }

        #endregion
    }
}
