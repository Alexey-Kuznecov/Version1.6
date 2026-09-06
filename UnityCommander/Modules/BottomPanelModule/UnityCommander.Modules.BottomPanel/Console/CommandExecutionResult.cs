
namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class CommandExecutionResult
    {
        public bool Success { get; init; }

        public CommandExecutionDirective Directives { get; init; }
    }
}
