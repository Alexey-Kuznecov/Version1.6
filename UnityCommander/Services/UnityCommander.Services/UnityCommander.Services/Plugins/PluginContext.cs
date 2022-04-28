
#nullable enable

namespace UnityCommander.Services.Plugins
{
    using System.Collections.Generic;

    using UnityCommander.Common.Commands;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The plugin context.
    /// </summary>
    public class PluginContext : IPluginContext
    {
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public IReadOnlyList<IColumn> Columns { get; set; } = new List<IColumn>();

        /// <summary>
        /// Gets or sets the option.
        /// </summary>
        public IReadOnlyList<IOption> Option { get; set; } = new List<IOption>();

        /// <summary>
        /// Gets or sets the option.
        /// </summary>
        public List<BaseCommand> Commands { get; set; } = new ();

        /// <summary>
        /// The get columns.
        /// </summary>
        /// <returns>
        /// The columns.
        /// </returns>
        public IEnumerable<IColumn> GetColumns()
        {
            foreach (var column in this.Columns)
            {
                yield return column;
            }
        }

        /// <summary>
        /// The get options.
        /// </summary>
        /// <returns>
        ///  The option builders.
        /// </returns>
        public IEnumerable<IOption> GetOptions()
        {
            foreach (var option in this.Option)
            {
                yield return option;
            }
        }

        /// <summary>
        /// The get options.
        /// </summary>
        /// <returns>
        ///  The option builders.
        /// </returns>
        public IEnumerable<BaseCommand> GetCommands()
        {
            foreach (var command in Commands)
            {
                yield return command;
            }
        }

        /// <summary>
        /// The add column.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void AddColumn(IColumnBuilder builder)
        {
            ColumnBuilder instance = new ();
            ColumnManager columnManager = new ();
            builder.ColumnInitial(instance);

            foreach (var column in instance.GetColumns())
            {
                column.ColumnBuilder = builder;
                column.ColumnManager = columnManager;
                ((List<IColumn>)this.Columns).Add(column);
            }
        }

        /// <summary>
        /// The add column.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void AddOption(IOptionBuilder builder)
        {
            OptionBuilder instance = new ();
            builder.OptionBuild(instance);

            foreach (var option in instance.GetOptions())
            {
                option.OptionBuilders = builder;
                ((List<IOption>)this.Option).Add(option);
            }
        }

        /// <summary>
        /// The add column.
        /// </summary>
        /// <param name="commands">
        /// The builder.
        /// </param>
        public void AddCommand(List<BaseCommand> commands)
        {
            foreach (var command in commands)
            {
                Commands.Add(command);
            }
        }
    }
}
