
namespace UnityCommander.CommandSurface
{
    public class FileSurfaceProvider : ICommandSurfaceProvider
    {
        public IEnumerable<CommandSurfaceNode> GetNodes(CommandSurfaceContext context)
        {
            var selected = context.CommandContext.Get<List<string>>();

            if (selected == null || selected.Count == 0)
                yield break;

            yield return new CommandGroupNode
            {
                Title = "File",
                Children =
                {
                    new CommandActionNode
                    {
                        Title = "Delete",
                        CommandName = "FileDelete"
                    },
                    new CommandActionNode
                    {
                        Title = "Copy",
                        CommandName = "FileCopy"
                    }
                }
            };
        }
    }
}
