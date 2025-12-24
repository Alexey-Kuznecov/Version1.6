
namespace UnityCommander.CLI.Input
{
    public interface IInputContextResolver
    {
        InputContext Resolve(TokenizationResult tokens);
    }
}
