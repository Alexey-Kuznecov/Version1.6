
using System;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.History;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleSession : IDisposable
    {
        public Guid Id => Guid.NewGuid();

        public ConsoleState State { get; }
        
        public ConsoleProfile Profile { get; }

        public ConsoleLifetime Lifetime { get; }

        public IConsoleHistory History { get; }

        public ConsoleInputProcessor InputProcessor { get; }
        
        public ConsoleAutocompleteProcessor CompleteProcessor { get; }
       
        public IConsoleOutput Output { get; }

        public IConsoleInput Input { get; }

        public IConsoleCommandContext Context { get; set; }

        public ConsoleSession(
            IConsoleHistory history,
            ConsoleInputProcessor inputProcessor,
            ConsoleAutocompleteProcessor completeProcessor,
            ConsoleLifetime lifetime,
            IConsoleOutput output,
            IConsoleInput input, 
            ConsoleProfile profile)
        {
            State = new ConsoleState(completeProcessor);
            History = history;
            InputProcessor = inputProcessor;
            CompleteProcessor = completeProcessor;
            Output = output;
            Input = input;
            Lifetime = lifetime;
            Profile = profile;

            Context = new ConsoleCommandContext(
                null,
                output: output,
                args: profile.StartupCommands.ToArray());

            State.SelectedIndex = 0;
        }

        public void Dispose()
        {
            Lifetime.Dispose();
        }
    }
}
