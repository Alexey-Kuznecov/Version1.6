
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.AI.ImageSearch;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.Commands.Helper;

namespace UnityCommander.Commands
{
    [ConsoleCommand("findsimilar", "Поиск схожих картинок", "fsim")]
    public class FindSimilarCommand : IConsoleCommand
    {
        public string Name => "findsimilar";
        public string Description => "Поиск схожих картинок";
        public IEnumerable<string> Aliases => new[] { "fsim" };

        public IConsoleOutput _output;

        private readonly IImageSimilarityService _similarity;

        public FindSimilarCommand(IImageSimilarityService similarity)
        {
            _similarity = similarity;

            ImageSearchLogger.OnLog += message =>
            {
                AIMessage($"[AYA {message.Level}] {message.Message}");
            };
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            _output = context.Output;
            var aliasMap = new Dictionary<string, string[]>
            {
                { "image", new[] { "--image", "--img", "-i" } },
                { "folder", new[] { "--folder", "--dir", "-d" } },
                { "top", new[] { "--top", "-t" } },
            };

            var parser = new ArgumentParser(context.Arguments.ToArray(), aliasMap);
            string image = parser.GetValue("image");
            string folder = parser.GetValue("folder");
            string topStr = parser.GetValue("top") ?? "10";
            int top = int.TryParse(topStr, out var t) ? t : 10;

            
            if (image == null || folder == null)
            {
                _output.WriteError("Использование: findsimilar --image path --folder path [--top 10]");
                return;
            }

            if (!File.Exists(image))
            {
                _output.WriteError("Файл не найден: " + image);
                return;
            }

            if (!Directory.Exists(folder))
            {
                _output.WriteError("Папка не найдена: " + folder);
                return;
            }

            var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
             .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"))
             .ToList();

            var results = _similarity.FindSimilarImages(image, files, top);

            _output.Write($"Проверяется файл: {image}");
            _output.Write("Результаты:");

            foreach (var r in results)
            {
                _output.Write($"{r.path} — {r.score * 100:F1}%");
            }
        }

        public void AIMessage(string message)
        {
            _output.Write(message);
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
