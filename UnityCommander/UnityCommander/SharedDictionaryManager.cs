
namespace UnityCommander
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// The shared dictionary manager.
    /// </summary>
    internal static class SharedDictionaryManager
    {
        /// <summary>
        /// The shared dictionaries.
        /// </summary>
        private static readonly List<string> DictionaryReferences = new List<string>
        {
            "/UnityCommander.Common.Styling;component/Styles/Window/Windows.xaml",
            "/UnityCommander.Common.Styling;component/Themes/MaterialTheme.xaml",
            "/UnityCommander.Common.Styling;component/Themes/DefaultTheme.xaml",
            "/UnityCommander.Controls;component/Ribbon/Ribbon.xaml",
            "/UnityCommander.Controls;component/Taber/Generic.xaml",
            "/UnityCommander.Modules.FilePanel;component/Resources/Generic.xaml",
        };

        /// <summary>
        /// The _shared dictionary.
        /// </summary>
        private static ResourceDictionary sharedDictionary;

        /// <summary>
        /// Gets the shared dictionary.
        /// </summary>
        internal static IEnumerable<ResourceDictionary> SharedDictionary
        {
            get
            {
                if (sharedDictionary == null)
                {
                    foreach (var reference in DictionaryReferences)
                    {
                        System.Uri resourceLocater = new System.Uri(reference, System.UriKind.Relative);

                        sharedDictionary =
                            (ResourceDictionary)Application.LoadComponent(resourceLocater);

                        yield return sharedDictionary;
                    }
                }
            }
        }
    }
}
