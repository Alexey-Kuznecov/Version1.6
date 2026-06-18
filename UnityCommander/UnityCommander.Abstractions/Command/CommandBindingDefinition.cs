
namespace UnityCommander.Abstractions.Command
{
    public class CommandBindingDefinition
    {
        public string? CommandId { get; set; }

        public UIRegion Region { get; set; }  // Ribbon / Sidebar / ContextMenu

        public string? Tab { get; set; }

        public string? Group { get; set; }

        public int Order { get; set; }

        public Dictionary<string, object>? Metadata { get; set; }
    }
}
