using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;

namespace RS.UtilityLib.WinFormCommon.UINotifier
{
    public class UIBalloonHelpForm : UIBalloonWindow
    {
        private WindowsHook windowsHook;
        private System.ComponentModel.IContainer components = null;

        #region Constants
        const int CLOSEBUTTON_WIDTH = 14;
        const int CLOSEBUTTON_HEIGHT = 14;
        const uint SHOWCLOSEBUTTON = 0x1;
        const uint CLOSEONMOUSECLICK = 0x2;
        const uint CLOSEONKEYPRESS = 0x4;
        const uint CLOSEONMOUSEMOVE = 0x10;
        const uint CLOSEONDEACTIVATE = 0x20;
        const uint ENABLETIMEOUT = 0x40;
        const int WM_ACTIVATEAPP = 0x001C;
        #endregion

        #region Fields
        private uint flags = CLOSEONMOUSECLICK | CLOSEONKEYPRESS | CLOSEONDEACTIVATE;
        private string content = String.Empty;
        private Font captionFont;
        private System.Windows.Forms.Button closeButton;
        private Size headerSize;
        private System.Windows.Forms.Timer timer1;
        #endregion

        public UIBalloonHelpForm() {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
        }

        #region Utility functions
        private void SetBoolProp(uint prop, bool b) {
            if (b)
                flags |= prop;
            else
                flags &= ~prop;
        }

        private bool GetBoolProp(uint prop) {
            return (flags & prop) != 0;
        }
        #endregion

        #region Public Properties

        [Category("Balloon")]
        public int Timeout {
            get {
                return timer1.Interval;
            }
            set {
                timer1.Interval = value;
            }
        }

        [Category("Balloon")]
        public bool ShowCloseButton {
            get {
                return Controls.Contains(closeButton);
            }
            set {
                if (!value) {
                    if (Controls.Contains(closeButton))
                        Controls.Remove(closeButton);
                }
                else {
                    if (!Controls.Contains(closeButton))
                        Controls.Add(closeButton);
                }

                SetBoolProp(SHOWCLOSEBUTTON, value);
            }
        }

        [Category("Balloon")]
        public bool EnableTimeout {
            get {
                return GetBoolProp(ENABLETIMEOUT);
            }
            set {
                SetBoolProp(ENABLETIMEOUT, value);
            }
        }

        [Category("Balloon")]
        public bool CloseOnMouseClick {
            get {
                return GetBoolProp(CLOSEONMOUSECLICK);
            }
            set {
                SetBoolProp(CLOSEONMOUSECLICK, value);
            }
        }

        [Category("Balloon")]
        public bool CloseOnKeyPress {
            get {
                return GetBoolProp(CLOSEONKEYPRESS);
            }
            set {
                SetBoolProp(CLOSEONKEYPRESS, value);
            }
        }

        [Category("Balloon")]
        public bool CloseOnMouseMove {
            get {
                return GetBoolProp(CLOSEONMOUSEMOVE);
            }
            set {
                SetBoolProp(CLOSEONMOUSEMOVE, value);
            }
        }

        [Category("Balloon")]
        public bool CloseOnDeactivate {
            get {
                return GetBoolProp(CLOSEONDEACTIVATE);
            }
            set {
                SetBoolProp(CLOSEONDEACTIVATE, value);
            }
        }

