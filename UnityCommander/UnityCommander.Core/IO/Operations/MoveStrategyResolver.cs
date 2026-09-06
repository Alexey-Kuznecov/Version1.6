
using System.Collections.Generic;
using System.Linq;

namespace UnityCommander.Core.IO.Operations
{
    public sealed class MoveStrategyResolver
    {
        private readonly IReadOnlyCollection<IMoveStrategy> _strategies;

        public MoveStrategyResolver(
            IEnumerable<IMoveStrategy> strategies)
        {
            _strategies = strategies.ToList();
        }

        public IMoveStrategy Resolve(
            string source,
            string destination)
        {
            return _strategies.First(
                x => x.CanHandle(source, destination));
        }
    }
}
