using System;
using System.Collections.Generic;
using System.Text;
using UnityCommander.Integration.Contracts;
using UnityCommander.Integration.Dialog;

namespace UnityCommander.Services.Interfaces
{
    public interface IPluginLoader
    {
        public bool UnloadPlugin();

        public IEnumerable<IPluginImplement> GetImplements();
        public IEnumerable<IPluginConfigure> GetConfigurations();
        public IEnumerable<IPluginDescriptor> GetDescriptors();
        public IEnumerable<IDialogService> GetDialogs();
    }
}
