
namespace UnityCommander.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using UnityCommander.Integration.Contracts;

    public interface IPluginManager
    {
        public IEnumerable<IPluginImplement> GetPluginImplement();

        public void PluginUnload();
    }
}
