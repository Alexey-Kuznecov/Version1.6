
namespace UnityCommander.Common.Models
{
    using System.Windows.Media;
    using System.Windows.Shapes;

    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The icon model.
    /// </summary>
    public class IconModel : IIcon
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
    }
}
