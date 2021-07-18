
namespace W3Manager.WP1
{
    using System.IO;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The game category column.
    /// </summary>
    public class GameCategoryColumn : IColumnBuilder, IPluginDescriptor
    {
        /// <summary>
        /// The option render.
        /// </summary>
        private OptionRender optionRender;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; } = "Game Category";

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = "Game Category Columns";

        /// <summary>
        /// The column initial.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("Game Category", 50);
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
            return this.optionRender is OptionRender.DropBox ? context : context;
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
            if (File.Exists(path))
            {
                return "File";
            }

            return "Folder";
        }

        /// <summary>
        /// The column value render.
        /// </summary>
        /// <returns>
        /// The <see cref="OptionRender"/>.
        /// </returns>
        public OptionRender ColumnValueRender()
        {
            this.optionRender = OptionRender.TextBlock;
            return this.optionRender;
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
            }
        }

        /// <summary>
        /// The install mod.
        /// </summary>
        private void InstallMod()
        {
            throw new System.NotImplementedException();
        }
    }
}
