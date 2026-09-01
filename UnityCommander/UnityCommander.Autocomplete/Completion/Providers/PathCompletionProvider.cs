
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Completion.Providers
{
    public sealed class PathCompletionProvider : ICompletionProvider
    {
        public int Priority => 200;

        public bool CanHandle(CliParseState ctx)
            => ctx.ExpectedValue?.ValueType == ArgumentValueType.Path;

        public IEnumerable<CompletionItem> GetCompletions(
            CliParseState ctx)
        {
            if (ctx.ExpectedValue == null)
                return Array.Empty<CompletionItem>();

            var value = Unquote(ctx.PartialValue);

            var pathDescriptor =
                ctx.ExpectedValue.Descriptor as IPathValueDescriptor;

            var pathKind = pathDescriptor?.PathKind
                           ?? PathKind.Any;

            // Пустой путь -> показываем диски
            if (string.IsNullOrEmpty(value))
                return GetDrives(pathKind);

            var separatorIndex = value.LastIndexOfAny(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);

            if (separatorIndex < 0)
                return Array.Empty<CompletionItem>();

            var directory = value[..(separatorIndex + 1)];
            var partial = value[(separatorIndex + 1)..];

            if (!Directory.Exists(directory))
                return Array.Empty<CompletionItem>();

            return GetEntries(directory, partial, pathKind);
        }

        // =====================================================
        // Диски
        // =====================================================

        private IEnumerable<CompletionItem> GetDrives(PathKind pathKind)
        {
            if (pathKind == PathKind.File)
            {
                // Диски всё равно нужны как начало пути.
                // Поэтому File здесь не означает "не показывать диски".
            }

            foreach (var drive in DriveInfo.GetDrives())
            {
                yield return new CompletionItem
                {
                    DisplayText = drive.Name,
                    InsertText = $"\"{drive.Name}\"",
                    CaretOffset = -1,
                    AppendSpace = false
                };
            }
        }

        // =====================================================
        // Папки + файлы
        // =====================================================

        private IEnumerable<CompletionItem> GetEntries(
          string directory,
          string partial,
          PathKind pathKind)
        {
            IEnumerable<string> entries;

            try
            {
                entries = Directory.EnumerateFileSystemEntries(directory);
            }
            catch
            {
                yield break;
            }

            foreach (var path in entries)
            {
                var name = Path.GetFileName(
                    path.TrimEnd(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar));

                if (!name.StartsWith(
                        partial,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (Directory.Exists(path))
                {
                    if (pathKind is PathKind.File)
                        continue;

                    yield return CreateDirectoryCompletion(
                        directory,
                        name);
                }
                else if (File.Exists(path))
                {
                    if (pathKind is PathKind.Directory)
                        continue;

                    yield return CreateFileCompletion(
                        directory,
                        name);
                }
            }
        }

        // =====================================================
        // Папка
        // =====================================================

        private CompletionItem CreateDirectoryCompletion(
            string directory,
            string name)
        {
            var fullPath = Path.Combine(directory, name);

            return new CompletionItem
            {
                DisplayText = name,
                InsertText = $"\"{fullPath}\\\"",
                CaretOffset = -1,
                AppendSpace = false
            };
        }

        // =====================================================
        // Файл
        // =====================================================

        private CompletionItem CreateFileCompletion(
            string directory,
            string name)
        {
            var fullPath = Path.Combine(directory, name);

            return new CompletionItem
            {
                DisplayText = name,
                InsertText = $"\"{fullPath}\"",
                CaretOffset = 0,
                AppendSpace = true
            };
        }

        // =====================================================
        // Убираем внешние кавычки
        // =====================================================

        private static string Unquote(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Length >= 2 &&
                value[0] == '"' &&
                value[^1] == '"')
            {
                return value[1..^1];
            }

            if (value[0] == '"')
                return value[1..];

            return value;
        }
    }
}
