
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.Commands.Parsing;
using UnityCommander.Services.Interfaces;
using UnityCommander.SystemMetrics.Monitoring;

namespace UnityCommander.Commands
{
    [ConsoleCommand("dirwatch", "Мониторит изменения в указанной директории", "dw", "watcher")]
    [Obsolete("Команда устарела. Используйте 'watch' вместо 'dirwatch'.")]
    public class DirectoryWatcherCommand : IConsoleCommand, IDisposable
    {
        private ICommandArgumentParser _parser;
        private ITabContextAccessor _accessor;
        private IDirectoryWatchManager _watchManager;

        private IConsoleOutput _output;

        private bool _subscribed;

        private readonly Dictionary<Guid, string> _sessions = new();

        public string Name => "dirwatch";
        public string Description => "Мониторит изменения в указанной директории";
        public IEnumerable<string> Aliases => ["dw", "watcher"];

        public DirectoryWatcherCommand(
            ICommandArgumentParser parse,
            ITabContextAccessor accessor,
            IDirectoryWatchManager watchManager)
        {
            _parser = parse;
            _accessor = accessor;
            _watchManager = watchManager;
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            _output = context.Output;
            var args = _parser.Parse(context.Arguments);
            var command = args.GetAt(0);
            var flag = args.HasFlag("all");

            var directory = _accessor.CurrentPath;
            var tab = _accessor.ActiveTab;

            if (command == "start")
            {
                if (_watchManager.IsWatching(tab.TabId))
                {
                    _output.WriteLine("Эта директория уже отслеживается.");
                    return;
                }

                if (!_subscribed)
                {
                    _watchManager.FileChanged += OnFileChanged;
                    _subscribed = true;
                }

                _sessions[tab.TabId] = directory;

                _watchManager.Watch(tab.TabId, directory);

                _output.WriteLine($"Начат мониторинг изменений в директории: {directory}");
            }
            else if (command == "stop")
            {
                if (flag)
                {
                    if (_subscribed)
                    {
                        _watchManager.FileChanged -= OnFileChanged;
                        _subscribed = false;
                    }

                    _sessions.Clear();
                    _watchManager.StopAll();
                    _output.WriteLine("Все наблюдатели остановлены.");
                }
                else
                {
                    _sessions.Remove(tab.TabId);
                    _watchManager.Unwatch(tab.TabId);
                    _output.WriteLine($"Мониторинг {directory} остановлен.");
                }
            }
            else if(command == "list")
            {
                foreach (var item in _watchManager.GetAll())
                {
                    _output.WriteLine($"TabId: {item.Tag}, Directory: {item.Path}");
                }
            }
            else
            {
                _output.WriteLine("Использование: dirwatch start|stop");
            }
        }

        private void OnFileChanged(object sender, FileSystemChangedEventArgs e)
        {
            string path = e.FullPath;
            string fileExtension = Path.GetExtension(path);

            if (string.IsNullOrEmpty(fileExtension))
            {
                _output.WriteLine($"Папка: {path} {e.ChangeType}");
            }
            else
            {
                _output.WriteLine($"Файл: {path} {e.ChangeType}");
            }
        }

        public void Dispose()
        {
            var directory = _accessor.CurrentPath;
            var tab = _accessor.ActiveTab;

            _watchManager.Unwatch(tab.TabId);
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
