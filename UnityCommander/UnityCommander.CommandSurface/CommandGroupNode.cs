namespace UnityCommander.CommandSurface
{
    public class CommandGroupNode : CommandSurfaceNode
    {
        public List<CommandSurfaceNode> Children { get; set; } = new();
    }
}
