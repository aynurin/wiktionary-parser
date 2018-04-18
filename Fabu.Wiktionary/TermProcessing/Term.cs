using System;
using System.Linq;
using System.Collections.Generic;
using Fabu.Wiktionary.TextConverters;

namespace Fabu.Wiktionary.TermProcessing
{
    public class Term : ICloneable
    {
        public string Title { get; set; }
        public string Language { get; set; }
        public string Content { get; internal set; }

        public Dictionary<string, Term> Properties { get; private set; } = new Dictionary<string, Term>();
        public TermStatus Status { get; set; }
        public bool IsEmpty { get => String.IsNullOrWhiteSpace(Content) && Properties.Count == 0; }

        public Term(string title)
        {
            Title = title;
        }
        
        public void SetProperty(string key, string content)
        {
            Properties[key] = new Term(key) { Content = content };
        }

        public void ConvertContent(ContextArguments args, ITextConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException("converter");
            if (String.IsNullOrEmpty(Content))
                return;
            Content = converter.ConvertToStructured(args, Content).ToHtml();
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
            Finalized,
            Empty
        }
    }
}