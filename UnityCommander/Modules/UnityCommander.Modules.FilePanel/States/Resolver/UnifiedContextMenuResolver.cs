
using System.Linq;
using UnityCommander.Abstractions.Panels;
using UnityCommander.CommandSurface;
using UnityCommander.Modules.FilePanel.States.Resolver.Models;

namespace UnityCommander.Modules.FilePanel.States.Resolver
{
    public sealed class UnifiedContextMenuResolver : IContextMenuResolver
    {
        public bool CanResolve(object context)
        {
            return context is UnifiedNodeContext;
        }

        public SurfaceContext Resolve(
            object context,
            object parameter)
        {
            var state = (UnifiedNodeContext)context;

            var result = new SurfaceContext();

            var items = parameter switch
            {
                IDirectoryItem item =>
                    [item],

                System.Collections.IEnumerable collection =>
                    collection
                        .Cast<object>()
                        .OfType<IDirectoryItem>()
                        .ToList(),

                _ =>
                    []
            };

            var paths = items
                .Select(x => x.Path)
                .ToList();

            result.Set(new FilePanelContextMenu
            {
                CurrentPath = state.Current,
                SelectedPaths = items
                    .Select(x => x.Path)
                    .ToList()
            });

            if (items.All(x => x is IFileItem))
            {
                result.Set(new FileContextMenu
                {
                    CurrentPath = state.Current,
                    SelectedPaths = paths
                });
            }

            else if (items.All(x => x is IFolderItem))
            {
                result.Set(new FolderContextMenu
                {
                    CurrentPath = state.Current,
                    SelectedPaths = paths
                });
            }

            return result;
        }
    }
}
