using System;
using System.Collections.Generic;
using System.Text;
using UnityCommander.Core.Commands.Base;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    /// <summary>
    /// The panel config record.
    /// </summary>
    public record TabRecord
    {
        public string Path { get; set; }
        public Guid Token { get; set; }
        public string Panel { get; set; }
    }
}
