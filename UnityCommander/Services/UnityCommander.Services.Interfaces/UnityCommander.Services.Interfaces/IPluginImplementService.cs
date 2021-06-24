using System;
using System.Collections.Generic;
using System.Text;
using UnityCommander.Integration.Contracts;
using UnityCommander.Integration.Dialog;

namespace UnityCommander.Services.Interfaces
{
    public interface IPluginImplementService
    {
        /// <summary>
        /// Gets  the imported plugin implementations.
        /// </summary>
        public IEnumerable<IPluginImplement> ImportPluginImplements { get; }

        /// <summary>
        /// Gets the import dialog service.
        /// </summary>
        public IEnumerable<IDialogService> ImportDialogService { get; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IPluginImplement> GetPluginImplements();
    }
}
