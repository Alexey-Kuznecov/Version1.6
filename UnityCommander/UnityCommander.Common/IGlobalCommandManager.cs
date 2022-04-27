
using System.Collections.Generic;
using UnityCommander.Integration.Commands;

namespace UnityCommander.Common
{
    using System;
    using System.Windows.Input;

    public interface IGlobalCommandManager
    {
        GlobalCommand GetCommand(string commandName);

        List<GlobalCommand> GetCommands();

        void CreateCommand(BaseCommand command);
        
        void CreateCommand(string commandName, object instance, Action<object> action);
    }
}
