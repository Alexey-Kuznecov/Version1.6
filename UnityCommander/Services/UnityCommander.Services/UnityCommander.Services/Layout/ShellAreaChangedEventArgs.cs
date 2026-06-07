
using System;

namespace UnityCommander.Services.Layout
{
    public sealed class ShellAreaChangedEventArgs : EventArgs
    {
        public ShellAreaChangedEventArgs(
            ShellArea area,
            ShellAreaState state)
        {
            Area = area;
            State = state;
        }

        public ShellArea Area { get; }

        public ShellAreaState State { get; }
    }
}
