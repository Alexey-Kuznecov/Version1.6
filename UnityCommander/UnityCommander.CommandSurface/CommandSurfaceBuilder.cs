
using CommandSystem.Abstractions;

namespace UnityCommander.CommandSurface
{
    public class CommandSurfaceBuilder : ICommandSurfaceBuilder
    {
        private readonly IEnumerable<ICommandSurfaceProvider> _providers;

        public CommandSurfaceBuilder(IEnumerable<ICommandSurfaceProvider> providers)
        {
            _providers = providers;
        }

        public CommandGroupNode Build(SurfaceContext context)
        {
            var root = new CommandGroupNode
            {
                Title = "Root"
            };

            foreach (var provider in _providers)
            {
                var nodes = provider.GetNodes(new CommandSurfaceContext(context));

                foreach (var node in nodes)
                    root.Children.Add(node);
            }

            return Normalize(root);
        }

        private CommandGroupNode Normalize(CommandGroupNode root)
        {
            // потом сюда добавим:
            // - сортировку
            // - grouping
            // - recent/favorites
            return root;
        }
    }
}
