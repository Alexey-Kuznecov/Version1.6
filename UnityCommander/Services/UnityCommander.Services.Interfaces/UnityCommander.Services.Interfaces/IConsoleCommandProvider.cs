
using System.Collections.Generic;
using UnityCommander.CLI.Core;

namespace UnityCommander.Services.Interfaces
{
    /// <summary>
    /// Поставщик всех консольных команд для приложения.
    /// </summary>
    public interface IConsoleCommandProvider
    {
        IEnumerable<IConsoleCommand> GetAllCommands();
    }
}
