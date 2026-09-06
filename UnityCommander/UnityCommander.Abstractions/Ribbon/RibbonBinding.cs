
namespace UnityCommander.Abstractions.Ribbon
{
    public sealed class RibbonBinding : IOwned
    {
        public string OwnerId { get; set; } = string.Empty;

        public string CommandId { get; init; } = string.Empty;

        public string TabId { get; init; } = string.Empty;

        public string? SectionId { get; init; } = string.Empty;

        public string GroupId { get; init; } = string.Empty;
        
        public int Order { get; init; }
    }
}
