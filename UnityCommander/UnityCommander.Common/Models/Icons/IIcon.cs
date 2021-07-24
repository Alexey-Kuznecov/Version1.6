
namespace UnityCommander.Common.Models.Icons
{
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// The icon interface.
    /// </summary>
    public interface IIcon
    {
        /// <summary>
        /// TODO The get icon path.
        /// </summary>
        /// <returns>
        /// The <see cref="Path"/>.
        /// </returns>
        Path GetIconPath();
    }
}
