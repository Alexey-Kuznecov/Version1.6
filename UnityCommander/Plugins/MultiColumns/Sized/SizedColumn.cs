
namespace MultiColumns.Sized
{
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;
    using UnityCommander.Integration.Options;

    public class SizedColumn : IColumnBuilder, IOptionBuilder, IPluginDescriptor, IPluginSettings
    {
        private OptionRender optionRender;

        private string sizedUnit;
        
        private ColumnManager.UpdateColumnValue updateColumn;

        private SizeSettings settings;

        private ColumnManager manager;

        public SizedColumn()
        {
            this.sizedUnit = "Auto";
        }

        public List<object> SizedUnit { get; set; }
        public string DisplayName { get; set; } = "Sized column";
        public string Description { get; set; } = "Sized columns";

        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("Sized", 60);
            builder.AddContextItem("Size format", this.InstallMod);
        }

        public object ColumnValueValidate(IPluginContext context)
        {
            return context;
        }

        public void UpdateColumnValue(ColumnManager columnManager)
        {
            this.manager = columnManager;
        }

        public object ColumnValueHandler(string columnName, string path, DirectoryItemType directoryItem)
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

        public void OnSettingsChanged(SettingsBase settings)
        {
            if (!(settings is SizeSettings myBase)) return;

            this.settings = myBase;

            if (this.settings.GetSizedUnit() != null)
            {
                this.sizedUnit = this.settings.GetSizedUnit();
            }

            this.manager.Update();
        }

        public OptionRender ColumnValueRender()
        {
            //this.optionRender = OptionRender.TextBlock;
            return this.optionRender;
        }

        public void OptionBuild(OptionBuilder optionBuilder)
        {
            //optionBuilder.Add("Unformation unit:", this.SizedUnit, this.sizedUnit, this.SeizedUnitHandler, OptionRender.DropBox);
        }

        private void SeizedUnitHandler(object selected)
        {
            this.sizedUnit = selected as string;
            this.updateColumn();
        }

        private void InstallMod(string path)
        {
            MessageBox.Show("Size Columns: " + path);
        }
    }
}
