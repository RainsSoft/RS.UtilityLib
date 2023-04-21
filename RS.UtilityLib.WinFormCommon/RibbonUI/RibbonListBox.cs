using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    public class RibbonListBox : ListBox
    {
        private RibbonScrollbar m_VScroll;
        private SolidBrush m_SelectedItemBrush = new SolidBrush(Color.FromArgb(180, 0, 255, 0));
        private SolidBrush m_TextBrush;
        public override Color ForeColor {
            get {
                return base.ForeColor;
            }
            set {
                if (base.ForeColor != value) {
                    base.ForeColor = value;
                    if (m_TextBrush == null) {
                        m_TextBrush = new SolidBrush(base.ForeColor);
                    }
                    else {
                        m_TextBrush.Color = base.ForeColor;
                    }
                }
            }
        }
        public RibbonListBox() {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.BackColor = Color.Transparent;
            this.BorderStyle = BorderStyle.None;
            this.m_TextBrush = new SolidBrush(this.ForeColor);
        }
        protected override void OnSelectedIndexChanged(EventArgs e) {
            this.Invalidate();
            base.OnSelectedIndexChanged(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);
            this.Invalidate();
        }
        protected override void OnInvalidated(InvalidateEventArgs e) {
            base.OnInvalidated(e);
            //System.Diagnostics.Debug.WriteLine(e.InvalidRect.ToString());
        }
        protected override void WndProc(ref Message m) {
            if (m.Msg == 277) { //WM_VSCROLL
                this.Invalidate();
            }
            else if (m.Msg == 513) { //WM_LBUTTONDOWN
                this.Invalidate();
            }
            base.WndProc(ref m);

        }
        protected override void OnPaint(PaintEventArgs e) {
            System.Diagnostics.Debug.WriteLine(e.ClipRectangle.ToString());
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, RibbonThemeManager.BackColor, ButtonBorderStyle.Solid);
            this.ScrollAlwaysVisible = false;
            //e.Graphics.DrawArc(
            if (this.SelectedItem != null) {
                Rectangle itemRect = this.GetItemRectangle(this.SelectedIndex);
                e.Graphics.FillRectangle(m_SelectedItemBrush, itemRect);
            }
            Rectangle targetRect = e.ClipRectangle;
            for (int i = 0; i < Items.Count; i++) {

                Rectangle rect = this.GetItemRectangle(i);
                if (rect.IntersectsWith(targetRect) == false) {
                    continue;
                }
                string s = this.GetItemText(this.Items[i]);
                if (i == this.SelectedIndex) {
                    s = "√" + s;
                }
                else {
                    s = "  " + s;
                }
                e.Graphics.DrawString(s, this.Font, m_TextBrush, rect);
            }
            //base.OnPaint(e);
        }
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (m_TextBrush != null) {
                m_TextBrush.Dispose();
            }
            if (m_SelectedItemBrush != null) {
                m_SelectedItemBrush.Dispose();
            }
        }
    }
}
