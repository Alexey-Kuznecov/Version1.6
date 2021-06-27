using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using UnityCommander.Integration.Contracts;

namespace UnityCommander.Plugin.NETCore
{
    public class Tests
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ExecuteAndUnload(string assemblyPath, out WeakReference alcWeakRef)
        {
            // Create the unloadable HostAssemblyLoadContext
            var alc = new HostAssemblyLoadContext(assemblyPath);

            // Create a weak reference to the AssemblyLoadContext that will allow us to detect
            // when the unload completes.
            alcWeakRef = new WeakReference(alc);

            // Load the plugin assembly into the HostAssemblyLoadContext.
            // NOTE: the assemblyPath must be an absolute path.
            Assembly a = alc.LoadFromAssemblyPath(assemblyPath);

            // Ge t the plugin interface by calling the PluginClass.GetInterface method via reflection.
            Type pluginType = a.GetType("Plugin.PluginClass");
            MethodInfo getInterface = pluginType.GetMethod("GetInterface", BindingFlags.Static | BindingFlags.Public);
            Interface plugin = (Interface)getInterface.Invoke(null, null);

            // Now we can call methods of the plugin using the interface
            string result = plugin.GetMessage();
            UnityCommander.Integration.Contracts.Version version = plugin.GetVersion();

            Debug.WriteLine($"Response from the plugin: GetVersion(): {version}, GetMessage(): {result}");

            // This initiates the unload of the HostAssemblyLoadContext. The actual unloading doesn't happen
            // right away, GC has to kick in later to collect all the stuff.
            var asm = AppDomain.CurrentDomain.GetAssemblies();
            alc.Unload();
            asm = AppDomain.CurrentDomain.GetAssemblies();
        }

        public void Setup()
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies();
            WeakReference hostAlcWeakRef;
            string currentAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configName = "Debug";
            string pluginFullPath = @"D:\Works\WPF\Version2.7.8\UnityCommander\UnityCommander\bin\Debug\netcoreapp3.1\plugins\Plugin\netcoreapp3.1\Plugin.dll";
            ExecuteAndUnload(pluginFullPath, out hostAlcWeakRef);

            for (int i = 0; hostAlcWeakRef.IsAlive && (i < 10); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            asm = AppDomain.CurrentDomain.GetAssemblies();

            Debug.WriteLine($"Unload success: {!hostAlcWeakRef.IsAlive}");
        }
    }

    public class HostAssemblyLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public HostAssemblyLoadContext(string pluginPath = "") : base(isCollectible: true)
        {
            //_resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName name)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                Debug.WriteLine($"Loading assembly {assemblyPath} into the HostAssemblyLoadContext");
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
    }
}