
using System;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Plugins;

namespace UnityCommander.Test
{
    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
#if NETCOREAPP3_1
            //var asm = AppDomain.CurrentDomain.GetAssemblies();
            //WeakReference<IPluginManager> weakReference = new WeakReference<IPluginManager>(new PluginManagerService().GetPluginManager());
            //IPluginManager manager;
            //asm = AppDomain.CurrentDomain.GetAssemblies();
            //weakReference.TryGetTarget(out manager);
            //if (manager != null) manager.PluginUnload();
            //weakReference = null;

            //var asm = AppDomain.CurrentDomain.GetAssemblies();
            //PluginSingleLoader weakReference = new PluginSingleLoader();
            //asm = AppDomain.CurrentDomain.GetAssemblies();

#endif

        }
    }
}