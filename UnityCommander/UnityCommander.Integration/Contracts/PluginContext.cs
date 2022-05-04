
#nullable enable

namespace UnityCommander.Integration.Contracts
{
    using System.Collections.Generic;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The plugin context.
    /// </summary>
    public class PluginContext : IPluginContext
    {
        /// <summary>
        /// Gets the columns.
        /// </summary>
        public IReadOnlyList<IColumn> Columns { get; } = new List<IColumn>();

        /// <summary>
        /// Gets the option.
        /// </summary>
        public IReadOnlyList<IOption> Option { get; } = new List<IOption>();

        /// <summary>
        /// Gets the option.
        /// </summary>
        public List<BaseCommand> Commands { get; } = new ();

        /// <summary>
        /// The get columns.
        /// </summary>
        /// <returns>
        /// The columns.
        /// </returns>
        public IEnumerable<IColumn> GetColumns() => this.Columns;

        /// <summary>
        /// The get options.
        /// </summary>
        /// <returns>
        ///  The option builders.
        /// </returns>
        public IEnumerable<IOption> GetOptions() => this.Option;

        /// <summary>
        /// The get options.
        /// </summary>
        /// <returns>
        ///  The option builders.
        /// </returns>
        public IEnumerable<BaseCommand> GetCommands() => this.Commands;
    }
}
