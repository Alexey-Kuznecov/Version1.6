
namespace W3Manager.WP1
{
    using System;

    using Microsoft.Extensions.DependencyInjection;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The plugin configuration.
    /// </summary>
    public class PluginConfiguration : IPluginFactory, ICommandFactory
    {
        /// <summary>
        /// The category column.
        /// </summary>
        private DateTimeColumn dateTimeColumn;

        public void CommandFactory(CommandBuilder command)
        {
            command.Register<IOOverrideCommand, IOCommands>();
            command.Register<IOOverrideCommand, IOCommands>();
            command.Register<IOOverrideCommand, IOCommands>();
        }

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void Configure(IServiceCollection services)
        {
            this.dateTimeColumn = new DateTimeColumn();

            services.AddSingleton<IOptionBuilder>(this.DateTimeFactory);
            services.AddSingleton<IPluginDescriptor>(this.DateTimeFactory);
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
