
namespace MultiColumns.DateTime
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The game category column.
    /// </summary>
    public class DateTimeColumn : IColumnBuilder, IOptionBuilder, IPluginDescriptor, IPluginSettings
    {
        /// <summary>
        /// The date and time format.
        /// </summary>
        private string dateTimeFormat;
        
        private bool includeTime;

        /// <summary>
        /// The settings.
        /// </summary>
        private DateTimeSettings settings;

        /// <summary>
        /// The settings.
        /// </summary>
        private ColumnManager manager;

        /// <summary>
        /// The option render.
        /// </summary>
        private OptionRender optionRender;

        public DateTimeColumn()
        {
            this.dateTimeFormat = "15/3/2008";
        }

        /// <summary>
        /// Gets or sets the display as.
        /// </summary>
        public List<object> DateTimeFormat { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; } = "Date creation column";

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = "Date creation columns";
        
        /// <summary>
        /// The column initial.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("Creation Date", 100);
            builder.AddContextItem("Select date format", this.InstallMod);
            builder.AddContextItem("Edit date format", this.InstallMod);
        }

        /// <summary>
        /// The column value handler.
        /// </summary>
        /// <param name="columnName">
        /// 
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="directoryItem">
        ///
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ColumnValueHandler(string columnName, string path, DirectoryItemType directoryItem)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            var nt = directoryInfo.CreationTime.ToLongTimeString();
            var nd = default(string);

            switch (this.dateTimeFormat)
            {
                case "15/3/2008":
                {
                    var d = directoryInfo.CreationTime.Date;
                    CultureInfo culture = new CultureInfo("pt-BR");
                    nd = d.ToString("d", culture);
                    break;
                }
                case "15.3.2008":
                {
                    var d = directoryInfo.CreationTime.Date;
                    nd = d.ToString("d");
                    break;
                }
            }

            return this.includeTime ? nd + " " + nt : nd;
        }

        public void OnSettingsChanged(SettingsBase settings)
        {
            if (!(settings is DateTimeSettings myBase)) return;
            
            this.settings = myBase;

            if (this.settings.GetDateTimeFormat() != null)
            {
                this.dateTimeFormat = this.settings.GetDateTimeFormat() == "15/3/2008" ? "15/3/2008" : "15.3.2008";
            }

            this.manager.Update();
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
        /// The option build.
        /// </summary>
        /// <param name="optionBuilder">
        /// The option builder.
        /// </param>
        public void OptionBuild(OptionBuilder optionBuilder)
        {
            optionBuilder.Add(
                "Select date format:", 
                this.DateTimeFormat, 
                dateTimeFormat, 
                this.DateTimeFormatHandler,
                OptionRender.DropBox);

            optionBuilder.Add("Shown date and time:", this.includeTime, this.IncludeTimeHandler, OptionRender.Checkbox);
        }

        /// <summary>
        /// The display as handler.
        /// </summary>
        /// <param name="value">
        /// The selected.
        /// </param>
        private void IncludeTimeHandler(bool value)
        {
            this.includeTime = value;      
        }

        /// <summary>
        /// The display as handler.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        private void DateTimeFormatHandler(object selected)
        {
            dateTimeFormat = selected as string;
        }

        /// <summary>
        /// The install mod.
        /// </summary>
        private void InstallMod(string path)
        {
            MessageBox.Show("Date Columns: " + path);
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
    }
}
