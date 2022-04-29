
namespace AIconBrowser
{
    using System.Reflection;
    using System.Runtime.Loader;

    using Microsoft.Extensions.DependencyInjection;

    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Dialog;

    /// <summary>
    /// The plugin configuration.
    /// </summary>
    public class PluginConfiguration : IPluginFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
            // We register handler for the Unloading event of the context that we are running in
            // so that we can perform cleanup of stuff that would otherwise prevent unloading
            // (Like freeing GCHandles for objects of types loaded into the unloadable AssemblyLoadContext,
            // terminating threads running code in assemblies loaded into the unloadable AssemblyLoadContext,
            // etc.)
            // NOTE: this is optional and likely not required for basic scenarios
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            AssemblyLoadContext currentContext = AssemblyLoadContext.GetLoadContext(currentAssembly);
            if (currentContext != null)
            {
                currentContext.Unloading += this.OnPluginUnloadingRequested;
            }
        }

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void Configure(IServiceCollection services)
        {
            services.AddSingleton<IDialogService, IconBrowserControl>();
            services.AddSingleton<IPluginDescriptor, IconBrowserControl>();
        }

        /// <summary>
        /// The on plugin unloading requested.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnPluginUnloadingRequested(AssemblyLoadContext obj)
        {
        }
    }
}
