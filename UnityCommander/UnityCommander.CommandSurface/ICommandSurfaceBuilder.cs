using CommandSystem.Abstractions;

namespace UnityCommander.CommandSurface
{
    public interface ICommandSurfaceBuilder
    {
        CommandGroupNode Build(SurfaceContext context);
    }
}
