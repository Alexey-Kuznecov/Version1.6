
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.Commands.Parsing;
using UnityCommander.Commands.Services;
using UnityCommander.Commands.UtilProcess;
using UnityCommander.Native;


namespace UnityCommander.Commands
{
    [ConsoleCommand("proc-openfiles", "Выводит список открытых файлов указанного процесса по имени.", "procof")]
    public class ProcessOpenFilesCommand : IConsoleCommand
    {
        private IConsoleOutput _output;

        public string Name => "proc-openfiles";

        public string Description => "Выводит список открытых файлов указанного процесса по имени.";
        public IEnumerable<string> Aliases => ["procof"];

        private IProcessOpenFilesService _service;
        private ICommandArgumentParser _parser;

        public ProcessOpenFilesCommand(
            IProcessOpenFilesService service,
            ICommandArgumentParser parser)
        {
            _service = service;
            _parser = parser;
        }

        public Task ExecuteAsync(
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var args =
                _parser.Parse(
                    context.Arguments);

            return _service.RunAsync(
                context.Output,
                args,
                cancellationToken);
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
