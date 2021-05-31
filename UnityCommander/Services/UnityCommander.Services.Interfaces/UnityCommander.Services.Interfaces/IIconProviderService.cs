

namespace UnityCommander.Services.Interfaces
{
    using System.Collections.ObjectModel;
    using UnityCommander.Common.Models;

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
    }
}
