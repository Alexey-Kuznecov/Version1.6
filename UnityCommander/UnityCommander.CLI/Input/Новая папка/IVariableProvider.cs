
namespace UnityCommander.CLI.Input
{
    public interface IVariableProvider
    {
        IEnumerable<CompletionItem> GetVariables();
    }
}
