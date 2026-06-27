
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Runtime.Loader;
using System.Windows.Baml2006;
using System.Windows.Controls;
using System.Windows.Markup;

namespace UnityCommander.WPF.Helper
{
    public class PluginViewFactory : IViewFactory
    {
        public UserControl Create(Type viewType)
        {
            // ВАЖНО: НЕ имя файла, НЕ строка

            var asm = viewType.Assembly;

            var resourceName = asm.GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith(".g.resources"));

            using var stream = asm.GetManifestResourceStream(resourceName);
            using var reader = new ResourceReader(stream);

            foreach (DictionaryEntry entry in reader)
            {
                if (entry.Value is not Stream s)
                    continue;

                try
                {
                    var baml = new Baml2006Reader(s);
                    var obj = XamlReader.Load(baml);
                    
                    Debug.WriteLine("VIEW TYPE:");
                    Debug.WriteLine(viewType.Assembly.Location);
                    Debug.WriteLine(viewType.Assembly.GetName().Name);
                    Debug.WriteLine(viewType.Assembly.GetHashCode());

                    Debug.WriteLine("LOADED TYPE:");
                    Debug.WriteLine(obj.GetType().Assembly.Location);
                    Debug.WriteLine(obj.GetType().Assembly.GetName().Name);
                    Debug.WriteLine(obj.GetType().Assembly.GetHashCode());

                    Debug.WriteLine("ALC:");
                    Debug.WriteLine(AssemblyLoadContext.GetLoadContext(viewType.Assembly));
                    Debug.WriteLine(AssemblyLoadContext.GetLoadContext(obj.GetType().Assembly));

                    if (obj is UserControl view &&
                        view.GetType() == viewType)
                    {
                        return view;
                    }
                }
                catch
                {
                    // skip
                }
            }

            throw new Exception($"View not found for {viewType.FullName}");
        }
    }
}
