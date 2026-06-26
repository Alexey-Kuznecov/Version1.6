
using System.Windows.Input;
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.WPF.Behaviors
{
    public static class ShortcutKeyValidator
    {
        public static bool IsValid(
            ShortcutKey key,
            ShortcutModifiers modifiers)
        {
            if (IsModifier(key))
                return false;

            // F-клавиши можно без модификаторов
            if (IsFunctionKey(key))
                return true;

            // Остальные требуют хотя бы один модификатор
            return modifiers != ShortcutModifiers.None;
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
