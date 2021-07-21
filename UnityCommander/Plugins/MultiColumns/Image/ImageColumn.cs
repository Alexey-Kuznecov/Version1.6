
namespace MultiColumns.Image
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The game category column.
    /// </summary>
    public class ImageColumn : IColumnBuilder, IOptionBuilder, IPluginDescriptor
    {
        /// <summary>
        /// The option render.
        /// </summary>
        private OptionRender optionRender;

        /// <summary>
        /// The date and time format.
        /// </summary>
        private string imageFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageColumn"/> class.
        /// </summary>
        public ImageColumn()
        {
            this.imageFormat = "15.3.2008";

            this.ImageFormat = new List<object>
            {
                "15.3.2008",
                "15/3/2008"
            };
        }

        /// <summary>
        /// Gets or sets the display as.
        /// </summary>
        public List<object> ImageFormat { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; } = "Image column";

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = "Image columns";
        
        /// <summary>
        /// The column initial.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("DateTime", 100);
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

            if (this.imageFormat == "15/3/2008")
            {
                var d = directoryInfo.CreationTime.Date;
                CultureInfo culture = new CultureInfo("pt-BR");
                var nd = d.ToString("d", culture); 
                return nd;
            }
            else if (this.imageFormat == "15.3.2008")
            {
                var d = directoryInfo.CreationTime.Date;
                var nd = d.ToString("d");
                return nd;
            }

            return directoryInfo.CreationTime.Date;
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
            optionBuilder.Add("Format output the date and time", this.ImageFormat, this.imageFormat, this.ImageFormatHandler, OptionRender.DropBox);
        }

        /// <summary>
        /// The display as handler.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        private void ImageFormatHandler(object selected)
        {
            this.imageFormat = selected as string;
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
