
using System.Collections.Concurrent;

namespace UnityCommander.Abstractions.IO
{
    public class OperationIndex : IOperationIndex
    {
        private ConcurrentDictionary<string, Guid> _pathToOp = new();
        private ConcurrentDictionary<Guid, IOperation> _ops = new();
        private readonly ConcurrentDictionary<Guid, (string Source, string Destination)> _opToPaths = new();

        public void Register(IOperation op, IEnumerable<string> paths)
        {
            var list = paths.ToList();

            foreach (var item in op.Items)
            {
                _ops[item.Id] = op;

                _opToPaths[item.Id] = (item.SourcePath, item.DestinationPath);

                _pathToOp[item.SourcePath] = item.Id;
                _pathToOp[item.DestinationPath] = item.Id;
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

        public bool TryGetItem(string path, out FileTransferItem item)
        {
            if (_pathToOp.TryGetValue(path, out var itemId) &&
                _ops.TryGetValue(itemId, out var op))
            {
                item = op.Items.First(i => i.Id == itemId);
                return true;
            }

            item = null;
            return false;
        }

        public void Unregister(Guid id)
        {
            _ops.TryRemove(id, out _);

            if (_opToPaths.TryRemove(id, out var paths))
            {
                _pathToOp.TryRemove(paths.Source, out _);
                _pathToOp.TryRemove(paths.Destination, out _);
            }
        }
    }
}
