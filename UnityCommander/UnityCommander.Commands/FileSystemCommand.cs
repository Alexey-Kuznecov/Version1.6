
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Parsing;

namespace UnityCommander.Commands
{
    [ConsoleCommand("fs", "Операции с файловой системой")]
    public sealed class FileSystemCommand : IConsoleCommand
    {
        private readonly ICommandArgumentParser _parser;

        public string Name => "fs";
        public string Description => "Операции с файловой системой";

        public CommandExecutionMode Mode
            => CommandExecutionMode.Immediate;

        public FileSystemCommand(
            ICommandArgumentParser parser)
        {
            _parser = parser;
        }

        public async Task ExecuteAsync(
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var args = _parser.Parse(context.Arguments);

            var operation = args.GetAt(0);

            if (string.IsNullOrWhiteSpace(operation))
            {
                WriteUsage(context);
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            switch (operation.ToLowerInvariant())
            {
                case "create":
                    CreateFile(args, context);
                    break;

                case "mkdir":
                    CreateDirectory(args, context);
                    break;

                case "delete":
                    Delete(args, context);
                    break;

                case "rename":
                    Rename(args, context);
                    break;

                case "stress":
                    Random(args, context);
                    break;

                default:
                    context.Output.WriteLine(
                        $"Unknown operation: {operation}");

                    WriteUsage(context);
                    break;
            }

            await Task.CompletedTask;
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }

        private static void CreateFile(
            IArgumentCollection args,
            IConsoleCommandContext context)
        {
            var path = args.GetAt(1);

            if (string.IsNullOrWhiteSpace(path))
            {
                context.Output.WriteLine(
                    "Usage: fs create <path>");
                return;
            }

            File.Create(path).Dispose();

            context.Output.WriteLine(
                $"File created: {path}");
        }

        private static void CreateDirectory(
            IArgumentCollection args,
            IConsoleCommandContext context)
        {
            var path = args.GetAt(1);

            if (string.IsNullOrWhiteSpace(path))
            {
                context.Output.WriteLine(
                    "Usage: fs mkdir <path>");
                return;
            }

            Directory.CreateDirectory(path);

            context.Output.WriteLine(
                $"Directory created: {path}");
        }

        private static void Delete(
            IArgumentCollection args,
            IConsoleCommandContext context)
        {
            var path = args.GetAt(1);

            if (string.IsNullOrWhiteSpace(path))
            {
                context.Output.WriteLine(
                    "Usage: fs delete <path>");
                return;
            }

            if (File.Exists(path))
            {
                File.Delete(path);

                context.Output.WriteLine(
                    $"File deleted: {path}");

                return;
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);

                context.Output.WriteLine(
                    $"Directory deleted: {path}");

                return;
            }

            context.Output.WriteLine(
                $"Path does not exist: {path}");
        }

        private static void Rename(
            IArgumentCollection args,
            IConsoleCommandContext context)
        {
            var source = args.GetAt(1);
            var destination = args.GetAt(2);

            if (string.IsNullOrWhiteSpace(source) ||
                string.IsNullOrWhiteSpace(destination))
            {
                context.Output.WriteLine(
                    "Usage: fs rename <source> <destination>");
                return;
            }

            if (File.Exists(source))
            {
                File.Move(source, destination);

                context.Output.WriteLine(
                    $"File renamed: {source} -> {destination}");

                return;
            }

            if (Directory.Exists(source))
            {
                Directory.Move(source, destination);

                context.Output.WriteLine(
                    $"Directory renamed: {source} -> {destination}");

                return;
            }

            context.Output.WriteLine(
                $"Source does not exist: {source}");
        }

        private void Random(IArgumentCollection args, IConsoleCommandContext context)
        {
            throw new NotImplementedException();
        }

        private static void WriteUsage(
            IConsoleCommandContext context)
        {
            context.Output.WriteLine(
                "Usage:");

            context.Output.WriteLine(
                "  fs create <path>");

            context.Output.WriteLine(
                "  fs mkdir <path>");

            context.Output.WriteLine(
                "  fs delete <path>");

            context.Output.WriteLine(
                "  fs rename <source> <destination>");
        }
    }
}
