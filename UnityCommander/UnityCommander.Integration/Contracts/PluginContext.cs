
#nullable enable

namespace UnityCommander.Integration.Contracts
{
    using System.Collections.Generic;

    using UnityCommander.Common.Commands;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Integration.Factories;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The plugin context.
    /// </summary>
    public class PluginContext : IPluginContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginContext"/> class.
        /// </summary>
        /// <param name="associatedTypes">
        /// The associated types.
        /// </param>
        public PluginContext(AssociatedTypes associatedTypes)
        {
            this.AssociatedTypes = associatedTypes;
        }

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
        public IReadOnlyList<BaseCommand> Commands { get; } = new List<BaseCommand>();

        /// <summary>
        /// Gets the option.
        /// </summary>
        public IReadOnlyList<ICommandBase> PluginCommands { get; } = new List<ICommandBase>();

        /// <summary>
        /// Gets or sets the associated types.
        /// </summary>
        public AssociatedTypes AssociatedTypes { get; }

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

        
        /// <summary>
        /// The get options.
        /// </summary>
        /// <returns>
        ///  The option builders.
        /// </returns>
        public IEnumerable<ICommandBase> GetPluginCommands() => this.PluginCommands;

        /// <summary>
        /// The get commands.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public AssociatedTypes GetAssociatedTypes() => this.AssociatedTypes;
    }
}
