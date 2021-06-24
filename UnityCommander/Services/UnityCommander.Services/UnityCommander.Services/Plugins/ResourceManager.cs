using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Baml2006;

namespace UnityCommander.Services
{
    public class ResourceManager
    {
        /// <summary>
        /// This method will attempt to find shared resource files in plugin assemblies.
        /// Searches only for files named General, because links can be looped 
        /// if multiple dictionaries refer the same resources.
        /// TODO: This method is called more than once, resulting in duplicate resource dictionaries. We'll need to fix it.
        /// </summary>
        /// <param name="assembly"> The assembly to search for resource files. </param>
        public static void GetResourceDictionary(Assembly assembly)
        {
            Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".g.resources");
            if (stream == null) return;

            using (ResourceReader reader = new ResourceReader(stream))
            {
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
                            var dictionary = Application.Current.Resources.MergedDictionaries;

                            if (!dictionary.Contains(resource))
                            {
                                dictionary.Add(resource);
                            }
                        }
                    }
                }
            }
        }

        //public void GetResourceDictionary(string assemblyName)
        //{
        //    List<Stream> bamlStreams = new List<Stream>();
        //    Assembly skinAssembly = Assembly.LoadFrom(this._mainAssemblyPath);
        //    string[] resourceDictionaries = skinAssembly.GetManifestResourceNames();
        //    foreach (string resourceName in resourceDictionaries)
        //    {
        //        ManifestResourceInfo info = skinAssembly.GetManifestResourceInfo(resourceName);
        //        if (info.ResourceLocation != ResourceLocation.ContainedInAnotherAssembly)
        //        {
        //            Stream resourceStream = skinAssembly.GetManifestResourceStream(resourceName);
        //            using (ResourceReader reader = new ResourceReader(resourceStream))
        //            {
        //                foreach (DictionaryEntry entry in reader)
        //                {
        //                    //Here you can see all your ResourceDictionaries
        //                    //entry is your ResourceDictionary from assembly
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
