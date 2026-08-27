
namespace UnityCommander.Modules.BottomPanel.Console
{
    public enum ConsoleInputActionKind
    {
        TextChanged,
        AcceptCompletion,
        Submit,
        Cancel,
        NavigateUp,
        NavigateDown
    }

    public sealed record ConsoleInputAction(
        ConsoleInputActionKind Kind);
}
