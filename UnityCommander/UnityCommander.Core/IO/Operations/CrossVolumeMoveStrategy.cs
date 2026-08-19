
using System;
using System.IO;
using System.Threading.Tasks;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Core.IO.Operations
{
    public sealed class CrossVolumeMoveStrategy : IMoveStrategy
    {
        public bool DeletesSource { get; } = true;

        public bool CanHandle(
            string source,
            string destination)
        {
            return !string.Equals(
                Path.GetPathRoot(source),
                Path.GetPathRoot(destination),
                StringComparison.OrdinalIgnoreCase);
        }

        public async Task ExecuteAsync(
            OperationContext context,
            string source,
            string destination)
        {
            await context.Manager.CopyAsync(
                context,
                source,
                destination);

            DeleteSource(source);
        }

        private static void DeleteSource(string source)
        {
            if (File.Exists(source))
            {
                File.Delete(source);
                return;
            }

            if (Directory.Exists(source))
            {
                Directory.Delete(source, true);
            }
        }
    }
}
