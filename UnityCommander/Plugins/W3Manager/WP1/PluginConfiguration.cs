using System;
using Microsoft.Extensions.DependencyInjection;
using UnityCommander.Integration.Columns;
using UnityCommander.Integration.Contracts;

namespace W3Manager.WP1
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
            services.AddSingleton<IColumnBuilder, GameStatusColumn>();
            services.AddSingleton<IColumnBuilder, GameCategoryColumn>(); 
            services.AddSingleton<IPluginDescriptor, PluginDescription>();
        }

        public object RenderRegister()
        {
            throw new NotImplementedException();
        }
    }
}
