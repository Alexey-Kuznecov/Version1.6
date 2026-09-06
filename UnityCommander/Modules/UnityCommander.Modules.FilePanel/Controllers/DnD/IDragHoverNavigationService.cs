
using System;
using System.Windows;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public interface IDragHoverNavigationService
    {
        void Begin(
            UIElement target,
            Action action,
            bool shiftPressed);

        void Cancel();
    }
}