        [Category("Balloon")]
        public string Content {
            get {
                return content;
            }
            set {
                content = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        [Category("Balloon")]
        public string Caption {
            get {
                string ret = this.Text;
                return (ret == null) ? String.Empty : ret;
            }
            set {
                this.Text = value;
            }
        }

        [Category("Balloon")]
        public Font CaptionFont {
            get {
                if (captionFont == null)
                    captionFont = new Font(this.Font, FontStyle.Bold);

                return captionFont;
            }
            set {
                captionFont = value;
            }
        }
        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Calculate size
        protected Size CalcClientSize() {
            System.Drawing.Size size = Size.Empty;

            using (System.Drawing.Graphics g = this.CreateGraphics()) {
                if (this.Icon != null)
                    size = this.Icon.Size;

                if (this.ShowCloseButton) {
                    if (size.Width != 0)
                        size.Width += TIPMARGIN;

                    size.Width += (CLOSEBUTTON_WIDTH + TIPMARGIN);
                    size.Height = Math.Max(size.Height, CLOSEBUTTON_HEIGHT);
                }

                if (this.Caption.Length != 0) {
                    if (size.Width != 0)
                        size.Width += TIPMARGIN;

                    System.Drawing.Size captionSize = Size.Ceiling(g.MeasureString(Caption, this.CaptionFont));
                    size.Width += captionSize.Width;
                    size.Height = Math.Max(captionSize.Height, size.Height);
                }

                headerSize = size;

                string text = this.Content;

                if ((text != null) && (text.Length != 0)) {
                    size.Height += TIPMARGIN;

                    System.Drawing.Size textSize = Size.Ceiling(g.MeasureString(text, this.Font));
                    size.Height += textSize.Height;
                    size.Width = Math.Max(textSize.Width, headerSize.Width);
                    headerSize.Width = size.Width;
                }
            }

            return size;
        }
        #endregion

        #region Client Area Drawing

        private void Draw(Graphics g) {
            Rectangle drawingRect = this.ClientRectangle;

            string content = this.Content;

            if (content.Length != 0) {
                RectangleF textRect = new RectangleF(0, headerSize.Height + TIPMARGIN, drawingRect.Width, drawingRect.Height - headerSize.Height - TIPMARGIN);
                Brush b = new SolidBrush(this.ForeColor);
                g.DrawString(content, this.Font, b, textRect);
                b.Dispose();
            }

            // calc & draw icon
            if (this.Icon != null) {
                g.DrawIcon(this.Icon, 0, 0);
                drawingRect.X += (this.Icon.Width);
                drawingRect.Width -= (this.Icon.Width);
            }

            // calc & draw close button
            if (this.ShowCloseButton) {
                drawingRect.Width -= (CLOSEBUTTON_WIDTH);
            }

            string caption = this.Caption;

            if (caption.Length != 0) {
                drawingRect.X += TIPMARGIN;
                drawingRect.Width -= TIPMARGIN;

                //StringFormat sf = new StringFormat();
                //sf.Alignment = StringAlignment.Center;

                Brush b = new SolidBrush(this.ForeColor);

                g.DrawString(caption, this.CaptionFont, b, (RectangleF)drawingRect);//, sf); 

                b.Dispose();
                //sf.Dispose();
            }
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
            Draw(e.Graphics);
        }

        #endregion

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.windowsHook = new RS.UtilityLib.WinFormCommon.UINotifier.UIBalloonHelpForm.WindowsHook(this.components);
            this.closeButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // windowsHook
            // 
            this.windowsHook.ThreadID = 0;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Location = new System.Drawing.Point(199, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(17, 15);
            this.closeButton.TabIndex = 0;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // UIBalloonHelpForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(216, 158);
            this.Controls.Add(this.closeButton);
            this.Name = "UIBalloonHelpForm";
            this.ResumeLayout(false);

        }
        #endregion

        protected override void OnLoad(System.EventArgs e) {
            this.ClientSize = CalcClientSize();

            if ((this.flags != 0) && (this.flags != SHOWCLOSEBUTTON)) {
                if (this.CloseOnKeyPress)
                    this.windowsHook.KeyBoardHook += new KeyBoardHookEventHandler(this.windowsHook_KeyBoardHook);

                if ((this.CloseOnMouseClick) || (this.CloseOnMouseMove))
                    this.windowsHook.MouseHook += new MouseHookEventHandler(this.windowsHook_MouseHook);

                windowsHook.HookCurrentThread();
            }

            if (this.EnableTimeout)
                timer1.Start();

            base.OnLoad(e);
        }

        private void windowsHook_KeyBoardHook(object sender, KeyBoardHookEventArgs e) {
            this.Close();
        }

        private void windowsHook_MouseHook(object sender, MouseHookEventArgs e) {
            switch (e.Message) {
                case MouseMessages.LButtonDblClk:
                case MouseMessages.LButtonDown:
                case MouseMessages.LButtonUp:
                case MouseMessages.MButtonDblClk:
                case MouseMessages.MButtonDown:
                case MouseMessages.MButtonUp:
                case MouseMessages.RButtonDblClk:
                case MouseMessages.RButtonDown:
                case MouseMessages.RButtonUp:
                case MouseMessages.XButtonDblClk:
                case MouseMessages.XButtonDown:
                    if (this.CloseOnMouseClick)
                        Close();
                    break;
                case MouseMessages.MouseMove:
                    if (this.CloseOnMouseMove)
                        Close();
                    break;
            }
        }

        protected virtual void OnDeactivateApp(System.EventArgs e) {
            if (this.CloseOnDeactivate)
                Close();
        }

        protected override void WndProc(ref Message m) {
            if ((m.Msg == WM_ACTIVATEAPP) && (m.WParam == IntPtr.Zero)) {
                OnDeactivateApp(EventArgs.Empty);
            }
            else
                base.WndProc(ref m);
        }

        protected override void OnClosed(System.EventArgs e) {
            windowsHook.Dispose();
            base.OnClosed(e);
        }

        private void closeButton_Click(object sender, System.EventArgs e) {
            Close();
        }

        private void timer1_Tick(object sender, System.EventArgs e) {
            Close();
        }


        #region hook辅助
        internal enum HookType
        {
            //MsgFilter		= -1,
            //JournalRecord    = 0,
            //JournalPlayback  = 1,
            Keyboard = 2,
            //GetMessage       = 3,
            CallWndProc = 4,
            //CBT              = 5,
            //SysMsgFilter     = 6,
            Mouse = 7,
            //Hardware         = 8,
            //Debug            = 9,
            //Shell           = 10,
            //ForegroundIdle  = 11,
            CallWndProcRet = 12,
            //KeyboardLL		= 13,
            //MouseLL			= 14,
        }

        /// <summary>
        /// Summary description for HookHandle.
        /// </summary>
        abstract class BaseHook : IDisposable
        {
            IntPtr handle;
            WindowsHook parent;
            GCHandle delegateHandle;

            internal BaseHook() {
            }

            [DllImport("User32.dll")]
            internal extern static void UnhookWindowsHookEx(IntPtr handle);

            [DllImport("User32.dll")]
            internal extern static IntPtr SetWindowsHookEx(int idHook, [MarshalAs(UnmanagedType.FunctionPtr)] HookProc lpfn, IntPtr hinstance, int threadID);

            [DllImport("User32.dll")]
            internal extern static IntPtr CallNextHookEx(IntPtr handle, int code, IntPtr wparam, IntPtr lparam);

            IntPtr HookProc(int code, IntPtr wparam, IntPtr lparam) {
                if (code >= 0) {
                    try {
                        InvokeHookEvent(code, wparam, lparam);
                    }
                    catch (Exception e) {
                        System.Diagnostics.Trace.WriteLine(String.Format("Unhandled Exception {0}", e));
                    }
                }

                return CallNextHookEx(handle, code, wparam, lparam);
            }

            internal abstract HookType Type {
                get;
            }

            protected abstract void InvokeHookEvent(int code, IntPtr wparam, IntPtr lparam);

            internal void SetHook(WindowsHook parent, IntPtr hinstance, int threadID) {
                if (handle != IntPtr.Zero)
                    Dispose(false);

                this.parent = parent;

                HookProc proc = new HookProc(this.HookProc);

                handle = SetWindowsHookEx((int)Type, proc, hinstance, threadID);

                if (handle == IntPtr.Zero) {
                    GC.SuppressFinalize(this);
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                this.delegateHandle = GCHandle.Alloc(proc);
            }

            private void Dispose(bool disposing) {
                if (handle != IntPtr.Zero) {
                    UnhookWindowsHookEx(handle);
                }

                if (delegateHandle.IsAllocated)
                    delegateHandle.Free();
            }
            ~BaseHook() {
                Dispose(false);
            }
            public void Dispose() {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected WindowsHook Parent {
                get {
                    return this.parent;
                }
            }
        }
        /// <summary>
        /// Summary description for CallWndProcHook.
        /// </summary>
        public class CallWndProcHookEventArgs : EventArgs
        {
            Message message;
            bool sentFromCurrentProcess;

            [StructLayout(LayoutKind.Sequential)]
            internal class CWPSTRUCT
            {
                public IntPtr lparam;
                public IntPtr wparam;
                public int message;
                public IntPtr hwnd;
            };

            internal CallWndProcHookEventArgs(IntPtr wparam, IntPtr lparam) {
                sentFromCurrentProcess = wparam != IntPtr.Zero;

                CWPSTRUCT cwp = (CWPSTRUCT)Marshal.PtrToStructure(lparam, typeof(CWPSTRUCT));
                message = Message.Create(cwp.hwnd, cwp.message, cwp.wparam, cwp.lparam);
            }

            public System.Windows.Forms.Message Message {
                get {
                    return message;
                }
            }

            public bool SentFromCurrentProcess {
                get {
                    return this.sentFromCurrentProcess;
                }
            }
        }

        public delegate void CallWndProcHookEventHandler(object sender, CallWndProcHookEventArgs e);

        /// <summary>
        /// Summary description for CallWndRetProcHook.
        /// </summary>
        class CallWndProcHook : BaseHook
        {
            internal CallWndProcHook() {
            }

            internal override HookType Type {
                get {
                    return HookType.CallWndProc;
                }
            }

            protected override void InvokeHookEvent(int code, System.IntPtr wparam, System.IntPtr lparam) {
                CallWndProcHookEventArgs e = new CallWndProcHookEventArgs(wparam, lparam);
                Parent.OnCallWndProcHook(this, e);
            }
        }
        public class CallWndProcRetHookEventArgs : EventArgs
        {
            Message message;
            bool sentFromCurrentProcess;

            [StructLayout(LayoutKind.Sequential)]
            internal class CWPRETSTRUCT
            {
                public IntPtr result;
                public IntPtr lparam;
                public IntPtr wparam;
                public int message;
                public IntPtr hwnd;
            };

            internal CallWndProcRetHookEventArgs(IntPtr wparam, IntPtr lparam) {
                sentFromCurrentProcess = wparam != IntPtr.Zero;

                CWPRETSTRUCT cwpr = (CWPRETSTRUCT)Marshal.PtrToStructure(lparam, typeof(CWPRETSTRUCT));
                message = Message.Create(cwpr.hwnd, cwpr.message, cwpr.wparam, cwpr.lparam);
                message.Result = cwpr.result;
            }

            public System.Windows.Forms.Message Message {
                get {
                    return message;
                }
            }

            public bool SentFromCurrentProcess {
                get {
                    return this.sentFromCurrentProcess;
                }
            }
        };

        public delegate void CallWndProcRetHookEventHandler(object sender, CallWndProcRetHookEventArgs e);

        /// <summary>
        /// Summary description for CallWndRetProcHook.
        /// </summary>
        class CallWndProcRetHook : BaseHook
        {
            internal CallWndProcRetHook() {
            }

            internal override HookType Type {
                get {
                    return HookType.CallWndProcRet;
                }
            }

            protected override void InvokeHookEvent(int code, System.IntPtr wparam, System.IntPtr lparam) {
                CallWndProcRetHookEventArgs e = new CallWndProcRetHookEventArgs(wparam, lparam);
                Parent.OnCallWndRetProcHook(this, e);
            }
        }
        public class KeyBoardHookEventArgs : EventArgs
        {
            const int KF_EXTENDED = 0x0100;
            const int KF_DLGMODE = 0x0800;
            const int KF_MENUMODE = 0x1000;
            const int KF_ALTDOWN = 0x2000;
            const int KF_REPEAT = 0x4000;
            const int KF_UP = 0x8000;

            private Keys virtKey;
            private int keyFlags;

            internal KeyBoardHookEventArgs(IntPtr wParam, IntPtr lParam) {
                int virtKeyInt = (int)wParam;

                if (!Enum.IsDefined(typeof(Keys), virtKeyInt))
                    virtKey = Keys.None;
                else
                    virtKey = (Keys)virtKeyInt;

                keyFlags = (int)lParam;
            }

            public Keys VirtKey {
                get {
                    return virtKey;
                }
            }

            public short Repeat {
                get {
                    return (short)(keyFlags & KF_EXTENDED);
                }
            }

            public bool AltDown {
                get {
                    return (keyFlags & KF_ALTDOWN) != 0;
                }
            }

            public bool IsDialogActive {
                get {
                    return (keyFlags & KF_DLGMODE) != 0;
                }
            }

            public bool IsMenuActive {
                get {
                    return (keyFlags & KF_MENUMODE) != 0;
                }
            }

            public bool IsKeyUp {
                get {
                    return (keyFlags & KF_UP) != 0;
                }
            }
        };

        public delegate void KeyBoardHookEventHandler(object sender, KeyBoardHookEventArgs e);

        /// <summary>
        /// Summary description for KeyBoardHook.
        /// </summary>
        class KeyBoardHook : BaseHook
        {
            public KeyBoardHook() {
            }

            protected override void InvokeHookEvent(int code, System.IntPtr wparam, System.IntPtr lparam) {
                Parent.OnKeyBoardHook(this, new KeyBoardHookEventArgs(wparam, lparam));
            }

            internal override HookType Type {
                get {
                    return HookType.Keyboard;
                }
            }
        }
        public enum MouseMessages
        {
            Unknown = 0x0000,
            MouseMove = 0x0200,
            LButtonDown = 0x0201,
            LButtonUp = 0x0202,
            LButtonDblClk = 0x0203,
            RButtonDown = 0x0204,
            RButtonUp = 0x0205,
            RButtonDblClk = 0x0206,
            MButtonDown = 0x0207,
            MButtonUp = 0x0208,
            MButtonDblClk = 0x0209,
            MouseWheel = 0x020A,
            XButtonDown = 0x020B,
            XButtonUP = 0x020C,
            XButtonDblClk = 0x020D,
        };

        public class MouseHookEventArgs
        {
            private MouseMessages message;
            private Point point;
            private IntPtr hwnd;
            private HitTestCodes hitTestCode;
            private IntPtr extraInfo;

            [StructLayout(LayoutKind.Sequential)]
            struct POINT
            {
                public int x;
                public int y;
            }

            [StructLayout(LayoutKind.Sequential)]
            class MOUSEHOOKSTRUCT
            {
                public POINT pt;
                public IntPtr hwnd;
                public int hitTestCode;
                public IntPtr extraInfo;
            }

            internal MouseHookEventArgs(IntPtr wparam, IntPtr lparam) {
                if (!Enum.IsDefined(typeof(MouseMessages), wparam.ToInt32()))
                    message = MouseMessages.Unknown;
                else
                    message = (MouseMessages)wparam.ToInt32();

                MOUSEHOOKSTRUCT hs = (MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lparam, typeof(MOUSEHOOKSTRUCT));

                point = new Point(hs.pt.x, hs.pt.y);
                hwnd = hs.hwnd;
                hitTestCode = (HitTestCodes)hs.hitTestCode;
                extraInfo = hs.extraInfo;
            }

            public MouseMessages Message {
                get {
                    return message;
                }
            }

            public Point Point {
                get {
                    return point;
                }
            }

            public IntPtr Hwnd {
                get {
                    return hwnd;
                }
            }

            public HitTestCodes HitTestCode {
                get {
                    return hitTestCode;
                }
            }

            public IntPtr ExtraInfo {
                get {
                    return extraInfo;
                }
            }
        };

        public delegate void MouseHookEventHandler(object sender, MouseHookEventArgs e);

        /// <summary>
        /// Summary description for MouseHook.
        /// </summary>
        class MouseHook : BaseHook
        {
            public MouseHook() {
            }

            protected override void InvokeHookEvent(int code, System.IntPtr wparam, System.IntPtr lparam) {
                Parent.OnMouseHook(this, new MouseHookEventArgs(wparam, lparam));
            }

            internal override HookType Type {
                get {
                    return HookType.Mouse;
                }
            }
        }
        internal delegate IntPtr HookProc(int code, IntPtr wparam, IntPtr lparam);

        /// <summary>
        /// Summary description for WindowsHook.
        /// </summary>
        public class WindowsHook : System.ComponentModel.Component
        {
            private ArrayList hooks;
            private int threadID;

            public WindowsHook(System.ComponentModel.IContainer container)
                : this() {
                container.Add(this);
            }

            public WindowsHook() {
                //Reserve space for atmost 4 hooks
                hooks = new ArrayList(4);
            }

            /// <summary>
            /// The thread which needs to be hooked
            /// </summary>
            public int ThreadID {
                get {
                    return threadID;
                }
                set {
                    threadID = value;

                    //Start hooking or change the hooks
                    foreach (BaseHook hook in hooks) {
                        hook.SetHook(this, IntPtr.Zero, value);
                    }
                }
            }

            [DllImport("kernel32.dll")]
            static extern int GetCurrentThreadId();

            public void HookCurrentThread() {
                this.ThreadID = GetCurrentThreadId();
            }

            protected override void Dispose(bool disposing) {
                if (disposing) {
                    //dispose individual hooks
                    foreach (IDisposable hook in hooks) {
                        hook.Dispose();
                    }
                }
            }

            protected internal virtual void OnMouseHook(object hook, MouseHookEventArgs e) {
                MouseHookEventHandler handler = (MouseHookEventHandler)Events[hook];
                handler(this, e);
            }

            protected internal virtual void OnKeyBoardHook(object hook, KeyBoardHookEventArgs e) {
                KeyBoardHookEventHandler handler = (KeyBoardHookEventHandler)Events[hook];
                handler(this, e);
            }

            protected internal virtual void OnCallWndRetProcHook(object key, CallWndProcRetHookEventArgs e) {
                CallWndProcRetHookEventHandler handler = (CallWndProcRetHookEventHandler)Events[key];
                handler(this, e);
            }

            protected internal virtual void OnCallWndProcHook(object key, CallWndProcHookEventArgs e) {
                CallWndProcHookEventHandler handler = (CallWndProcHookEventHandler)Events[key];
                handler(this, e);
            }

            private BaseHook GetHookObjectForType(HookType type) {
                BaseHook ret = null;

                foreach (BaseHook hook in hooks) {
                    if (hook.Type == type) {
                        ret = hook;
                        break;
                    }
                }

                return ret;
            }

            private void AddHookEventHandler(HookType type, Type classType, Delegate value) {
                BaseHook key = GetHookObjectForType(type);

                if (key == null) {
                    key = (BaseHook)Activator.CreateInstance(classType, true);

                    if (threadID != 0)
                        key.SetHook(this, IntPtr.Zero, threadID);

                    hooks.Add(key);
                }

                Events.AddHandler(key, value);
            }

            private void RemoveHookEventHandler(HookType type, Delegate value) {
                BaseHook key = GetHookObjectForType(type);
                Events.RemoveHandler(key, value);

                if (Events[key] == null)
                    key.Dispose();

                hooks.Remove(key);
            }

            public event MouseHookEventHandler MouseHook {
                add {
                    AddHookEventHandler(HookType.Mouse, typeof(MouseHook), value);
                }
                remove {
                    RemoveHookEventHandler(HookType.Mouse, value);
                }
            }

            public event KeyBoardHookEventHandler KeyBoardHook {
                add {
                    AddHookEventHandler(HookType.Keyboard, typeof(KeyBoardHook), value);
                }
                remove {
                    RemoveHookEventHandler(HookType.Keyboard, value);
                }
            }

            public event CallWndProcRetHookEventHandler CallWndProcRetHook {
                add {
                    AddHookEventHandler(HookType.CallWndProcRet, typeof(CallWndProcRetHook), value);
                }
                remove {
                    RemoveHookEventHandler(HookType.CallWndProcRet, value);
                }
            }

            public event CallWndProcHookEventHandler CallWndProcHook {
                add {
                    AddHookEventHandler(HookType.CallWndProc, typeof(CallWndProcHook), value);
                }
                remove {
                    RemoveHookEventHandler(HookType.CallWndProc, value);
                }
            }
        }

        #endregion

    }
}
