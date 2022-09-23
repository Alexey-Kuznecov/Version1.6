
namespace W3Manager.WP1
{
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The game category column.
    /// </summary>
    public class ModStatusColumn : IPluginDescriptor, IColumnBuilder, IPluginSettings
    {
        /// <summary>
        /// The settings.
        /// </summary>
        private ModSettings settings;

        /// <summary>
        /// The settings.
        /// </summary>
        private ColumnManager manager;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; } = "W3 Manager Mod";

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = "Plug-in for your W3 Manager Mod.";

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public SettingsBase Settings { get; set; } = new ModSettings();

        /// <summary>
        /// The column initial.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("Mod Status", 80);
            builder.Add("Mod Status 2", 80);
        }

        /// <summary>
        /// The update column value.
        /// </summary>
        /// <param name="columnManager">
        /// The column manager.
        /// </param>
        public void UpdateColumnValue(ColumnManager columnManager)
        {
            this.manager = columnManager;
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
        public object ColumnValueHandler(string columnName, string path, DirectoryItemType directoryItem)
        {
            if (directoryItem.Equals(DirectoryItemType.File))
            {
                if (columnName == "Mod Status")
                    return "File";

                if (columnName == "Mod Status 2")
                    return "File 2";
            }
            else if (directoryItem.Equals(DirectoryItemType.Folder))
            {
                if (columnName == "Mod Status")
                    return "Folder";

                if (columnName == "Mod Status 2")
                    return "Folder 2";
            }

            return "Unknown";
        }

        /// <summary>
        /// The on settings changed.
        /// </summary>
        /// <param name="newSettings">
        /// The settings.
        /// </param>
        public void OnSettingsChanged(SettingsBase newSettings)
        {
            if (newSettings is ModSettings myBase)
            {
                this.settings = myBase;

                if (this.settings.GetShowModStatus() != null)
                {
                    if (this.settings.GetShowModStatus() == "On")
                        this.manager.Add("Mod Status");
                    else
                        this.manager.Hide("Mod Status");
                }
                
                if (this.settings.GetShowModStatus2() != null)
                {
                    if (this.settings.GetShowModStatus2() == "On")
                        this.manager.Add("Mod Status 2");
                    else
                        this.manager.Hide("Mod Status 2");
                }
               
                this.manager.Update();
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
