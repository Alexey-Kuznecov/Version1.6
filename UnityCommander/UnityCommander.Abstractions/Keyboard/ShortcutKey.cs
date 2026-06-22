
namespace UnityCommander.Abstractions.Keyboard
{
    public enum ShortcutKey
    {
        // Letters
        A, B, C, D, E, F, G, H, I, J, K, L, M,
        N, O, P, Q, R, S, T, U, V, W, X, Y, Z,

        // Numbers (top row)
        D0, D1, D2, D3, D4, D5, D6, D7, D8, D9,

        // Function keys
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
        F13, F14, F15, F16, F17, F18, F19, F20, F21, F22, F23, F24,

        // Navigation
        Up,
        Down,
        Left,
        Right,

        Home,
        End,
        PageUp,
        PageDown,

        // Editing
        Enter,
        Escape,
        Backspace,
        Tab,
        Space,
        Delete,
        Insert,

        // Numpad
        NumPad0,
        NumPad1,
        NumPad2,
        NumPad3,
        NumPad4,
        NumPad5,
        NumPad6,
        NumPad7,
        NumPad8,
        NumPad9,

        NumPadMultiply,
        NumPadAdd,
        NumPadSubtract,
        NumPadDecimal,
        NumPadDivide,
      
        LeftCtrl,
        RightCtrl,
        LeftShift,
        RightShift,
        LeftAlt,
        RightAlt,

        // OEM / punctuation
        OemMinus,
        OemPlus,
        OemOpenBrackets,
        OemCloseBrackets,
        OemPipe,
        OemSemicolon,
        OemQuotes,
        OemComma,
        OemPeriod,
        OemQuestion,
        OemTilde,

        // Special
        CapsLock,
        ScrollLock,
        NumLock,
        PrintScreen,
        Pause,
        Apps,

        // Media (если вдруг захочешь хоткеи как в нормальных приложениях)
        MediaPlayPause,
        MediaStop,
        MediaNextTrack,
        MediaPreviousTrack,
        VolumeUp,
        VolumeDown,
        VolumeMute,

        Oem1, Oem2, Oem3, Oem4, Oem5, Oem6, Oem7, Oem8, Oem9,

        System,
    }
}
