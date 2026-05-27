
using System.Collections.Generic;
using System.Linq;
using UnityCommander.CommandSurface;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Controllers
{
    public class FilePanelContextResolver
      : IContextResolver
    {
        public SurfaceContext Resolve(
            TabState state,
            object? parameter)
        {
            var ctx = new SurfaceContext();

            switch (parameter)
            {
                case BaseDirectory dir:
                    var selected = state.SelectedItems?.Select(x => x.Path).ToList()
                           ?? new List<string>();

                    if (selected.Count == 0)
                        selected.Add(dir.Path);

                    ctx.Set(new FilePanelContext
                    {
                        CurrentPath = state.CurrentDirectory,
                        SelectedFiles = selected
                    });

                    break;

                default:
                    ctx.Set(new FilePanelContext
                    {
                        CurrentPath = state.CurrentDirectory,
                        SelectedFiles = state.SelectedItems
                            .Select(x => x.Path)
                            .ToList()
                    });
                    break;
            }

            return ctx;
        }
    }
}
