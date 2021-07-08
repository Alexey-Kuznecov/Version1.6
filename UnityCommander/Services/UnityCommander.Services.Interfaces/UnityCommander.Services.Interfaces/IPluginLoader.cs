
namespace UnityCommander.Services.Interfaces
{
    using System.Collections.Generic;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Dialog;
    public interface IPluginLoader
    {
        public bool UnloadPlugin();

        public IEnumerable<IPluginImplement> GetImplements();
        public IEnumerable<IPluginConfigure> GetConfigurations();
        public IEnumerable<IPluginDescriptor> GetDescriptors();
        public IEnumerable<IDialogService> GetDialogs();
        public IEnumerable<IColumnBuilder> GetColumnBuilders();
    }
}
