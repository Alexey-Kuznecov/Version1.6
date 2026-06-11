
namespace MultiColumns.DateTime
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Factories;
    using UnityCommander.Integration.Options;

    public class PluginConfiguration : IPluginFactory, ICommandFactory
    {
        private DateTimeColumn dateTimeColumn;

        public void Configure(IServiceCollection services)
        {
            this.dateTimeColumn = new DateTimeColumn();

            services.AddSingleton<IColumnBuilder>(this.DateTimeFactory);
            services.AddSingleton<IOptionBuilder>(this.DateTimeFactory);
            services.AddSingleton<IPluginDescriptor>(this.DateTimeFactory);
        }

        public void CommandFactory(CommandBuilder command)
        {
            command.Register<IOOverrideCommand2, IOCommands>();
            command.RegisterWithArgument<IPluginSettings, DateTimeColumn>(this.dateTimeColumn, new DateTimeSettings());
        }

        public void SetAssociatedTypes(AssociatedTypesRegister typesRegister)
        {
            typesRegister.RegisterSettings<DateTimeSettings>(this.dateTimeColumn);
        }

        private DateTimeColumn DateTimeFactory(IServiceProvider service)
        {
            return this.dateTimeColumn;
        }

        public void SetToken(string token)
        {
        }
    }
}