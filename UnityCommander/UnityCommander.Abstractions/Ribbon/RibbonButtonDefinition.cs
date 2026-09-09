
namespace UnityCommander.Abstractions.Ribbon
{
    public class RibbonButtonDefinition : RibbonItemDefinition
    {
        public string? Text { get; init; }
        public string? CommandId { get; set; }
    }
}
