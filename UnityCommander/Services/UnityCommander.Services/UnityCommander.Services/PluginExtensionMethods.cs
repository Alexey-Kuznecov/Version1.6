

namespace UnityCommander.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Contracts.Columns;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The plugin extension methods.
    /// </summary>
    public static class PluginExtensionMethods
    {
        /// <summary>
        /// The get plugin implementation.
        /// </summary>
        /// <param name="loaderService">
        /// The plugin implementation.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static IPluginImplements GetPluginImplement(this IPluginLoaderService loaderService)
        {
            return null;
        }

        /// <summary>
        /// The get plugin implementation.
        /// </summary>
        /// <param name="loaderService">
        /// The plugin implementation.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static IEnumerable<IPluginImplements> GetPluginImplements(this IPluginLoaderService loaderService)
        {
            return loaderService.ImportPluginImplements;
        }

        /// <summary>
        /// The get plugin implementation.
        /// </summary>
        /// <param name="implements">
        /// The plugin implementation.
        /// </param>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static IEnumerable<IColumn> GetColumns(this IEnumerable<IPluginImplements> implements, IPluginLoaderService service)
        {
#if NETCOREAPP3_1
            foreach (var attribute in service.GetHandlerAttributes<IPluginImplements>())
            {
                var getColumnsDelegate  = attribute?.OptionHandler as GetColumnsDelegate;

                if (getColumnsDelegate?.Invoke() is IEnumerable<IColumn> columns)
                {
                   foreach (var column in columns)
                   {
                       yield return column;
                   }
                }
            }
#endif
#if NET472
            yield return null;
#endif
        }

        /// <summary>
        /// The get content.
        /// </summary>
        /// <param name="implements">
        /// The implements.
        /// </param>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetContent(this IEnumerable<IPluginImplements> implements, IPluginLoaderService service)
        {
            var attributes = service.GetHandlerAttributes<IPluginImplements>();

            return null;
        }

        /// <summary>
        /// The get handler attributes.
        /// </summary>
        /// <param name="service">
        /// The contract.
        /// </param>
        /// <typeparam name="T">
        /// The contract
        /// </typeparam>
        /// <returns>
        /// The option handler attribute.
        /// </returns>
        public static IEnumerable<OptionHandlerAttribute> GetHandlerAttributes<T>(this IPluginLoaderService service)
        {
#if NETCOREAPP3_1
            foreach (var parameter in service.GetPluginContract<T>())
            {
                foreach (PropertyInfo property in parameter.GetType().GetProperties())
                {
                    foreach (var attribute in property.GetCustomAttributes())
                    {
                        yield return attribute as OptionHandlerAttribute;
                    }
                }
            }
#endif
#if NET472
            yield return null;
#endif
        }
    }
}
