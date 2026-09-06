
namespace UnityCommander.Abstractions.Ribbon
{
    public sealed class RibbonTabBuilder
    {
        private readonly RibbonTabDefinition _tab;

        public RibbonTabBuilder(
            RibbonTabDefinition tab)
        {
            _tab = tab;
        }

        public RibbonGroupBuilder Group(
            string id,
            string title)
        {
            var group = new RibbonGroupDefinition
            {
                Id = id,
                TabId = _tab.Id,
                Title = title
            };

            _tab.Groups.Add(group);

            return new RibbonGroupBuilder(this, group);
        }
    }
}
