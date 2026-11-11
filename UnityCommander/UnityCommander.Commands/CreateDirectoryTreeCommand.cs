using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Commands
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityCommander.CLI.Core;
    using UnityCommander.CLI.Integration;
    using UnityCommander.CLI.Mode;
    using UnityCommander.Commands.Parsing;

    [ConsoleCommand("tree", "Создать снимок структуры директории")]
    public sealed class CreateTreeCommand : IConsoleCommand
    {
        private readonly ICommandArgumentParser _parser;

        private static readonly HashSet<string> ExcludedDirectories =
            new(StringComparer.OrdinalIgnoreCase)
            {
            ".git",
            ".vs",
            ".idea",
            "bin",
            "obj"
            };

        public string Name => "tree";

        public string Description =>
            "Создать снимок структуры директории";

        public CommandExecutionMode Mode =>
            CommandExecutionMode.Immediate;

        public CreateTreeCommand(ICommandArgumentParser parser)
        {
            _parser = parser;
        }

        public async Task ExecuteAsync(
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var args = _parser.Parse(context.Arguments);

            var path = args.GetAt(0);

            if (string.IsNullOrWhiteSpace(path))
            {
                context.Output.WriteLine(
                    "Usage: tree <directory-path>");

                return;
            }

            if (!Directory.Exists(path))
            {
                context.Output.WriteLine(
                    $"Directory not found: {path}");

                return;
            }

            try
            {
                var root = new DirectoryInfo(path);
                var builder = new StringBuilder();

                builder.AppendLine($"{root.Name}/");

                BuildTree(
                    root.FullName,
                    string.Empty,
                    builder,
                    cancellationToken);

                var snapshot = builder.ToString();

                foreach (var line in snapshot.Split(
                             Environment.NewLine,
                             StringSplitOptions.RemoveEmptyEntries))
                {
                    context.Output.WriteLine(line);
                }

                await Task.CompletedTask;
            }
            catch (OperationCanceledException)
            {
                context.Output.WriteLine("Directory snapshot cancelled.");
            }
            catch (Exception ex)
            {
                context.Output.WriteLine(
                    $"Failed to create directory snapshot: {ex.Message}");
            }
        }

        private static void BuildTree(
            string directoryPath,
            string indent,
            StringBuilder builder,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            FileSystemInfo[] entries;

            try
            {
                entries = new DirectoryInfo(directoryPath)
                    .GetFileSystemInfos();
            }
            catch (UnauthorizedAccessException)
            {
                builder.AppendLine($"{indent}└── [Access denied]");
                return;
            }
            catch (IOException)
            {
                builder.AppendLine($"{indent}└── [I/O error]");
                return;
            }

            var items = entries
                .Where(item =>
                    !(item is DirectoryInfo &&
                      ExcludedDirectories.Contains(item.Name)))
                .OrderByDescending(item => item is DirectoryInfo)
                .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var item = items[i];

                bool isLast = i == items.Length - 1;
                bool isDirectory = item is DirectoryInfo;

                string connector = isLast ? "└── " : "├── ";

                builder.AppendLine(
                    $"{indent}{connector}{item.Name}{(isDirectory ? "/" : string.Empty)}");

                if (!isDirectory)
                    continue;

                if ((item.Attributes & FileAttributes.ReparsePoint) != 0)
                    continue;

                string childIndent =
                    indent + (isLast ? "    " : "│   ");

                BuildTree(
                    item.FullName,
                    childIndent,
                    builder,
                    cancellationToken);
            }
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
