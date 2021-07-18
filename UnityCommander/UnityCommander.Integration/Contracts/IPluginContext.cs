
namespace UnityCommander.Integration.Contracts
{
    using System.Collections.Generic;

    using Columns;

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
        IEnumerable<OptionBuilder> GetOptions();

        /// <summary>
        /// The get column builder.
        /// </summary>
        /// <returns>
        /// The <see cref="IColumnBuilder"/>.
        /// </returns>
        IColumnBuilder GetColumnBuilder();
    }
}
