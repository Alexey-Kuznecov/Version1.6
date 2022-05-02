
namespace UnityCommander.Common.Module
{
    using System;
    using System.Windows.Controls;

    /// <summary>
    /// The panel config record.
    /// </summary>
    public record TabPanelRecord
    {
        public string Path { get; set; }
        public Guid Token { get; set; }
        public string Panel { get; set; }
        public UserControl ViewType { get; set; }
    }
}
