
namespace MultiColumns.Sized
{
    using System;
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
    public class SizedColumn : IColumnBuilder, IOptionBuilder, IPluginDescriptor
    {
        /// <summary>
        /// The option render.
        /// </summary>
        private OptionRender optionRender;

        /// <summary>
        /// The sized format.
        /// </summary>
        private string sizedUnit;
        
        private ColumnManager.UpdateColumnValue updateColumn;

        /// <summary>
        /// Initializes a new instance of the <see cref="SizedColumn"/> class.
        /// </summary>
        public SizedColumn()
        {
            this.sizedUnit = "Auto";

            this.SizedUnit = new List<object>
            {
                "Auto",
                "In bytes",
                "In kbyte",
                "In mbyte",
                "In gbyte"
            }; 
        }

        /// <summary>
        /// Gets or sets the display as.
        /// </summary>
        public List<object> SizedUnit { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; } = "Sized column";

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = "Sized columns";
        
        /// <summary>
        /// The column initial.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("Sized", 60);
            builder.AddContextItem("Size format", this.InstallMod);
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
        /// The update column value.
        /// </summary>
        /// <param name="columnManager">
        /// The column manager.
        /// </param>
        public void UpdateColumnValue(ColumnManager columnManager)
        {
            this.updateColumn = columnManager.Update;
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
                FileInfo info = new FileInfo(path);
                
                switch (this.sizedUnit)
                {
                    case "In bytes":
                        return $"{info.Length:f2} b";
                    case "In kbyte":
                        return $"{ConverterBytes.BytesToKibiBytes(info.Length):f2} kb";
                    case "In mbyte":
                        return $"{ConverterBytes.BytesToMebiBytes(info.Length):f2} mb";
                    case "In gbyte":
                        return $"{ConverterBytes.BytesToGibiBytes(info.Length):f2} gb";
                    default:
                        return ConverterBytes.AutoConvertFormatBytes(info.Length);
                }
            }

            return null;
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
            optionBuilder.Add("Unformation unit:", this.SizedUnit, this.sizedUnit, this.SeizedUnitHandler, OptionRender.DropBox);
        }

        /// <summary>
        /// The display as handler.
        /// </summary>
        /// <param name="selected">
        /// The selected.
        /// </param>
        private void SeizedUnitHandler(object selected)
        {
            this.sizedUnit = selected as string;
            this.updateColumn();
        }

        /// <summary>
        /// The install mod.
        /// </summary>
        private void InstallMod(string path)
        {
            MessageBox.Show("Size Columns: " + path);
        }
    }
}
