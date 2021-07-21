
namespace W3Manager.WP1
{
    using System;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The game status column.
    /// </summary>
    public class GameStatusColumn : IColumnBuilder
    {
        /// <summary>
        /// The option render.
        /// </summary>
        private OptionRender optionRender;

        /// <summary>
        /// The column initial.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("Game Status", 50);
            builder.AddContextItem("Install", this.InstallMod);
            builder.BindingOption(typeof(PluginSettings), nameof(PluginSettings.DisplayAs), this.DisplayAsHandler, OptionRender.DropBox);
        }

        /// <summary>
        /// The column value validate.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ColumnValueValidate(IPluginContext context)
        {
            return context;
        }

        /// <summary>
        /// The column value handler.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ColumnValueHandler(string path)
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        /// <summary>
        /// The column value render.
        /// </summary>
        /// <returns>
        /// The <see cref="OptionRender"/>.
        /// </returns>
        public OptionRender ColumnValueRender()
        {
            return OptionRender.TextBlock;
        }

        /// <summary>
        /// The install mod.
        /// </summary>
        private void InstallMod()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The display as handler.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        private void DisplayAsHandler(object selected)
        {
            if (selected is OptionRender render)
            {
                this.optionRender = render;
            }
        }
    }
}
