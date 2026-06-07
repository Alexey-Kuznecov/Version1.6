namespace UnityCommander.CommandSurface
{
    public abstract class CommandSurfaceNode
    {
        public string Title { get; set; } = "";
        public string? Icon { get; set; }
        public int Order { get; set; }
    }
}
