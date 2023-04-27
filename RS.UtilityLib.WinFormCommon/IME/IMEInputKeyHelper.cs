using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace RS.UtilityLib.WinFormCommon.IME
{
   
    public class IMEInputKeyHelper
    {

        #region Fields
        private static KeyCode[] mKeys = new KeyCode[256];
        #endregion

        #region Static constructor
        static IMEInputKeyHelper() {
            mKeys[(int)System.Windows.Forms.Keys.Back] = KeyCode.Backspace;
            mKeys[(int)System.Windows.Forms.Keys.Tab] = KeyCode.Tab;
            mKeys[(int)System.Windows.Forms.Keys.Return] = KeyCode.Return;
            mKeys[(int)System.Windows.Forms.Keys.ShiftKey] = KeyCode.LeftShift;
            mKeys[(int)System.Windows.Forms.Keys.ControlKey] = KeyCode.LeftControl;
            mKeys[(int)System.Windows.Forms.Keys.Menu] = KeyCode.LeftAlt;
            mKeys[(int)System.Windows.Forms.Keys.Capital] = KeyCode.Capital;
            mKeys[(int)System.Windows.Forms.Keys.Escape] = KeyCode.Escape;
            mKeys[(int)System.Windows.Forms.Keys.Space] = KeyCode.Space;
            mKeys[(int)System.Windows.Forms.Keys.PageUp] = KeyCode.PageUp;
            mKeys[(int)System.Windows.Forms.Keys.PageDown] = KeyCode.PageDown;
            mKeys[(int)System.Windows.Forms.Keys.End] = KeyCode.End;
            mKeys[(int)System.Windows.Forms.Keys.Home] = KeyCode.Home;
            mKeys[(int)System.Windows.Forms.Keys.Left] = KeyCode.ArrowLeft;
            mKeys[(int)System.Windows.Forms.Keys.Up] = KeyCode.ArrowUp;
            mKeys[(int)System.Windows.Forms.Keys.Right] = KeyCode.ArrowRight;
            mKeys[(int)System.Windows.Forms.Keys.Down] = KeyCode.ArrowDown;
            mKeys[(int)System.Windows.Forms.Keys.PrintScreen] = KeyCode.SysRq;
            mKeys[(int)System.Windows.Forms.Keys.Insert] = KeyCode.Insert;
            mKeys[(int)System.Windows.Forms.Keys.Delete] = KeyCode.Delete;

            mKeys[(int)System.Windows.Forms.Keys.D0] = KeyCode.Zero;
            mKeys[(int)System.Windows.Forms.Keys.D1] = KeyCode.One;
            mKeys[(int)System.Windows.Forms.Keys.D2] = KeyCode.Two;
            mKeys[(int)System.Windows.Forms.Keys.D3] = KeyCode.Three;
            mKeys[(int)System.Windows.Forms.Keys.D4] = KeyCode.Four;
            mKeys[(int)System.Windows.Forms.Keys.D5] = KeyCode.Five;
            mKeys[(int)System.Windows.Forms.Keys.D6] = KeyCode.Six;
            mKeys[(int)System.Windows.Forms.Keys.D7] = KeyCode.Seven;
            mKeys[(int)System.Windows.Forms.Keys.D8] = KeyCode.Eight;
            mKeys[(int)System.Windows.Forms.Keys.D9] = KeyCode.Nine;

            mKeys[(int)System.Windows.Forms.Keys.A] = KeyCode.A;
            mKeys[(int)System.Windows.Forms.Keys.B] = KeyCode.B;
            mKeys[(int)System.Windows.Forms.Keys.C] = KeyCode.C;
            mKeys[(int)System.Windows.Forms.Keys.D] = KeyCode.D;
            mKeys[(int)System.Windows.Forms.Keys.E] = KeyCode.E;
            mKeys[(int)System.Windows.Forms.Keys.F] = KeyCode.F;
            mKeys[(int)System.Windows.Forms.Keys.G] = KeyCode.G;
            mKeys[(int)System.Windows.Forms.Keys.H] = KeyCode.H;
            mKeys[(int)System.Windows.Forms.Keys.I] = KeyCode.I;
            mKeys[(int)System.Windows.Forms.Keys.J] = KeyCode.J;
            mKeys[(int)System.Windows.Forms.Keys.K] = KeyCode.K;
            mKeys[(int)System.Windows.Forms.Keys.L] = KeyCode.L;
            mKeys[(int)System.Windows.Forms.Keys.M] = KeyCode.M;
            mKeys[(int)System.Windows.Forms.Keys.N] = KeyCode.N;
            mKeys[(int)System.Windows.Forms.Keys.O] = KeyCode.O;
            mKeys[(int)System.Windows.Forms.Keys.P] = KeyCode.P;
            mKeys[(int)System.Windows.Forms.Keys.Q] = KeyCode.Q;
            mKeys[(int)System.Windows.Forms.Keys.R] = KeyCode.R;
            mKeys[(int)System.Windows.Forms.Keys.S] = KeyCode.S;
            mKeys[(int)System.Windows.Forms.Keys.T] = KeyCode.T;
            mKeys[(int)System.Windows.Forms.Keys.U] = KeyCode.U;
            mKeys[(int)System.Windows.Forms.Keys.V] = KeyCode.V;
            mKeys[(int)System.Windows.Forms.Keys.W] = KeyCode.W;
            mKeys[(int)System.Windows.Forms.Keys.X] = KeyCode.X;
            mKeys[(int)System.Windows.Forms.Keys.Y] = KeyCode.Y;
            mKeys[(int)System.Windows.Forms.Keys.Z] = KeyCode.Z;

            mKeys[(int)System.Windows.Forms.Keys.LWin] = KeyCode.LeftWindows;
            mKeys[(int)System.Windows.Forms.Keys.RWin] = KeyCode.RightWindows;
            mKeys[(int)System.Windows.Forms.Keys.Apps] = KeyCode.AppMenu;
            mKeys[(int)System.Windows.Forms.Keys.Sleep] = KeyCode.Sleep;

            mKeys[(int)System.Windows.Forms.Keys.NumPad0] = KeyCode.Numpad0;
            mKeys[(int)System.Windows.Forms.Keys.NumPad1] = KeyCode.Numpad1;
            mKeys[(int)System.Windows.Forms.Keys.NumPad2] = KeyCode.Numpad2;
            mKeys[(int)System.Windows.Forms.Keys.NumPad3] = KeyCode.Numpad3;
            mKeys[(int)System.Windows.Forms.Keys.NumPad4] = KeyCode.Numpad4;
            mKeys[(int)System.Windows.Forms.Keys.NumPad5] = KeyCode.Numpad5;
            mKeys[(int)System.Windows.Forms.Keys.NumPad6] = KeyCode.Numpad6;
            mKeys[(int)System.Windows.Forms.Keys.NumPad7] = KeyCode.Numpad7;
            mKeys[(int)System.Windows.Forms.Keys.NumPad8] = KeyCode.Numpad8;
            mKeys[(int)System.Windows.Forms.Keys.NumPad9] = KeyCode.Numpad9;

            mKeys[(int)System.Windows.Forms.Keys.Multiply] = KeyCode.Multiply;
            mKeys[(int)System.Windows.Forms.Keys.Add] = KeyCode.Add;
            mKeys[(int)System.Windows.Forms.Keys.Subtract] = KeyCode.Subtract;
            mKeys[(int)System.Windows.Forms.Keys.Decimal] = KeyCode.Decimal;
            mKeys[(int)System.Windows.Forms.Keys.Divide] = KeyCode.Divide;

            mKeys[(int)System.Windows.Forms.Keys.F1] = KeyCode.F1;
            mKeys[(int)System.Windows.Forms.Keys.F2] = KeyCode.F2;
            mKeys[(int)System.Windows.Forms.Keys.F3] = KeyCode.F3;
            mKeys[(int)System.Windows.Forms.Keys.F4] = KeyCode.F4;
            mKeys[(int)System.Windows.Forms.Keys.F5] = KeyCode.F5;
            mKeys[(int)System.Windows.Forms.Keys.F6] = KeyCode.F6;
            mKeys[(int)System.Windows.Forms.Keys.F7] = KeyCode.F7;
            mKeys[(int)System.Windows.Forms.Keys.F8] = KeyCode.F8;
            mKeys[(int)System.Windows.Forms.Keys.F9] = KeyCode.F9;
            mKeys[(int)System.Windows.Forms.Keys.F10] = KeyCode.F10;
            mKeys[(int)System.Windows.Forms.Keys.F11] = KeyCode.F11;
            mKeys[(int)System.Windows.Forms.Keys.F12] = KeyCode.F12;
            mKeys[(int)System.Windows.Forms.Keys.F13] = KeyCode.F13;
            mKeys[(int)System.Windows.Forms.Keys.F14] = KeyCode.F14;
            mKeys[(int)System.Windows.Forms.Keys.F15] = KeyCode.F15;

            mKeys[(int)System.Windows.Forms.Keys.NumLock] = KeyCode.NumLock;
            mKeys[(int)System.Windows.Forms.Keys.LShiftKey] = KeyCode.LeftShift;
            mKeys[(int)System.Windows.Forms.Keys.RShiftKey] = KeyCode.RightShift;
            mKeys[(int)System.Windows.Forms.Keys.LControlKey] = KeyCode.LeftControl;
            mKeys[(int)System.Windows.Forms.Keys.RControlKey] = KeyCode.RightControl;
            mKeys[(int)System.Windows.Forms.Keys.LMenu] = KeyCode.LeftAlt;
            mKeys[(int)System.Windows.Forms.Keys.RMenu] = KeyCode.RightAlt;

            mKeys[(int)System.Windows.Forms.Keys.Oemtilde] = KeyCode.Grave;
            mKeys[(int)System.Windows.Forms.Keys.OemMinus] = KeyCode.Minus;
            mKeys[(int)System.Windows.Forms.Keys.Oemplus] = KeyCode.Equals;
            mKeys[(int)System.Windows.Forms.Keys.OemPipe] = KeyCode.Backslash;
            mKeys[(int)System.Windows.Forms.Keys.OemOpenBrackets] = KeyCode.LeftBracket;
            mKeys[(int)System.Windows.Forms.Keys.OemCloseBrackets] = KeyCode.RightBracket;
            mKeys[(int)System.Windows.Forms.Keys.OemSemicolon] = KeyCode.Semicolon;
            mKeys[(int)System.Windows.Forms.Keys.OemQuotes] = KeyCode.Apostrophe;
            mKeys[(int)System.Windows.Forms.Keys.Oemcomma] = KeyCode.Comma;
            mKeys[(int)System.Windows.Forms.Keys.OemPeriod] = KeyCode.Period;
            mKeys[(int)System.Windows.Forms.Keys.OemQuestion] = KeyCode.Slash;
            mKeys[(int)System.Windows.Forms.Keys.Scroll] = KeyCode.ScrollLock;
            mKeys[(int)System.Windows.Forms.Keys.Pause] = KeyCode.Pause;
        }
        #endregion

        #region Export
        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int ToUnicodeEx(
            uint wVirtKey,
            uint wScanCode,
            System.Windows.Forms.Keys[] lpKeyState,
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags,
            IntPtr dwhkl);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetKeyboardLayout(uint threadId);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern bool GetKeyboardState(System.Windows.Forms.Keys[] keyStates);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hwindow, out uint processId);
        #endregion

        #region Methods
        private static string CodeToString(int scanCode) {
            uint procId;
            uint thread = GetWindowThreadProcessId(Process.GetCurrentProcess().MainWindowHandle, out procId);
            IntPtr hkl = GetKeyboardLayout(thread);

            if (hkl == IntPtr.Zero) {
                Console.WriteLine("Sorry, that keyboard does not seem to be valid.");
                return string.Empty;
            }

            System.Windows.Forms.Keys[] keyStates = new System.Windows.Forms.Keys[256];
            if (!GetKeyboardState(keyStates))
                return string.Empty;

            StringBuilder sb = new StringBuilder(10);//todo:china
            int rc = ToUnicodeEx((uint)scanCode, (uint)scanCode, keyStates, sb, sb.Capacity, 0, hkl);
            return sb.ToString();
        }

        public static uint VirtualKeyToChar(System.Windows.Forms.Keys _virtualKey) {
            string result = CodeToString((int)_virtualKey);
            return (uint)(result.Length > 0 ? result[0] : 0);
        }

        public static KeyCode VirtualKeyToScanCode(System.Windows.Forms.Keys _virtualKey) {
            int value = (int)_virtualKey;
            if (value < mKeys.Length) {
                KeyCode result = mKeys[value];
                return result;
            }
            return KeyCode.None;
        }

        public static MouseButton MouseFlagToMouseButton(System.Windows.Forms.MouseButtons _button) {
            if (_button == System.Windows.Forms.MouseButtons.Left)
                return MouseButton.Left;
            else if (_button == System.Windows.Forms.MouseButtons.Right)
                return MouseButton.Right;
            else if (_button == System.Windows.Forms.MouseButtons.Middle)
                return MouseButton.Middle;

            return MouseButton.None;
        }
        #endregion

    }

    public enum KeyCode
    {
        None = 0,
        Escape = 1,
        One = 2,
        Two = 3,
        Three = 4,
        Four = 5,
        Five = 6,
        Six = 7,
        Seven = 8,
        Eight = 9,
        Nine = 10,
        Zero = 11,
        Minus = 12,
        Equals = 13,
        Backspace = 14,
        Tab = 15,
        Q = 16,
        W = 17,
        E = 18,
        R = 19,
        T = 20,
        Y = 21,
        U = 22,
        I = 23,
        O = 24,
        P = 25,
        LeftBracket = 26,
        RightBracket = 27,
        Return = 28,
        LeftControl = 29,
        A = 30,
        S = 31,
        D = 32,
        F = 33,
        G = 34,
        H = 35,
        J = 36,
        K = 37,
        L = 38,
        Semicolon = 39,
        Apostrophe = 40,
        Grave = 41,
        LeftShift = 42,
        Backslash = 43,
        Z = 44,
        X = 45,
        C = 46,
        V = 47,
        B = 48,
        N = 49,
        M = 50,
        Comma = 51,
        Period = 52,
        Slash = 53,
        RightShift = 54,
        Multiply = 55,
        LeftAlt = 56,
        Space = 57,
        Capital = 58,
        F1 = 59,
        F2 = 60,
        F3 = 61,
        F4 = 62,
        F5 = 63,
        F6 = 64,
        F7 = 65,
        F8 = 66,
        F9 = 67,
        F10 = 68,
        NumLock = 69,
        ScrollLock = 70,
        Numpad7 = 71,
        Numpad8 = 72,
        Numpad9 = 73,
        Subtract = 74,
        Numpad4 = 75,
        Numpad5 = 76,
        Numpad6 = 77,
        Add = 78,
        Numpad1 = 79,
        Numpad2 = 80,
        Numpad3 = 81,
        Numpad0 = 82,
        Decimal = 83,
        OEM_102 = 86,
        F11 = 87,
        F12 = 88,
        F13 = 100,
        F14 = 101,
        F15 = 102,
        Kana = 112,
        ABNT_C1 = 115,
        YesConvert = 121,
        NoConvert = 123,
        Yen = 125,
        ABNT_C2 = 126,
        NumpadEquals = 141,
        PrevTrack = 144,
        At = 145,
        Colon = 146,
        Underline = 147,
        Kanji = 148,
        Stop = 149,
        AX = 150,
        Unlabeled = 151,
        NextTrack = 153,
        NumpadEnter = 156,
        RightControl = 157,
        Mute = 160,
        Calculator = 161,
        PlayPause = 162,
        MediaStop = 164,
        VolumeDown = 174,
        VolumeUp = 176,
        WebHome = 178,
        NumpadComma = 179,
        Divide = 181,
        SysRq = 183,
        RightAlt = 184,
        Pause = 197,
        Home = 199,
        ArrowUp = 200,
        PageUp = 201,
        ArrowLeft = 203,
        ArrowRight = 205,
        End = 207,
        ArrowDown = 208,
        PageDown = 209,
        Insert = 210,
        Delete = 211,
        LeftWindows = 219,
        RightWindows = 220,
        RightWindow = 220,
        AppMenu = 221,
        Power = 222,
        Sleep = 223,
        Wake = 227,
        WebSearch = 229,
        WebFavorites = 230,
        WebRefresh = 231,
        WebStop = 232,
        WebForward = 233,
        WebBack = 234,
        MyComputer = 235,
        Mail = 236,
        MediaSelect = 237
    }

    public enum MouseButton
    {
        None = -1,
        Button0 = 0,
        Left = 0,
        Button1 = 1,
        Right = 1,
        Button2 = 2,
        Middle = 2,
        Button3 = 3,
        Button4 = 4,
        Button5 = 5,
        Button6 = 6,
        Button7 = 7
    }
}
