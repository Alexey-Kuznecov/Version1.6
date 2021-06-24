
namespace UnityCommander.Services.Interfaces
{
    using System;
    using System.Reflection;
#if NETCOREAPP3_1
    using System.Runtime.Loader;
#endif
    public interface IPluginRecord
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
        /// Gets or sets plugin token.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsUnload { get; set; }
    }
}
