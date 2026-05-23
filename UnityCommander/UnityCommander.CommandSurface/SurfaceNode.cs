
namespace UnityCommander.CommandSurface
{
    public class SurfaceNode
    {
        public string Title { get; set; }
        public string? CommandName { get; set; } // null = группа

        public List<SurfaceNode> Children { get; } = new();
    }
}
