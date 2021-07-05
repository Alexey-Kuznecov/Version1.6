

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityCommander.Integration.Attributes;
using UnityCommander.Integration.Contracts;

namespace UnityCommander.Services.Plugins
{
    /// <summary>
    /// The plugin extension methods.
    /// </summary>
    public static class PluginExtensionMethods
    {
        /// <summary>
        /// Attempts to find methods that match the signature of the
        /// <see cref="InsertValueUsePath"/> delegate in plugin assemblies using attributes.
        /// </summary>
        /// <param name="implements">
        /// Plugin implementation interface.
        /// </param>
        /// <param name="action">
        /// Dispatches the <see cref="InsertValueUsePath"/> delegate in the external function parameters.
        /// </param>
        public static void GetContent(this IEnumerable<IPluginImplement> implements, Action<InsertValueUsePath> action)
        {
            var pluginImplements = implements as IPluginImplement[] ?? implements.ToArray();
            
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
        /// Attempts to find all handlers attributes in assemblies with plugins.
        /// </summary>
        /// <param name="pluginConfigures">
        /// Interfaces for managing loaded plugins.
        /// </param>
        /// <typeparam name="T">
        /// Required plugin interface.
        /// </typeparam>
        /// <returns>
        /// All handler attributes found.
        /// </returns>
        public static IEnumerable<Attribute> GetHandlerAttributes<T>(this IEnumerable<IPluginConfigure> pluginConfigures)
        {
            foreach (var configure in pluginConfigures)
            {
                foreach (PropertyInfo property in configure.GetType().GetProperties())
                {
                    foreach (var attribute in property.GetCustomAttributes())
                    {
                        yield return attribute;
                    }
                }
            }
        }
        
        public static IEnumerable<Attribute> GetPluginConfigs(this Type type)
        {
            var pluginType = type.GetTypeByContract<IPluginConfigure>();

            if (pluginType == null) yield break;
            foreach (PropertyInfo property in pluginType.GetProperties())
            {
                foreach (var attribute in property.GetCustomAttributes())
                {
                    yield return attribute;
                }
            }
        }

        public static Type GetTypeByContract<T>(this Type type)
        {
            foreach (var exportedType in type.Assembly.ExportedTypes
                .Where(t => typeof(T).IsAssignableFrom(t)))
            {
                if (type.Namespace == exportedType.Namespace)
                {
                    return exportedType;
                }
            }

            return null;
        }


        public static Assembly GetAssembly<T>(this IEnumerable<T> contracts)
        {
            return contracts.Select(contract => Assembly.GetAssembly(contract.GetType())).FirstOrDefault();
        }

        /// <summary>
        /// Attempts to find all handlers attributes in assemblies with plugins.
        /// </summary>
        /// <param name="contract">
        /// Any plugin interface needed to manage plugin.
        /// </param>
        /// <typeparam name="T">
        /// Required plugin interface.
        /// </typeparam>
        /// <returns>
        /// All handler attributes found.
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
