
namespace MultiColumns.Image
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    using MultiColumns.Sized;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The plugin configuration.
    /// </summary>
    public class PluginConfiguration : IPluginFactory
    {
        /// <summary>
        /// The image column.
        /// </summary>
        private ImageColumn imageColumn;

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void Configure(IServiceCollection services)
        {
            this.imageColumn = new ImageColumn();

            services.AddSingleton<IColumnBuilder>(this.ImageFactory);
            services.AddSingleton<IOptionBuilder>(this.ImageFactory);
            services.AddSingleton<IPluginDescriptor>(this.ImageFactory);
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
        /// The <see cref="SizedColumn"/>.
        /// </returns>
        private ImageColumn ImageFactory(IServiceProvider service)
        {
            return this.imageColumn;
        }
    }
}