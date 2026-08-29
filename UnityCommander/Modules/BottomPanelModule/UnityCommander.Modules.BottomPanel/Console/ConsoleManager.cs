
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using UnityCommander.CLI.History;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Lifecicle;
using UnityCommander.Mvvm;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleManager : IConsoleManager
    {
        private IConsoleProfileStore _profileStore;

        private readonly List<ConsoleProfile> _profiles;

        public readonly ConsoleState _state;

        public readonly ConsoleApplicationLifetime _lifetime;

        public readonly IConsoleHistory _history;

        public readonly ConsoleInputProcessor _inputProcessor;

        public readonly ConsoleAutocompleteProcessor _completeProcessor;
        
        private readonly ConsoleLineExecutor _executor;
        
        private readonly ConsoleCommandLoop _loop;

        private readonly List<ConsoleSession> _sessions = new();

        public IReadOnlyCollection<ConsoleSession> Sessions => _sessions;

        public ICommand SaveCommand { get; }

        public ConsoleManager(
            IConsoleHistory history,
            IConsoleProfileStore profileStore,
            ConsoleInputProcessor inputProcessor,
            ConsoleAutocompleteProcessor completeProcessor,
            ConsoleApplicationLifetime lifetime,
            ConsoleCommandLoop loop,
            ConsoleLineExecutor executor, 
            IMultiCommandService multiCommand)
        {
            SaveCommand = new RelayCommand(SaveProfiles);
            multiCommand.SaveCommand.RegisterCommand(SaveCommand);

            _profileStore = profileStore;
            _loop = loop;
            _history = history;
            _inputProcessor = inputProcessor;
            _completeProcessor = completeProcessor;
            _lifetime = lifetime;
            _executor = executor;

            _profiles = _profileStore.Load().ToList();
        }

        private bool _initialized;
        
        private void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;
        }

        private ConsoleSession CreateSession(ConsoleProfile? profile)
        {
            profile ??= new ConsoleProfile
            {
                ConsoleId = Guid.NewGuid(),
                Name = "Default"
            };

            return new ConsoleSession(
                _history,
                _inputProcessor,
                _completeProcessor,
                new ConsoleLifetime(),
                new InternalConsoleOutput(),
                new InternalConsoleInput(),
                profile);
        }

        public ConsoleSession Create()
        {
            var profile = GetNextProfile();

            var session = CreateSession(profile);

            _sessions.Add(session);

            Initialize();

            _ = _loop.RunAsync(session);

            _ = ExecuteLineAsync(session, CancellationToken.None);

            return session;
        }

        private ConsoleProfile? GetNextProfile()
        {
            if (_profiles == null)
                return null;

            if (_profiles.Count == 0)
                return null;

            var profile = _profiles[0];
            _profiles.RemoveAt(0);

            return profile;
        }

        public ConsoleSession Restore(ConsoleProfile profile)
        {
            var session = CreateSession(profile);

            _sessions.Add(session);

            Initialize();

            _ = _loop.RunAsync(session);

            return session;
        }

        public void Close(ConsoleSession session)
        {
            if (!_sessions.Remove(session))
                return;

            session.Lifetime.Stop();
            session.Dispose();
        }

        private async Task ExecuteLineAsync(
            ConsoleSession session,
            CancellationToken token)
        {
            var command = session.Profile.StartupCommand;

            if (string.IsNullOrWhiteSpace(command))
                return;

            var result = await _executor.ExecuteAsync(
                session,
                command,
                token);

            if (result.Success && result.Directives.HasFlag(CommandExecutionDirective.Startup))
            {
                session.Profile.StartupCommand = command;
                _profileStore.Save(session.Profile);
            }
        }

        private async Task ExecuteLinesAsync(
           ConsoleSession session,
           CancellationToken token)
        {
            while (true)
            {
                var command = session.Profile.PeekStartupCommand();

                if (command == null)
                    return;

                var result = await _executor.ExecuteAsync(
                    session,
                    command,
                    token);

                if (result.Success)
                {
                    session.Profile.RemoveStartupCommand();
                    _profileStore.Save(session.Profile);
                }
            }
        }

        private void SaveProfiles()
        {
            foreach (var session in _sessions)
            {
                _profileStore.Save(session.Profile);
            }
        }
    }
}
