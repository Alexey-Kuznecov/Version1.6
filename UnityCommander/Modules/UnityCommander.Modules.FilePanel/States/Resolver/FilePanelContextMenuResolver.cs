
using System.Linq;
using UnityCommander.CommandSurface;
using UnityCommander.Modules.FilePanel.States.Resolver.Models;

namespace UnityCommander.Modules.FilePanel.States.Resolver
{
    public class FilePanelContextMenuResolver
      : IContextMenuResolver
    {
        public bool CanResolve(object context)
        {
            return context is BaseNodeContext;
        }

        public SurfaceContext Resolve(
            object context,
            object parameter)
        {
            var state = (BaseNodeContext)context;

            var result = new SurfaceContext();

            result.Set(new FileContextMenu
            {
                CurrentPath = state.Current,
                SelectedPaths = state.SelectedItems
                    .Select(x => x.Path)
                    .ToList()
            });

            return result;
        }
    }
}
