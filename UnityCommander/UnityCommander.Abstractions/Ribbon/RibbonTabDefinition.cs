
namespace UnityCommander.Abstractions.Ribbon
{
    public class RibbonTabDefinition
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public IList<RibbonGroupDefinition> Groups { get; } =
            new List<RibbonGroupDefinition>();
    }
}
