
namespace UnityCommander.Common.Styling
{
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Resources;
    using System.Windows;
    using System.Windows.Baml2006;

    /// <summary>
    /// The resource manager.
    /// </summary>
    public class ResourceManager
    {
        /// <summary>
        /// The get resource dictionary.
        /// </summary>
        /// <param name="assemblyName">
        /// The assembly name.
        /// </param>
        /// <returns>
        /// The <see cref="ResourceDictionary"/>.
        /// </returns>
        public ResourceDictionary GetResourceDictionary(string assemblyName)
        {
            Assembly asm = Assembly.LoadFrom(assemblyName);
            Stream stream = asm.GetManifestResourceStream(asm.GetName().Name + ".g.resources");
            using (ResourceReader reader = new ResourceReader(stream))
            {
                foreach (DictionaryEntry entry in reader)
                {
                    var readStream = entry.Value as Stream;
                    Baml2006Reader bamlReader = new Baml2006Reader(readStream);
                    var loadedObject = System.Windows.Markup.XamlReader.Load(bamlReader);
                    if (loadedObject is ResourceDictionary)
                    {
                        return loadedObject as ResourceDictionary;
                    }
                }
            }
            return null;
        }
    }
}
