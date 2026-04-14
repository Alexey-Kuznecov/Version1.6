
using CommandSystem.Abstractions;
using System;
using System.Threading.Tasks;

namespace UnityCommander.Modules.FilePanel
{
    using Services.Interfaces;

    public class FilePanelCommandProvider
    {
        private readonly IDockingService _dockingService;

        public FilePanelCommandProvider(IDockingService dockingService)
        {
            _dockingService = dockingService;
        }

        public Action<CommandContext> GetCurrentPathCommand => ctx =>
        {
            var path = _dockingService.GetActiveTabPath();
            ctx.Result = path;
        };

        public Func<CommandContext, Task<UndoToken?>> SetCurrentPathCommand => async ctx =>
        {
            var value = ctx.Parameter?.ToString();
            var panel = _dockingService.GetActiveDirectoryPanel();

            panel?.SetCurrentPath(value);

            return null; // 🔥 ВОТ И ВСЁ
        };

        // В будущем сюда можно добавлять команды, которые не имеют отношения к FilePanelModule
    }
}
