
namespace UnityCommander.Abstractions.Ribbon
{
    public interface IRibbonRegistry
    {
        void Register(RibbonContribution contribution);

        bool Unregister(string pluginId);

        IReadOnlyCollection<RibbonContribution> Contributions { get; }
    }
}
