
using System;
using System.Collections.Generic;

namespace UnityCommander.Controls.Ribbon2.Models
{
    public class RibbonToolModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public RibbonToolKind Kind { get; set; }
        public string DisplayName { get; set; }
        public string IconResourceKey { get; set; }
        public string CommandId { get; set; } // resolved by command registry
        public Dictionary<string, object> Parameters { get; set; } = new();
        public string CustomControlType { get; set; } // for plugin-provided UI
    }
}
