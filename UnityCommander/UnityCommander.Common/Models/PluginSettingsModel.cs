using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Common.Models
{
    public class PluginSettingsModel
    {
        public string Default { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string[] Tags { get; set; }
        public object Options { get; set; }
        public string SelectedOption { get; set; }
        public object Source { get; set; }
        public string OptionName { get; set; }

        public void SetValue(object val)
        {
            var method = this.Source.GetType().GetMethod($"Set{OptionName}");
            method?.Invoke(this.Source, new object[] { val });
        }
    }
}
