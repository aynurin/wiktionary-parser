using Fabu.Wiktionary.TextConverters.Wiki.Templates;
using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    public class ConverterFactory
    {
        private Dictionary<string, BaseNodeConverter> _knownConverters = new Dictionary<string, BaseNodeConverter>();
        private string[] _ignoredTemplates;

        public ConverterFactory(IEnumerable<string> ignoredTemplates)
        {
            _ignoredTemplates = ignoredTemplates.OrderBy(_ => _).ToArray();
        }

        public BaseNodeConverter GetConverter(Node node)
        {
            var converter = GetConverter(node.GetType().Name);
            var substitute = converter.GetSubstitute(node);
            if (substitute != null)
            {
                var substituteConverter = GetConverter(substitute);
                if (substituteConverter.GetType() != typeof(BaseNodeConverter))
                    converter = substituteConverter;
            }
            return converter;
        }

        private BaseNodeConverter GetConverter(string name)
        {
            if (!_knownConverters.TryGetValue(name, out BaseNodeConverter converter))
            {
                converter = LookupConverter(name) ?? new BaseNodeConverter();

                lock (this)
                {
                    if (_knownConverters.TryGetValue(name, out BaseNodeConverter racingConverter))
                        converter = racingConverter;
                    else _knownConverters.Add(name, converter);
                }
            }
            if (converter is TemplateConverter templateConverter)
            {
                templateConverter.IgnoredTemplates = _ignoredTemplates;
            }
            return converter;
        }

        private BaseNodeConverter LookupConverter(string name)
        {
            var typeName = "Fabu.Wiktionary.TextConverters.Wiki." + name + "Converter";
            var type = Type.GetType(typeName);
            if (type == null)
            {
                typeName = "Fabu.Wiktionary.TextConverters.Wiki.Templates." + name + "Converter";
                type = Type.GetType(typeName);
            }
            if (type == null)
                return null;
            return Activator.CreateInstance(type) as BaseNodeConverter;
        }
    }
}
