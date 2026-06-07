
using System.Collections.Generic;
using UnityCommander.Common.Commands;

namespace UnityCommander.Services.Interfaces
{
    /// <summary>
    /// Поставщик всех консольных команд для приложения.
    /// </summary>
    public interface IConsoleCommandProvider
    {
        IEnumerable<IConsoleCommandBase> GetAllCommands();
    }
}
