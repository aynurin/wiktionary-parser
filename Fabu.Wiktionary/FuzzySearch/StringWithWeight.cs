using System.Collections.Generic;

namespace Fabu.Wiktionary.FuzzySearch
{
    public class StringWithWeight
    {
        public StringWithWeight(string data, double value)
        {
            Data = data;
            Value = value;
        }

        public string Data { get; set; }
        public double Value { get; set; }
    }
}
