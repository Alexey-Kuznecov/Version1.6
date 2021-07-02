
using System;
using UnityCommander.Integration.Enums;

namespace DateTimeColumns
{
    /// <summary>
    /// The image model.
    /// </summary>
    internal class DateTimeModel
    {
        /// <summary>
        /// Gets or sets the date and time the file/folder was created.
        /// </summary>
        public DateTime NewCreationTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time the file/folder was last accessed.
        /// </summary>
        public DateTime NewLastAccessTime { get; set; }
    }
}
