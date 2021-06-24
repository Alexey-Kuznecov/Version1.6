
namespace UnityCommander.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    public interface IPluginManager
    {
        public void EnablePlugin();

        public void DisablePlugin();

        public void UninstallPlugin();

        public void InstallPlugin();

        public void UnloadPlugin(IPluginRecord record);

        public IEnumerable<IPluginRecord> GetPluginRecords();
    }
}
