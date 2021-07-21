
namespace UnityCommander.Integration.Contracts
{
    using System.Collections.Generic;

    using Columns;

    using UnityCommander.Integration.Options;

    /// <summary>
    /// The PluginContext interface.
    /// </summary>
    public interface IPluginContext
    {
        /// <summary>
        /// The get columns.
        /// </summary>
        /// <returns>
        /// List of the <see cref="IColumn"/>.
        /// </returns>
        IEnumerable<IColumn> GetColumns();

        /// <summary>
        /// The get options.
        /// </summary>
        /// <returns>
        /// List of the <see cref="OptionBuilder"/>.
        /// </returns>
        IEnumerable<IOption> GetOptions();
    }
}
