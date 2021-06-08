
namespace UnityCommander.Common.Models.Columns
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using UnityCommander.Integration.Contracts;
#if NETCOREAPP3_1
    using UnityCommander.Plugin.Core;
#else
    using UnityCommander.Plugin48.Core;
#endif

    /// <summary>
    /// The additional columns.
    /// </summary>
    public class AdditionalColumns : DefaultColumns
    {
        /// <summary>
        /// The loaders.
        /// </summary>
        private readonly List<PluginLoader> loaders = new List<PluginLoader>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalColumns"/> class.
        /// </summary>
        public AdditionalColumns()
        {
#if NETCOREAPP3_1
            this.CreatePluginContext();
            this.CreatePluginInstance();
#endif
#if NET472
            this.SetImport();
#endif
        }

#if NETCOREAPP3_1
        /// <summary>
        /// The create plugin context.
        /// </summary>
        private void CreatePluginContext()
        {
            // create plugin loaders
            var pluginsDir = Path.Combine(AppContext.BaseDirectory, "plugins");
            foreach (var dir in Directory.GetDirectories(pluginsDir))
            {
                var dirName = Path.GetFileName(dir);
                var pluginDll = Path.Combine(dir + "\\netcoreapp3.1\\", dirName + ".dll");

                if (File.Exists(pluginDll))
                {
                    var loader = PluginLoader.CreateFromAssemblyFile(
                        pluginDll,
                        sharedTypes: new[] { typeof(IColumnService) });
                    loaders.Add(loader);
                }
            }
        }

        /// <summary>
        /// Creates an instance of plugin types.
        /// </summary>
        private void CreatePluginInstance()
        {
            List<IColumnService> services = new List<IColumnService>();

            foreach (var loader in loaders)
            {
                try
                {
                    foreach (var pluginType in loader
                   .LoadDefaultAssembly()
                   .GetTypes()
                   .Where(t => typeof(IColumnService).IsAssignableFrom(t) && !t.IsAbstract))
                    {
                        // This assumes the implementation of IPlugin has a parameter less constructor
                        var plugin = Activator.CreateInstance(pluginType, null) as IColumnService;
                        services.Add(plugin);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            ImportColumnService = services;
        }
#endif
        /// <summary>
        /// Gets or sets the column service.
        /// </summary>
        [ImportMany(typeof(IColumnService))]
        public IEnumerable<IColumnService> ImportColumnService { get; set; }
#if NET472
        /// <summary>
        /// Configures the import of plug-ins for column expansion.
        /// </summary>
        private void SetImport()
        {
            // An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();

            // Add all the parts found in all assemblies in
            // the same directory as the executing program
            foreach (var item in Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\plugins"))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), item);
                catalog.Catalogs.Add(new DirectoryCatalog(path + @"\net472"));
            }

            // Create the CompositionContainer with the parts in the catalog.
            var container = new CompositionContainer(catalog);

            // Fill the imports of this object
            container.ComposeParts(this);
        }
#endif
    }
}
