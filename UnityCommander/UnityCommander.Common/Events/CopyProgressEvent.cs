
using System;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Common.Events
{
    public sealed class CopyProgressEvent : EventArgs
    {
        public CopyInfo Info { get; }

        public CopyProgressEvent(CopyInfo info)
        {
            Info = info;
        }
    }
}