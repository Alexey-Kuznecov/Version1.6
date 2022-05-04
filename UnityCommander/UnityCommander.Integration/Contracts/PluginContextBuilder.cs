
namespace UnityCommander.Integration.Contracts
{
    using System.Collections.Generic;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The plugin context builder.
    /// </summary>
    public class PluginContextBuilder
    {
        /// <summary>
        /// The commands.
        /// </summary>
        private readonly PluginContext pluginContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginContextBuilder"/> class.
        /// </summary>
        /// <param name="pluginContext">
        /// The plugin context.
        /// </param>
        public PluginContextBuilder(PluginContext pluginContext)
        {
            this.pluginContext = pluginContext;
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
                if (this.pluginContext != null)
                {
                    ((List<IColumn>)this.pluginContext.Columns).Add(column);
                }
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
            OptionBuilder instance = new();
            builder.OptionBuild(instance);

            foreach (var option in instance.GetOptions())
            {
                option.OptionBuilders = builder;
                ((List<IOption>)this.pluginContext.Option).Add(option);
            }
        }

        /// <summary>
        /// The add column.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void AddOption(IPluginSettings builder)
        {
            OptionBuilder instance = new();
            builder.OptionBuild(instance);

            foreach (var option in instance.GetOptions())
            {
                option.OptionBuilders = builder;
                ((List<IOption>)this.pluginContext.Option).Add(option);
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
                this.pluginContext.Commands.Add(command);
            }
        }
    }
}
