
using System;
using UnityCommander.CLI.Core;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public interface IOutputRouter
    {
        IConsoleOutput GetOutput(Guid consoleId);
    }
}
