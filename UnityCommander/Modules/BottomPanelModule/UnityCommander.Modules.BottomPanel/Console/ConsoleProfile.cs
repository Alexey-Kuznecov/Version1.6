
using System;
using System.Collections.Generic;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleProfile
    {
        public Guid ConsoleId { get; init; }

        public string Name { get; init; } = "";

        public string? StartupCommand { get; set; }

        public List<string> StartupCommands { get; init; } = new();

        public List<string> CommandHistory { get; init; } = new();

        public string? TakeNextStartupCommand()
        {
            if (StartupCommands.Count == 0)
                return null;

            var command = StartupCommands[0];
            StartupCommands.RemoveAt(0);

            return command;
        }

        public string? PeekStartupCommand()
        {
            return StartupCommands.Count > 0
                ? StartupCommands[0]
                : null;
        }

        public void RemoveStartupCommand()
        {
            if (StartupCommands.Count > 0)
                StartupCommands.RemoveAt(0);
        }

        // На будущее:
        // public ConsoleSettings Settings { get; init; }

        public ConsoleProfile()
        {
            StartupCommands.Add("inspect cli.parse.state.builder --report --watch --interval=1000");
        }
    }
}
