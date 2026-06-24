
namespace UnityCommander.Abstractions.Keyboard
{
    public interface IShortcutContextService
    {
        ShortcutScope Current { get; }

        void Push(object owner, ShortcutScope scope);

        void Pop(object owner);

        //public Window? ActiveWindow { get; set; }

        //public ShortcutScope CurrentScope { get; set; }

        //public void SetActive(Window window, ShortcutScope scope)
        //{
        //    ActiveWindow = window;
        //    CurrentScope = scope;
        //}
    }
}
