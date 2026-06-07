
namespace UnityCommander.Integration.Plugins
{
    using System.Collections.Generic;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The plugin extension methods.
    /// </summary>
    public static class PluginExtensionMethods
    {
        /// <summary>
        /// The get option.
        /// </summary>
        /// <param name="contexts">
        /// The contexts.
        /// </param>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The list of <see cref="IOption"/>.
        /// </returns>
        public static IEnumerable<IOption> GetOption(this IEnumerable<IPluginContext> contexts, object o)
        {
            foreach (var context in contexts)
            {
                var options = context.GetOptions();

               foreach (var option in options)
               {
                    if (o is null)
                    {
                        yield return option;
                        yield break;
                    }

                   if (option.OptionBuilders.Equals(o))
                   {
                       yield return option;
                   }
                }
            }
        }
    }
}
