
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Baml2006;
using System.Windows.Markup;
using UnityCommander.Services.Interfaces.Plugins;

namespace UnityCommander.Services.Plugins
{
    public class BamlResourceLoader : IResourceLoader
    {
        public IReadOnlyList<ResourceDictionary> Load(Assembly assembly)
        {
            var result = new List<ResourceDictionary>();
            var seenSources = new HashSet<string>();

            using var stream =
                assembly.GetManifestResourceStream($"{assembly.GetName().Name}.g.resources");

            if (stream == null)
                return result;

            using var reader = new ResourceReader(stream);

            foreach (DictionaryEntry entry in reader)
            {
                if (entry.Value is not Stream resourceStream)
                    continue;

                try
                {
                    var temp = new ResourceDictionary();

                    var obj = LoadBaml(resourceStream);

                    if (obj is not ResourceDictionary dict)
                        continue;

                    // 🔥 ключевая защита от дублей
                    var key = dict.Source?.ToString();

                    if (!string.IsNullOrEmpty(key))
                    {
                        if (!seenSources.Add(key))
                            continue;
                    }

                    result.Add(dict);
                }
                catch
                {
                    // намеренно игнорируем мусорные ресурсы
                }
            }

            return result;
        }

        private static object LoadBaml(Stream stream)
        {
            // 🔥 важно: копируем поток, иначе возможны повторные чтения/битые позиции
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;

            var bamlReader = new Baml2006Reader(ms);
            try
            {
                return XamlReader.Load(bamlReader);
            }
            catch (XamlParseException ex)
            {
                // лог + пропуск проблемного dictionary
            }

            return null;
        }
    }
}
