
using System.Collections.Generic;

namespace UnityCommander.Common.Commands
{
    public class CommandPresentationProvider
    {
        private static readonly Dictionary<string, CommandPresentation> _map =
            new()
            {
                [CommandNames.UI.ToggleBottomPanel] = new(
                    "Переключить нижнюю панель",
                    "Скрывает/показывает нижнюю панель"
                ),

                // Panel commands
                [CommandNames.Panel.Refresh] = new(
                    "Update directory",
                    "Reload current directory content"
                ),
                [CommandNames.Panel.GetCurrentPath] = new(
                    "Получить текщий путь",
                    "Получает текущий путь директории"
                ),
                [CommandNames.Panel.SetCurrentPath] = new(
                    "Устанавить текущий путь",
                    " Устанавливает текущий путь директории"
                ),

                // File commands
                [CommandNames.File.Delete] = new(
                    "Удалить файл",
                    "Удалить файл/Восстановить файл"
                ),
                                
                [CommandNames.File.OpenInViewer] = new(
                    "Открыть файл",
                    "Открыть файл во внутренем просмоторщике"
                ),

                // History commands
                [CommandNames.History.Undo] = new(
                    "Откатить действие",
                    "Откатить последнее действие (например: восстановить файл который был удален)"
                ),

                [CommandNames.History.Redo] = new(
                    "Повторить действие",
                    "Повторить действие (например: выделить файлы занова если случайно пропало выделение)"
                )
            };

        public static CommandPresentation Get(string commandId)
        {
            if (!_map.TryGetValue(commandId, out var v))
            {
                // лог + fallback
                return new CommandPresentation(commandId, null);
            }

            return v;
        }
    }
}
