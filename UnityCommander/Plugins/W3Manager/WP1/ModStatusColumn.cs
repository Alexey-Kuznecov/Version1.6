
namespace W3Manager.WP1
{
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The game category column.
    /// </summary>
    public class ModStatusColumn : IPluginDescriptor, IColumnBuilder, IPluginSettings
    {
        /// <summary>
        /// The settings.
        /// </summary>
        private Settings settings;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; } = "W3 Manager Mod";

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = "Plug-in for your W3 Manager Mod.";

        /// <summary>
        /// The column initial.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("Mod Status", 100);
        }

        /// <summary>
        /// The update column value.
        /// </summary>
        /// <param name="columnManager">
        /// The column manager.
        /// </param>
        public void UpdateColumnValue(ColumnManager columnManager)
        {
            // throw new  System.NotImplementedException();
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
            if (this.settings.GamePath == path)
                return true;
            else
                return false;
        }

        /// <summary>
        /// The on settings changed.
        /// </summary>
        /// <param name="newSettings">
        /// The settings.
        /// </param>
        public void OnSettingsChanged(SettingsBase newSettings)
        {
            if (newSettings is Settings myBase)
            {
                this.settings = myBase;
            }
        }

        #region Not Implemented

        public OptionRender ColumnValueRender()
        {
            throw new System.NotImplementedException();
        }

        public object ColumnValueValidate(IPluginContext context)
        {
            return null;
        }

        #endregion
    }
}
