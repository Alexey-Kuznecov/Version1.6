
using Microsoft.Extensions.DependencyInjection;
using UnityCommander.Integration.Columns;
using UnityCommander.Integration.Contracts;
using UnityCommander.Integration.Options;

namespace W3Manager
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
            services.AddSingleton<IColumnBuilder, ModStatus>();
            services.AddSingleton<IColumnBuilder, ModCategory>();
        }
    }
}
