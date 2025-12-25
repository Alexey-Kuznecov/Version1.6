
namespace UnityCommander.CLI.Input
{
    public interface IInputPipeline
    {
        InputUpdateResult Update(InputState state);
        InputUpdateResult Accept(InputState state, CompletionItem item);
        InputUpdateResult Cancel(InputState state);
    }
}
