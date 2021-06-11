
namespace ImagesColumns
{
    using System.Diagnostics;
    using System.IO;

    using UnityCommander.Integration.Contracts.Columns;

    /// <summary>
    /// The plugin option handler.
    /// </summary>
    internal class PluginOptionHandler : IColumnService
    {
        /// <summary>
        /// The get column.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public object GetColumns()
        {
            return Plugin.Columns;
        }

        /// <summary>
        /// The get dpi value.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public object GetDpiValue(string path)
        {
            return null;
        }

        /// <summary>
        /// The get dpi value.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public object GetSizeValue(string path)
        {
            return null;
        }

        /// <summary>
        /// The get dpi value.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public object GetColorValue(string path)
        {
            return null;
        }

        /// <summary>
        /// The get column value.
        /// </summary>
        /// <param name="currentPath">
        /// The current path.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetColumnValues(string currentPath)
        {
            ImageModel imageModel = new ImageModel();
            var dir = Path.GetFileName(currentPath);

            if (dir != null && dir.Contains("dot"))
            {
                imageModel.Dpi = "2dpi";
                imageModel.Sized = "100x50";
                imageModel.Colors = "Greed";
            }
            else
            {
                imageModel.Dpi = "72dpi";
                imageModel.Sized = "1200x880";
                imageModel.Colors = "Blue";
            }

            return imageModel;
        }
    }
}
