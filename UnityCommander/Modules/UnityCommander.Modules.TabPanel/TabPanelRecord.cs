using System;

namespace UnityCommander.Modules.TabPanel
{
    /// <summary>
    /// The panel config record.
    /// </summary>
    public record TabPanelRecord2
    {
        public string Path { get; set; }
        public Guid Token { get; set; }
        public string Panel { get; set; }
    }
}
