// Firefox Theme.
// Made by AeroRev9.
// 25/07/2015.
// Updated : 29/07/2015 [2].
// Credits : Mavaamarten, Xertz.
// Converted by LaPanthere

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UIFirefoxTheme
{
    static class Theme
    {

        public static Font GlobalFont(FontStyle B, int S) {
            return new Font("Segoe UI", S, B);
        }

        public static string GetCheckMark() {
            return "iVBORw0KGgoAAAANSUhEUgAAABMAAAAQCAYAAAD0xERiAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAEySURBVDhPY/hPRUBdw/79+/efVHz77bf/X37+wRAn2bDff/7+91l+83/YmtsYBpJs2ITjz/8rTbrwP2Dlrf9XXn5FkSPJsD13P/y3nHsVbNjyy28w5Ik27NWXX//TNt8DG1S19zFWNRiGvfzy8//ccy9RxEB4wvFnYIMMZl7+//brLwx5EEYx7MP33/9dF18Ha1py8RVcHBR7mlMvgsVXX8X0Hgwz/P379z8yLtz5AKxJdcpFcBj9+v3nf/CqW2Cx5E13UdSiYwzDvv36/d9/BUSzzvRL/0t2PQSzQd57+vEHilp0jGEYCJ9+8hnuGhiee+4Vhjp0jNUwEN566/1/m/mQZJC/48H/zz9+YVWHjHEaBsKgwAZ59eH771jl0TFew0D48osvWMWxYYKGEY///gcAqiuA6kEmfEMAAAAASUVORK5CYII=";
        }

    }

    static class Helpers
    {

        public enum MouseState : byte
        {
            None = 0,
            Over = 1,
            Down = 2
        }

        public static Rectangle FullRectangle(Size S, bool Subtract) {
            if (Subtract) {
                return new Rectangle(0, 0, S.Width - 1, S.Height - 1);
            }
            else {
                return new Rectangle(0, 0, S.Width, S.Height);
            }
        }

        public static Color GreyColor(int G) {
            return Color.FromArgb(G, G, G);
        }

        public static void CenterString(Graphics G, string T, Font F, Color C, Rectangle R) {
            SizeF TS = G.MeasureString(T, F);

            using (SolidBrush B = new SolidBrush(C)) {
                G.DrawString(T, F, B, new Point((int)(R.Width / 2 - (TS.Width / 2)), (int)(R.Height / 2 - (TS.Height / 2))));
            }
        }


        public static void FillRoundRect(Graphics G, Rectangle R, int Curve, Color C) {
            using (SolidBrush B = new SolidBrush(C)) {
                G.FillPie(B, R.X, R.Y, Curve, Curve, 180, 90);
                G.FillPie(B, R.X + R.Width - Curve, R.Y, Curve, Curve, 270, 90);
                G.FillPie(B, R.X, R.Y + R.Height - Curve, Curve, Curve, 90, 90);
                G.FillPie(B, R.X + R.Width - Curve, R.Y + R.Height - Curve, Curve, Curve, 0, 90);
                G.FillRectangle(B, Convert.ToInt32(R.X + Curve / 2), R.Y, R.Width - Curve, Convert.ToInt32(Curve / 2));
                G.FillRectangle(B, R.X, Convert.ToInt32(R.Y + Curve / 2), R.Width, R.Height - Curve);
                G.FillRectangle(B, Convert.ToInt32(R.X + Curve / 2), Convert.ToInt32(R.Y + R.Height - Curve / 2), R.Width - Curve, Convert.ToInt32(Curve / 2));
            }

        }


        public static void DrawRoundRect(Graphics G, Rectangle R, int Curve, Color C) {
            using (Pen P = new Pen(C)) {
                G.DrawArc(P, R.X, R.Y, Curve, Curve, 180, 90);
                G.DrawLine(P, Convert.ToInt32(R.X + Curve / 2), R.Y, Convert.ToInt32(R.X + R.Width - Curve / 2), R.Y);
                G.DrawArc(P, R.X + R.Width - Curve, R.Y, Curve, Curve, 270, 90);
                G.DrawLine(P, R.X, Convert.ToInt32(R.Y + Curve / 2), R.X, Convert.ToInt32(R.Y + R.Height - Curve / 2));
                G.DrawLine(P, Convert.ToInt32(R.X + R.Width), Convert.ToInt32(R.Y + Curve / 2), Convert.ToInt32(R.X + R.Width), Convert.ToInt32(R.Y + R.Height - Curve / 2));
                G.DrawLine(P, Convert.ToInt32(R.X + Curve / 2), Convert.ToInt32(R.Y + R.Height), Convert.ToInt32(R.X + R.Width - Curve / 2), Convert.ToInt32(R.Y + R.Height));
                G.DrawArc(P, R.X, R.Y + R.Height - Curve, Curve, Curve, 90, 90);
                G.DrawArc(P, R.X + R.Width - Curve, R.Y + R.Height - Curve, Curve, Curve, 0, 90);
            }

        }


        public static void CenterStringTab(Graphics G, string text, Font font, Brush brush, Rectangle rect, bool shadow = false, int yOffset = 0) {
            SizeF textSize = G.MeasureString(text, font);
            int textX = (int)(rect.X + (rect.Width / 2) - (textSize.Width / 2));
            int textY = (int)(rect.Y + (rect.Height / 2) - (textSize.Height / 2) + yOffset);

            if (shadow)
                G.DrawString(text, font, Brushes.Black, textX + 1, textY + 1);
            G.DrawString(text, font, brush, textX, textY + 1);

        }

    }


    [DefaultEvent("CheckedChanged")]
    class FirefoxRadioButton : Control
    {

        #region " Public "
        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender, EventArgs e);
        #endregion

        #region " Private "
        private Helpers.MouseState State;
        private Color ETC = Color.Blue;

        private Graphics G;
        private bool _EnabledCalc;
        private bool _Checked;
        #endregion
        private bool _Bold;

        #region " Properties "

        public bool Checked {
            get {
                return _Checked;
            }
            set {
                _Checked = value;
                Invalidate();
            }
        }

        public new bool Enabled {
            get {
                return EnabledCalc;
            }
            set {
                _EnabledCalc = value;
                Invalidate();
            }
        }

        [DisplayName("Enabled")]
        public bool EnabledCalc {
            get {
                return _EnabledCalc;
            }
            set {
                Enabled = value;
                Invalidate();
            }
        }

        public bool Bold {
            get {
                return _Bold;
            }
            set {
                _Bold = value;
                Invalidate();
            }
        }

        #endregion

        #region " Control "

        public FirefoxRadioButton() {
            DoubleBuffered = true;
            ForeColor = Color.White;
            Font = Theme.GlobalFont(FontStyle.Regular, 10);
            Size = new Size(160, 27);
            Enabled = true;
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            G.Clear(Parent.BackColor);

            if (Enabled) {
                ETC = Color.FromArgb(66, 78, 90);

                switch (State) {

                    case Helpers.MouseState.Over:
                    case Helpers.MouseState.Down:

                        using (Pen P = new Pen(Color.FromArgb(34, 146, 208))) {
                            G.DrawEllipse(P, new Rectangle(2, 2, 22, 22));
                        }


                        break;
                    default:

                        using (Pen P = new Pen(Helpers.GreyColor(190))) {
                            G.DrawEllipse(P, new Rectangle(2, 2, 22, 22));
                        }


                        break;
                }


                if (Checked) {
                    using (SolidBrush B = new SolidBrush(Color.FromArgb(34, 146, 208))) {
                        G.FillEllipse(B, new Rectangle(7, 7, 12, 12));
                    }

                }

            }
            else {
                ETC = Helpers.GreyColor(170);

                using (Pen P = new Pen(Helpers.GreyColor(210))) {
                    G.DrawEllipse(P, new Rectangle(2, 2, 22, 22));
                }


                if (Checked) {
                    using (SolidBrush B = new SolidBrush(Color.FromArgb(34, 146, 208))) {
                        G.FillEllipse(B, new Rectangle(7, 7, 12, 12));
                    }

                }

            }

            using (SolidBrush B = new SolidBrush(ETC)) {

                if (Bold) {
                    G.DrawString(Text, Theme.GlobalFont(FontStyle.Bold, 10), B, new Point(32, 4));
                }
                else {
                    G.DrawString(Text, Theme.GlobalFont(FontStyle.Regular, 10), B, new Point(32, 4));
                }

            }


        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            State = Helpers.MouseState.Down;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);


            if (Enabled) {

                if (!Checked) {
                    foreach (Control C in Parent.Controls) {
                        if (C is FirefoxRadioButton) {
                            ((FirefoxRadioButton)C).Checked = false;
                        }
                    }

                }

                Checked = true;
                if (CheckedChanged != null) {
                    CheckedChanged(this, e);
                }
            }

            State = Helpers.MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            State = Helpers.MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            State = Helpers.MouseState.None;
            Invalidate();
        }

        #endregion

    }

    [DefaultEvent("CheckedChanged")]
    class FirefoxCheckBox : Control
    {

        #region " Public "
        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender, EventArgs e);
        #endregion

        #region " Private "
        private Helpers.MouseState State;
        private Color ETC = Color.Blue;

        private Graphics G;
        private bool _EnabledCalc;
        private bool _Checked;
        #endregion
        private bool _Bold;

        #region " Properties "

        public bool Checked {
            get {
                return _Checked;
            }
            set {
                _Checked = value;
                Invalidate();
            }
        }

        public new bool Enabled {
            get {
                return EnabledCalc;
            }
            set {
                _EnabledCalc = value;
                Invalidate();
            }
        }

        [DisplayName("Enabled")]
        public bool EnabledCalc {
            get {
                return _EnabledCalc;
            }
            set {
                Enabled = value;
                Invalidate();
            }
        }

        public bool Bold {
            get {
                return _Bold;
            }
            set {
                _Bold = value;
                Invalidate();
            }
        }

        #endregion

        #region " Control "

        public FirefoxCheckBox() {
            DoubleBuffered = true;
            ForeColor = Color.White;
            Font = Theme.GlobalFont(FontStyle.Regular, 10);
            Size = new Size(160, 27);
            Enabled = true;
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            G.Clear(Parent.BackColor);

            if (Enabled) {
                ETC = Color.White;

                switch (State) {

                    case Helpers.MouseState.Over:
                    case Helpers.MouseState.Down:
                        Helpers.DrawRoundRect(G, new Rectangle(3, 3, 20, 20), 3, Color.FromArgb(44, 156, 218));

                        break;
                    default:
                        Helpers.DrawRoundRect(G, new Rectangle(3, 3, 20, 20), 3, Helpers.GreyColor(200));

                        break;
                }


                if (Checked) {
                    using (Image I = Image.FromStream(new System.IO.MemoryStream(Convert.FromBase64String(Theme.GetCheckMark())))) {
                        G.DrawImage(I, new Point(4, 5));
                    }

                }


            }
            else {
                ETC = Helpers.GreyColor(170);
                Helpers.DrawRoundRect(G, new Rectangle(3, 3, 20, 20), 3, Helpers.GreyColor(220));


                if (Checked) {
                    using (Image I = Image.FromStream(new System.IO.MemoryStream(Convert.FromBase64String(Theme.GetCheckMark())))) {
                        G.DrawImage(I, new Point(4, 5));
                    }

                }

            }


            using (SolidBrush B = new SolidBrush(ETC)) {

                if (Bold) {
                    G.DrawString(Text, Theme.GlobalFont(FontStyle.Bold, 10), B, new Point(32, 4));
                }
                else {
                    G.DrawString(Text, Theme.GlobalFont(FontStyle.Regular, 10), B, new Point(32, 4));
                }

            }


        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            State = Helpers.MouseState.Down;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if (Enabled) {
                Checked = !Checked;
                if (CheckedChanged != null) {
                    CheckedChanged(this, e);
                }
            }

            State = Helpers.MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            State = Helpers.MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            State = Helpers.MouseState.None;
            Invalidate();
        }

        #endregion

    }

    class FirefoxH1 : Label
    {

        #region " Private "
        #endregion
        private Graphics G;

        #region " Control "

        public FirefoxH1() {
            DoubleBuffered = true;
            AutoSize = false;
            Font = new Font("Segoe UI Semibold", 20);
            ForeColor = Color.FromArgb(76, 88, 100);
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            using (Pen P = new Pen(Helpers.GreyColor(200))) {
                G.DrawLine(P, new Point(0, 50), new Point(Width, 50));
            }

        }

        #endregion

    }

    class FirefoxH2 : Label
    {

        #region " Control "

        public FirefoxH2() {
            Font = Theme.GlobalFont(FontStyle.Bold, 10);
            ForeColor = Color.FromArgb(76, 88, 100);
            BackColor = Color.White;
        }

        #endregion

    }

    class FirefoxButton : Control
    {

        #region " Private "
        private Helpers.MouseState State;
        private Color ETC = Color.Blue;

        private Graphics G;
        #endregion
        private bool _EnabledCalc;

        #region " Properties "

        public new bool Enabled {
            get {
                return EnabledCalc;
            }
            set {
                _EnabledCalc = value;
                Invalidate();
            }
        }

        [DisplayName("Enabled")]
        public bool EnabledCalc {
            get {
                return _EnabledCalc;
            }
            set {
                Enabled = value;
                Invalidate();
            }
        }

        #endregion

        #region " Control "

        public FirefoxButton() {
            DoubleBuffered = true;
            Enabled = true;
            ForeColor = Color.FromArgb(56, 68, 80);
            Font = Theme.GlobalFont(FontStyle.Regular, 10);
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            G.Clear(Parent.BackColor);

            if (Enabled) {
                ETC = Color.FromArgb(56, 68, 80);

                switch (State) {

                    case Helpers.MouseState.None:

                        using (SolidBrush B = new SolidBrush(Helpers.GreyColor(245))) {
                            G.FillRectangle(B, new Rectangle(1, 1, Width - 2, Height - 2));
                        }


                        Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(193));

                        break;
                    case Helpers.MouseState.Over:

                        using (SolidBrush B = new SolidBrush(Helpers.GreyColor(232))) {
                            G.FillRectangle(B, new Rectangle(1, 1, Width - 2, Height - 2));
                        }


                        Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(193));

                        break;
                    default:

                        using (SolidBrush B = new SolidBrush(Helpers.GreyColor(212))) {
                            G.FillRectangle(B, new Rectangle(1, 1, Width - 2, Height - 2));
                        }


                        Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(193));

                        break;
                }

            }
            else {
                ETC = Helpers.GreyColor(170);

                using (SolidBrush B = new SolidBrush(Helpers.GreyColor(245))) {
                    G.FillRectangle(B, new Rectangle(1, 1, Width - 2, Height - 2));
                }

                Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(223));

            }

            Helpers.CenterString(G, Text, Theme.GlobalFont(FontStyle.Regular, 10), ETC, Helpers.FullRectangle(Size, false));

        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            State = Helpers.MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseUp(e);
            State = Helpers.MouseState.Down;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            State = Helpers.MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseEnter(e);
            State = Helpers.MouseState.None;
            Invalidate();
        }

        #endregion

    }

    class FirefoxRedirect : Control
    {

        #region " Private "
        private Helpers.MouseState State;

        private Graphics G;
        private Color FC = Color.Blue;
        #endregion
        private Font FF = null;

        #region " Control "

        public FirefoxRedirect() {
            DoubleBuffered = true;
            Cursor = Cursors.Hand;
            BackColor = Color.White;
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            switch (State) {

                case Helpers.MouseState.Over:
                    FC = Color.FromArgb(23, 140, 229);
                    FF = Theme.GlobalFont(FontStyle.Underline, 10);

                    break;
                case Helpers.MouseState.Down:
                    FC = Color.FromArgb(255, 149, 0);
                    FF = Theme.GlobalFont(FontStyle.Regular, 10);

                    break;
                default:
                    FC = Color.FromArgb(0, 149, 221);
                    FF = Theme.GlobalFont(FontStyle.Regular, 10);

                    break;
            }

            using (SolidBrush B = new SolidBrush(FC)) {
                G.DrawString(Text, FF, B, new Point(0, 0));
            }

        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            State = Helpers.MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseUp(e);
            State = Helpers.MouseState.Down;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            State = Helpers.MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseEnter(e);
            State = Helpers.MouseState.None;
            Invalidate();
        }

        #endregion

    }

    class FirefoxSubTabControl : TabControl
    {

        #region " Private "
        private Graphics G;
        #endregion
        private Rectangle TabRect;

        #region " Control "

        public FirefoxSubTabControl() {
            DoubleBuffered = true;
            Alignment = TabAlignment.Top;
        }

        protected override void OnCreateControl() {
            base.OnCreateControl();
            SetStyle(ControlStyles.UserPaint, true);
            ItemSize = new Size(100, 40);
            SizeMode = TabSizeMode.Fixed;
        }

        protected override void OnControlAdded(ControlEventArgs e) {
            base.OnControlAdded(e);
            try {
                for (int i = 0; i <= TabPages.Count - 1; i++) {
                    TabPages[i].BackColor = Color.FromArgb(40, 40, 40);
                    TabPages[i].ForeColor = Color.FromArgb(153, 153, 153);
                    TabPages[i].Font = Theme.GlobalFont(FontStyle.Regular, 10);
                }
            }
            catch {
            }
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            G.Clear(Parent.BackColor);


            for (int i = 0; i <= TabPages.Count - 1; i++) {
                TabRect = GetTabRect(i);


                if (GetTabRect(i).Contains(this.PointToClient(Cursor.Position)) & !(SelectedIndex == i)) {
                    using (SolidBrush B = new SolidBrush(Helpers.GreyColor(240))) {
                        G.FillRectangle(B, new Rectangle(GetTabRect(i).Location.X - 2, GetTabRect(i).Location.Y - 2, GetTabRect(i).Width, GetTabRect(i).Height + 1));
                    }


                }
                else if (SelectedIndex == i) {
                    using (SolidBrush B = new SolidBrush(Helpers.GreyColor(240))) {
                        G.FillRectangle(B, new Rectangle(GetTabRect(i).Location.X - 2, GetTabRect(i).Location.Y - 2, GetTabRect(i).Width, GetTabRect(i).Height + 1));
                    }

                    using (Pen P = new Pen(Color.FromArgb(255, 149, 0), 4)) {
                        G.DrawLine(P, new Point(TabRect.X - 2, TabRect.Y + ItemSize.Height - 2), new Point(TabRect.X + TabRect.Width - 2, TabRect.Y + ItemSize.Height - 2));
                    }

                }
                else if (!(SelectedIndex == i)) {
                    G.FillRectangle(Brushes.White, GetTabRect(i));
                }

                using (SolidBrush B = new SolidBrush(Color.FromArgb(56, 69, 80))) {
                    Helpers.CenterStringTab(G, TabPages[i].Text, Theme.GlobalFont(FontStyle.Regular, 10), B, GetTabRect(i));
                }

            }

            using (Pen P = new Pen(Helpers.GreyColor(200))) {
                G.DrawLine(P, new Point(0, ItemSize.Height + 2), new Point(Width, ItemSize.Height + 2));
            }

        }

        #endregion

    }

    class FirefoxMainTabControl : TabControl
    {

        #region " Private "
        private Graphics G;
        private Rectangle TabRect;
        #endregion
        private Color FC = Color.Blue;

        #region " Control "
        private Image webPng;
        private Image settingsPng;
        private Image dashboardPng;
        public FirefoxMainTabControl() {
            DoubleBuffered = true;
            ItemSize = new Size(43, 152);
            Alignment = TabAlignment.Left;
            SizeMode = TabSizeMode.Fixed;
            string web64 = "iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAYAAADDPmHLAAAACXBIWXMAAAsTAAALEwEAmpwYAAAKTWlDQ1BQaG90b3Nob3AgSUNDIHByb2ZpbGUAAHjanVN3WJP3Fj7f92UPVkLY8LGXbIEAIiOsCMgQWaIQkgBhhBASQMWFiApWFBURnEhVxILVCkidiOKgKLhnQYqIWotVXDjuH9yntX167+3t+9f7vOec5/zOec8PgBESJpHmomoAOVKFPDrYH49PSMTJvYACFUjgBCAQ5svCZwXFAADwA3l4fnSwP/wBr28AAgBw1S4kEsfh/4O6UCZXACCRAOAiEucLAZBSAMguVMgUAMgYALBTs2QKAJQAAGx5fEIiAKoNAOz0ST4FANipk9wXANiiHKkIAI0BAJkoRyQCQLsAYFWBUiwCwMIAoKxAIi4EwK4BgFm2MkcCgL0FAHaOWJAPQGAAgJlCLMwAIDgCAEMeE80DIEwDoDDSv+CpX3CFuEgBAMDLlc2XS9IzFLiV0Bp38vDg4iHiwmyxQmEXKRBmCeQinJebIxNI5wNMzgwAABr50cH+OD+Q5+bk4eZm52zv9MWi/mvwbyI+IfHf/ryMAgQAEE7P79pf5eXWA3DHAbB1v2upWwDaVgBo3/ldM9sJoFoK0Hr5i3k4/EAenqFQyDwdHAoLC+0lYqG9MOOLPv8z4W/gi372/EAe/tt68ABxmkCZrcCjg/1xYW52rlKO58sEQjFu9+cj/seFf/2OKdHiNLFcLBWK8ViJuFAiTcd5uVKRRCHJleIS6X8y8R+W/QmTdw0ArIZPwE62B7XLbMB+7gECiw5Y0nYAQH7zLYwaC5EAEGc0Mnn3AACTv/mPQCsBAM2XpOMAALzoGFyolBdMxggAAESggSqwQQcMwRSswA6cwR28wBcCYQZEQAwkwDwQQgbkgBwKoRiWQRlUwDrYBLWwAxqgEZrhELTBMTgN5+ASXIHrcBcGYBiewhi8hgkEQcgIE2EhOogRYo7YIs4IF5mOBCJhSDSSgKQg6YgUUSLFyHKkAqlCapFdSCPyLXIUOY1cQPqQ28ggMor8irxHMZSBslED1AJ1QLmoHxqKxqBz0XQ0D12AlqJr0Rq0Hj2AtqKn0UvodXQAfYqOY4DRMQ5mjNlhXIyHRWCJWBomxxZj5Vg1Vo81Yx1YN3YVG8CeYe8IJAKLgBPsCF6EEMJsgpCQR1hMWEOoJewjtBK6CFcJg4Qxwicik6hPtCV6EvnEeGI6sZBYRqwm7iEeIZ4lXicOE1+TSCQOyZLkTgohJZAySQtJa0jbSC2kU6Q+0hBpnEwm65Btyd7kCLKArCCXkbeQD5BPkvvJw+S3FDrFiOJMCaIkUqSUEko1ZT/lBKWfMkKZoKpRzame1AiqiDqfWkltoHZQL1OHqRM0dZolzZsWQ8ukLaPV0JppZ2n3aC/pdLoJ3YMeRZfQl9Jr6Afp5+mD9HcMDYYNg8dIYigZaxl7GacYtxkvmUymBdOXmchUMNcyG5lnmA+Yb1VYKvYqfBWRyhKVOpVWlX6V56pUVXNVP9V5qgtUq1UPq15WfaZGVbNQ46kJ1Bar1akdVbupNq7OUndSj1DPUV+jvl/9gvpjDbKGhUaghkijVGO3xhmNIRbGMmXxWELWclYD6yxrmE1iW7L57Ex2Bfsbdi97TFNDc6pmrGaRZp3mcc0BDsax4PA52ZxKziHODc57LQMtPy2x1mqtZq1+rTfaetq+2mLtcu0W7eva73VwnUCdLJ31Om0693UJuja6UbqFutt1z+o+02PreekJ9cr1Dund0Uf1bfSj9Rfq79bv0R83MDQINpAZbDE4Y/DMkGPoa5hpuNHwhOGoEctoupHEaKPRSaMnuCbuh2fjNXgXPmasbxxirDTeZdxrPGFiaTLbpMSkxeS+Kc2Ua5pmutG003TMzMgs3KzYrMnsjjnVnGueYb7ZvNv8jYWlRZzFSos2i8eW2pZ8ywWWTZb3rJhWPlZ5VvVW16xJ1lzrLOtt1ldsUBtXmwybOpvLtqitm63Edptt3xTiFI8p0in1U27aMez87ArsmuwG7Tn2YfYl9m32zx3MHBId1jt0O3xydHXMdmxwvOuk4TTDqcSpw+lXZxtnoXOd8zUXpkuQyxKXdpcXU22niqdun3rLleUa7rrStdP1o5u7m9yt2W3U3cw9xX2r+00umxvJXcM970H08PdY4nHM452nm6fC85DnL152Xlle+70eT7OcJp7WMG3I28Rb4L3Le2A6Pj1l+s7pAz7GPgKfep+Hvqa+It89viN+1n6Zfgf8nvs7+sv9j/i/4XnyFvFOBWABwQHlAb2BGoGzA2sDHwSZBKUHNQWNBbsGLww+FUIMCQ1ZH3KTb8AX8hv5YzPcZyya0RXKCJ0VWhv6MMwmTB7WEY6GzwjfEH5vpvlM6cy2CIjgR2yIuB9pGZkX+X0UKSoyqi7qUbRTdHF09yzWrORZ+2e9jvGPqYy5O9tqtnJ2Z6xqbFJsY+ybuIC4qriBeIf4RfGXEnQTJAntieTE2MQ9ieNzAudsmjOc5JpUlnRjruXcorkX5unOy553PFk1WZB8OIWYEpeyP+WDIEJQLxhP5aduTR0T8oSbhU9FvqKNolGxt7hKPJLmnVaV9jjdO31D+miGT0Z1xjMJT1IreZEZkrkj801WRNberM/ZcdktOZSclJyjUg1plrQr1zC3KLdPZisrkw3keeZtyhuTh8r35CP5c/PbFWyFTNGjtFKuUA4WTC+oK3hbGFt4uEi9SFrUM99m/ur5IwuCFny9kLBQuLCz2Lh4WfHgIr9FuxYji1MXdy4xXVK6ZHhp8NJ9y2jLspb9UOJYUlXyannc8o5Sg9KlpUMrglc0lamUycturvRauWMVYZVkVe9ql9VbVn8qF5VfrHCsqK74sEa45uJXTl/VfPV5bdra3kq3yu3rSOuk626s91m/r0q9akHV0IbwDa0b8Y3lG19tSt50oXpq9Y7NtM3KzQM1YTXtW8y2rNvyoTaj9nqdf13LVv2tq7e+2Sba1r/dd3vzDoMdFTve75TsvLUreFdrvUV99W7S7oLdjxpiG7q/5n7duEd3T8Wej3ulewf2Re/ranRvbNyvv7+yCW1SNo0eSDpw5ZuAb9qb7Zp3tXBaKg7CQeXBJ9+mfHvjUOihzsPcw83fmX+39QjrSHkr0jq/dawto22gPaG97+iMo50dXh1Hvrf/fu8x42N1xzWPV56gnSg98fnkgpPjp2Snnp1OPz3Umdx590z8mWtdUV29Z0PPnj8XdO5Mt1/3yfPe549d8Lxw9CL3Ytslt0utPa49R35w/eFIr1tv62X3y+1XPK509E3rO9Hv03/6asDVc9f41y5dn3m978bsG7duJt0cuCW69fh29u0XdwruTNxdeo94r/y+2v3qB/oP6n+0/rFlwG3g+GDAYM/DWQ/vDgmHnv6U/9OH4dJHzEfVI0YjjY+dHx8bDRq98mTOk+GnsqcTz8p+Vv9563Or59/94vtLz1j82PAL+YvPv655qfNy76uprzrHI8cfvM55PfGm/K3O233vuO+638e9H5ko/ED+UPPR+mPHp9BP9z7nfP78L/eE8/sl0p8zAAAAIGNIUk0AAHolAACAgwAA+f8AAIDpAAB1MAAA6mAAADqYAAAXb5JfxUYAABMpSURBVHja7J15lFXFnccfImBkFxixoRMQXCPqEFdEwS2KSMy4JYyijlHGhURRGRNFFFwCAlFEZnTMJGaC2YxGMXEbg4CKIIYoMC5o0FYIzY6szdJ85o/7e2deV9e9VXXvrdvvPd/vnDoH+t3av1X1q99WOSBXJqkFUA0MAC4H7gIeB14A3gAWAR8DnwE1kj6Tvy2Wb16QPOOAq4DTgZ5AqzIapwaplBvfBTgVuBV4GngP2Ez6tBVYCswARgNnAPtXANA06ShgJPA8sJqmo7XAi8DNwD8CzSoA8JcOA34EzAfqKT7aA7wN3AH0qQAgndQGuFhW+g5Kh3YBLwGXAu0qAHBPVcAPgY8offpEdoXqCgDsJv6+Jj7XfdE6YFKxAqGpG9BeVsmqjM9s3b+zYBzvATpVABCkS4EPUx7ketl6d4b8Ng6oLfhbLXB7yPe7pKzdHo6Gq4C9vqwAOEKYuzRoJ7AQmAZcARwOTJDJK6TtwIVyXVurrMpmwLnAJg1gHpIyh8m/F6TIlL4K9P0yAWAvYJRmoF1pPfAH4Bq5Ihbewa8J+f50+b2jnMmF5/N+8lv/EB7kFqUfhwJXA08qYIpD2+QIbFHuADgYeDnhuf0acB3QPaSOfwmZ/P4F3+wXAYAccHwICK6PYF7/FZiZ8KiYJTtNWQJgaIKVshF4BDjGUMcZmq1ZnXwbAISBoB4YYmjD0cBUpXzXnW1YOQGgOTA+5mDUAvcCPSwlhWs0W+upmm9tAJADTgK2aMB4tEV7qoGxwIqYfZ+c5ZHgq+COojxxpS1yZ+5qWU9b4B1NOUNDvrcFQA64QCN6/iDiezX9g8g2vogxDi8AnUsVAD2FW3alX8sNwaWu6Zpybo743gUAOWCEpvynHZU/hwC/iDEe7wAHlRoAjpR7rgstBc6LUdcPNGU9asjjCoCcXP9U+mGM9g4CljiOzWe+r4ppFnYs8HfHDk5z2FJVoG1VypoHfMUDAFoCc5S6dgiz6NrudsAURwnkKqBfsQPgWEc5/krg2zHr2gd4SylvDdDbIm8cAOSAr0mbC2mxaC3j9OEc4HPHG0K/YgVAH83gRNFrCc+2sZoyL7LMGxcAOeBbmnonJehHD+AVh3Fb7eM4SFpAD8cz/xFZwUl4jO1KmY875E8CgJzwGKoo+rgE/WkBPOAwfsvTZgyTZO4A/MWh8XemIEp+VSlzmeMEJgVAO2FaC2l+Cvf2WxzGcVGaGsUkQp4/WjZ4g2i/kjb2ck3ZQxzLSAqAHHCmph3XpiQxXWM5pi+nJSyKm3GCA2IXimAoqd1AjVLuUzHKSQMAOvnD31NYle2ANx3G9cGmAsB3YurAz03Q0HFKeZuBXk0IgO4amf/9Cfo3CPhbjHG9LGsA9JItPS5NEvGtS53dNOLUMTE7nBYAcsBNGjF2D8cyWou+JK618yZRT2cCgOai9rSh8SFiWkQa5rIbTFTyf058i9s0AbAvjY1XpzrkPwt4N2SMfi9Msw29kYQfcPn4VssG/aYgz0jNta3w+tbTYqvdqOQbkQDxaQJAZ4Ow2aJP1cBjIWOyE7it4Nv/tBzzMb4BcLhG9KqjtzVb/AkhGrv8BIyN0HypzOaHFuLeLAHQQrOKp0RoSO+I4PSXAAOVPK3EWMREdQReU14AsJfl1r86gjFrTWARWxeh9LhRmYxOGvHy8IRMT9oAyAH/rBHbdlXkJSNEZhG26idGHGtVmhuQjl4H9vYBgGGW29DFljqDOQbt113AAcANmt/aFiEA9tEIh24VEIwGPo3o7zwxPjHVcbblHFydNgDaGToQRxzbXARDHxl2E/W2cVMK914fAMjJzqRaD9UarsXXiabRto6plqLi/dIEwF0WlS6LOYjtRa9ea3nGDY6zxWUAgOYEVsc2PNIa4e7j1NmGwAXeRD9OCwD7Y2fgeE7CATxAuFjTObcb+F9ZCYNiSt7SAkBH4JuiyFlMYz8E3cq8m3CLZts00EJmsNGlnqgf77eY/GdS1Ex1FGbJ1mpmlXDIE0UdfIiFfr61BgCtLfIcRGAjOAH4s4P6+wNhbtO073vCot6HkgKgq4XEbzt+bNn/LaZUbIcwinMJ7AvzYV7Ok5XTl8C8fL3CsX+DIMjDAPn2Sjn6pouQpSZClmGisR7G50DMkVA2Y+mMGvbDbRade8BD55oJZ1xIjxEYVcY1s1Z5CdU5tC6FcmtlZU5T/v5uCnyLLt1j0aZxcQHQxoLzX0tg9uzDb3CXIuvuVHBEDAb+Xc7d7TQd1clR9ajsGp0LxMNrFYAd62GcOlgsiBXCaDsD4LsWA3C/h07pDCNmRHDeveRcvl94geXovXyT0i5R984mcNq4SHiCsJX9GyX/nZ7G6g6Ltl8RBwAmH744Wi/b9EoCwUZb4UkGE/jyTQB+SRDMaZ7sGp9ojoBP5Ld5BOFdpguoRojS6uuOyqehGmWNj7HqqvAzOprtCoDDLFbRzzx16ABF7VuHnaWvy329SsMEVslvadXTjYZuZdssFERx04MWV+cjXQAw2mI7PNpTZ/5Jqest0g+/1l5zDWzvoS+zlb4M9TRmB1nwQvfYAqA5ZiPP1z11JEdj69iJHurwJQpWk6rL/w+P4/aiYc6WRNkLqO7NJh/3EZ460YzGzh5DShgApyt9eSflY6YwXWaYsz1EmK4X/meUhXChu0eG5gvl+tethAHQSbkObvfIOHexENnfYQMAE/f/rMdtbKBS10L8BFHKCgA5OS4LabDH8TOJh2ebANDF4kpxhccOqAaW/+2pniwBMM12FaaQLjLM3Sa57YQC4EwLuf+BHjvwuFLfjWUAgO8pffq9x/Grwhx8a3AUAMYYMr+N34jYc5X6ziwDAPTL6FjLJ5Pt4N1RADDF7ZvqseFtRIxbqNXrXQYAqFK0duvwG/blXsMcvhQGgFYEr2ZE0Xc9NvxgRfr4qShVSh0AeysWPHs8CtFyssVH0SdoLKrz+uU6gzjxCI8NP0Wp702PdWUJAN3N6myPdfU0SAW1O2tOTJui6HMLq5kk6ZIMmaWsAfBzpW/DPdbVksbWySoN0gFguCHTHI+NzhEYhhbS5DICwFgbRixDsfD1OgCYmIefeW70T5T6RpURAK5V+vao57E0mY5P1AFguiHTaM+N/mWGAqesAXCh0renPY/lSMNcPqkDwEuGTFdmvG2dU0YAGJDxcXqxYS5n6QAwz5DpbM+NVmXm/csIAMcoffuL57EcYJjLhToAmLxNjvPcaDWsrM+7clsNAHy+7nUoDY1cl3geyz5EB6H8WAeAZQZd8gBZJT5SFwWAu0WE6qu+XhqTsO5yzfWRjhCTsEJhTJXH/h1H9Ismy3UA+NwAgA2yUnyl3Up9Gz3WtV5ZIfXS/xoP6VMZ8HqlvvUe+7fBsAOs0gHAJcpnhUqb1lUAUAFA0R8BPutTj4A9GW/JRXkERDGB9aKs8ckEvt/ETGAvj/X1VRQ0RckEvt/E18D5TXwNbOuxvkOUa+DiJr4G/k0HgPkGAJzludGqE8XJZSwIesvzWJ5smMu/6gBgsga+3HOjn89Q8pg1AE4xiWJTThcY5nK2DgAmk+LbMlYGDSsjAKjubn/wPJY/MMzlU3HUwb5VmJO+ROrgRzyP5WTDXE7WAeBqQ6Y/e260GhJmUhkBQDUIGet5LJ8zzOX3dQA4w5BpGcmeeXH1bft1GQHgp0rfrvVYV3OCKGpRdI4OADbGhAdnqMIsJ6NQNcTuII91VdP4uVvjPOaNCU1m4ed7bHgvGlol+zRCzRIALWkYDXU3QbQRX+N4lmEOa9CY2+f/8SdD5gkeG96GhgGP6vDnhpa1Y0ihu9ZaUnzsCfeYQa8Q4Rl0pyHzXI8Nb0bjt3K+WQYA6K/0yWeMAF18JZXuiwKAafvYTIh3qSdmaVQZAEC9Av7K4/h1whwj4FtRANgf8zPnPt3D1Fe6p5cBANRHJm/1OH5DDHO3lZDIoVEcq0pPZCgyXexpu8wKALqQNz5vAKanZUJD1bmEh63FT0QtXYCKOuI9C1csAOhGQ8/gLVjG7o2R9sUcaX2cDQCOwfys+cUeUaxqBb9TwgA4V+nLAvzFVzB5BUPEqySqO/MiQ0HPeASA+kDU1BIGwHilLw97HDeTMu9DIl4mUf8wzlDYNk9bs27VLCH9SNtZAKAZjX0dfO2c3SyY98i4zuofjsIcK/AeT53prEyOMcxpkQKgt2KW5fMKbXrL0Rit3OYsVmmFR2ZQlUiOLEEAqNrVmZ7GqrXBnhMCt79mrgC43IKp8BXF6/ueBy8LAKgg/pGnsbrKYp6uMZUTFlDZ9BjBMk/GlAcr22fa4ek6aADQIcXyuyvXPx/HWE7U8x8a5mi1je4h7Ie7LdDlS1w7x2TEkCB1pLFZeEeP2/8C/ISGu85ifqwe9YjSLZseJlqFn2djbrCVYimplazA40V9fQtBBPJfEIS5fZXAKna3skL/Kr/NIIhQ+qCA+wKCd4+rHQxiZmZgT9mBaGcep9taknAjvu63PWnoUVtP8LKX+l0PsXAZLcaWSy1AG4c2i17/GYKAmueGHEt9aOgDsJPgAQ7fMgYdPWZbXtSPX7O4Y+4UCWLanXxWIxRqSxCG/T4CX4atNB1tE1n/BDGpa0cQf6eQXvYwLqq7uY624BBo01U6p6P5uL2Ba2vdUq9os2pSmrw9If9OQp8p5lh7CF4TS3NM9sLuKfkpLuXa6Jlt3usbk3JnT7DYfXS0kiCu8Qw5y0cCl8q2PQA4lSD+QJ42yN9OkW8ukSvuT2QXWiD93xNjhzgt5TG5yaLe1TR8uj4xAGzMxvNHwcCEHdxbJmEGdo85bhSh1QMElsV9La50X6HhQw5r0YRPVVI7kZBeKsCYhTm0fp7BfJHAOaRVwrE53mLrjyU4s52YNywqr3FFH/9vPHkZ5veK8nU8LBx6nBdF0hIEVcnEPkTg8WuiRbKQ9onZ5qUWdSyMAzTbD/tarso/OVTeXLbchQ5ba1KTcR+SwFkO7V9EEHbPhWd60nK3OTFO+10+vtuyk3dalHWaxa7ykmxp6tZ3SREB4HylbTvkrH6O6GfeF2DnBDvKcsxjv7Dm8nErzDEF8zQ84o7/c8Od+1E58/J5/kv55n3ieyqlCYC9NbvX7wp+/wbBszFRzOwTBG//6cofZsl8LiRBeH3XDF+3FLbUE7xjozKTayImfhpBQAWdfmB7SsqoNAGgKmN2ETxDr1MPT4kAwgYCr17VNsLmHeRtJAyoESfTlZa7wHa5YlWLFC2MHicIqBhV50NKnrUxdexpAaAzwYPShWQKqt2bwHgzzN7ieZFunoj5/Z88JfY1jJvxYcsGbhSdgY5myd3cpr6umt3jp00IgClKW74AvmqZtx/wPyFjsgazfX+S/qcGgFaiQIlDq4VHcDWSHKk5Zk5rAgCcqNme4zwJd4WlkE1Hr1vIL7wCICeawPccGr1HNHNfjVlfS42s4H3cHEmTAqCVhvF7LwETViXHQr3DOH5EiiZmaQRDtg00WU+EfbplOlnDGU/OEAA6o9mkQbRGY7bDLDwi+qQpYk6jkOMdzi1E1JvEjmCqZmcZlAEABmq2/iSvqXQiiNljSxvxEEo/zSAPLsqbD+SGEDfWn2oOtcJSNBwXAJ1pbID5KfHdvU8iMHu3pS14ekwzzcIGOu4Eu0S62DIm4HYp5b2I2fwqLgCeSrDrqOLv24mO5qlb+b5eUs2lXeAJmvuxieYS4brkKCad7AEA42KKu9V0bIyb02r8Bs7M+Sj0cMfbQV6ZMQ04wLGu32rK+l6KABiqKf85xytsF1FZ73Ack6Wihs6VGgDygpuZuNMKAmNOW1Pt9jSOjFUXsT27AGAAjYMufSwTasur3EBgLeRKcwgMXHOlCoC87fq0mIKOZTJ4NkA4SqOf2CjHUVwAHCnbryraPsGiPe1ERLs0Zt8fw9/byZkCIJ+GE99at4bAF/EgQx0XauQDK2nslGEDgN7oXa4uM7ThQOAuzO5aUYqd67Oa+CwBkBON1ZvEp62iar2QcEeOG0OOlL4OADiMIKS6SrdFHEHfJghuuSlB/97G4MRZ6gDIHwn3xmCGVFouipDzNSLRMZrvVxYMbhQAjpC7vUrjNfzNeWK3kNRSeTdBaNx9m2LyswZA4VVxLunQBoLwaGMInESqRVOnilbXy126GQ2NQtfJ307SiLR3yyRXi/XO7QRWSutSavsC31e8YgVADmhBEBlsOenSZpGw7QyxT7hJmeiVBB60uu17p5S1KeU2rgJuxm/85aIHQGF4uknE8wGIS/Uh//ZNW8SwpVsxTHyxAKCQ857mYbUVA22Vq92hxTTxxQaAwsDRP05gKFFMVEtgrXtIMU58sQKgUPs2HHitBCd+rvA3+xfzxBc7AArTcQROqu8V8aR/IIqofqUw6aUGgMKbQ3/Rzr1O9OMIWZzrb4qU8hSS+/5VABAj9RAp3HiCd41qsLOld6WdotCZSRB25Xz8vWdQAUCC1FpEuUNo6OI9B3hXZPS1NH5vt5bAwXOR8BzPivp2JEGI9cMJHrXIlWPKlWvHKqkCgEqqAKCSKgCopMj0fwMAjoW5s0Mb8PMAAAAASUVORK5CYII=";
            webPng = Image.FromStream(new System.IO.MemoryStream(Convert.FromBase64String(web64)));
            string setting64 = "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";
            settingsPng = Image.FromStream(new System.IO.MemoryStream(Convert.FromBase64String(setting64)));
            string dashboard64 = "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";
            dashboardPng = Image.FromStream(new System.IO.MemoryStream(Convert.FromBase64String(dashboard64)));
        }

        protected override void OnCreateControl() {
            base.OnCreateControl();
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnControlAdded(ControlEventArgs e) {
            base.OnControlAdded(e);
            try {
                for (int i = 0; i <= TabPages.Count - 1; i++) {
                    TabPages[i].BackColor = Color.FromArgb(40, 40, 40);
                    TabPages[i].ForeColor = Color.FromArgb(153, 153, 153);
                    TabPages[i].Font = Theme.GlobalFont(FontStyle.Regular, 10);
                }
            }
            catch {
            }
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            G.Clear(Color.FromArgb(40, 40, 40));


            for (int i = 0; i <= TabPages.Count - 1; i++) {
                TabRect = GetTabRect(i);


                if (SelectedIndex == i) {
                    using (SolidBrush B = new SolidBrush(Color.FromArgb(52, 63, 72))) {
                        G.FillRectangle(B, TabRect);
                    }

                    FC = Helpers.GreyColor(245);

                    /// COLOR OR VERTICAL BAR

                    using (SolidBrush B = new SolidBrush(Color.FromArgb(209, 43, 43))) {
                        G.FillRectangle(B, new Rectangle(TabRect.Location.X - 3, TabRect.Location.Y + 1, 5, TabRect.Height - 2));
                    }

                }
                else {
                    FC = Helpers.GreyColor(192);

                    using (SolidBrush B = new SolidBrush(Color.FromArgb(40, 40, 40))) {
                        G.FillRectangle(B, TabRect);
                    }

                }

                using (SolidBrush B = new SolidBrush(FC)) {
                    G.DrawString(TabPages[i].Text, Theme.GlobalFont(FontStyle.Regular, 10), B, new Point(TabRect.X + 50, TabRect.Y + 12));
                }

                if (i == 0) {
                    //string dashBase64 = "data:image/png;base64,
                    //https://github.com/romeo007-mi/AstarothGenerator/blob/main/Resources/dashboard.png
                    if (dashboardPng != null)
                        G.DrawImage(dashboardPng, new Rectangle(TabRect.X + 19, TabRect.Y + ((TabRect.Height / 2) - 10), 22, 22));
                }
                else if (i == 1) {
                    //string setBase64 = "data:image/png;base64,
                    //https://github.com/romeo007-mi/AstarothGenerator/blob/main/Resources/settings.png
                    if (settingsPng != null)
                        G.DrawImage(settingsPng, new Rectangle(TabRect.X + 19, TabRect.Y + ((TabRect.Height / 2) - 10), 22, 22));
                }
                else if (i == 2) {
                    //string webBase64 = "data:image/png;base64,
                    //https://github.com/romeo007-mi/AstarothGenerator/blob/main/Resources/web.png
                    if (webPng != null)
                        G.DrawImage(webPng, new Rectangle(TabRect.X + 19, TabRect.Y + ((TabRect.Height / 2) - 10), 22, 22));
                }
            }

        }

        #endregion

        protected override void Dispose(bool disposing) {
            if (settingsPng != null) {
                settingsPng.Dispose();
                settingsPng = null;
            }
            if (webPng != null) {
                webPng.Dispose();
                webPng = null;
            }
            if (dashboardPng != null) {
                dashboardPng.Dispose();
                dashboardPng = null;
            }
            base.Dispose(disposing);
        }

    }

    [DefaultEvent("TextChanged")]
    class FirefoxTextbox : Control
    {

        #region " Private "
        private TextBox withEventsField_TB = new TextBox();
        private TextBox TB {
            get {
                return withEventsField_TB;
            }
            set {
                if (withEventsField_TB != null) {
                    withEventsField_TB.TextChanged -= new EventHandler(TextChangeTb);
                }
                withEventsField_TB = value;
                if (withEventsField_TB != null) {
                    withEventsField_TB.TextChanged += TextChangeTb;
                }
            }
        }
        private Graphics G;
        private Helpers.MouseState State;

        private bool IsDown;
        private bool _EnabledCalc;
        private bool _allowpassword = false;
        private int _maxChars = 32767;
        private HorizontalAlignment _textAlignment;
        private bool _multiLine = false;
        #endregion
        private bool _readOnly = false;

        #region " Properties "

        public new bool Enabled {
            get {
                return EnabledCalc;
            }
            set {
                TB.Enabled = value;
                _EnabledCalc = value;
                Invalidate();
            }
        }

        [DisplayName("Enabled")]
        public bool EnabledCalc {
            get {
                return _EnabledCalc;
            }
            set {
                Enabled = value;
                Invalidate();
            }
        }

        public new bool UseSystemPasswordChar {
            get {
                return _allowpassword;
            }
            set {
                TB.UseSystemPasswordChar = UseSystemPasswordChar;
                _allowpassword = value;
                Invalidate();
            }
        }

        public new int MaxLength {
            get {
                return _maxChars;
            }
            set {
                _maxChars = value;
                TB.MaxLength = MaxLength;
                Invalidate();
            }
        }

        public new HorizontalAlignment TextAlign {
            get {
                return _textAlignment;
            }
            set {
                _textAlignment = value;
                Invalidate();
            }
        }

        public new bool MultiLine {
            get {
                return _multiLine;
            }
            set {
                _multiLine = value;
                TB.Multiline = value;
                OnResize(EventArgs.Empty);
                Invalidate();
            }
        }

        public new bool ReadOnly {
            get {
                return _readOnly;
            }
            set {
                _readOnly = value;
                if (TB != null) {
                    TB.ReadOnly = value;
                }
            }
        }

        #endregion

        #region " Control "

        protected override void OnTextChanged(EventArgs e) {
            base.OnTextChanged(e);
            Invalidate();
        }

        protected override void OnBackColorChanged(EventArgs e) {
            base.OnBackColorChanged(e);
            Invalidate();
        }

        protected override void OnForeColorChanged(EventArgs e) {
            base.OnForeColorChanged(e);
            TB.ForeColor = ForeColor;
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            TB.Font = Font;
        }

        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            TB.Focus();
        }

        private void TextChangeTb(object sender, EventArgs e) {
            Text = TB.Text;
        }

        private void TextChng(object sender, EventArgs e) {
            TB.Text = Text;
        }

        public void NewTextBox() {
            var _with1 = TB;
            _with1.Text = string.Empty;
            _with1.BackColor = Color.White;
            _with1.ForeColor = Color.FromArgb(66, 78, 90);
            _with1.TextAlign = HorizontalAlignment.Left;
            _with1.BorderStyle = BorderStyle.None;
            _with1.Location = new Point(3, 3);
            _with1.Font = Theme.GlobalFont(FontStyle.Regular, 10);
            _with1.Size = new Size(Width - 3, Height - 3);
            _with1.UseSystemPasswordChar = UseSystemPasswordChar;
        }

        public FirefoxTextbox()
            : base() {
            TextChanged += new EventHandler(TextChng);
            NewTextBox();
            Controls.Add(TB);
            SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            TextAlign = HorizontalAlignment.Left;
            ForeColor = Color.FromArgb(66, 78, 90);
            Font = Theme.GlobalFont(FontStyle.Regular, 10);
            Size = new Size(130, 29);
            Enabled = true;
        }


        protected override void OnPaint(PaintEventArgs e) {
            G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);

            G.Clear(Parent.BackColor);


            if (Enabled) {
                TB.ForeColor = Color.FromArgb(66, 78, 90);

                if (State == Helpers.MouseState.Down) {
                    Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 3, Color.FromArgb(44, 156, 218));
                }
                else {
                    Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 3, Helpers.GreyColor(200));
                }

            }
            else {
                TB.ForeColor = Helpers.GreyColor(170);
                Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 3, Helpers.GreyColor(230));
            }

            TB.TextAlign = TextAlign;
            TB.UseSystemPasswordChar = UseSystemPasswordChar;

        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (!MultiLine) {
                int tbheight = TB.Height;
                TB.Location = new Point(10, Convert.ToInt32(((Height / 2) - (tbheight / 2) - 0)));
                TB.Size = new Size(Width - 20, tbheight);
            }
            else {
                TB.Location = new Point(10, 10);
                TB.Size = new Size(Width - 20, Height - 20);
            }
        }

        protected override void OnEnter(EventArgs e) {
            base.OnEnter(e);
            State = Helpers.MouseState.Down;
            Invalidate();
        }

        protected override void OnLeave(EventArgs e) {
            base.OnLeave(e);
            State = Helpers.MouseState.None;
            Invalidate();
        }

        #endregion

    }

    class FirefoxNumericUpDown : Control
    {

        #region " Private "
        private Graphics G;
        private int _Value;
        private int _Min;
        private int _Max;
        private Point Loc;
        private bool Down;
        private bool _EnabledCalc;
        #endregion
        private Color ETC = Color.Blue;

        #region " Properties "

        public new bool Enabled {
            get {
                return EnabledCalc;
            }
            set {
                _EnabledCalc = value;
                Invalidate();
            }
        }

        [DisplayName("Enabled")]
        public bool EnabledCalc {
            get {
                return _EnabledCalc;
            }
            set {
                Enabled = value;
                Invalidate();
            }
        }

        public int Value {
            get {
                return _Value;
            }

            set {
                if (value <= _Max & value >= Minimum) {
                    _Value = value;
                }

                Invalidate();

            }
        }

        public int Minimum {
            get {
                return _Min;
            }

            set {
                if (value < Maximum) {
                    _Min = value;
                }

                if (value < Minimum) {
                    value = Minimum;
                }

                Invalidate();
            }
        }

        public int Maximum {
            get {
                return _Max;
            }

            set {
                if (value > Minimum) {
                    _Max = value;
                }

                if (value > Maximum) {
                    value = Maximum;
                }

                Invalidate();
            }
        }

        #endregion

        #region " Control "

        public FirefoxNumericUpDown() {
            try {
                DoubleBuffered = true;
                Value = 0;
                Minimum = 0;
                Maximum = 100;
                Cursor = Cursors.IBeam;
                BackColor = Color.White;
                ForeColor = Color.White;
                Font = Theme.GlobalFont(FontStyle.Regular, 10);
                Enabled = true;
            }
            catch {

            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            try {
                base.OnMouseMove(e);
                Loc.X = e.X;
                Loc.Y = e.Y;
                Invalidate();

                if (Loc.X < Width - 23) {
                    Cursor = Cursors.IBeam;
                }
                else {
                    Cursor = Cursors.Default;
                }

            }
            catch {

            }
        }

        protected override void OnResize(EventArgs e) {
            try {
                base.OnResize(e);
                Height = 30;
            }
            catch {

            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            try {
                base.OnMouseClick(e);


                if (Enabled) {
                    if (Loc.X > Width - 21 && Loc.X < Width - 3) {
                        if (Loc.Y < 15) {
                            if ((Value + 1) <= Maximum) {
                                Value += 1;
                            }
                        }
                        else {
                            if ((Value - 1) >= Minimum) {
                                Value -= 1;
                            }
                        }
                    }
                    else {
                        Down = !Down;
                        Focus();
                    }

                }

                Invalidate();
            }
            catch {

            }
        }

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e) {
            try {
                base.OnKeyPress(e);
                try {
                    if (Down) {
                        Value = Convert.ToInt32(Value.ToString() + e.KeyChar.ToString());
                    }

                    if (Value > Maximum) {
                        Value = Maximum;
                    }

                }
                catch {
                }
            }
            catch {

            }
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e) {
            try {
                base.OnKeyUp(e);


                if (e.KeyCode == Keys.Up) {
                    if ((Value + 1) <= Maximum) {
                        Value += 1;
                    }

                    Invalidate();


                }
                else if (e.KeyCode == Keys.Down) {
                    if ((Value - 1) >= Minimum) {
                        Value -= 1;
                    }

                }
                else if (e.KeyCode == Keys.Back) {
                    string BC = Value.ToString();
                    BC = BC.Remove(Convert.ToInt32(BC.Length - 1));

                    if ((BC.Length == 0)) {
                        BC = "0";
                    }

                    Value = Convert.ToInt32(BC);

                }

                Invalidate();

            }
            catch {

            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            try {
                G = e.Graphics;
                G.SmoothingMode = SmoothingMode.HighQuality;
                G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                base.OnPaint(e);

                G.Clear(Parent.BackColor);


                if (Enabled) {
                    ETC = Color.FromArgb(66, 78, 90);

                    using (Pen P = new Pen(Helpers.GreyColor(190))) {
                        Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(190));
                        G.DrawLine(P, new Point(Width - 24, (int)13.5f), new Point(Width - 5, (int)13.5f));
                    }

                    Helpers.DrawRoundRect(G, new Rectangle(Width - 24, 4, 19, 21), 3, Helpers.GreyColor(200));

                }
                else {
                    ETC = Helpers.GreyColor(170);

                    using (Pen P = new Pen(Helpers.GreyColor(230))) {
                        Helpers.DrawRoundRect(G, Helpers.FullRectangle(Size, true), 2, Helpers.GreyColor(190));
                        G.DrawLine(P, new Point(Width - 24, (int)13.5f), new Point(Width - 5, (int)13.5f));
                    }

                    Helpers.DrawRoundRect(G, new Rectangle(Width - 24, 4, 19, 21), 3, Helpers.GreyColor(220));

                }

                using (SolidBrush B = new SolidBrush(ETC)) {
                    G.DrawString("t", new Font("Marlett", 8, FontStyle.Bold), B, new Point(Width - 22, 5));
                    G.DrawString("u", new Font("Marlett", 8, FontStyle.Bold), B, new Point(Width - 22, 13));
                    Helpers.CenterString(G, Value.ToString(), new Font("Segoe UI", 10), ETC, new Rectangle(Width / 2 - 10, 0, Width - 5, Height));
                }

            }
            catch {

            }
        }

        #endregion

    }
}
