
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.WPF.Behaviors
{
    public static class ShortcutKeyValidator
    {
        public static bool IsValid(
            ShortcutKey key,
            ShortcutModifiers modifiers)
        {
            if (CanBeUsedAlone(key))
                return true;

            // F-клавиши можно без модификаторов
            if (IsFunctionKey(key))
                return true;

            if (IsModifier(key))
                return false;

            // Остальные требуют хотя бы один модификатор
            return modifiers != ShortcutModifiers.None;
        }

        private static bool CanBeUsedAlone(ShortcutKey key)
        {
            return IsFunctionKey(key) || key is
                ShortcutKey.Delete or
                ShortcutKey.Insert or
                ShortcutKey.Home or
                ShortcutKey.End or
                ShortcutKey.PageUp or
                ShortcutKey.PageDown or
                ShortcutKey.Up or
                ShortcutKey.Down or
                ShortcutKey.Left or
                ShortcutKey.Right or
                ShortcutKey.PrintScreen or
                ShortcutKey.Pause or
                ShortcutKey.Escape;
        }

        private static bool IsFunctionKey(ShortcutKey key)
        {
            return key is
                ShortcutKey.F1 or
                ShortcutKey.F2 or
                ShortcutKey.F3 or
                ShortcutKey.F4 or
                ShortcutKey.F5 or
                ShortcutKey.F6 or
                ShortcutKey.F7 or
                ShortcutKey.F8 or
                ShortcutKey.F9 or
                ShortcutKey.F10 or
                ShortcutKey.F11 or
                ShortcutKey.F12;
        }

        public static bool IsModifier(ShortcutKey key)
        {
            return key is
                ShortcutKey.LeftCtrl or
                ShortcutKey.RightCtrl or
                ShortcutKey.LeftShift or
                ShortcutKey.RightShift or
                ShortcutKey.LeftAlt or
                ShortcutKey.RightAlt or
                ShortcutKey.LWin or
                ShortcutKey.RWin or
                ShortcutKey.System;
        }
    }
}
