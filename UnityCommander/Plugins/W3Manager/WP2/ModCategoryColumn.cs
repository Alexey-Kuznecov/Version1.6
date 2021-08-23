
namespace W3Manager.WP2
{
    using System.IO;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The mod category column.
    /// </summary>
    public class ModCategoryColumn : IColumnBuilder
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
            builder.Add("Mod Category", 50);
            builder.Add("Mod Category2", 50);
            builder.AddContextItem("Install", this.InstallMod);
            //builder.BindingOption(typeof(PluginSettings), nameof(PluginSettings.DisplayAs), this.DisplayAsHandler, OptionRender.DropBox);
        }

        /// <summary>
        /// The display as handler.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        public void DisplayAsHandler(object selected)
        {
            if (selected is OptionRender render)
            { 
            }
        }

        /// <summary>
        /// The install mod.
        /// </summary>
        public void InstallMod()
        {
           // throw new System.NotImplementedException();
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
            foreach (var item in context.GetColumns())
            {
                return item.Header;
            }

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
            if (File.Exists(path))
            {
                return "File";
            }

            return "Folder";
        }

        /// <summary>
        /// The update column value.
        /// </summary>
        /// <param name="columnManager">
        /// The column manager.
        /// </param>
        public void UpdateColumnValue(ColumnManager columnManager)
        {
            // throw new System.NotImplementedException();
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
    }
}
