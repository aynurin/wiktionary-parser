using MwParserFromScratch.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace Fabu.Wiktionary.TextConverters.Wiki
{
    static class WikiNodesExtensions
    {
        /// <summary>
        /// This is a dirty hack to convert all templates if specific converters are not implemented.
        /// </summary>
        /// <remarks>
        /// It ignores template argument names. But for best results this shouldn't be run at all.
        /// </remarks>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Run ToRun(this IEnumerable<TemplateArgument> args)
        {
            return args.SelectMany(
                arg => arg.Value.Lines.SelectMany(
                    line => line.EnumChildren().Select(
                        n => (InlineNode)n))).ToRun();
        }
        /// <summary>
        /// This is a dirty hack to convert all templates if specific converters are not implemented.
        /// </summary>
        /// <remarks>
        /// It ignores template argument names. But for best results this shouldn't be run at all.
        /// </remarks>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Run ToRun(this IEnumerable<InlineNode> args)
        {
            return new Run(args);
        }
        /// <summary>
        /// This is a dirty hack to convert all templates if specific converters are not implemented.
        /// </summary>
        /// <remarks>
        /// It ignores template argument names. But for best results this shouldn't be run at all.
        /// </remarks>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Run ToRun(this IEnumerable<TagAttribute> args)
        {
            return args.SelectMany(
                arg => arg.Value.Lines.SelectMany(
                    line => line.EnumChildren().Select(
                        n => (InlineNode)n))).ToRun();
        }
    }
}
