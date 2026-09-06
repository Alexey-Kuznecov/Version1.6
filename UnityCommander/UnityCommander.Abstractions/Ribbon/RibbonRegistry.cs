
namespace UnityCommander.Abstractions.Ribbon
{
    public class RibbonRegistry : IRibbonRegistry
    {
        private readonly Dictionary<string, RibbonContribution>
            _contributions = new();

        public void Register(RibbonContribution contribution)
        {
            ArgumentNullException.ThrowIfNull(contribution);

            _contributions.Add(
                contribution.PluginId,
                contribution);
        }

        public bool Unregister(string pluginId)
        {
            return _contributions.Remove(pluginId);
        }

        public IReadOnlyCollection<RibbonContribution>
            Contributions => _contributions.Values;
    }
}
