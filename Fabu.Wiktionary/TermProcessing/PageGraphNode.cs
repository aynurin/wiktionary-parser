using Fabu.Wiktionary.TextConverters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.TermProcessing
{
    public class PageGraphNode
    {
        private readonly PageGraphNode _parent;
        private readonly List<PageGraphNode> _children = new List<PageGraphNode>();
        private readonly string[] _allowedMembers;
        protected readonly List<Term> _createdTerms;

        private Term _term;

        private bool _isTermUpdated = false;
        private bool _hasDefinedATerm = false;

        private bool _canDefineTerm;

        public readonly string ItemTitle;
        public readonly string OwnerPageTitle;
        public readonly string RelatedSectionContent;
        public readonly bool IsLanguage;

        public IEnumerable<PageGraphNode> Children => _children;

        public bool IsTermDefined { get => _term?.Status == Term.TermStatus.Defined; }

        public string Language { get; private set; }

        public IEnumerable<Term> AllItems => _createdTerms;
        public IEnumerable<Term> DefinedTerms => _createdTerms.Where(_ => _.Status == Term.TermStatus.Defined && !_.IsEmpty);

        public bool CanDefineTerm { get => _canDefineTerm; set => _canDefineTerm = value; }

        public static PageGraphNode CreateRoot(string pageTitle) => new PageGraphNode("PAGE", null, pageTitle, null, false, false, null, new List<Term>());

        internal List<Term> GetItems(Term.TermStatus defined) => _createdTerms.Where(i => i.Status == Term.TermStatus.Defined).ToList();

        public PageGraphNode CreateChild(string title, string sectionContent, bool isLanguage, bool canDefineTerm, string[] allowedMembers) => 
            new PageGraphNode(title, this, OwnerPageTitle, sectionContent, isLanguage, canDefineTerm, allowedMembers, _createdTerms);

        private PageGraphNode(string title, 
            PageGraphNode parent, string pageTitle, string sectionContent, 
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
            _term = new Term(OwnerPageTitle, RelatedSectionContent);
        }

        private void ForEachChild(PageGraphNode parent, Action<PageGraphNode, PageGraphNode> p)
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

            _term.SetProperty(ItemTitle, RelatedSectionContent);
            _isTermUpdated = true;
        }

        /// <summary>
        /// A term must always be defined on this node, all its children, on all its siblings that have a different names, and all their children, recursively.
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
            if (_term.Status == Term.TermStatus.Finalized)
                throw new InvalidOperationException("A term has already been created");
            if (_hasDefinedATerm)
                throw new InvalidOperationException("This item has already defined a term");
            if (!CanDefineTerm)
                throw new InvalidOperationException("This item cannot define a term");

            // If this node can define a term, we need to:
            _term.Status = Term.TermStatus.Void; // trash the already defined term
            _term = _term.CloneTerm(); // but inherit all its properties
            _term.Language = Language;
            _isTermUpdated = false; // the newly created term was not updated yet, so state that.
            _createdTerms.Add(_term); // and store the new term

            if (_parent != null && !IsLanguage) // languages are all twins
            {
                foreach (var sibling in _parent._children)
                {
                    if (sibling != this && sibling.ItemTitle != this.ItemTitle) // not twins
                    {
                        sibling._term = _term; // inherit this term
                        ForEachChild(sibling, (p, nephew) => nephew._term = _term);
                    }
                }
            }
            ForEachChild(this, (p, child) => child._term = _term); // children inherit this term
            _term.Status = Term.TermStatus.Defined; // redefine the term
            _hasDefinedATerm = true;

            if (!IsLanguage) // Language sections do not set their values to terms as they cannot define terms
                UpdateTerm(); // save content of this node to the defined term
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

        internal void UpdateMember(PageGraphNode child)
        {
            _term[ItemTitle].SetProperty(child.ItemTitle, child.RelatedSectionContent);
        }

        internal bool AllowsMember(PageGraphNode item) => 
            _allowedMembers != null && Array.BinarySearch(_allowedMembers, item.ItemTitle) >= 0;

        public void AddChild(PageGraphNode item)
        {
            _children.Add(item);
        }

        public override string ToString()
        {
            return ItemTitle;
        }
    }
}