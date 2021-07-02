
namespace DateTimeColumns
{
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// The plugin option handler.
    /// </summary>
    internal class PluginOptionHandler
    {
        /// <summary>
        /// The date format handler.
        /// TODO: Add description here.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="selected">
        /// The selected.
        /// </param>
        /// <returns>
        /// The string
        /// </returns>
        public string DateFormatHandler(string path, string selected)
        {
            Debug.WriteLine("This is DateTimeColumns plugin!");
            Debug.WriteLine(path);

            return string.Empty;
        }
    }
}
