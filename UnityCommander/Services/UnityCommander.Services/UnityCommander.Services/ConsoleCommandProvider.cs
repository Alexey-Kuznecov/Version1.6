
using System.Collections.Generic;
using UnityCommander.AI.ImageSearch;
using UnityCommander.CLI.Core;
using UnityCommander.Commands;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class ConsoleCommandProvider : IConsoleCommandProvider
    {
        private readonly List<IConsoleCommand> _commands = new();

        public ConsoleCommandProvider()
        {
            //// Здесь регистрируем команды вручную
            _commands.Add(new ClearCommand());
            //_commands.Add(new CopyFilesCommand());
            _commands.Add(new EchoCommand());
            //_commands.Add(new HelpCommand());
            //_commands.Add(new ExitCommand());
            _commands.Add(new FileUnlockCommand());
            _commands.Add(new PluginDirectoryMonitorCommand());
            _commands.Add(new ProcessControlCommand());
            _commands.Add(new ProcessOpenFilesCommand());
            _commands.Add(new SysStatCommand());
            _commands.Add(new TestCommand());
            _commands.Add(new TestFlashCommand());
            _commands.Add(new FindSimilarCommand(new ImageSimilarityService("F:\\01. Active\\CSharp\\UnityCommander\\UnityCommander\\Resources\\ai_models\\model.onnx")));
        }

        public IEnumerable<IConsoleCommand> GetAllCommands() => _commands;
    }
}
