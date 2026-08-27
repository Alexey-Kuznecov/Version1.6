
using Prism.Events;
using System.Collections.Generic;
using UnityCommander.CLI.History;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Lifecicle;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleManager : IConsoleManager
    {
        public readonly ConsoleState _state;

        public readonly ConsoleApplicationLifetime _lifetime;

        public readonly IConsoleHistory _history;

        public readonly ConsoleInputProcessor _inputProcessor;

        public readonly ConsoleAutocompleteProcessor _completeProcessor;

        private readonly ConsoleCommandLoop _loop;

        private readonly List<ConsoleSession> _sessions = new();

        public IReadOnlyCollection<ConsoleSession> Sessions => _sessions;

        private readonly ConsoleCommandDispatcher _dispatcher;
        private readonly IConsoleCommandProvider _commandProvider;

        public ConsoleManager(
            IConsoleHistory history,
            ConsoleInputProcessor inputProcessor,
            ConsoleAutocompleteProcessor completeProcessor,
            ConsoleApplicationLifetime lifetime,
            ConsoleCommandLoop loop,  
            ConsoleCommandDispatcher dispatcher,
            IConsoleCommandProvider commandProvider)
        {
            _loop = loop;
            _history = history;
            _inputProcessor = inputProcessor;
            _completeProcessor = completeProcessor;
            _lifetime = lifetime;
            _dispatcher = dispatcher;
            _commandProvider = commandProvider;
        }

        private bool _initialized;

        private void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;

            foreach (var command in _commandProvider.GetAllCommands())
            {
                _dispatcher.RegisterCommand(command);
            }
        }

        private ConsoleSession CreateSession()
        {
            return new ConsoleSession(
                _history,
                _inputProcessor,
                _completeProcessor,
                new ConsoleLifetime(),
                new InternalConsoleOutput(),
                new InternalConsoleInput());
        }

        public ConsoleSession Create()
        {
            var session = CreateSession();

            _sessions.Add(session);

            Initialize();

            _ = _loop.RunAsync(session);

            return session;
        }

        //public async Task StartAsync(
        //    CancellationToken cancellationToken = default)
        //{
        //    var session = Create();
        //    await _loop.RunAsync(session);
        //}

        public void Close(ConsoleSession session)
        {
            if (!_sessions.Remove(session))
                return;

            session.Lifetime.Stop();
            session.Dispose();
        }
    }
}
