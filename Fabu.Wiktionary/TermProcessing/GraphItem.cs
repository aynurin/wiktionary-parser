using Fabu.Wiktionary.TextConverters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.TermProcessing
{
    public class GraphItem
    {
        private readonly GraphItem _parent;
        private readonly List<GraphItem> _children = new List<GraphItem>();
        private readonly string[] _allowedMembers;
        protected readonly List<Term> _createdTerms;

        private Term _term;

        private bool _isTermUpdated = false;
        private bool _hasDefinedATerm = false;

        private bool _canDefineTerm;

        public readonly string ItemTitle;
        public readonly string OwnerPageTitle;
        public readonly FormattedString RelatedSectionContent;
        public readonly bool IsLanguage;

        public IEnumerable<GraphItem> Children => _children;

        public bool IsTermDefined { get => _term?.Status == Term.TermStatus.Defined; }

        public string Language { get; private set; }

        public IEnumerable<Term> FinalizedTerms => _createdTerms.Where(t => t.Status == Term.TermStatus.Finalized);
        public IEnumerable<Term> AllItems => _createdTerms;

        public bool CanDefineTerm { get => _canDefineTerm; set => _canDefineTerm = value; }

        public static GraphItem CreateRoot(string pageTitle) => new GraphItem("PAGE", null, pageTitle, null, false, false, null, new List<Term>());

        internal List<Term> GetItems(Term.TermStatus defined) => _createdTerms.Where(i => i.Status == Term.TermStatus.Defined).ToList();

        public GraphItem CreateChild(string title, FormattedString sectionContent, bool isLanguage, bool canDefineTerm, string[] allowedMembers) => 
            new GraphItem(title, this, OwnerPageTitle, sectionContent, isLanguage, canDefineTerm, allowedMembers, _createdTerms);

        private GraphItem(string title, 
            GraphItem parent, string pageTitle, FormattedString sectionContent, 
            bool isLanguage, bool canDefineTerm, string[] allowedMembers,
            List<Term> termsStore)
        {
            ItemTitle = title;
            _parent = parent;
            OwnerPageTitle = pageTitle;
            RelatedSectionContent = sectionContent;
            IsLanguage = isLanguage;
            _canDefineTerm = canDefineTerm;
            _allowedMembers = allowedMembers;
            _createdTerms = termsStore;
            _term = new Term(OwnerPageTitle);
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
            Language = ItemTitle;
            ForEachChild(this, (parent, child) => child.Language = ItemTitle);
        }

        internal void UpdateTerm()
        {
            if (_term?.Status == Term.TermStatus.Finalized)
                throw new InvalidOperationException("A term has already been created");
            if (_isTermUpdated)
                throw new InvalidOperationException("A term has already been updated");

            AddMember(_term, this);
            _isTermUpdated = true;
        }

        /// <summary>
        /// A term is always defined on this node, all its children, on all its siblings that have a different names, and all their children, recursively.
        /// </summary>
        /// <remarks>
        /// This is because of the nature of how sections are structured on the page. In general, if a section can create a term, then this term relates
        /// to all neighbour and child sections, unless someone else wants to define a term in this tree.
        /// The exception is similarly named siblings, i.e. same name section titles on the same level within the same tree, e.g. 
        /// "Etymology 1", "Etymology 2", etc - they define different terms.
        /// The unsolved thing is when a page does not have any term definers, but does have POS sections. Probably has to be resolved in the second run,
        /// but not yet implemented.
        /// </remarks>
        internal void DefineTerm()
        {
            if (_term?.Status == Term.TermStatus.Finalized)
                throw new InvalidOperationException("A term has already been created");
            if (_hasDefinedATerm)
                throw new InvalidOperationException("This item has already defined a term");
            if (!CanDefineTerm)
                throw new InvalidOperationException("This item cannot define a term");
            
            // we will need to redefine the term
            // to do this, we must mark the original term as void,
            // so that duplicated terms are not created.
            // Only the finally defined and never voided terms 
            // should be saved.
            _term.Status = Term.TermStatus.Void;
            _term = _term.Clone();
            _term.Language = Language;
            _createdTerms.Add(_term);

            if (_parent != null && !IsLanguage)
            {
                foreach (var sibling in _parent._children)
                {
                    if (sibling != this && sibling.ItemTitle != this.ItemTitle)
                    {
                        sibling._term = _term;
                        ForEachChild(sibling, (p, nephew) => nephew._term = _term);
                    }
                }
            }
            ForEachChild(this, (p, child) => child._term = _term);
            _term.Status = Term.TermStatus.Defined;
            _hasDefinedATerm = true;

            if (!IsLanguage)
                AddMember(_term, this);
        }

        internal Term CreateTerm()
        {
            if (!IsTermDefined)
                throw new InvalidOperationException("A term has not yet been defined");

            _term.Status = Term.TermStatus.Finalized;

            if (_createdTerms.Contains(_term))
                throw new InvalidOperationException("This term has already been created");

            _createdTerms.Add(_term);

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
            term.SetProperty(graphItem.ItemTitle, graphItem.RelatedSectionContent?.ToHtml());
        }

        public void AddChild(GraphItem item)
        {
            _children.Add(item);
        }

        public override string ToString()
        {
            return ItemTitle;
        }
    }
}