
using UnityCommander.CommandSurface;

namespace UnityCommander.Modules.FilePanel.States.Resolver
{
    public class TextEditorContextMenuResolver
    : IContextMenuResolver
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
