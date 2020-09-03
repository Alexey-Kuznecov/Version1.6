using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Core.IO
{
    public class CopyErrorModel
    {
        /// <summary>
        /// Gets or sets the destination directory.
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// Gets or sets the source directory.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; }
    }
}
