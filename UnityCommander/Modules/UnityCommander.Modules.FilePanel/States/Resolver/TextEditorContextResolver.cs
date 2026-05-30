
using UnityCommander.CommandSurface;

namespace UnityCommander.Modules.FilePanel.States.Resolver
{
    public class TextEditorContextResolver
    : IContextResolver
    {
        public bool CanResolve(object context)
        {
            //return context is TextEditorNodeContext;
            return false;
        }

        public SurfaceContext Resolve(
            object context,
            object? parameter)
        {
            return null;
        }
    }
}
