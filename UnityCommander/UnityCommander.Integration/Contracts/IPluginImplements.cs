
namespace UnityCommander.Integration.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The PluginImplementation interface.
    /// </summary>
    public interface IPluginImplements
    {
        /// <summary>
        /// Gets or sets the register.
        /// </summary>
        List<Type> Register { get; set; }

        /// <summary>
        /// The register type.
        /// </summary>
        void RegisterType();
    }
}
