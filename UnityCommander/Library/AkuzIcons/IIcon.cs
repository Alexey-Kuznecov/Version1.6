
namespace AkuzIcons
{
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// The Icon interface.
    /// </summary>
    public interface IIcon
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        string Category { get; set; }

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
        DrawingBrush Brush { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        Path Path { get; set; }
    }
}
