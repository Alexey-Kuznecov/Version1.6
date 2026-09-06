
using System;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Core.Events
{
    public sealed class CopyCompleteEvent : EventArgs
    {
        public CopyInfo Info { get; }

        public CopyCompleteEvent(CopyInfo info)
        {
            Info = info;
        }
    }
}