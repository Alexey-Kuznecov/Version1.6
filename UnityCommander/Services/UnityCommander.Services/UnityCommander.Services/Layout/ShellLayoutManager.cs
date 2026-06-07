
using System;
using System.Collections.Generic;

namespace UnityCommander.Services.Layout
{
    public sealed class ShellLayoutManager : IShellLayoutManager
    {
        private readonly Dictionary<ShellArea, ShellAreaState> _areas =
            new();

        public event EventHandler<ShellAreaChangedEventArgs>? AreaChanged;

        public ShellAreaState GetState(ShellArea area)
            => _areas[area];

        public void SetState(
            ShellArea area,
            ShellAreaState state)
        {
            _areas[area] = state;

            AreaChanged?.Invoke(
                this,
                new ShellAreaChangedEventArgs(area, state));
        }
    }
}
