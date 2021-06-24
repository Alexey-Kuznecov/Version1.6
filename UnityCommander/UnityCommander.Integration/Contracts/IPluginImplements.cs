
namespace UnityCommander.Integration.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Plugin implementation interface.
    /// </summary>
    public interface IPluginImplement
    {
        /// <summary>
        /// Gets or sets the list of registered plugin types. 
        /// In registered plugin types, the host application
        /// will search for all properties marked with 
        /// the <see cref="OptionDescriptionAttribute"/> 
        /// and <see cref="AttachHandlerAttribute"/> attributes.
        /// </summary>
        List<Type> Register { get; set; }

        /// <summary>
        /// Registers plugin types to allow the host application 
        /// to access properties marked with plugin settings attributes.
        /// </summary>
        void RegisterType();

        /// <summary>
        /// Configures and sets the host application context with a plugin, 
        /// and then passes the generated list to the host application.
        /// </summary>
        /// <returns>
        /// List of contexts that have been configured with a plugin 
        /// for the host application.
        /// </returns>
        List<HostAppContext> SetHostAppContext();
    }
}
