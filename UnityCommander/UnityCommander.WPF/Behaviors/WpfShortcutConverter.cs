
using System.Windows.Input;
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.WPF.Behaviors
{
    public static class WpfShortcutConverter
    {
        private static readonly Dictionary<ShortcutKey, Key> Map = new()
        {
            // Letters
            [ShortcutKey.A] = Key.A,
            [ShortcutKey.B] = Key.B,
            [ShortcutKey.C] = Key.C,
            [ShortcutKey.D] = Key.D,
            [ShortcutKey.E] = Key.E,
            [ShortcutKey.F] = Key.F,
            [ShortcutKey.G] = Key.G,
            [ShortcutKey.H] = Key.H,
            [ShortcutKey.I] = Key.I,
            [ShortcutKey.J] = Key.J,
            [ShortcutKey.K] = Key.K,
            [ShortcutKey.L] = Key.L,
            [ShortcutKey.M] = Key.M,
            [ShortcutKey.N] = Key.N,
            [ShortcutKey.O] = Key.O,
            [ShortcutKey.P] = Key.P,
            [ShortcutKey.Q] = Key.Q,
            [ShortcutKey.R] = Key.R,
            [ShortcutKey.S] = Key.S,
            [ShortcutKey.T] = Key.T,
            [ShortcutKey.U] = Key.U,
            [ShortcutKey.V] = Key.V,
            [ShortcutKey.W] = Key.W,
            [ShortcutKey.X] = Key.X,
            [ShortcutKey.Y] = Key.Y,
            [ShortcutKey.Z] = Key.Z,

            // Numbers (top row)
            [ShortcutKey.D0] = Key.D0,
            [ShortcutKey.D1] = Key.D1,
            [ShortcutKey.D2] = Key.D2,
            [ShortcutKey.D3] = Key.D3,
            [ShortcutKey.D4] = Key.D4,
            [ShortcutKey.D5] = Key.D5,
            [ShortcutKey.D6] = Key.D6,
            [ShortcutKey.D7] = Key.D7,
            [ShortcutKey.D8] = Key.D8,
            [ShortcutKey.D9] = Key.D9,

            // Function keys
            [ShortcutKey.F1] = Key.F1,
            [ShortcutKey.F2] = Key.F2,
            [ShortcutKey.F3] = Key.F3,
            [ShortcutKey.F4] = Key.F4,
            [ShortcutKey.F5] = Key.F5,
            [ShortcutKey.F6] = Key.F6,
            [ShortcutKey.F7] = Key.F7,
            [ShortcutKey.F8] = Key.F8,
            [ShortcutKey.F9] = Key.F9,
            [ShortcutKey.F10] = Key.F10,
            [ShortcutKey.F11] = Key.F11,
            [ShortcutKey.F12] = Key.F12,

            // Navigation
            [ShortcutKey.Up] = Key.Up,
            [ShortcutKey.Down] = Key.Down,
            [ShortcutKey.Left] = Key.Left,
            [ShortcutKey.Right] = Key.Right,

            [ShortcutKey.Home] = Key.Home,
            [ShortcutKey.End] = Key.End,
            [ShortcutKey.PageUp] = Key.PageUp,
            [ShortcutKey.PageDown] = Key.PageDown,

            // Editing
            [ShortcutKey.Enter] = Key.Enter,
            [ShortcutKey.Escape] = Key.Escape,
            [ShortcutKey.Backspace] = Key.Back,
            [ShortcutKey.Tab] = Key.Tab,
            [ShortcutKey.Space] = Key.Space,
            [ShortcutKey.Delete] = Key.Delete,
            [ShortcutKey.Insert] = Key.Insert,

            [ShortcutKey.LeftCtrl] = Key.LeftCtrl,
            [ShortcutKey.RightCtrl] = Key.RightCtrl,
            [ShortcutKey.LeftShift] = Key.LeftShift,
            [ShortcutKey.RightShift] = Key.RightShift,
            [ShortcutKey.LeftAlt] = Key.LeftAlt,
            [ShortcutKey.RightAlt] = Key.RightAlt,

            // OEM (если захочешь расширять)
            [ShortcutKey.Oem1] = Key.Oem1,
            [ShortcutKey.Oem2] = Key.Oem2,
            [ShortcutKey.Oem3] = Key.Oem3,
            [ShortcutKey.Oem4] = Key.Oem4,
            [ShortcutKey.Oem5] = Key.Oem5,
            [ShortcutKey.Oem6] = Key.Oem6,
            [ShortcutKey.Oem7] = Key.Oem7,
            [ShortcutKey.Oem8] = Key.Oem8,
            //[ShortcutKey.OemMinus] = Key.OemMinus,
            //[ShortcutKey.OemPlus] = Key.OemPlus,
            //[ShortcutKey.OemOpenBrackets] = Key.OemOpenBrackets,
            //[ShortcutKey.OemCloseBrackets] = Key.OemCloseBrackets,
            //[ShortcutKey.OemPipe] = Key.OemPipe,
            //[ShortcutKey.OemSemicolon] = Key.OemSemicolon,
            //[ShortcutKey.OemQuotes] = Key.OemQuotes,
            //[ShortcutKey.OemComma] = Key.OemComma,
            //[ShortcutKey.OemPeriod] = Key.OemPeriod,
            //[ShortcutKey.OemQuestion] = Key.OemQuestion,
            //[ShortcutKey.OemTilde] = Key.OemTilde,

            [ShortcutKey.System] = Key.System,
            [ShortcutKey.LWin] = Key.LWin,
            [ShortcutKey.RWin] = Key.RWin,
            [ShortcutKey.None] = Key.None,
        };

        private static readonly Dictionary<Key, ShortcutKey> ReverseMap =
            Map.ToDictionary(x => x.Value, x => x.Key);

        // forward
        public static KeyGesture ToKeyGesture(ShortcutDefinition d)
        {
            var key = Map[d.Key];
            return new KeyGesture(key, Convert(d.Modifiers));
        }

        // reverse (правильный вариант)
        public static (ShortcutKey key, ShortcutModifiers modifiers) FromKeyGesture(KeyEventArgs ev, ModifierKeys mods)
        {
            var normalkey = ev.Key == Key.System
                ? ev.SystemKey
                : ev.Key;

            var shortcutKey = ReverseMap.TryGetValue(normalkey, out var k)
                ? k
                : throw new NotSupportedException($"Key not mapped: {normalkey}");

            return (shortcutKey, Convert(mods));
        }

        private static ModifierKeys Convert(ShortcutModifiers mods)
        {
            ModifierKeys result = ModifierKeys.None;

            if (mods.HasFlag(ShortcutModifiers.Ctrl))
                result |= ModifierKeys.Control;

            if (mods.HasFlag(ShortcutModifiers.Alt))
                result |= ModifierKeys.Alt;

            if (mods.HasFlag(ShortcutModifiers.Shift))
                result |= ModifierKeys.Shift;

            if (mods.HasFlag(ShortcutModifiers.Win))
                result |= ModifierKeys.Windows;

            return result;
        }

        private static ShortcutModifiers Convert(ModifierKeys mods)
        {
            ShortcutModifiers result = 0;

            if (mods.HasFlag(ModifierKeys.Control))
                result |= ShortcutModifiers.Ctrl;

            if (mods.HasFlag(ModifierKeys.Alt))
                result |= ShortcutModifiers.Alt;

            if (mods.HasFlag(ModifierKeys.Shift))
                result |= ShortcutModifiers.Shift;

            if (mods.HasFlag(ModifierKeys.Windows))
                result |= ShortcutModifiers.Win;

            return result;
        }
    }
}
