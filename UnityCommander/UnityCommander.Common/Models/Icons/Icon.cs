
namespace UnityCommander.Common.Models.Icons
{
    using System;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// The icon model.
    /// </summary>
    [Serializable]
    public class Icon : IIcon
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
        public DrawingBrush Brush { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public Path Path { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// TODO The get icon path.
        /// </summary>
        /// <returns>
        /// The <see cref="Path"/>.
        /// </returns>
        public Path GetIconPath()
        {
            return Path;
        }
    }
}
