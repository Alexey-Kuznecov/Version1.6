
namespace UnityCommander.Services.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using UnityCommander.Services.Interfaces;

    public class PluginManager : IPluginManager
    {
        internal List<ALCRecords> ALCRecords { get; } = new List<ALCRecords>();

        internal List<PluginRecord> PluginRecords { get; } = new List<PluginRecord>();

        public PluginManager()
        {
        }

        public void EnablePlugin()
        {
            throw new NotImplementedException();
        }

        public void DisablePlugin()
        {
            throw new NotImplementedException();
        }

        public void UninstallPlugin()
        {
            throw new NotImplementedException();
        }

        public void InstallPlugin()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPluginRecord> GetPluginRecords()
        {
            return PluginRecords;
        }

        public void UnloadPlugin(IPluginRecord record)
        {
#if NETCOREAPP3_1
            ALCRecords alcRecords = ALCRecords.Single(r => r.Token.Equals(record.Token));
            
            bool isUnload = PluginRecords.Any(r => r.IsUnload == true && r.AssemblyName.FullName == record.AssemblyName.FullName);

            if (!isUnload)
            {
                alcRecords.Alc.Unload(); 
                alcRecords.Alc = null;
            }

            record.IsUnload = true;
#endif
        }
    }
}
