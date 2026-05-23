
namespace UnityCommander.CommandSurface
{
    public interface ICommandSurfaceProvider
    {
        IEnumerable<CommandSurfaceNode> GetNodes(CommandSurfaceContext context);
    }
}
