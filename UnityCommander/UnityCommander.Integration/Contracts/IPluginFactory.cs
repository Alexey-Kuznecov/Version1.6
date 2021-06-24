
namespace UnityCommander.Integration.Contracts
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The PluginFactory interface.
    /// </summary>
    public interface IPluginFactory
    {
        /// <summary>
        /// The configure.
        /// TODO: Add description here.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        void Configure(IServiceCollection services);
    }
}
