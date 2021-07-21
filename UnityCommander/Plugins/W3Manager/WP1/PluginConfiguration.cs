
namespace W3Manager.WP1
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The plugin configuration.
    /// </summary>
    public class PluginConfiguration : IPluginFactory
    {
        /// <summary>
        /// The category column.
        /// </summary>
        private GameCategoryColumn categoryColumn;

        /// <summary>
        /// The status column.
        /// </summary>
        private GameStatusColumn statusColumn;

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void Configure(IServiceCollection services)
        {
            this.categoryColumn = new GameCategoryColumn();
            this.statusColumn = new GameStatusColumn();

            services.AddSingleton<IColumnBuilder>(this.StatusFactory);
            services.AddSingleton<IOptionBuilder>(this.StatusFactory);
            services.AddSingleton<IPluginDescriptor>(this.StatusFactory);

            services.AddSingleton<IColumnBuilder>(this.CategoryFactory);
            services.AddSingleton<IOptionBuilder>(this.CategoryFactory);
            services.AddSingleton<IPluginDescriptor>(this.CategoryFactory);
        }
        
        /// <summary>
        /// The render register.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object RenderRegister()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The implementation factory.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <returns>
        /// The <see cref="GameCategoryColumn"/>.
        /// </returns>
        private GameCategoryColumn CategoryFactory(IServiceProvider service)
        {
            return this.categoryColumn;
        }

        /// <summary>
        /// The game status column factory.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <returns>
        /// The <see cref="GameStatusColumn"/>.
        /// </returns>
        private GameStatusColumn StatusFactory(IServiceProvider service)
        {
            return this.statusColumn;
        }
    }
}
