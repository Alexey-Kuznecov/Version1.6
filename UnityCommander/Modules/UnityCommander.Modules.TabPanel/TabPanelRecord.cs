using System;

namespace UnityCommander.Modules.TabPanel
{
    /// <summary>
    /// The panel config record.
    /// </summary>
    public record TabPanelRecord
    {
        public string Path { get; set; }
        public Guid Token { get; set; }
        public string Panel { get; set; }
    }
}
