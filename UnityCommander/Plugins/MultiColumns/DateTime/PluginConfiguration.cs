
namespace MultiColumns.DateTime
{
    using Microsoft.Extensions.DependencyInjection;

    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The plugin configuration.
    /// </summary>
    public class PluginConfiguration : IPluginFactory
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void Configure(IServiceCollection services)
        {
            services.AddSingleton<IPluginImplement, Plugin>();
            services.AddSingleton<IPluginDescriptor, Plugin>();
            services.AddSingleton<IPluginConfigure, PluginSettings>();
        }
    }
}
