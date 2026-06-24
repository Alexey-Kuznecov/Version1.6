
using System.Windows.Input;

namespace UnityCommander.WPF.Behaviors
{
    public static class ShortcutKeyValidator
    {
        public static bool IsValid(Key key)
        {
            return !IsModifier(key);
        }

        public static bool IsModifier(Key key)
        {
            return key is
                Key.LeftCtrl or
                Key.RightCtrl or
                Key.LeftShift or
                Key.RightShift or
                Key.LeftAlt or
                Key.RightAlt or
                Key.LWin or
                Key.RWin;
        }
    }
}
