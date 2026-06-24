using System.Windows;
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.WPF.Behaviors
{
    public interface IInputContextService
    {
        Window? ActiveWindow { get; }

        ShortcutScope CurrentScope { get; }

        void Attach(Window window, ShortcutScope scope);

        void Detach(Window window);

        void SetActive(Window window);
    }
}