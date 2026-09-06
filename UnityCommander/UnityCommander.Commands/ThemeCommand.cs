
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Parsing;
using UnityCommander.Theme;

namespace UnityCommander.Commands
{
    [ConsoleCommand("theme", "Выводит список открытых файлов указанного процесса по имени.", "procof")]
    public class ThemeCommand : IConsoleCommand
    {
        private ICommandArgumentParser _parser;

        public string Name => "theme";

        public string Description => "Выводит список открытых файлов указанного процесса по имени.";

        public IEnumerable<string> Aliases => ["th"];

        public ThemeCommand(
          ICommandArgumentParser parse)
        {
            _parser = parse;
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            var args = _parser.Parse(context.Arguments);

            var theme = args.GetAt(0);

            if (theme == "Light") 
            {
                ThemeManager.SetTheme("Light");
            }
            else
            {
                ThemeManager.SetTheme("Dark");
            }
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
