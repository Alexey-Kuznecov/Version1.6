
using UnityCommander.Ribbon.Abstractions.Models;

namespace UnityCommander.Abstractions.Ribbon
{
    public sealed class RibbonGroupBuilder
    {
        private readonly RibbonGroupDefinition _group;
        
        private readonly RibbonTabBuilder _parent;

        public RibbonGroupBuilder(
            RibbonTabBuilder parent,
            RibbonGroupDefinition group)
        {
            _group = group;
            _parent = parent;
        }

        public RibbonSectionBuilder Section(
            string id,
            RibbonGroupLayout layout)
        {
            var section = new RibbonSectionDefinition
            {
                Id = id,
                GroupId = _group.Id,
                Layout = layout
            };

            _group.Sections.Add(section);

            return new RibbonSectionBuilder(this, section);
        }

        public RibbonTabBuilder EndGroup() => _parent;
    }
}
