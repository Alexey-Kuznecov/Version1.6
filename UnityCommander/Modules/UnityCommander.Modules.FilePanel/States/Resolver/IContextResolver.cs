using UnityCommander.CommandSurface;

namespace UnityCommander.Modules.FilePanel.States.Resolver
{
    public interface IContextResolver
    {
        bool CanResolve(object context);

        SurfaceContext Resolve(
            object context,
            object parameter);
    }
}
