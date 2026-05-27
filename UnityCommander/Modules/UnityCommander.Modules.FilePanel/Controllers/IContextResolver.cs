
using UnityCommander.CommandSurface;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Controllers
{
    public interface IContextResolver
    {
        SurfaceContext Resolve(
            TabState state,
            object? parameter);
    }
}
