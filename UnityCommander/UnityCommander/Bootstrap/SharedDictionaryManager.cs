
namespace UnityCommander.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using UnityCommander.WPF;

    /// <summary>
    /// The shared dictionary manager.
    /// </summary>
    public static class SharedDictionaryManager
    {
        /// <summary>
        /// The _shared dictionary.
        /// </summary>
        private static ResourceDictionary sharedDictionary;


        public static IEnumerable<ResourceDictionary> Load(IEnumerable<string> uris)
        {
            if (sharedDictionary == null)
            {
                sharedDictionary = new ResourceDictionary();
            }

            foreach (var reference in uris)
            {
                var url = new Uri(reference, UriKind.Relative);

                var dict = (ResourceDictionary)
                    Application.LoadComponent(url);
                
                dict.Source = url;

                sharedDictionary.MergedDictionaries.Add(dict);
                ResourceManager.Register(dict);

                yield return dict;
            }
        }
    }
}
