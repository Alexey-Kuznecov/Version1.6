
namespace UnityCommander.Common.Commands
{
    public record CommandPresentation(
        string DisplayName,
        string Description
    )
    {
        public static CommandPresentation Fallback(string id)
            => new(id, null);
    }
}
