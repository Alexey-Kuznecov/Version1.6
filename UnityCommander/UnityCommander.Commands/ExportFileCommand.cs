
using IconMaker.Core.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UnityCommander.Abstractions.Icons;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Parsing;
using UnityCommander.Core.Binary;


namespace UnityCommander.Commands
{
    [ConsoleCommand("export", "Выводит список открытых файлов указанного процесса по имени.", "procof")]
    public class ExportFileCommand : IConsoleCommand
    {
        private ICommandArgumentParser _parser;

        public string Name => "export";

        public string Description => "Выводит список открытых файлов указанного процесса по имени.";

        public IEnumerable<string> Aliases => ["export"];

        public CommandExecutionMode Mode 
            => CommandExecutionMode.Background;

        public ExportFileCommand(
            ICommandArgumentParser parse)
        {
            _parser = parse;
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            var args = _parser.Parse(context.Arguments);
            var output = context.Output;

            var icons = new Dictionary<string, RuntimeIcon>(StringComparer.OrdinalIgnoreCase);

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var name in Enum.GetNames(typeof(PackIconKind)))
                {
                    var kind = Enum.Parse<PackIconKind>(name);

                    var icon = new PackIcon { Kind = kind };

                    icon.Measure(new Size(100, 100));
                    icon.Arrange(new Rect(0, 0, 100, 100));

                    icons[name] = new RuntimeIcon
                    {
                        Key = name,
                        Data = icon.Data,
                        Color = null
                    };
                }
            });

            var duplicates = Enum.GetValues<PackIconKind>()
                .GroupBy(x => x)
                .Where(g => g.Count() > 1);

            output.Write(icons.Count().ToString());

            var path = Path.Combine(@"G:\", "material.iconpack");
            //icons = IconPackBinaryReader.Load(path);

            IconPackBinaryWriter.Save(path, icons);
        }

        private static void ExportJson()
        {
            var icons = new List<IconDefinition>();

            const int packSize = 1000;

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (PackIconKind kind in Enum.GetValues(typeof(PackIconKind)))
                {
                    var icon = new PackIcon { Kind = kind };

                    icon.Measure(new System.Windows.Size(100, 100));
                    icon.Arrange(new Rect(0, 0, 100, 100));

                    icons.Add(new IconDefinition
                    {
                        Id = Guid.NewGuid(),
                        Name = kind.ToString(),
                        Scale = 1,
                        Background = "#000000",
                        Foreground = "#FFFFFF",
                        Layers = new List<IconLayer>()
                        {
                            new IconLayer()
                            {
                                Geometry = icon.Data,
                                Fill = "#FF8CB072",
                                Order = 1,
                            }
                        },
                    });
                }
            });

            var chunks = icons
               .Select((icon, index) => new { icon, index })
               .GroupBy(x => x.index / packSize)
               .Select(g => g.Select(x => x.icon).ToList())
               .ToList();

            for (int i = 0; i < chunks.Count; i++)
            {
                var pack = new IconPack(
                    $"material-0{i + 1}",
                    $"Material 0{i + 1}",
                    chunks[i]);

                var path = Path.Combine(@"G:\", $"material-{i + 1}.json");

                File.WriteAllText(
                    path,
                    JsonSerializer.Serialize(
                        pack,
                        new JsonSerializerOptions
                        {
                            WriteIndented = true
                        }));
            }
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
