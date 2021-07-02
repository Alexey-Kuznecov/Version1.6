
using Microsoft.Extensions.DependencyInjection;
using MultiColumns.Images;
using UnityCommander.Integration.Contracts;

namespace MultiColumns.Image
{
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
            services.AddSingleton<IPluginConfigure, PluginSettings>();
            services.AddSingleton<IPluginImplement, Plugin>();
            services.AddSingleton<IPluginDescriptor, Plugin>();
        }
    }
}
