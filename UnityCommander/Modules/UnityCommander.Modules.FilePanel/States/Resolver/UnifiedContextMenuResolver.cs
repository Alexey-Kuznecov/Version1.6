
using System.Linq;
using UnityCommander.Abstractions.Panels;
using UnityCommander.CommandSurface;

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

            if (items.Count == 1)
            {
                var item = items[0];

                if (item is IFileItem)
                {
                    result.Set(new FilePanelContextMenu
                    {
                        CurrentPath = state.Current,
                        SelectedPaths = [item.Path]
                    });

                    return result;
                }

                if (item is IFolderItem)
                {
                    result.Set(new FilePanelContextMenu
                    {
                        CurrentPath = state.Current,
                        SelectedPaths = [item.Path]
                    });

                    return result;
                }
            }

            result.Set(new FilePanelContextMenu
            {
                CurrentPath = state.Current,
                SelectedPaths = items
                    .Select(x => x.Path)
                    .ToList()
            });

            return result;
        }
    }
}
