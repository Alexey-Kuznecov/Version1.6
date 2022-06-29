
namespace UnityCommander.Integration.Contracts
{
    using System.Collections.Generic;

    using Columns;

    using UnityCommander.Common.Commands;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Integration.Factories;
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

        /// <summary>
        /// The get commands.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<BaseCommand> GetCommands();

        /// <summary>
        /// The get plugin commands.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<ICommandBase> GetPluginCommands();

        /// <summary>
        /// The get commands.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public AssociatedTypes GetAssociatedTypes();
    }
}
