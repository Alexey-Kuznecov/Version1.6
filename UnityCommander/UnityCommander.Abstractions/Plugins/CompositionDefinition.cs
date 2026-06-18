
namespace UnityCommander.Abstractions.Plugin
{
    public sealed class CompositionDefinition : IOwned
    {
        public string Id { get; set; }

        public string OwnerId { get; set ; }

        public Type WindowType { get; }

        public List<CompositionPart> Parts { get; } = new();

        public CompositionDefinition(Type windowType)
        {
            Id = $"{OwnerId}:{windowType.FullName}";
            WindowType = windowType;
        }
    }
}
