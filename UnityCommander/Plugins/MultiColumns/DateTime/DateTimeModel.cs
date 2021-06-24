
namespace MultiColumns.DateTime
{
    /// <summary>
    /// The image model.
    /// </summary>
    internal class DateTimeModel
    {
        /// <summary>
        /// Gets or sets the date and time the file/folder was created.
        /// </summary>
        public System.DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time the file/folder was last accessed.
        /// </summary>
        public System.DateTime LastAccessTime { get; set; }
    }
}
