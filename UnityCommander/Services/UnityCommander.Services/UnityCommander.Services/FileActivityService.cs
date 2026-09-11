
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Services
{
    public sealed class FileActivityService : IFileActivityService
    {
        private readonly ConcurrentDictionary<Guid, FileActivity> _active = new();

        public bool IsActive(Guid itemId)
            => _active.ContainsKey(itemId);

        public bool IsActive(string path)
            => _active.Values.Any(x =>
                string.Equals(
                    x.SourcePath,
                    path,
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    x.DestinationPath,
                    path,
                    StringComparison.OrdinalIgnoreCase));

        public void Activate(
            Guid itemId,
            string sourcePath,
            string destinationPath)
        {
            _active[itemId] = new FileActivity(
                itemId,
                sourcePath,
                destinationPath);
        }

        public void Deactivate(Guid itemId)
            => _active.TryRemove(itemId, out _);
    }
}
