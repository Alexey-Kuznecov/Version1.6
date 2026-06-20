namespace UnityCommander.Abstractions.Ribbon
{
    public sealed class RibbonDefinition
    {
        public IList<RibbonTabDefinition> Tabs { get; } =
            new List<RibbonTabDefinition>();
    }
}