
using UnityCommander.Integration.Commands;

namespace UnityCommander.Common
{
    using System.Windows.Input;

    public interface IGlobalCommandManager
    {
        GlobalCommand GetCommand(string commandName);

        void CreateCommand(BaseCommand command);
    }
}
