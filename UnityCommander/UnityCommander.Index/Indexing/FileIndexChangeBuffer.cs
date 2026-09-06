
//using UnityCommander.Index.Models;

//namespace UnityCommander.Index.Indexing
//{
//    public sealed class FileIndexChangeBuffer
//    {
//        private readonly Dictionary<string, IndexChangeType> _changes =
//            new(StringComparer.OrdinalIgnoreCase);

//        public void Add(string path, IndexChangeType type)
//        {
//            if (!_changes.TryGetValue(path, out var existing))
//            {
//                _changes[path] = type;
//                return;
//            }

//            var merged = Merge(existing, type);

//            if (merged.HasValue)
//                _changes[path] = merged.Value;
//            else
//                _changes.Remove(path);
//        }

//        public IReadOnlyList<IndexChange> Drain()
//        {
//            var result = _changes
//                .Select(x => new IndexChange(x.Key, x.Value))
//                .ToArray();

//            _changes.Clear();

//            return result;
//        }

//        private static IndexChangeType? Merge(
//            IndexChangeType existing,
//            IndexChangeType incoming)
//        {
//            return (existing, incoming) switch
//            {
//                (IndexChangeType.Add, IndexChangeType.Update)
//                    => IndexChangeType.Add,

//                (IndexChangeType.Add, IndexChangeType.Delete)
//                    => null,

//                (IndexChangeType.Update, IndexChangeType.Update)
//                    => IndexChangeType.Update,

//                (IndexChangeType.Update, IndexChangeType.Delete)
//                    => IndexChangeType.Delete,

//                (IndexChangeType.Delete, IndexChangeType.Add)
//                    => IndexChangeType.Update,

//                (IndexChangeType.Delete, IndexChangeType.Update)
//                    => IndexChangeType.Update,

//                (IndexChangeType.Delete, IndexChangeType.Delete)
//                    => IndexChangeType.Delete,

//                _ => incoming
//            };
//        }
//    }
//}
