
namespace UnityCommander.Services.Plugins.Manager
{
    using System;
    using System.Collections.Generic;

    using UnityCommander.Integration.Contracts;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The column helper.
    /// </summary>
    public static class ColumnHelper
    {
        /// <summary>
        /// Attempts to find all methods in the objects implementing 
        /// the <see cref="IPluginImplement"/> interface that are responsible
        /// for getting the list of contexts of the host application 
        /// using the attribute.
        /// </summary>
        /// <param name="implements">
        /// List of interfaces <see cref="IPluginImplement"/>.
        /// </param>
        /// <param name="service">
        /// The interface for managing loaded plugins.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static IEnumerable<IColumn> GetContextByAttribute(this IEnumerable<IPluginImplement> implements, IPluginLoaderService service)
        {
            foreach (var attribute in service.GetHandlerAttributes<IPluginImplement>())
            {
                if (attribute is AttachHandlerAttribute attachHandler)
                {
                    var getColumnsDelegate = attachHandler.Handler as AddColumnsDelegate;

                    if (getColumnsDelegate?.Invoke() is IEnumerable<IColumn> columns)
                    {
                        foreach (var column in columns)
                        {
                            yield return column;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all application contexts from all plugin assemblies using 
        /// the required <see cref="IPluginImplement.SetHostAppContext"/> method
        /// that the plugin must implement. TODO: Rename the name of method.
        /// </summary>
        /// <param name="implements">
        /// List of interfaces <see cref="IPluginImplement"/>.
        /// </param>
        /// <param name="service">
        /// The interface for managing loaded plugins.
        /// </param>
        /// <returns>
        /// List all host application context found.
        /// </returns>
        public static IEnumerable<HostAppContext> GetHostAppContexts(this IEnumerable<IPluginImplement> implements, IPluginLoaderService service)
        {
            foreach (var instance in service.GetPluginInstances<IPluginImplement>())
            {
               var contexts = ((IPluginImplement)instance).SetHostAppContext();

               foreach (var context in contexts)
               {
                   context.DelegateCommand = Delegate.CreateDelegate(
                       typeof(InsertValueUsePath), instance, context.CommandModel.Command);
                   yield return context;
               }
            }
        }
    }
}
