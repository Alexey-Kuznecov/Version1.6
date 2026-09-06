
namespace UnityCommander.Modules.FilePanel.Models
{
    public sealed record CreationContext(
        string InputName,
        string TargetDirectory,
        string Extension,
        CreationType Type,
        string? TemplateId
        );
}
