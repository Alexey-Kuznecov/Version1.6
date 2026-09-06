
namespace UnityCommander.Abstractions.Ribbon
{
    public class RibbonItemDefinition : IOwned
    {
        public string? SectionId { get; init; } = string.Empty;

        public int Order { get; set; }

        public string IconKey { get; set; } = string.Empty;

        public string OwnerId { get; set; } = string.Empty;
    }
}
