
namespace UnityCommander.Services
{
    using System.Collections.ObjectModel;

    using UnityCommander.Common.Models;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The icon provider.
    /// </summary>
    public class IconProvider : IIconProvider
    {
        /// <summary>
        /// The get icons collection.
        /// </summary>
        /// <returns>
        /// Icons collection.
        /// </returns>
        public ObservableCollection<IconModel> GetIcons()
        {
            return null;
        }
    }
}
