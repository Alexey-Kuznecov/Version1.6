
namespace MultiColumns.Sized
{
    using System;

    using Microsoft.Extensions.DependencyInjection;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Factories;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The plugin configuration.
    /// </summary>
    public class PluginConfiguration : IPluginFactory
    {
        /// <summary>
        /// The category column.
        /// </summary>
        private SizedColumn sizedColumn;

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void Configure(IServiceCollection services)
        {
            this.sizedColumn = new SizedColumn();

            services.AddSingleton<IColumnBuilder>(this.SizedFactory);
            services.AddSingleton<IOptionBuilder>(this.SizedFactory);
            services.AddSingleton<IPluginDescriptor>(this.SizedFactory);
        }


        /// <summary>
        /// The set associated types.
        /// </summary>
        /// <param name="typesRegister">
        /// The types register.
        /// </param>
        public void SetAssociatedTypes(AssociatedTypesRegister typesRegister)
        {
            typesRegister.RegisterSettings<SizeSettings>(this.sizedColumn);
        }

        public void SetToken(string token)
        {
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
        private SizedColumn SizedFactory(IServiceProvider service)
        {
            return this.sizedColumn;
        }
    }
}