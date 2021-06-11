

namespace UnityCommander.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Contracts.Columns;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The plugin extension methods.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public static class PluginExtensionMethods
    {
        /// <summary>
        /// The set plugin dependencies.
        /// </summary>
        /// <param name="loaderService">
        /// The loader service.
        /// </param>
        public static void SetPluginDependencies(this IPluginLoaderService loaderService)
        {
            foreach (var item in loaderService.GetPluginImplements())
            {
                PropertyInfo[] properties = item.GetType().GetProperties();
            }
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
        /// The get content.
        /// </summary>
        /// <param name="implements">
        /// The implements.
        /// </param>
        /// <param name="action">
        /// The path.
        /// </param>
        public static void GetContent(this IEnumerable<IPluginImplements> implements, Action<InsertValueUsePath> action)
        {
            var pluginImplements = implements as IPluginImplements[] ?? implements.ToArray();
            
            foreach (var implement in pluginImplements)
            {
                if (implement.Register.Count == 0)
                    implement.RegisterType();

                foreach (var type in implement.Register)
                {
                    foreach (var attribute in pluginImplements.GetHandlerAttributes())
                    {
                        var attr = attribute as ValueHandlerAttribute;
                        var getValueDelegate = attr?.OptionHandler as InsertValueUsePath;
                        action(getValueDelegate);
                    }
                }
            }
        }

        /// <summary>
        /// The get content.
        /// </summary>
        /// <param name="services">
        /// The implements.
        /// </param>
        /// <param name="action">
        /// The path.
        /// </param>
        public static void GetContent(this IPluginLoaderService services, Action<IColumnService> action)
        {
            foreach (var service in services.ImportColumnServices)
            {
                action(service);
            }
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
        public static IEnumerable<Attribute> GetHandlerAttributes<T>(this IPluginLoaderService service)
        {
            foreach (var parameter in service.GetPluginContract<T>())
            {
                foreach (PropertyInfo property in parameter.GetType().GetProperties())
                {
                    foreach (var attribute in property.GetCustomAttributes())
                    {
                        yield return attribute;
                    }
                }
            }
        }

        /// <summary>
        /// The get handler attributes.
        /// </summary>
        /// <param name="contract">
        /// The contract.
        /// </param>
        /// <typeparam name="T">
        /// The contract
        /// </typeparam>
        /// <returns>
        /// The option handler attribute.
        /// </returns>
        public static IEnumerable<Attribute> GetHandlerAttributes<T>(this IEnumerable<T> contract)
        {
            foreach (var parameter in contract)
            {
                foreach (PropertyInfo property in parameter.GetType().GetProperties())
                {
                    foreach (var attribute in property.GetCustomAttributes())
                    {
                        yield return attribute;
                    }
                }
            }
        }
    }
}
