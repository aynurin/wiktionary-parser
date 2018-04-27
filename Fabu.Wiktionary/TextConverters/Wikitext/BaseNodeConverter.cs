using Fabu.Wiktionary.TextConverters.Wiki.Templates;
using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    public class BaseNodeConverter
    {
        public readonly static Stats<string> ConvertedNodes = new Stats<string>();

        private readonly Type[] _okNodes = new Type[]
        {
            typeof(Wikitext),
            typeof(Run),
            typeof(TemplateArgument)
        };

        public virtual ConversionResult Convert(Node node, ConversionContext context)
        {
            if (!_okNodes.Any(type => type == node.GetType()))
            {
                Debugger.Break();
                ConvertedNodes.Add(node.GetType().Name);
            }
            var result = new ConversionResult();
            result.Write(node);
            return result;
        }

        public virtual string GetSubstitute(Node node) => null;

        protected Node MaybeARun(Wikitext content)
        {
            if (content.Lines.Count == 1)
            {
                var first = content.Lines.FirstNode;
                if (first is Paragraph)
                    return new Run(first.EnumChildren().Select(n => n as InlineNode));
            }
            return content;
        }
    }

    public class ConversionContext
    {
        public bool ItalicsSwitched { get; set; }
        public bool BoldSwitched { get; set; }

        public ContextArguments Arguments { get; private set; }
        public Dictionary<string, string> LanguageCodes { get; private set; }
        public bool AllowLinks { get; private set; }

        public List<Proninciation> Proninciations { get; private set; } = new List<Proninciation>();

        public ConversionContext(ContextArguments args, Dictionary<string, string> languageCodes, bool allowLinks)
        {
            Arguments = args;
            LanguageCodes = languageCodes;
            AllowLinks = allowLinks;
        }

        internal void AddPronunciation(string language, string fileName, string label)
        {
            Proninciations.Add(new Proninciation(language, fileName, label));
        }
    }

    public class Proninciation
    {
        public string Language { get; }
        public string FileName { get; }
        public string Label { get; }

        public Proninciation(string language, string fileName, string label)
        {
            Language = language;
            FileName = fileName;
            Label = label;
        }
    }

    public class ConversionResult : List<object>
    {
        public void Write(IEnumerable<object> data) => AddItem(data.ToArray());
        public void Write(params object[] data) => AddItem(data);

        private bool _writeSpaceBeforeNextItem = false;

        private void AddItem(params object[] items)
        {
            if (!items.All(i => i is String || i is Node))
                throw new ArgumentException("Added items must be strings or Nodes");

            if (_writeSpaceBeforeNextItem)
            {
                Add(" ");
                _writeSpaceBeforeNextItem = false;
            }
            foreach (var element in items)
                Add(element);
        }

        internal void WriteTrailingSpace()
        {
            _writeSpaceBeforeNextItem = true;
        }

        internal void WriteSpaceIfNotEmpty()
        {
            if (Count > 0) Add(" ");
        }

        internal void WriteTemplateArgumentIfExists(Template template, string argName, string followup)
        {
            if (template.Arguments.ContainsNotEmpty(argName))
            {
                Write(template.Arguments[argName].Value.TooSmart());
                Write(followup);
            }
        }

        internal void WriteTemplateArgumentIfExists(Template template, string argName, string prefix, string followup)
        {
            if (template.Arguments.ContainsNotEmpty(argName))
            {
                Write(prefix);
                Write(template.Arguments[argName].Value.TooSmart());
                Write(followup);
            }
        }
    }
}
