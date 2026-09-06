
using System.Collections.Generic;
using UnityCommander.Modules.FilePanel.States;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class NodeContextRegistry
    {
        private readonly object _lock = new();

        public List<FileNodeContext> FileContexts { get; }
            = new List<FileNodeContext>();
        public List<FolderNodeContext> FolderContexts { get; }
            = new List<FolderNodeContext>();

        public void Register(FileNodeContext context)
        {
            FileContexts.Add(context);
        }

        public void Register(FolderNodeContext context)
        {
            FolderContexts.Add(context);
        }

        public void Unregister(FileNodeContext context)
        {
            lock (_lock)
                FileContexts.Remove(context);
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

        public void Unregister(FolderNodeContext context)
        {
            FolderContexts.Remove(context);
        }
    }
}
