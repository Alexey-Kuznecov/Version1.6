using System;
using UnityCommander.Abstractions.Panels;

namespace UnityCommander.Common.State
{
    public class TabSessionState
    {
        public string Title { get; set; }
        public Guid TabId { get; set; }
        public string Path { get; set; }
        public ViewMode? ViewMode { get; set; }
    }
}