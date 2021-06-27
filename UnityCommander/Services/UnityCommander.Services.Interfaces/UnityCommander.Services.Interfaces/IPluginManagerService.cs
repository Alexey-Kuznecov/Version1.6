
namespace UnityCommander.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using UnityCommander.Integration.Contracts;

    public interface IPluginManagerService
    {
        /// <summary>
        /// Gets the imported plugin settings.
        /// </summary>
        public IEnumerable<IPluginConfigure> ImportPluginSettings { get; }

        /// <summary>
        /// Gets the imported plugin settings.
        /// </summary>
        public IEnumerable<IPluginDescriptor> ImportPluginMeta { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IPluginManager GetPluginManager();
    }
}
