
namespace UnityCommander.Integration.Contracts
{
    /// <summary>
    /// The PluginDescriptor interface.
    /// </summary>
    public interface IPluginDescriptor : IPluginService
    { 
        /// <summary>
        /// Gets or sets plugin name.
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Gets or sets plugin description.
        /// </summary>
        public string Description { get; set; }
    }
}
