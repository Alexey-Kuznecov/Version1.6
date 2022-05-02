
namespace MultiColumns.DateTime
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The game category column.
    /// </summary>
    public class DateTimeColumn : IColumnBuilder, IOptionBuilder, IPluginDescriptor
    {
        /// <summary>
        /// The date and time format.
        /// </summary>
        private string dateTimeFormat;
        
        private bool includeTime;

        /// <summary>
        /// The option render.
        /// </summary>
        private OptionRender optionRender;

        /// <summary>
        /// The update column value.
        /// </summary>
        private ColumnManager.UpdateColumnValue updateColumnValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeColumn"/> class.
        /// </summary>
        public DateTimeColumn()
        {
            this.dateTimeFormat = "15.3.2008";
            this.DateTimeFormat = new List<object>
            {
                "15.3.2008",
                "15/3/2008"
            };
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
            builder.AddContextItem("Edit date fornmat", this.InstallMod);
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

        /// <summary>
        /// The update column value.
        /// </summary>
        /// <param name="columnManager">
        /// The column manager.
        /// </param>
        public void UpdateColumnValue(ColumnManager columnManager)
        {
            this.updateColumnValue = columnManager.Update;
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
            this.updateColumnValue();        
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
            this.updateColumnValue();
        }

        /// <summary>
        /// The install mod.
        /// </summary>
        private void InstallMod(string path)
        {
            MessageBox.Show("Date Columns: " + path);
        }
    }
}
