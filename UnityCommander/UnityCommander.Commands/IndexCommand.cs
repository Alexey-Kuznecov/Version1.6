
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Parsing;
using UnityCommander.Common.Commands;
using UnityCommander.Index.Abstractions;
using UnityCommander.Index.Models;
using UnityCommander.Search.Indexing;

namespace UnityCommander.Commands
{
    [ConsoleCommand("index", "Работа с индексом файловой системы")]
    public sealed class IndexCommand : IConsoleCommand
    {
        private readonly ICommandArgumentParser _parser;
        private readonly IFileIndexService _indexService;

        public string Name => "index";

        public string Description =>
            "Работа с индексом файловой системы";

        public CommandExecutionMode Mode =>
            CommandExecutionMode.Immediate;

        public IndexCommand(
            ICommandArgumentParser parser,
            IFileIndexService indexService)
        {
            _parser = parser;
            _indexService = indexService;
        }

        public async Task ExecuteAsync(
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var args = _parser.Parse(context.Arguments);

            var operation = args.GetAt(0);

            switch (operation)
            {
                case "add":
                    await ExecuteAddAsync(
                        args,
                        context,
                        cancellationToken);
                    break;

                case "get":
                    await ExecuteGetAsync(
                        args,
                        context,
                        cancellationToken);
                    break;

                case "list":
                    await ExecuteListAsync(
                        args,
                        context,
                        cancellationToken);
                    break;

                case "update":
                    await ExecuteUpdateAsync(
                        args,
                        context,
                        cancellationToken);
                    break;

                case "delete":
                    await ExecuteDeleteAsync(
                        args,
                        context,
                        cancellationToken);
                    break;

                default:
                    context.Output.WriteLine(
                        "Usage: index <add|get|list|update|delete> ...");
                    break;
            }
        }

        private async Task ExecuteAddAsync(
            IArgumentCollection args,
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var path = args.GetAt(1);

            if (string.IsNullOrWhiteSpace(path))
            {
                context.Output.WriteLine("Usage: index add <path> [--recursive]");
                return;
            }

            if (!File.Exists(path) && !Directory.Exists(path))
            {
                context.Output.WriteLine(
                    $"Path does not exist: {path}");

                return;
            
            }

            var recursive = args.HasFlag("recursive");

            cancellationToken.ThrowIfCancellationRequested();

            if (recursive)
            {
                await AddRecursiveAsync(path, context, cancellationToken);
                return;
            }

            var id = await AddSingleAsync(path, context, cancellationToken);

            var result = await _indexService.GetAsync(
                id,
                cancellationToken);

            if (result == null)
            {
                context.Output.WriteLine(
                    "ERROR: record was not found after insertion.");

                return;
            }

            WriteFile(context, result);
        }

        private async Task AddRecursiveAsync(
          string path,
          IConsoleCommandContext context,
          CancellationToken cancellationToken)
        {
            if (!Directory.Exists(path))
            {
                context.Output.WriteLine(
                    $"Directory not found: {path}");

                return;
            }

            var directory = new DirectoryInfo(path);

            await AddDirectoryRecursiveAsync(
                directory,
                null,
                context,
                cancellationToken);
        }

        private async Task AddDirectoryRecursiveAsync(
            DirectoryInfo directory,
            long? parentId,
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var directoryFile = CreateIndexedDirectory(directory);

            directoryFile.ParentId = parentId;

            var directoryId = await _indexService.AddAsync(
                directoryFile,
                cancellationToken);

            context.Output.WriteLine(
                $"Indexed: {directory.FullName} (Id: {directoryId}, ParentId: {parentId})");

            var files = new List<IndexedFile>();

            foreach (var file in directory.EnumerateFiles())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var indexedFile = CreateIndexedFile(file);

                indexedFile.ParentId = directoryId;

                files.Add(indexedFile);
            }

            if (files.Count > 0)
            {
                await _indexService.AddRangeAsync(
                    files,
                    cancellationToken);

                foreach (var file in files)
                {
                    context.Output.WriteLine(
                        $"Indexed: {file.Path} (ParentId: {directoryId})");
                }
            }

            foreach (var childDirectory in directory.EnumerateDirectories())
            {
                cancellationToken.ThrowIfCancellationRequested();

                await AddDirectoryRecursiveAsync(
                    childDirectory,
                    directoryId,
                    context,
                    cancellationToken);
            }
        }

        private async Task<long> AddSingleAsync(
            string path,
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var indexed = CreateIndexedFile(path);

            context.Output.WriteLine(
                $"Indexed: {indexed.Path}");

            return await _indexService.AddAsync(
                    indexed,
                    cancellationToken);
        }

