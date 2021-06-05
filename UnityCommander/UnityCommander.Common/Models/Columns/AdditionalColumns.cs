
namespace UnityCommander.Common.Models.Columns
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Reflection;

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
        /// Initializes a new instance of the <see cref="AdditionalColumns"/> class.
        /// </summary>
        public AdditionalColumns()
        {
            var loaders = new List<PluginLoader>();
            List<IColumnService> services = new List<IColumnService>();

            // create plugin loaders
            var pluginsDir = Path.Combine(AppContext.BaseDirectory, "plugins");
            foreach (var dir in Directory.GetDirectories(pluginsDir))
            {
                var dirName = Path.GetFileName(dir);
#if NETCOREAPP3_1
                var pluginDll = Path.Combine(dir + "\\netcoreapp3.1\\", dirName + ".dll");
#else
                var pluginDll = Path.Combine(dir + "\\net48\\", dirName + ".dll");
#endif
                if (File.Exists(pluginDll))
                {
                    var loader = PluginLoader.CreateFromAssemblyFile(
                        pluginDll,
                        sharedTypes: new[] { typeof(IColumnService) });
                    loaders.Add(loader);
                }
            }

            // Create an instance of plugin types
            foreach (var loader in loaders)
            {
                foreach (var pluginType in loader
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(IColumnService).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    // This assumes the implementation of IPlugin has a parameterless constructor
                    var plugin = Activator.CreateInstance(pluginType, null) as IColumnService;
                    services.Add(plugin);
                }
            }

            ImportColumnService = services;
        }

        /// <summary>
        /// Gets or sets the column service.
        /// </summary>
        [ImportMany(typeof(IColumnService))]
        public IEnumerable<IColumnService> ImportColumnService { get; set; }

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
                catalog.Catalogs.Add(new DirectoryCatalog(path));
            }

            // Create the CompositionContainer with the parts in the catalog.
            var container = new CompositionContainer(catalog);

            // Fill the imports of this object
            container.ComposeParts(this);
        }
    }
}
