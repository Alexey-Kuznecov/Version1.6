using System;
using System.IO;
using System.Reflection;

namespace UnityCommander.Plugin.NET48
{
    public class ManagedLoadContext
    {
        private object defaultLoadContext;
        private string mainAssemblyPath;

        public ManagedLoadContext(string mainAssemblyPath, object defaultLoadContext, bool isCollectible)
        {
            this.defaultLoadContext = defaultLoadContext;
        }

        public Assembly LoadAssemblyFromFilePath(string path)
        {
            return null;
        }
    }
}
