
namespace UnityCommander.Common
{
    using System.Windows.Input;

    public interface IGlobalCommandManager
    {
        ICommand GetCommand(string commandName);
    }
}
