using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using UnityCommander.Integration.Contracts;
using UnityCommander.Integration.Dialog;

namespace AIconBrowser
{
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
            services.AddSingleton<IPluginDescriptor, IconBrowserControl>();

            // We register handler for the Unloading event of the context that we are running in
            // so that we can perform cleanup of stuff that would otherwise prevent unloading
            // (Like freeing GCHandles for objects of types loaded into the unloadable AssemblyLoadContext,
            // terminating threads running code in assemblies loaded into the unloadable AssemblyLoadContext,
            // etc.)
            // NOTE: this is optional and likely not required for basic scenarios
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            AssemblyLoadContext currentContext = AssemblyLoadContext.GetLoadContext(currentAssembly);
            if (currentContext != null) currentContext.Unloading += OnPluginUnloadingRequested;
        }

        private void OnPluginUnloadingRequested(AssemblyLoadContext obj)
        {
            ///throw new NotImplementedException();
        }
    }
}
