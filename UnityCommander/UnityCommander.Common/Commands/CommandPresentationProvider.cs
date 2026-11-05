
using System.Collections.Generic;

namespace UnityCommander.Common.Commands
{
    public class CommandPresentationProvider
    {
        private static readonly Dictionary<string, CommandPresentation> _map =
            new()
            {
                [CommandNames.Navigation.GoUp] = new(
                    "Перейти вверх",
                    "Перейти в родительский каталог текущей папки"
                ),

                [CommandNames.ToolBar.Create] = new(
                    "Добавить инструмент",
                    "Добавить инструмент на панель инструментов"
                ),

                // Layout commands
                [CommandNames.UI.ToggleBottomPanel] = new(
                    "Переключить нижнюю панель",
                    "Скрывает/показывает нижнюю панель"
                ),

                [CommandNames.UI.ToggleRibbon] = new(
                    "Переключить ленту",
                    "Скрывает/показывает ленту инструментов"
                ),

                [CommandNames.UI.ShowSettings] = new(
                    "Открыть настройки",
                    "Открывает окно настроек приложения"
                ),

                [CommandNames.UI.ToggleSidebar] = new(
                    "Переключить боковую панель",
                    "Показывает или скрывает боковую панель (Sidebar)"
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


                [CommandNames.Panel.SelectFilesAll] = new(
                    "Выделить все файлы",
                    "Выделить все файлы в текущем каталоге"
                ),

                [CommandNames.Panel.SelectFoldersAll] = new(
                    "Выделить все папки",
                    "Выделить все папки в текущем каталоге"
                ),

                [CommandNames.Panel.SelectAll] = new(
                    "Выделить все элементы",
                    "Выделить все элементы в текущей панели"
                ),

                // File commands
                [CommandNames.Item.Delete] = new(
                    "Удалить",
                    "Удалить файл/папку"
                ),

                // File commands
                [CommandNames.Item.Rename] = new(
                    "Переименовать",
                    "Переименовать выбранный файл или папку"
                ),

                // Clipboard commands
                [CommandNames.Clipboard.Copy] = new(
                    "Копировать",
                    "Копировать выбранные файлы и папки в буфер обмена"
                ),

                [CommandNames.Clipboard.Paste] = new(
                    "Вставить",
                    "Вставить файлы и папки из буфера обмена в текущую папку"
                ),

                [CommandNames.Clipboard.Cut] = new(
                    "Вырезать",
                    "Вырезать выбранные файлы и папки и переместить их в текущую папку при вставке"
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
