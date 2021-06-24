
namespace UnityCommander.Services.Plugins
{
    using System;
    using System.Reflection;
    using UnityCommander.Services.Interfaces;

    public record PluginRecord : IPluginRecord
    {
        /// <summary>
        /// Gets or set plugin name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets plugin description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets plugin assembly name.
        /// </summary>
        public AssemblyName AssemblyName { get; set; }

        /// <summary>
        /// Gets or sets plugin token
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsUnload { get; set; }
    }
}
