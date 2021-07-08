
using System;
using System.Collections.Generic;
using UnityCommander.Integration.Columns;
using UnityCommander.Integration.Enums;
using UnityCommander.Integration.Options;

namespace W3Manager
{
    public class ModCategory : IColumnBuilder
    {
        public void ColumnInitial(ColumnBuilder builder)
        {
            builder.Add("Mod Category", 50, TargetPanel.Folders);
            builder.AddContextItem("Install", InstallMod);
            builder.BindOption(typeof(PluginSettings), nameof(PluginSettings.DisplayAs), DisplayAsHandler);
        }

        private void DisplayAsHandler(object selected)
        {
            if (selected is OptionRender render)
            {
                
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
            return "Ca";
        }

        public OptionRender ColumnValueRender()
        {
            return OptionRender.TextBlock;
        }
    }
}
