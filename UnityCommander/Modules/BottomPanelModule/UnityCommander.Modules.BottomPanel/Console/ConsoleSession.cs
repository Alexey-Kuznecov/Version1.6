

using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.History;
using UnityCommander.CLI.Lifecicle;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleSession
    {
        public ConsoleState State { get; }

        public IConsoleHistory History { get; }

        public ConsoleInputProcessor InputProcessor { get; }
        
        public ConsoleAutocompleteProcessor CompleteProcessor { get; }
       
        public IConsoleOutput Output { get; }

        public IConsoleInput Input { get; }

        private ConsoleCommandLoop _loop;

        public ConsoleSession(
            IConsoleHistory history,
            ConsoleInputProcessor inputProcessor,
            ConsoleAutocompleteProcessor completeProcessor,
            IConsoleOutput output,
            IConsoleInput input, 
            ConsoleCommandLoop loop)
        {
            State = new ConsoleState(completeProcessor);
            History = history;
            InputProcessor = inputProcessor;
            CompleteProcessor = completeProcessor;
            Output = output;
            Input = input;
            _loop = loop;
        }

        public Task StartAsync(CancellationToken cancellationToken)
            => _loop.RunAsync(this, cancellationToken);
    }
}
