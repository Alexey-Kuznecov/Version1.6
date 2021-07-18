
using System;
using UnityCommander.Integration.Columns;
using UnityCommander.Integration.Contracts;
using UnityCommander.Integration.Options;

namespace W3Manager.WP2
{
    public class ModStatusColumn : IColumnBuilder
    {
        private OptionRender optionRender;

        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("Mod Status", 50);
            builder.AddContextItem("Install", InstallMod);
            builder.BindingOption(typeof(PluginSettings), nameof(PluginSettings.DisplayAs), DisplayAsHandler, OptionRender.DropBox);
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

        public object ColumnValueValidate(IPluginContext context)
        {
            return context;
        }

        public object ColumnValueHandler(string path)
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
