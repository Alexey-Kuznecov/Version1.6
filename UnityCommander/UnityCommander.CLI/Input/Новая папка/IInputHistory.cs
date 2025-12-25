
namespace UnityCommander.CLI.Input
{
    public interface IInputHistory
    {
        InputState Previous();
        InputState Next();
    }
}
