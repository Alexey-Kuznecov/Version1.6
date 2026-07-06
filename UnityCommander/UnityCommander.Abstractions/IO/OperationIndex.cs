
using System.Collections.Concurrent;

namespace UnityCommander.Abstractions.IO
{
    public class OperationIndex : IOperationIndex
    {
        private ConcurrentDictionary<string, Guid> _pathToOp = new();
        private ConcurrentDictionary<Guid, IOperation> _ops = new();
        private readonly ConcurrentDictionary<Guid, List<string>> _opToPaths = new();

        public void Register(IOperation op, IEnumerable<string> paths)
        {
            var list = paths.ToList();

            _ops[op.Id] = op;
            _opToPaths[op.Id] = list;

            foreach (var path in list)
            {
                _pathToOp[path] = op.Id;
            }
        }

        public bool TryGetOperation(string path, out IOperation op)
        {
            if (_pathToOp.TryGetValue(path, out var id))
            {
                return _ops.TryGetValue(id, out op);
            }

            op = null;
            return false;
        }

        public void Unregister(Guid id)
        {
            _ops.TryRemove(id, out _);

            if (_opToPaths.TryRemove(id, out var paths))
            {
                foreach (var path in paths)
                {
                    _pathToOp.TryRemove(path, out _);
                }
            }
        }
    }
}
