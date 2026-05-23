
using CommandSystem.Abstractions;

namespace UnityCommander.CommandSurface
{
    public class CommandSurfaceEngine
    {
        public List<SurfaceNode> Build(
            IEnumerable<CommandMetadata> commands,
            SurfaceContext context)
        {
            var root = new SurfaceNode { Title = "Root" };

            var filtered = commands.Where(cmd =>
                cmd.ContextTypes.Count == 0 ||
                cmd.ContextTypes.Any(t => context.Has(t)));

            foreach (var cmd in filtered)
            {
                var parts = (cmd.Category ?? "").Split('/', StringSplitOptions.RemoveEmptyEntries);

                var current = root;

                // строим путь
                foreach (var part in parts)
                {
                    var next = current.Children.FirstOrDefault(c => c.Title == part);

                    if (next == null)
                    {
                        next = new SurfaceNode { Title = part };
                        current.Children.Add(next);
                    }

                    current = next;
                }

                // добавляем саму команду
                current.Children.Add(new SurfaceNode
                {
                    Title = cmd.Name, // временно, потом заменим через Presentation
                    CommandName = cmd.Name
                });
            }

            return root.Children;
        }
    }
}
