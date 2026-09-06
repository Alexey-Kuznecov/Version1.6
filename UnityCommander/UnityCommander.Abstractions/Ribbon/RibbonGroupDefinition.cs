
namespace UnityCommander.Abstractions.Ribbon
{
    public class RibbonGroupDefinition
    {
        public string Id { get; init; }

        public string TabId { get; init; }

        public string Title { get; init; }

        public List<RibbonSectionDefinition> Sections { get; set; }
            = new List<RibbonSectionDefinition>();
    }
}
