
namespace MultiColumns.Image
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    using MultiColumns.Sized;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Factories;
    using UnityCommander.Integration.Options;

    public class PluginConfiguration : IPluginFactory
    {
        private ImageColumn imageColumn;

        public void Configure(IServiceCollection services)
        {
            this.imageColumn = new ImageColumn();

            services.AddSingleton<IColumnBuilder>(this.ImageFactory);
            services.AddSingleton<IOptionBuilder>(this.ImageFactory);
            services.AddSingleton<IPluginDescriptor>(this.ImageFactory);
        }

        public void SetAssociatedTypes(AssociatedTypesRegister typesRegister)
        {
        }

        public void SetToken(string token)
        {
        }

        private ImageColumn ImageFactory(IServiceProvider service)
        {
            return this.imageColumn;
        }
    }
}