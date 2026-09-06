
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
            string iconKey,
            int order = 0)
        {
            _section.Items.Add(
                new RibbonButtonDefinition
                {
                    CommandId = commandId,
                    SectionId = _section.Id,
                    IconKey = iconKey,
                    Order = order
                });

            return this;
        }

        public RibbonSectionBuilder ComboBox(
            string Id, 
            List<RibbonComboBoxItemDefinition> itemDefinitions)
        {
            _section.Items.Add(
                new RibbonComboBoxDefinition
                {
                    Id = Id,
                    SectionId = _section.Id,
                    Items = itemDefinitions
                });

            return this;
        }

        public RibbonGroupBuilder EndSection()
            => _parent;
    }
}
