
namespace MultiColumns.DateTime
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
        /// The get column value.
        /// </summary>
        /// <param name="path">
        /// The path
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public object SetColumnValue(string path)
        {
            var dateTimeModel = new DateTimeModel();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            dateTimeModel.CreationTime = directoryInfo.CreationTime;
            dateTimeModel.LastAccessTime = directoryInfo.LastAccessTime;
            return dateTimeModel;
        }
    }
}
