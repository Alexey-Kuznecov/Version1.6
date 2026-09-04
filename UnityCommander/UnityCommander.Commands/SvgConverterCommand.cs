
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Parsing;
using System.IO;
using System.Linq;
using UnityCommander.Commands.Converters;

namespace UnityCommander.Commands
{
    [ConsoleCommand("svg", "Команда нужна для примера")]
    public class SvgConverterCommand : IConsoleCommand
    {
        private ICommandArgumentParser _parser;

        public string Name => "svg";
        public string Description => "Команда нужна для примера";

        public CommandExecutionMode Mode 
            => CommandExecutionMode.Immediate;

        public SvgConverterCommand(
            ICommandArgumentParser parse)
        {
            _parser = parse;
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            var args = _parser.Parse(context.Arguments);

            string inputDirectory = "C:\\Users\\Алексей\\Downloads\\file-manager-icons\\input";
            string outputDirectory = "C:\\Users\\Алексей\\Downloads\\file-manager-icons\\output";

            Directory.CreateDirectory(inputDirectory);
            Directory.CreateDirectory(outputDirectory);

            var files = Directory
                .EnumerateFiles(inputDirectory, "*.svg", SearchOption.AllDirectories)
                .ToArray();

            if (files.Length == 0)
            {
                Console.WriteLine($"No SVG files found in '{inputDirectory}'.");
                return;
            }

            Console.WriteLine($"Found {files.Length} SVG files.");

            foreach (var inputFile in files)
            {
                var relativePath = Path.GetRelativePath(inputDirectory, inputFile);
                var outputFile = Path.Combine(outputDirectory, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);

                Console.WriteLine($"Converting: {relativePath}");

                try
                {
                    SvgNormalizer.Convert(inputFile, outputFile);
                    Console.WriteLine("  OK");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ERROR: {ex.Message}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Done.");
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
