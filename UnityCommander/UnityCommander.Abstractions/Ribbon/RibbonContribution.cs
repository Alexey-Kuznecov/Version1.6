
namespace UnityCommander.Abstractions.Ribbon
{
    public sealed class RibbonContribution
    {
        public string PluginId { get; }

        public RibbonDefinition Definition { get; }

        public RibbonContribution(
            string pluginId,
            RibbonDefinition definition)
        {
            PluginId = pluginId;
            Definition = definition;
        }
    }
}
