
namespace UnityCommander.Abstractions.Ribbon
{
    public sealed class RibbonBuilder
    {
        private readonly RibbonDefinition _definition;

        public RibbonBuilder(RibbonDefinition definition)
        {
            _definition = definition;
        }

        public RibbonTabBuilder Tab(
            string id,
            string title)
        {
            var tab = new RibbonTabDefinition
            {
                Id = id,
                Title = title
            };

            _definition.Tabs.Add(tab);

            return new RibbonTabBuilder(tab);
        }
    }
}
