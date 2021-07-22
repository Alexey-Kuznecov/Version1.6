
namespace W3Manager.WP2
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
            //services.AddSingleton<IColumnBuilder, ModStatusColumn>();
            //services.AddSingleton<IColumnBuilder, ModCategoryColumn>();
            //services.AddSingleton<IPluginDescriptor, PluginDescription>();
        }
    }
}
