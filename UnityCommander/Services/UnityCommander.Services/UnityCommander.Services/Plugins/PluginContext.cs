
#nullable enable

namespace UnityCommander.Services.Plugins
{
    using System.Collections.Generic;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The plugin context.
    /// </summary>
    public class PluginContext : IPluginContext
    {
        /// <summary>
        /// The column builder.
        /// </summary>
        private IColumnBuilder? columnBuilder;

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public IReadOnlyList<IColumn> Columns { get; set; } = new List<IColumn>();

        /// <summary>
        /// The get column builder.
        /// </summary>
        /// <returns>
        /// The interface column builder.
        /// </returns>
        public IColumnBuilder? GetColumnBuilder() => this.columnBuilder;

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
        public IEnumerable<OptionBuilder> GetOptions()
        {
            foreach (var column in this.Columns)
            {
                foreach (var option in column.OptionBuilders)
                {
                    yield return option;
                }
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
            builder.ColumnInitial(instance);

            foreach (var column in instance.GetColumns())
            {
                ((List<IColumn>)this.Columns).Add(column);
            }

            this.columnBuilder = builder;
        }
    }
}
