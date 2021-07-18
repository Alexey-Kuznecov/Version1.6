
namespace UnityCommander.Services.Plugins
{
    using System;
    using System.Collections.Generic;

    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The column helper.
    /// </summary>
    public static class PluginColumnExtensions
    {
        /// <summary>
        /// Gets all application contexts from all plugin assemblies using the required <see cref="IPluginImplement.SetHostAppContext"/> method
        /// that the plugin must implement.
        /// </summary>
        /// <param name="implements">
        /// List of interfaces <see cref="IPluginImplement"/>.
        /// </param>
        /// <returns>
        /// List all host application context found.
        /// </returns>
        public static IEnumerable<PluginBuilder> GetHostAppContexts(this IEnumerable<IPluginImplement> implements)
        {
            foreach (var instance in implements)
            {
                var contexts = instance.SetHostAppContext();

                foreach (var context in contexts)
                {
                    context.DelegateCommand = Delegate.CreateDelegate(
                        typeof(InsertValueUsePath), instance, context.HostAppCommandModel.Command);
                    yield return context;
                }
            }
        }
    }
}
