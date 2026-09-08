
namespace UnityCommander.Abstractions.Widget
{
    public sealed record WidgetDefinition(
       string Id,
       string Name,
       Type ViewModelType,
       Type ViewType);
}