        private async Task ExecuteGetAsync(
            IArgumentCollection args,
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var value = args.GetAt(1);

            if (string.IsNullOrWhiteSpace(value))
            {
                context.Output.WriteLine(
                    "Usage: index get <id>");

                return;
            }

            if (!long.TryParse(value, out var id))
            {
                context.Output.WriteLine(
                    $"Invalid ID: {value}");

                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var result = await _indexService.GetAsync(
                id,
                cancellationToken);

            if (result == null)
            {
                context.Output.WriteLine(
                    $"Record not found: {id}");

                return;
            }

            WriteFile(context, result);
        }

        private async Task ExecuteListAsync(
            IArgumentCollection args,
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var count = 0;

            await foreach (var file in _indexService.EnumerateAsync(
                cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                WriteFile(context, file);

                context.Output.WriteLine(
                    "--------------------");

                count++;
            }

            context.Output.WriteLine(
                $"Total: {count}");
        }

        private async Task ExecuteUpdateAsync(
            IArgumentCollection args,
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var value = args.GetAt(1);

            if (string.IsNullOrWhiteSpace(value))
            {
                context.Output.WriteLine(
                    "Usage: index update <id>");

                return;
            }

            if (!long.TryParse(value, out var id))
            {
                context.Output.WriteLine(
                    $"Invalid ID: {value}");

                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var existing = await _indexService.GetAsync(
                id,
                cancellationToken);

            if (existing == null)
            {
                context.Output.WriteLine(
                    $"Record not found: {id}");

                return;
            }

            /*
             * Пока просто меняем запись для проверки UpdateAsync.
             *
             * Позже здесь появятся реальные параметры:
             *
             * --name
             * --path
             * --size
             * --attributes
             * etc.
             */

            existing.Name = $"{existing.Name}_updated";

            await _indexService.UpdateAsync(
                existing,
                cancellationToken);

            context.Output.WriteLine(
                $"Updated: {existing.Id}");

            var result = await _indexService.GetAsync(
                id,
                cancellationToken);

            if (result == null)
            {
                context.Output.WriteLine(
                    "ERROR: record was not found after update.");

                return;
            }

            WriteFile(context, result);
        }

        private async Task ExecuteDeleteAsync(
           IArgumentCollection args,
           IConsoleCommandContext context,
           CancellationToken cancellationToken)
        {
            var value = args.GetAt(1);

            if (string.IsNullOrWhiteSpace(value))
            {
                context.Output.WriteLine(
                    "Usage: index delete <id>");

                return;
            }

            if (!long.TryParse(value, out var id))
            {
                context.Output.WriteLine(
                    $"Invalid ID: {value}");

                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var existing = await _indexService.GetAsync(
                id,
                cancellationToken);

            if (existing == null)
            {
                context.Output.WriteLine(
                    $"Record not found: {id}");

                return;
            }

            await _indexService.DeleteAsync(
                id,
                cancellationToken);

            context.Output.WriteLine(
                $"Deleted: {id}");

            var result = await _indexService.GetAsync(
                id,
                cancellationToken);

            if (result != null)
            {
                context.Output.WriteLine(
                    "ERROR: record still exists after deletion.");

                return;
            }

            context.Output.WriteLine(
                "Verified: record no longer exists.");
        }

        private static void WriteFile(
            IConsoleCommandContext context,
            IndexedFile file)
        {
            context.Output.WriteLine($"ID: {file.Id}");
            context.Output.WriteLine($"Name: {file.Name}");
            context.Output.WriteLine($"Path: {file.Path}");
            context.Output.WriteLine($"Extension: {file.Extension}");
            context.Output.WriteLine($"Directory: {file.IsDirectory}");
            context.Output.WriteLine($"Size: {file.Size}");
            context.Output.WriteLine($"Created: {file.CreationTime}");
            context.Output.WriteLine($"Modified: {file.LastWriteTime}");
            context.Output.WriteLine($"Accessed: {file.LastAccessTime}");
            context.Output.WriteLine($"Attributes: {file.Attributes}");
        }

        private IndexedFile CreateIndexedFile(string path)
        {
            IndexedFile indexed;

            if (File.Exists(path))
            {
                var info = new FileInfo(path);

                indexed = CreateIndexedFile(info);
            }
            else
            {
                var info = new DirectoryInfo(path);

                indexed = CreateIndexedDirectory(info);
            }

            return indexed;
        }

        private static IndexedFile CreateIndexedFile(FileInfo file)
        {
            return new IndexedFile
            {
                Path = file.FullName,
                Name = file.Name,
                Extension = file.Extension,
                IsDirectory = false,
                Size = file.Length,
                CreationTime = file.CreationTime,
                LastWriteTime = file.LastWriteTime,
                LastAccessTime = file.LastAccessTime,
                Attributes = file.Attributes
            };
        }

        private static IndexedFile CreateIndexedDirectory(DirectoryInfo directory)
        {
            return new IndexedFile
            {
                Path = directory.FullName,
                Name = directory.Name,
                Extension = string.Empty,
                IsDirectory = true,
                Size = 0,
                CreationTime = directory.CreationTime,
                LastWriteTime = directory.LastWriteTime,
                LastAccessTime = directory.LastAccessTime,
                Attributes = directory.Attributes
            };
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
