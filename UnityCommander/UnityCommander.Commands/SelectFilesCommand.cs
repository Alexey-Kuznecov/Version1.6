using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.Services.Interfaces;
using UnityCommander.Common.Selection;

namespace UnityCommander.Commands
{
    [ConsoleCommand("select", "Выделяет файлы по расширению или регулярному выражению.", "sel", "pick")]
    public class SelectFilesCommand : IConsoleCommand
    {
        public string Name => "select";
        public string Description => "Выделяет файлы по расширению или регулярному выражению.";
        public IEnumerable<string> Aliases => new[] { "sel", "pick" };

        private readonly IPanelRegistry _panelRegistry;
        private readonly ISelectionService _selectionService;

        public SelectFilesCommand(IPanelRegistry panelRegistry, ISelectionService selectionService)
        {
            _panelRegistry = panelRegistry;
            _selectionService = selectionService;
        }

        public Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            var args = context.Arguments;

            if (args.Length < 2)
            {
                context.Output.WriteLine("Использование:");
                context.Output.WriteLine("  select extension .txt,.jpg [--panel <token>]");
                context.Output.WriteLine("  select regex \"^img_.*\",\"\\.log$\" [--panel <token>]");
                return Task.CompletedTask;
            }

            // Определяем токен панели, если передан
            string? panelId = null;
            var argList = args.ToList();
            int panelIndex = argList.FindIndex(a => a.Equals("--panel", StringComparison.OrdinalIgnoreCase));
            if (panelIndex >= 0 && panelIndex + 1 < argList.Count)
            {
                panelId = argList[panelIndex + 1];
                // убираем из аргументов
                argList.RemoveAt(panelIndex + 1);
                argList.RemoveAt(panelIndex);
                args = argList.ToArray();
            }

            var mode = args[0].ToLower();
            var rawValue = args[1];

            // Получаем менеджер через SelectionService
            var manager = panelId != null ? _selectionService.Get(panelId) : _selectionService.GetActive();
            if (manager == null)
            {
                context.Output.Write("Не найден менеджер выделения для указанной панели.");
                return Task.CompletedTask;
            }

            switch (mode)
            {
                case "extension":
                case "ext":
                    SelectByExtension(context, manager, rawValue);
                    break;

                case "regex":
                case "re":
                    SelectByRegex(context, manager, rawValue);
                    break;

                default:
                    context.Output.Write($"Неизвестный режим: {mode}. Ожидается: extension или regex.");
                    break;
            }

            return Task.CompletedTask;
        }

        private void SelectByExtension(IConsoleCommandContext context, ISelectionManager manager, string extensionsList)
        {
            context.Output.WriteLine($"Выделение по расширениям: {extensionsList}");

            var panel = _panelRegistry.GetActivePanel();
            var allItems = panel.GetCurrentDirectoryFiles(); // BaseDirectory

            if (allItems == null)
            {
                context.Output.WriteLine($"Файлы или папки не найдены");
                return;
            }

            // создаём контекст на все элементы панели
            var ctx = new SelectionContext(allItems.Cast<ISelectableItem>());

            // формируем action с параметром (строкой расширений)
            var action = new SelectionAction
            {
                Type = SelectionActionType.SelectByExtension,
                Parameter = extensionsList // тут просто строка с расширениями
            };

            // передаём в менеджер
            manager.Handle(ctx, action);

            // выводим количество выделенных элементов
            context.Output.WriteLine($"Выделено файлов: {ctx.Items.Count(i => i.IsSelected)}");
        }

        private void SelectByRegex(IConsoleCommandContext context, ISelectionManager manager, string regexList)
        {
            var patterns = regexList
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim(' ', '"'))
                .ToList();

            List<Regex> compiled = new();

            foreach (var pattern in patterns)
            {
                try
                {
                    compiled.Add(new Regex(pattern, RegexOptions.IgnoreCase));
                }
                catch (Exception ex)
                {
                    context.Output.WriteLine($"Ошибка в регулярке '{pattern}': {ex.Message}");
                    return;
                }
            }

            //context.Output.WriteLine($"Выделение по шаблонам: {string.Join(", ", patterns)}");

            //var allFiles = null;

            //var selected = allFiles
            //    .Where(f => compiled.Any(r => r.IsMatch(f.Name)))
            //    .ToList();

            //manager.Select(selected);

            //context.Output.WriteLine($"Выделено файлов: {selected.Count}");
        }

        public Task FinalizeAsync() => Task.CompletedTask;
    }
}