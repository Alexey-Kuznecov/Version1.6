
using UnityCommander.Common.Diagnostic;
using UnityCommander.Index.Abstractions;
using UnityCommander.Index.Models;

namespace UnityCommander.Index.Indexing
{
    public sealed class FileIndexChangeQueue : IFileIndexChangeQueue, IDiagnosticReporter
    {
        private readonly Dictionary<string, IndexChangeType> _changes =
            new(StringComparer.OrdinalIgnoreCase);

        public string Name => "file.index.change.queue";

        public DiagnosticCardinality Cardinality => DiagnosticCardinality.Single;

        public FileIndexChangeQueue(IDiagnosticRegistry registry)
        {
            registry.Register(this);
        }

        public int Count => _changes.Count;

        public void Enqueue(IndexChange change)
        {
            if (!_changes.TryGetValue(change.Path, out var existing))
            {
                _changes[change.Path] = change.Type;
                return;
            }

            var merged = Merge(existing, change.Type);

            if (merged.HasValue)
                _changes[change.Path] = merged.Value;
            else
                _changes.Remove(change.Path);
        }

        public bool TryDequeue(out IndexChange? change)
        {
            if (_changes.Count == 0)
            {
                change = null;
                return false;
            }

            var item = _changes.First();

            _changes.Remove(item.Key);

            change = new IndexChange(
                item.Key,
                item.Value);

            return true;
        }

        private static IndexChangeType? Merge(
            IndexChangeType current,
            IndexChangeType incoming)
        {
            return (current, incoming) switch
            {
                // Файл появился и потом изменился.
                // Для индекса это всё равно Add.
                (IndexChangeType.Created, IndexChangeType.Changed)
                    => IndexChangeType.Created,

                // Создали и удалили до обработки.
                // Нечего индексировать.
                (IndexChangeType.Created, IndexChangeType.Deleted)
                    => null,

                // Несколько изменений подряд.
                (IndexChangeType.Changed, IndexChangeType.Changed)
                    => IndexChangeType.Changed,

                // Изменился и был удалён.
                (IndexChangeType.Changed, IndexChangeType.Deleted)
                    => IndexChangeType.Deleted,

                // Удалили и снова создали.
                // Конечное состояние — объект существует,
                // поэтому его надо заново привести в соответствие.
                (IndexChangeType.Deleted, IndexChangeType.Created)
                    => IndexChangeType.Changed,

                // После удаления ещё пришёл Changed.
                (IndexChangeType.Deleted, IndexChangeType.Changed)
                    => IndexChangeType.Changed,

                // Повторное удаление.
                (IndexChangeType.Deleted, IndexChangeType.Deleted)
                    => IndexChangeType.Deleted,

                _ => incoming
            };
        }

        public void Report(IDiagnosticWriter writer)
        {
            writer.BeginTable("File Index Change Queue");

            for (int i = 0; i < _changes.Count; i++)
            {
                var change = _changes.ElementAt(i);
                writer.Row(change.Key, change.Value.ToString());
            }

            writer.Row("Change Count", _changes.Count.ToString());

            writer.EndTable();
        }
    }
}
