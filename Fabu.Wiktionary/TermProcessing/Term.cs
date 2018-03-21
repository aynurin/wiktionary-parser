using System;
using System.Linq;
using System.Collections.Generic;

namespace Fabu.Wiktionary.TermProcessing
{
    public class Term : ICloneable
    {
        public string Title { get; set; }
        public Dictionary<string, Term> Properties { get; private set; } = new Dictionary<string, Term>();
        public string Content { get; private set; }

        public TermStatus Status { get; set; }

        public Term(string title)
        {
            Title = title;
        }

        public void SetProperty(string key, string content)
        {
            Properties[key] = new Term(key) { Content = content };
        }

        internal Term Clone()
        {
            return ((ICloneable)this).Clone() as Term;
        }

        object ICloneable.Clone()
        {
            return new Term(Title)
            {
                Content = Content,
                Properties = new Dictionary<string, Term>(
                    Properties.Select(kvp => 
                        new KeyValuePair<string, Term>(kvp.Key, kvp.Value.Clone()))),
                Status = TermStatus.None
            };
        }

        public override string ToString()
        {
            return Title;
        }

        public enum TermStatus
        {
            None,
            Void,
            Defined,
            Finalized
        }
    }
}