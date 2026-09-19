
using System.Collections.Generic;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class NodeContextRegistry
    {
        private readonly object _lock = new();

        public List<FileNodeContext> FileContexts { get; } = [];
        public List<FolderNodeContext> FolderContexts { get; } = [];
        public List<UnifiedNodeContext> UnifiedContexts { get; } = [];

        public void Register(FileNodeContext context)
        {
            lock (_lock)
                FileContexts.Add(context);
        }

        public void Register(FolderNodeContext context)
        {
            lock (_lock)
                FolderContexts.Add(context);
        }

        public void Register(UnifiedNodeContext context)
        {
            lock (_lock)
                UnifiedContexts.Add(context);
        }

        public void Unregister(FileNodeContext context)
        {
            lock (_lock)
                FileContexts.Remove(context);
        }

        public void Unregister(FolderNodeContext context)
        {
            lock (_lock)
                FolderContexts.Remove(context);
        }

        public void Unregister(UnifiedNodeContext context)
        {
            lock (_lock)
                UnifiedContexts.Remove(context);
        }

        public bool TryUnregister(FileNodeContext ctx)
        {
            lock (_lock)
            {
                if (!FileContexts.Contains(ctx))
                    return false;

                FileContexts.Remove(ctx);
                return true;
            }
        }

        public bool TryUnregister(FolderNodeContext ctx)
        {
            lock (_lock)
            {
                if (!FolderContexts.Contains(ctx))
                    return false;

                FolderContexts.Remove(ctx);
                return true;
            }
        }
    }
}
