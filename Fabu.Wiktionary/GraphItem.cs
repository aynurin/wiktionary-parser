using System;
using System.Collections.Generic;
using System.Linq;
using WikimediaProcessing;

namespace Fabu.Wiktionary
{
    public class GraphItem
    {
        private readonly GraphItem _parent;
        private readonly List<GraphItem> _children = new List<GraphItem>();
        private readonly string[] _allowedMembers;

        private Term _term;

        private bool _isTermCreated = false;
        private bool _isTermUpdated = false;
        private bool _isTermDefined = false;
        private bool _hasDefinedATerm = false;

        public readonly string ItemTitle;
        public readonly WikimediaPage OwnerPage;
        public readonly WikiSection RelatedSection;
        public readonly bool IsLanguage;
        public readonly bool CanDefineTerm;

        public IEnumerable<GraphItem> Children => _children;

        public bool IsTermDefined { get => _isTermDefined; }

        public string Language { get; private set; }

        public static GraphItem CreateRoot(WikimediaPage page) => new GraphItem("PAGE", null, page, null, false, false, null);

        public GraphItem(string title, GraphItem parent, WikimediaPage page, WikiSection section, bool isLanguage, bool canDefineTerm, string[] allowedMembers)
        {
            ItemTitle = title;
            _parent = parent;
            OwnerPage = page;
            RelatedSection = section;
            IsLanguage = isLanguage;
            CanDefineTerm = canDefineTerm;
            _allowedMembers = allowedMembers;
        }

        private void ForEachChild(GraphItem parent, Action<GraphItem, GraphItem> p)
        {
            foreach (var child in parent._children)
            {
                p(parent, child);
                ForEachChild(child, p);
            }
        }

        internal void SetLanguage()
        {
            ForEachChild(this, (parent, child) => child.Language = ItemTitle);
        }

        internal void UpdateTerm()
        {
            if (_isTermCreated)
                throw new InvalidOperationException("A term has already been created");
            if (_isTermUpdated)
                throw new InvalidOperationException("A term has already been updated");
            if (!IsTermDefined)
                throw new InvalidOperationException("A term has not yet been defined");

            AddMember(_term, this);
            _isTermUpdated = true;
        }

        /// <summary>
        /// A term is always defined on this child, on all its siblings that have a different names, and all its children
        /// </summary>
        internal void DefineTerm()
        {
            if (_isTermCreated)
                throw new InvalidOperationException("A term has already been created");
            if (_hasDefinedATerm)
                throw new InvalidOperationException("This item has already defined a term");
            if (!CanDefineTerm)
                throw new InvalidOperationException("This item cannot define a term");

            Term term;
            if (_isTermDefined)
            {
                // we will need to redefine the term
                term = _term.Clone();
            }
            else
            {
                term = new Term();
            }

            if (_parent != null)
            {
                foreach (var sibling in _parent._children)
                {
                    if (sibling != this && sibling.ItemTitle != this.ItemTitle)
                    {
                        DefineTerm(sibling, term);
                        ForEachChild(sibling, (p, nephew) => DefineTerm(nephew, term));
                    }
                }
            }
            ForEachChild(this, (p, child) => DefineTerm(child, term));
            DefineTerm(this, term);
            _hasDefinedATerm = true;
            AddMember(_term, this);
        }

        private void DefineTerm(GraphItem item, Term term)
        {
            item._term = term;
            item._isTermDefined = true;
        }

        internal Term CreateTerm()
        {
            if (!IsTermDefined)
                throw new InvalidOperationException("A term has not yet been defined");

            _isTermCreated = true;

            return _term;
        }

        internal void UpdateMember(GraphItem child)
        {
            AddMember(_term.Properties[ItemTitle], child);
        }

        internal bool AllowsMember(GraphItem item) => 
            _allowedMembers != null && Array.BinarySearch(_allowedMembers, item.ItemTitle) >= 0;

        private void AddMember(Term term, GraphItem graphItem)
        {
            term.SetProperty(graphItem.ItemTitle, graphItem.RelatedSection.Content);
        }

        internal void AddChild(GraphItem item)
        {
            _children.Add(item);
        }

        public override string ToString()
        {
            return ItemTitle;
        }
    }

    public class Term : ICloneable
    {
        public Dictionary<string, Term> Properties { get; private set; } = new Dictionary<string, Term>();
        public string Content { get; private set; }

        public void SetProperty(string key, string content)
        {
            Properties[key] = new Term() { Content = content };
        }

        internal Term Clone()
        {
            return ((ICloneable)this).Clone() as Term;
        }

        object ICloneable.Clone()
        {
            return new Term
            {
                Content = Content,
                Properties = new Dictionary<string, Term>(
                    Properties.Select(kvp => 
                        new KeyValuePair<string, Term>(kvp.Key, kvp.Value.Clone())))
            };
        }
    }
}