using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fabu.Wiktionary
{
    public class SectionName
    {
        private SectionName _originalSection;

        public string Name { get; set; }
        public double Weight { get; set; }

        public string Category { get; set; }
        public bool IsLanguage { get; set; }

        public List<string> Spellings { get; set; } = new List<string>();

        public Stats<int> DepthStats { get; set; } = new Stats<int>();
        public Stats<string> Parents { get; set; } = new Stats<string>();
        public Stats<string> Children { get; set; } = new Stats<string>();

        public override string ToString()
        {
            return Name ?? base.ToString();
        }

        [JsonIgnore]
        public SectionName OriginalSection { get => _originalSection ?? this; }

        public SectionName CloneWithName(string newName)
        {
            return new SectionName
            {
                Name = newName,
                Weight = Weight,
                Spellings = new List<string>(Spellings.ToArray()),
                Category = Category,
                _originalSection = OriginalSection ?? this
            };
        }

        internal void AddSpelling(string name, double weight)
        {
            if (!Spellings.Contains(name))
                Spellings.Add(name);
            Weight += weight;
        }
    }

    public class SectionNameComparer : IComparer<SectionName>
    {
        public int Compare(SectionName x, SectionName y) => x.Weight.CompareTo(y.Weight);
    }

    public class Examples : Dictionary<string,List<string>>
    {
        public void Add(string key, string example)
        {
            if (this.ContainsKey(key))
                this[key].Add(example);
            else this.Add(key, new List<string>() { example });
        }
    }

    public class Stats<TKey> : Dictionary<TKey, double>
    {
        public Stats(IEnumerable<KeyValuePair<TKey, double>> initialData)
            : base(initialData)
        {
        }
        public Stats()
        {
        }

        public void Add(TKey key)
        {
            if (this.ContainsKey(key))
                this[key] += 1;
            else this.Add(key, 1);
        }

        /// <summary>
        /// Normalizes weights of values and takes all values that are mentioned 
        /// more frequently than the <paramref name="freqToCutOff"/>.
        /// </summary>
        /// <param name="freqToCutOff">Min frequesy to keep</param>
        /// <returns>New <see cref="Stats&lt;TKey&gt;"/> containing the requested values</returns>
        public Stats<TKey> CutOff(double freqToCutOff)
        {
            var full = this.Sum(p => p.Value);
            var cut = this.Where(kvp => (kvp.Value / full) >= freqToCutOff)
                .OrderByDescending(kvp => kvp.Value);

            var newStats = new Stats<TKey>(cut);

            var left = this.Where(kvp => (kvp.Value / full) < freqToCutOff).Sum(kvp => kvp.Value);
            if (typeof(TKey) == typeof(String) && left > 0)
                newStats.Add((TKey)Convert.ChangeType(SimpleSectionsCategorizer.OtherSectionName, typeof(TKey)), left);

            return newStats;
        }
    }
}
