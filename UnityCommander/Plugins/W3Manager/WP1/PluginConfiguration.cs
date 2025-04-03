
namespace W3Manager.WP1
{
    using System;

    using Microsoft.Extensions.DependencyInjection;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Factories;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The plugin configuration.
    /// </summary>
    public class PluginConfiguration : IPluginFactory, ICommandFactory
    {
        /// <summary>
        /// The category column.
        /// </summary>
        private readonly ModStatusColumn modStatusColumn;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
            this.modStatusColumn = new ModStatusColumn();
        }

        /// <summary>
        /// The command factory.
        /// </summary>
        /// <param name="commandBuilder">
        /// The command.
        /// </param>
        public void CommandFactory(CommandBuilder commandBuilder)
        {
            commandBuilder.Register<IOOverrideCommand, IOCommands>();
            commandBuilder.RegisterWithArgument<IPluginSettings, ModStatusColumn>(this.modStatusColumn, new ModSettings());
        }

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void Configure(IServiceCollection services)
        {
            services.AddSingleton<IPluginDescriptor>(this.ModStatusFactory);
            services.AddSingleton<IColumnBuilder>(this.ModStatusFactory);
            services.AddSingleton<IPluginSettings>(this.ModStatusFactory);
        }

        /// <summary>
        /// The set associated types.
        /// </summary>
        /// <param name="typesRegister">
        /// The types register.
        /// </param>
        public void SetAssociatedTypes(AssociatedTypesRegister typesRegister)
        {
            typesRegister.RegisterSettings<ModSettings>(this.modStatusColumn);
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
        /// The <see cref="ModStatusColumn"/>.
        /// </returns>
        private ModStatusColumn ModStatusFactory(IServiceProvider service)
        {
            return this.modStatusColumn;
        }
    }
}
