using CommandSystem.Abstractions;

namespace UnityCommander.CommandSurface
{
    public class CommandSurfaceContext
    {
        public SurfaceContext CommandContext { get; }

        public CommandSurfaceContext(SurfaceContext commandContext)
        {
            CommandContext = commandContext;
        }
    }
}
