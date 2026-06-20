
using UnityCommander.Ribbon.Abstractions.Models;

namespace UnityCommander.Abstractions.Ribbon
{
    public sealed class RibbonSectionDefinition
    {
        public string? Id { get; set; }

        public string? GroupId { get; set; }

        public RibbonGroupLayout Layout { get; set; }

        public IList<RibbonItemDefinition> Items { get; } =
            new List<RibbonItemDefinition>();
    }
}
