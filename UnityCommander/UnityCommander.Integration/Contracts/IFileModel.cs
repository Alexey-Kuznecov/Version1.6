using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Integration.Contracts
{
    interface IFileModel
    {
        /// <summary>
        /// Gets or sets the column title.
        /// </summary>
        Enums.TargetPanel TargetType { get; set; }
    }
}
