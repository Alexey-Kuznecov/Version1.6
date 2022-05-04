
namespace W3Manager.WP1
{
    using System;

    using Microsoft.Extensions.DependencyInjection;
    using UnityCommander.Integration.Columns;
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
        private ModStatusColumn modStatusColumn;

        /// <summary>
        /// The command factory.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        public void CommandFactory(CommandBuilder command)
        {
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
            this.modStatusColumn = new ModStatusColumn();

            services.AddSingleton<IPluginDescriptor>(this.ModStatusFactory);
            services.AddSingleton<IColumnBuilder>(this.ModStatusFactory);
            services.AddSingleton<IPluginSettings>(this.ModStatusFactory);
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
