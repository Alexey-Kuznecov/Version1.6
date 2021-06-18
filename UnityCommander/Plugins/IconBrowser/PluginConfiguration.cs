
namespace IconBrowser
{
    using Microsoft.Extensions.DependencyInjection;

    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Dialog;

    /// <summary>
    /// The plugin configuration.
    /// </summary>
    public class PluginConfiguration : IPluginFactory
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void Configure(IServiceCollection services)
        {
            services.AddSingleton<IDialogService, IconBrowserControl>();
        }
    }
}
