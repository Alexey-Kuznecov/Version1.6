
using System;
using System.IO;

namespace UnityCommander.Common
{
    public sealed class UnityCommanderPath
    {
        public UnityCommanderPath()
        {
            BaseDirectory = AppContext.BaseDirectory;
        }

        public string BaseDirectory { get; }

        public string ConfigDirectory =>
            Path.Combine(BaseDirectory, "config");

        public string PluginsDirectory =>
            Path.Combine(BaseDirectory, "Plugins");

        public string ResourcesDirectory =>
            Path.Combine(BaseDirectory, "Resources");

        public string IconsDirectory =>
           Path.Combine(BaseDirectory, "Icons");

        public string DataDirectory =>
          Path.Combine(BaseDirectory, "Data");

        public string Config(string fileName) =>
            Path.Combine(ConfigDirectory, fileName);

        public string Plugin(string path) =>
            Path.Combine(PluginsDirectory, path);

        public string Resource(string path) =>
            Path.Combine(ResourcesDirectory, path);
    }
}
