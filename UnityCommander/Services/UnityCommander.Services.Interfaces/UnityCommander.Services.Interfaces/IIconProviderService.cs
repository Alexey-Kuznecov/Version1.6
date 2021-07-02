

namespace UnityCommander.Services.Interfaces
{
    using System.Collections.ObjectModel;
    using UnityCommander.Common.Models;
    using UnityCommander.Integration.Models;

    /// <summary>
    /// The i icon provider.
    /// </summary>
    public interface IIconProviderService
    {
        /// <summary>
        /// The get icons.
        /// </summary>
        /// <returns>
        /// The collection icons of Icon Model type.
        /// </returns>
        ObservableCollection<IconModel> GetIcons();

        /// <summary>
        /// Provides an icon by the name specified in the arguments.
        /// </summary>
        /// <param name="iconName"> Icon name. </param>
        /// <returns></returns>
        public IconModel GetIcon(string iconName);
    }
}
