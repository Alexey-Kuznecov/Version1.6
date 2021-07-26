

namespace UnityCommander.Services.Interfaces
{
    using System.Collections.ObjectModel;

    using MaterialDesignThemes.Wpf;

    using UnityCommander.Common.Models.Icons;

    /// <summary>
    /// The service consists of methods that provide icons.
    /// </summary>
    public interface IIconProviderService
    {
        /// <summary>
        /// Provides all icons is available.
        /// </summary>
        /// <returns>
        /// The collection icons of <see cref="IIcon"/> types.
        /// </returns>
        ObservableCollection<IIcon> GetIcons();

        /// <summary>
        /// Provides an icon using the name specified in the arguments.
        /// </summary>
        /// <param name="iconName"> Icon name. </param>
        /// <returns> Icon as type of <see cref="IIcon"/>. </returns>
        public IIcon GetIcon(string iconName);

        /// <summary>
        /// Gets an icon provided by the material design library.
        /// </summary>
        /// <param name="icon">
        /// Specifies an icon from list of available icons.
        /// </param>
        /// <returns>
        /// Returns an icon of type <see cref="IIcon"/> that is known to the program.
        /// </returns>
        public IIcon GetIcon(PackIconKind icon);
    }
}
