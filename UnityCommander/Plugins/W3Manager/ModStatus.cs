
namespace W3Manager
{
    using System;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Enums;
    using UnityCommander.Integration.Options;

    public class ModStatus : IColumnBuilder
    {
        private OptionRender optionRender;

        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("Mod Status", 50, TargetPanel.Folders);
            builder.AddContextItem("Install", InstallMod);
            builder.BindOption(typeof(PluginSettings), nameof(PluginSettings.DisplayAs), DisplayAsHandler);
        }

        private void DisplayAsHandler(object selected)
        {
            if (selected is OptionRender render)
            {
                this.optionRender = render;
            }
        }

        private void InstallMod()
        {
            throw new System.NotImplementedException();
        }

        public object ColumnValueValidate(object value)
        {
            return value;
        }

        public object ColumnValueHandler(object path)
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        public OptionRender ColumnValueRender()
        {
            return OptionRender.TextBlock;
        }
    }
}
