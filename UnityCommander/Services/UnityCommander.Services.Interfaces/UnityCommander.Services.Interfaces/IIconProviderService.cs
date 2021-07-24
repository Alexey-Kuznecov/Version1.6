

namespace UnityCommander.Services.Interfaces
{
    using System.Collections.ObjectModel;
    using UnityCommander.Common.Models.Icons;

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
        ObservableCollection<IIcon> GetIcons();

        /// <summary>
        /// Provides an icon by the name specified in the arguments.
        /// </summary>
        /// <param name="iconName"> Icon name. </param>
        /// <returns> Icon as type of <see cref="Icon"/>. </returns>
        public IIcon GetIcon(string iconName);
    }
}
