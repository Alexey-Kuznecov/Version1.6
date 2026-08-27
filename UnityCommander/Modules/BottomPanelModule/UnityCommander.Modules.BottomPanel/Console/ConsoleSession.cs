

using System;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.History;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleSession : IDisposable
    {
        public ConsoleState State { get; }

        public ConsoleLifetime Lifetime { get; }

        public IConsoleHistory History { get; }

        public ConsoleInputProcessor InputProcessor { get; }
        
        public ConsoleAutocompleteProcessor CompleteProcessor { get; }
       
        public IConsoleOutput Output { get; }

        public IConsoleInput Input { get; }

        public ConsoleSession(
            IConsoleHistory history,
            ConsoleInputProcessor inputProcessor,
            ConsoleAutocompleteProcessor completeProcessor,
            ConsoleLifetime lifetime,
            IConsoleOutput output,
            IConsoleInput input)
        {
            State = new ConsoleState(completeProcessor);
            History = history;
            InputProcessor = inputProcessor;
            CompleteProcessor = completeProcessor;
            Output = output;
            Input = input;
            Lifetime = lifetime;
        }

        public void Dispose()
        {
            Lifetime.Dispose();
        }
    }
}
