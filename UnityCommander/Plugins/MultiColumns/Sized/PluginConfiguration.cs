
namespace MultiColumns.Sized
{
    using System;

    using Microsoft.Extensions.DependencyInjection;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Factories;
    using UnityCommander.Integration.Options;

    public class PluginConfiguration : IPluginFactory
    {
        private SizedColumn sizedColumn;

        public void Configure(IServiceCollection services)
        {
            this.sizedColumn = new SizedColumn();

            services.AddSingleton<IColumnBuilder>(this.SizedFactory);
            services.AddSingleton<IOptionBuilder>(this.SizedFactory);
            services.AddSingleton<IPluginDescriptor>(this.SizedFactory);
        }

        public void SetAssociatedTypes(AssociatedTypesRegister typesRegister)
        {
            typesRegister.RegisterSettings<SizeSettings>(this.sizedColumn);
        }

        public void SetToken(string token)
        {
        }

        private SizedColumn SizedFactory(IServiceProvider service)
        {
            return this.sizedColumn;
        }
    }
}