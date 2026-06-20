
namespace UnityCommander.Abstractions.Ribbon
{
    public sealed class RibbonSectionBuilder
    {
        private readonly RibbonSectionDefinition _section;

        private readonly RibbonGroupBuilder _parent;

        public RibbonSectionBuilder(
            RibbonGroupBuilder parent,
            RibbonSectionDefinition section)
        {
            _section = section;
            _parent = parent;
        }

        public RibbonSectionBuilder Button(
            string commandId,
            int order = 0)
        {
            _section.Items.Add(
                new RibbonButtonDefinition
                {
                    CommandId = commandId,
                    SectionId = _section.Id,
                    Order = order
                });

            return this;
        }

        public RibbonGroupBuilder EndSection()
            => _parent;
    }
}
