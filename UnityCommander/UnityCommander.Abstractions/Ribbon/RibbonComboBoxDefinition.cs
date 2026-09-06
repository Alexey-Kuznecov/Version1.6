
namespace UnityCommander.Abstractions.Ribbon
{
    public sealed class RibbonComboBoxDefinition : RibbonItemDefinition
    {
        public string Id { get; init; } = string.Empty; 
        
        public List<RibbonComboBoxItemDefinition> Items { get; init; } = [];
    }
}
