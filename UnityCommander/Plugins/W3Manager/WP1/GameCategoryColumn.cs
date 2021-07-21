
namespace W3Manager.WP1
{
    using System.Collections.Generic;
    using System.IO;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The game category column.
    /// </summary>
    public class GameCategoryColumn : IColumnBuilder, IOptionBuilder, IPluginDescriptor
    {
        /// <summary>
        /// The option render.
        /// </summary>
        private OptionRender optionRender;

        /// <summary>
        /// The date and time format.
        /// </summary>
        private string dateTimeFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCategoryColumn"/> class.
        /// </summary>
        public GameCategoryColumn()
        {
            this.DateTimeFormat = new List<object>
            {
                "12-30-11",
                "12/30/2011"
            };

            this.DateTimeFormat2 = new List<object>
            {
                "ddd",
                "aaaa"
            };
        }

        /// <summary>
        /// Gets or sets the display as.
        /// </summary>
        public List<object> DateTimeFormat { get; set; }

        /// <summary>
        /// Gets or sets the date time format 2.
        /// </summary>
        public List<object> DateTimeFormat2 { get; set; }

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

            if (this.dateTimeFormat == "12/30/2011")
            {
            }

            return directoryInfo.CreationTime;
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
            optionBuilder.Add("Format output the date and time", this.DateTimeFormat, this.DateTimeFormatHandler, OptionRender.DropBox);
            optionBuilder.Add("Format output the date and time2", this.DateTimeFormat2, this.DateTimeFormatHandler2, OptionRender.DropBox);
        }

        /// <summary>
        /// The display as handler.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        private void DateTimeFormatHandler(object selected)
        {
            this.dateTimeFormat = selected as string;
        }


        /// <summary>
        /// The display as handler.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        private void DateTimeFormatHandler2(object selected)
        {
            this.dateTimeFormat = selected as string;
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
