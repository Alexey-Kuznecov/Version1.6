
using System;

namespace UnityCommander.Services.Layout
{
    public interface IShellLayoutManager
    {
        ShellAreaState GetState(ShellArea area);

        void SetState(ShellArea area, ShellAreaState state);

        event EventHandler<ShellAreaChangedEventArgs>? AreaChanged;
    }
}
