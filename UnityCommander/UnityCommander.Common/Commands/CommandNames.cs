
namespace UnityCommander.Common.Commands
{
    public static class CommandNames
    {
        public static class Directory
        {
            public const string Create = "directory.create";
            public const string Delete = "directory.delete";
            public const string Rename = "directory.rename";
        }

        public static class Command
        {
            public const string Execute = "command.execute";
            public const string List = "command.list";
            public const string Help = "command.help";
        }

        public static class File
        {
            public const string Copy = "file.copy";
            public const string Move = "file.move";
            public const string Delete = "file.delete";
            public const string Rename = "file.rename";

            public const string CreateFile = "file.create";
            //public const string CreateDirectory = "file.create-directory";

            public const string Open = "file.open";
            public const string OpenWith = "file.open-with";
            public const string OpenInViewer = "file.open-in-viewer"; // ContentViewer

            public const string GetInfo = "file.get-info";
            public const string SetAttributes = "file.set-attributes";
        }

        public static class History
        {
            public const string Undo = "history.undo";
            public const string Redo = "history.redo";
            public const string Clear = "history.clear";
        }

        public static class Panel
        {
            public const string GetCurrentItem = "panel.get-current-item";
            public const string SetCurrentItem = "panel.set-current-item";

            public const string GetCurrentPath = "panel.get-current-path";
            public const string SetCurrentPath = "panel.set-current-path";

            public const string GetSelectedItems = "panel.get-selected-items";
            public const string SelectItem = "panel.select-item";
            public const string UnselectItem = "panel.unselect-item";
            public const string ClearSelection = "panel.clear-selection";

            public const string ChangeDirectory = "panel.change-directory";
            public const string Refresh = "panel.refresh";

            public const string Focus = "panel.focus";
            public const string Switch = "panel.switch"; // левая/правая
        }

        public static class Plugin
        {
            public const string Load = "plugin.load";
            public const string Unload = "plugin.unload";
            public const string Reload = "plugin.reload";
            public const string List = "plugin.list";
        }

        public static class Process
        {
            public const string Kill = "process.kill";
        }

        public static class Search
        {
            public const string Find = "search.find";
            public const string Filter = "search.filter";
        }

        public static class System
        {
            public const string Execute = "system.execute";
            public const string OpenTerminal = "system.open-terminal";

            public const string ProcessKill = "system.process.kill";
            public const string ProcessSuspend = "system.process.suspend";
            public const string ProcessResume = "system.process.resume";
            public const string ProcessInfo = "system.process.info";
        }

        public static class UI
        {
            public const string ShowContextMenu = "ui.show-context-menu";
            public const string ShowDialog = "ui.show-dialog";
            public const string ShowMessage = "ui.show-message";
            public const string ToggleBottomPanel = "ui.toggle.bottom.panel";
        }
    }
}
