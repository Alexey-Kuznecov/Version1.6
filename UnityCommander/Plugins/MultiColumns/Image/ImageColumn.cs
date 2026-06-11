
namespace MultiColumns.Image
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;
    using UnityCommander.Integration.Options;

    public class ImageColumn : IColumnBuilder, IOptionBuilder, IPluginDescriptor
    {
        private OptionRender optionRender;

        private string imageFormat;

        public ImageColumn()
        {
        }

        public List<object> ImageFormat { get; set; }

        public string DisplayName { get; set; } = "Image column";

        public string Description { get; set; } = "Image columns";
        
        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("Dimension", 80);
            builder.AddContextItem("Image format", this.InstallMod);
        }

        public object ColumnValueValidate(IPluginContext context)
        {
            return context;
        }

        public object ColumnValueHandler(string columnName, string path, DirectoryItemType directoryItem)
        {
            if (File.Exists(path))
            {
                return "Imaged";
            }

            return null;
        }

        public void UpdateColumnValue(ColumnManager columnManager)
        {
            // throw new System.NotImplementedException();
        }

        public OptionRender ColumnValueRender()
        {
            this.optionRender = OptionRender.TextBlock;
            return this.optionRender;
        }
        public void OptionBuild(OptionBuilder optionBuilder)
        {
            // optionBuilder.Add("Format output the date and time", this.SizedUnit, this.imageFormat, this.ImageFormatHandler, OptionRender.DropBox);
        }

        private void ImageFormatHandler(object selected)
        {
            this.imageFormat = selected as string;
        }

        private void InstallMod(string path)
        {
            MessageBox.Show("Image Columns: " + path);
        }
    }
}
