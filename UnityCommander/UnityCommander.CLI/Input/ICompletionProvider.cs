
namespace UnityCommander.CLI.Input
{
    public interface ICompletionProvider
    {
        bool CanHandle(InputContext context);
        IEnumerable<CompletionItem> GetCompletions(InputContext context);
    }
}
