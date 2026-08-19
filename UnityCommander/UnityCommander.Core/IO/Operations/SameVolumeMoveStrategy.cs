
using System;
using System.IO;
using System.Threading.Tasks;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Core.IO.Operations
{
    public sealed class SameVolumeMoveStrategy : IMoveStrategy
    {
        public bool DeletesSource { get; }

        public bool CanHandle(string source, string destination)
        {
            return string.Equals(
                Path.GetPathRoot(source),
                Path.GetPathRoot(destination),
                StringComparison.OrdinalIgnoreCase);
        }

        public Task ExecuteAsync(
            OperationContext context,
            string source,
            string destinationDirectory)
        {
            var destinationPath = Path.Combine(
                destinationDirectory,
                Path.GetFileName(source));

            //Directory.CreateDirectory(destinationDirectory);

            if (File.Exists(source))
            {
                File.Move(source, destinationPath);
            }
            else if (Directory.Exists(source))
            {
                Directory.Move(source, destinationDirectory);
            }

            return Task.CompletedTask;
        }
    }
}
