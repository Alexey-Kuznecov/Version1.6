
namespace MultiColumns.DateTime
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
        private DateTimeColumn dateTimeColumn;

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void Configure(IServiceCollection services)
        {
            this.dateTimeColumn = new DateTimeColumn();

            services.AddSingleton<IColumnBuilder>(this.DateTimeFactory);
            services.AddSingleton<IOptionBuilder>(this.DateTimeFactory);
            services.AddSingleton<IPluginDescriptor>(this.DateTimeFactory);
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
        /// The <see cref="DateTimeColumn"/>.
        /// </returns>
        private DateTimeColumn DateTimeFactory(IServiceProvider service)
        {
            return this.dateTimeColumn;
        }
    }
}