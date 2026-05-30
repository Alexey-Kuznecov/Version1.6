
using System.Collections.Generic;
using System.Linq;
using UnityCommander.CommandSurface;

namespace UnityCommander.Modules.FilePanel.States.Resolver
{
    public class ContextResolverDispatcher
    {
        private readonly IReadOnlyList<IContextResolver>
            _resolvers;

        public ContextResolverDispatcher(
            IEnumerable<IContextResolver> resolvers)
        {
            _resolvers = resolvers.ToList();
        }

        public SurfaceContext Resolve(
            object context,
            object? parameter)
        {
            var resolver = _resolvers
                .First(x => x.CanResolve(context));

            return resolver.Resolve(
                context,
                parameter);
        }
    }
}
