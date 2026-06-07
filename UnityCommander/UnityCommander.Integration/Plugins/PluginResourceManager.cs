
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Baml2006;

namespace UnityCommander.Integration.Plugins
{
    public class PluginResourceManager
    {
        /// <summary>
        /// This method will attempt to find shared resource files in plugin assemblies.
        /// Searches only for files named General, because links can be looped 
        /// if multiple dictionaries refer the same resources.
        /// TODO: This method is called more than once, resulting in duplicate resource dictionaries. We'll need to fix it.
        /// </summary>
        /// <param name="assembly"> The assembly to search for resource files. </param>
        public static HashSet<ResourceDictionary> GetResourceDictionary(Assembly assembly)
        {
            HashSet<ResourceDictionary> pluginResource = new();

            Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".g.resources");
            if (stream == null) return null;

            using ResourceReader reader = new ResourceReader(stream);

            foreach (DictionaryEntry entry in reader)
            {
                var extension = Path.GetExtension(entry.Key.ToString());
                if (extension != ".baml") continue;

                if (entry.Key.ToString().Contains("general"))
                {
                    var readStream = entry.Value as Stream;
                    Baml2006Reader bamlReader = new Baml2006Reader(readStream);

                    var loadedObject = System.Windows.Markup.XamlReader.Load(bamlReader);
                    if (loadedObject is ResourceDictionary resource)
                    {
                        if (!Application.Current.Resources.MergedDictionaries.Contains(resource))
                        {
                            pluginResource.Add(resource);
                        }
                    }
                }
            }

            return pluginResource;
        }
    }
}
