using System;
using System.Linq;
using System.Collections.Generic;
using Fabu.Wiktionary.TextConverters;
using Newtonsoft.Json;

namespace Fabu.Wiktionary.TermProcessing
{
    public class TermProperty : Dictionary<string, TermProperty>
    {
        public string Title { get; internal set; }
        public string Content { get; internal set; }

        [JsonIgnore]
        public Dictionary<string, TermProperty> Properties => this;

        public bool IsEmpty { get => String.IsNullOrWhiteSpace(Content) && this.Count == 0; }

        public TermProperty(string title, string content)
        {
            Title = title;
            Content = content;
        }

        public TermProperty(string title, string content, IEnumerable<KeyValuePair<string,TermProperty>> values)
            : base (values)
        {
            Title = title;
            Content = content;
        }

        public void SetProperty(string key, string content)
        {
            this[key] = new TermProperty(key, content);
        }

        private TermProperty CloneProperty() => 
            new TermProperty(Title, Content, InternalCloneProperties());

        protected IEnumerable<KeyValuePair<string, TermProperty>> InternalCloneProperties() => 
            this.Select(_ => new KeyValuePair<string, TermProperty>(_.Key, _.Value.CloneProperty()));

        public override string ToString()
        {
            return Title;
        }
    }
    public class Term : TermProperty
    {
        public string Language { get; set; }
        public TermStatus Status { get; set; }

        public Term(string title, string content) : base(title, content)
        {
        }

        public Term(string title, string content, string language, TermStatus status, IEnumerable<KeyValuePair<string, TermProperty>> values) 
            : base(title, content, values)
        {
            Language = language;
            Status = status;
        }

        public Term CloneTerm() => new Term(Title, Content, Language, Status, InternalCloneProperties());

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