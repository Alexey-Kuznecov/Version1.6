using UnityCommander.Autocomplete.Input;

namespace UnityCommander.CLI.Input
{
    public interface IInputHistory
    {
        InputState Previous();
        InputState Next();
    }
}
