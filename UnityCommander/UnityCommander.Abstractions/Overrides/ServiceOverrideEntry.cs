
namespace UnityCommander.Abstractions.Overrides
{
    public sealed class ServiceOverrideEntry
    {
        public string? OwnerId { get; init; }

        public Type? ServiceType { get; init; }

        public Type? ImplementationType { get; init; }
    }
}
