using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Common.Models
{
    public class PluginSettingsModel
    {
        public string Description { get; set; }
        public string Category { get; set; }
        public string[] Tags { get; set; }
        public object Options { get; set; }
    }
}
