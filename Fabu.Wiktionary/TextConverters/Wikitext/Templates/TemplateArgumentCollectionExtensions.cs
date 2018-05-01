using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki.Templates
{
    internal static class TemplateArgumentCollectionExtensions
    {
        /// <summary>
        /// The given flag is set and equals to "1"
        /// </summary>
        public static bool IsSet(this TemplateArgumentCollection args, string flagName) => 
            args.ContainsNotEmpty(flagName) && args[flagName].Value.ToString() == "1";
        /// <summary>
        /// If the given argument exists and is not empty
        /// </summary>
        /// <returns></returns>
        public static bool ContainsNotEmpty(this TemplateArgumentCollection args, string name) => 
            args != null && args.Contains(name) && !args[name].Value.IsEmpty();
        /// <summary>
        /// If the given argument exists and is not empty
        /// </summary>
        public static bool ContainsNotEmpty(this TemplateArgumentCollection args, int name) => 
            args != null && args.Contains(name) && !args[name].Value.IsEmpty();

        /// <summary>
        /// Tries to get a value of using the given argument name.
        /// </summary>
        /// <param name="value">The value of the given argument, or <code>null</code> if argument not found.</param>
        /// <param name="argName">name of the argument</param>
        /// <returns><code>true</code> if the argument was found, otherwise <code>false</code></returns>
        public static bool TryGet(this TemplateArgumentCollection args, out Wikitext value, int argName)
        {
            if (args.ContainsNotEmpty(argName))
            {
                value = args[argName].Value;
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Tries to get a value of using the given argument name.
        /// </summary>
        /// <param name="value">The value of the given argument, or <code>null</code> if argument not found.</param>
        /// <param name="argName">name of the argument</param>
        /// <returns><code>true</code> if the argument was found, otherwise <code>false</code></returns>
        public static bool TryGet(this TemplateArgumentCollection args, out Wikitext value, string argName)
        {
            if (args.ContainsNotEmpty(argName))
            {
                value = args[argName].Value;
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Tries to get a value of using the given argument name with a given array index, e.g. alt1, alt2, etc.
        /// </summary>
        /// <param name="value">The value of the given argument, or <code>null</code> if argument not found.</param>
        /// <param name="argArrayIndex">Array index of the given argument.</param>
        /// <param name="argName">name of the argument</param>
        /// <returns><code>true</code> if the argument was found, otherwise <code>false</code></returns>
        public static bool TryGet(this TemplateArgumentCollection args, out Wikitext value, int argArrayIndex, string argName) =>
            args.TryGet(out value, argName + argArrayIndex);

        /// <summary>
        /// Tries to get a value of the first of arguments found by the given set of argument names.
        /// </summary>
        /// <param name="value">The value of the given argument, or <code>null</code> if argument not found.</param>
        /// <param name="argNames">name of the argument</param>
        /// <returns><code>true</code> if the argument was found, otherwise <code>false</code></returns>
        public static bool TryGetOneOf(this TemplateArgumentCollection args, out Wikitext value, params object[] argNames)
        {
            foreach (var arg in argNames)
            {
                if (arg is string)
                {
                    var strArg = arg as string;
                    if (args.ContainsNotEmpty(strArg))
                    {
                        value = args[strArg].Value;
                        return true;
                    }
                }
                else
                {
                    var intArg = (int)arg;
                    if (args.ContainsNotEmpty(intArg))
                    {
                        value = args[intArg].Value;
                        return true;
                    }

                }
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Tries to get a value of the first of arguments found by the given set of argument names, given array index.
        /// </summary>
        /// <remarks>
        /// For example, for both following examples:
        /// {{name|value1|value2|gloss2=xxx}}
        /// {{name|value1|value2|t2=xxx}}
        /// the code: args.TryGetOneOf(out Wikitext value, 2, "gloss", "t") would return "xxx".
        /// Which is handy because in most cases "gloss" and "t" are alternative names of the same argument, and there are more similar examples.
        /// </remarks>
        /// <param name="value">The value of the given argument, or <code>null</code> if argument not found.</param>
        /// <param name="argArrayIndex">Array index of the given argument.</param>
        /// <param name="argNames">name of the argument</param>
        /// <returns><code>true</code> if the argument was found, otherwise <code>false</code></returns>
        public static bool TryGetOneOfAt(this TemplateArgumentCollection args, out Wikitext value, int argArrayIndex, params string[] argNames) => 
            args.TryGetOneOf(out value, argNames.Select(argName => argName + argArrayIndex).ToArray());

        public static bool TryGetArray(this TemplateArgumentCollection args, string arrayName, out Wikitext[] values)
        {
            var buffer = new Dictionary<int,TemplateArgument>();
            int anonymousCounter = 0;
            foreach (var arg in args)
            {
                if (arg.Name == null)
                {
                    if (arrayName != null)
                        continue;

                    buffer.Add(anonymousCounter, arg);
                    anonymousCounter++;
                }
                else if (arrayName != null)
                {
                    var argName = arg.Name.ToString();
                    var argNumStr = argName.Replace(arrayName, "");
                    int argNum = 1;
                    if (argName.StartsWith(arrayName) && (argNumStr == "" || Int32.TryParse(argNumStr, out argNum)))
                    {
                        argNum -= 1;
                        if (buffer.ContainsKey(argNum))
                            buffer[argNum] = arg;
                        else
                            buffer.Add(argNum, arg);
                    }
                }
            }
            if (buffer.Count == 0)
            {
                values = null;
                return false;
            }
            else
            {
                var maxIndex = buffer.Keys.Max();
                values = new Wikitext[maxIndex + 1];
                foreach (var item in buffer)
                    values[item.Key] = item.Value.Value;
                return true;
            }
        }
    }
}
