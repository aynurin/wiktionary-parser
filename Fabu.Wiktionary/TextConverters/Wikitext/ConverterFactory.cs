using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    public class ConverterFactory
    {
        private static Dictionary<string, BaseNodeConverter> _knownConverters = new Dictionary<string, BaseNodeConverter>();

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
            if (_knownConverters.TryGetValue(name, out BaseNodeConverter converter))
                return converter;
            else
            {
                converter = LookupConverter(name);
                if (converter == null)
                    converter = new BaseNodeConverter();
                _knownConverters.Add(name, converter);
            }
            return converter;
        }

        private BaseNodeConverter LookupConverter(string name)
        {
            var typeName = "Fabu.Wiktionary.TextConverters.Wiki." + name + "Converter";
            var type = Type.GetType(typeName);
            if (type == null)
                return null;
            return Activator.CreateInstance(type) as BaseNodeConverter;
        }
    }
}
