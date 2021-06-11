
namespace UnityCommander.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Contracts.Columns;
    using UnityCommander.Services.Interfaces;

    public static class ColumnHelper
    {
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
            foreach (var attribute in service.GetHandlerAttributes<IPluginImplements>())
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
    }
}
