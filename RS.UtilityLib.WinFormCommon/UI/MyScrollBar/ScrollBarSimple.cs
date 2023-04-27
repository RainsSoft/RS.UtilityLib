using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI.MyScrollBar
{
    public class ScrollBarSimple : Control
    {
        public event ScrollEventHandler Scroll;

        public ScrollBarSimple() {
            this.BackColor = System.Drawing.Color.White;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }
        private int @_value;

        public int Value {
            get {
                return @_value;
            }
            set {
                if (this.@_value == value)
                    return;
                this.@_value = value;
                Invalidate();
                OnScroll();
            }
        }

        private int _maximum = 100;
        public int Maximum {
            get {
                return _maximum;
            }
            set {
                _maximum = value;
                Invalidate();
            }
        }

        private int _thumbSize = 18;
        public int ThumbSize {
            get {
                return _thumbSize;
            }
            set {
                _thumbSize = value;
                Invalidate();
            }
        }
        /// <summary>
        /// 滚动位置颜色
        /// </summary>
        private Color _thumbColor = System.Drawing.Color.Black; //Color.Gray;
        public Color ThumbColor {
            get {
                return _thumbColor;
            }
            set {
                _thumbColor = value;
                Invalidate();
            }
        }
        /// <summary>
        /// 边框颜色
        /// </summary>
        private Color _borderColor = System.Drawing.Color.Black;//Color.Silver;
        public Color BorderColor {
            get {
                return _borderColor;
            }
            set {
                _borderColor = value;
                Invalidate();
            }
        }

        private ScrollOrientation orientation;
        public ScrollOrientation Orientation {
            get {
                return orientation;
            }
            set {
                orientation = value;
                Invalidate();
            }
        }



        protected override void OnMouseDown(MouseEventArgs e) {
            if (e.Button == MouseButtons.Left)
                MouseScroll(e);
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if (e.Button == MouseButtons.Left)
                MouseScroll(e);
            base.OnMouseMove(e);
        }

        /*
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                OnScroll(ScrollEventType.EndScroll);
            base.OnMouseUp(e);
        }*/

        private void MouseScroll(MouseEventArgs e) {
            int v = 0;
            switch (Orientation) {
                case ScrollOrientation.VerticalScroll:
                    v = Maximum * (e.Y - _thumbSize / 2) / (Height - _thumbSize);
                    break;
                case ScrollOrientation.HorizontalScroll:
                    v = Maximum * (e.X - _thumbSize / 2) / (Width - _thumbSize);
                    break;
            }
            Value = Math.Max(0, Math.Min(Maximum, v));
        }
        public virtual void OnScroll() {
            OnScroll(ScrollEventType.ThumbPosition);
        }
        public virtual void OnScroll(ScrollEventType type) {
            if (Scroll != null)
                Scroll(this, new ScrollEventArgs(type, Value, Orientation));
        }
        /// <summary>
        /// 滚动条 有效端(左端，上端) 颜色
        /// </summary>
        private System.Drawing.Color _ActiveColor = Color.FromArgb(210, 71, 38); //System.Drawing.SystemColors.ActiveBorder;
        [DefaultValue("System.Drawing.SystemColors.ActiveBorder")]
        public System.Drawing.Color ActiveColor {
            get {
                return _ActiveColor;
            }
            set {
                if (value != _ActiveColor) {
                    _ActiveColor = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        ///滚动条  无效端（右端，下端）颜色
        /// </summary>
        private System.Drawing.Color _InactiveColor = System.Drawing.SystemColors.ScrollBar;
        [DefaultValue("System.Drawing.SystemColors.ScrollBar")]
        public System.Drawing.Color InactiveColor {
            get {
                return _InactiveColor;
            }
            set {
                if (value != _InactiveColor) {
                    _InactiveColor = value;
                    this.Invalidate();
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e) {
            if (Maximum <= 0)
                return;

            Rectangle thumbRect = Rectangle.Empty;
            Rectangle thumbRect_left = Rectangle.Empty;
            Rectangle thumbRect_right = Rectangle.Empty;
            switch (Orientation) {
                case ScrollOrientation.HorizontalScroll:
                    //thumbRect = new Rectangle(@_value * (Width - _thumbSize) / Maximum, 2, _thumbSize, Height - 4);
                    thumbRect = new Rectangle(@_value * (Width - _thumbSize) / Maximum, 0, _thumbSize, Height);
                    thumbRect_left = new Rectangle(2, 2, thumbRect.Left, Height - 4);
                    thumbRect_right = new Rectangle(thumbRect.Right - 2, 2, Width - thumbRect.Right, Height - 4);
                    break;
                case ScrollOrientation.VerticalScroll:
                    //thumbRect = new Rectangle(2, @_value * (Height - _thumbSize) / Maximum, Width - 4, _thumbSize);
                    thumbRect = new Rectangle(0, @_value * (Height - _thumbSize) / Maximum, Width, _thumbSize);
                    thumbRect_left = new Rectangle(2, 2, Width - 4, thumbRect.Top);
                    thumbRect_right = new Rectangle(2, thumbRect.Bottom - 2, Width - 4, Height - thumbRect.Bottom);
                    break;
            }
            //绘制左有效位置
            using (var brush = new SolidBrush(_ActiveColor)) {
                e.Graphics.FillRectangle(brush, thumbRect_left);
            }
            //绘制右无效位置
            using (var brush = new SolidBrush(_InactiveColor)) {
                e.Graphics.FillRectangle(brush, thumbRect_right);
            }
            //绘制滚动位置
            using (var brush = new SolidBrush(_thumbColor))
                e.Graphics.FillRectangle(brush, thumbRect);
            //绘制边框
            using (var pen = new Pen(_borderColor))
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
            //debug
            if (this.orientation == ScrollOrientation.VerticalScroll) {
                this.Text = string.Format("{0}\n%\n{1}", Value, Maximum);
            }
            else {
                this.Text = string.Format("{0}%{1}", Value, Maximum);
            }
            e.Graphics.DrawString(this.Text, this.Font, SystemBrushes.MenuBar, 0, 0);
        }

        /// <summary>
        /// This method for MyScrollBar
        /// </summary>
        public static void AdjustScrollbar(ScrollBarSimple scrollBarMy, int max, int value, int clientSize) {
            scrollBarMy.Maximum = max;
            scrollBarMy.Visible = max > 0;
            scrollBarMy.Value = Math.Min(scrollBarMy.Maximum, value);
        }
        public void AdjustScrollbar(int max, int value, int target_clientSize) {
            AdjustScrollbar(this, max, value, target_clientSize);
        }

        /// <summary>
        /// This method for System.Windows.Forms.ScrollBar and inherited classes
        /// </summary>
        [Obsolete()]
        private void AdjustScrollbar(ScrollBar scrollBar, int max, int value, int clientSize) {
            scrollBar.LargeChange = clientSize / 3;
            scrollBar.SmallChange = clientSize / 11;
            scrollBar.Maximum = max + scrollBar.LargeChange;
            scrollBar.Visible = max > 0;
            scrollBar.Value = Math.Min(scrollBar.Maximum, value);
        }


        //private void ScrollBar_Scroll(object sender, ScrollEventArgs e) {
        //第一步：注册滚动事件  驱动对象滚动到某个位置
        //    fctb.OnScroll(e, e.Type != ScrollEventType.ThumbTrack && e.Type != ScrollEventType.ThumbPosition);
        //}
        //private void fctb_ScrollbarsUpdated(object sender, EventArgs e) {
        //第2步：对象滚动更新 设置当前滚动条位置
        //    AdjustScrollbar(vMyScrollBar, fctb.VerticalScroll.Maximum, fctb.VerticalScroll.Value, fctb.ClientSize.Height);

        //}
        /// <summary>
        /// 绑定系统水平或者垂直滚动条
        /// </summary>
        /// <param name="sBar"></param>
        private void BindSysHScrollBar(ScrollableControl control) {//, HScrollBar hScrollBar) {

            //AdjustScrollbar(vMyScrollBar, fctb.VerticalScroll.Maximum, fctb.VerticalScroll.Value, fctb.ClientSize.Height);
            this.AdjustScrollbar(control.HorizontalScroll.Maximum, control.HorizontalScroll.Value, control.ClientSize.Width);

            ////AdjustScrollbar(vScrollBar, fctb.VerticalScroll.Maximum, fctb.VerticalScroll.Value, fctb.ClientSize.Height);
            //this.AdjustScrollbar(hScrollBar, control.HorizontalScroll.Maximum, control.HorizontalScroll.Value, control.ClientSize.Width);
            ////订阅滚动事件
            //hScrollBar.Scroll -= HScrollBar_Scroll;
            //hScrollBar.Scroll += HScrollBar_Scroll;
            //
            this.Scroll -= HScrollBar_Scroll;
            this.Scroll += HScrollBar_Scroll;
        }


        private void HScrollBar_Scroll(object sender, ScrollEventArgs e) {
            //if(se.ScrollOrientation == ScrollOrientation.VerticalScroll) {
            //    //align by line height
            //    int newValue = se.NewValue;
            //    if(alignByLines)
            //        newValue = (int)(Math.Ceiling(1d * newValue / CharHeight) * CharHeight);
            //    //
            //    VerticalScroll.Value = Math.Max(VerticalScroll.Minimum, Math.Min(VerticalScroll.Maximum, newValue));
            //}
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll &&
                this.m_ContainerControl != null) {

                this.m_ContainerControl.HorizontalScroll.Value = Math.Max(this.m_ContainerControl.HorizontalScroll.Minimum, Math.Min(this.m_ContainerControl.HorizontalScroll.Maximum, e.NewValue));
                //
                this.AdjustScrollbar(this.m_ContainerControl.HorizontalScroll.Maximum, this.m_ContainerControl.HorizontalScroll.Value, this.m_ContainerControl.ClientSize.Width);
            }
        }

        private void BindSysVScrollBar(ScrollableControl control) {//, VScrollBar vScrollBar) {
            this.AdjustScrollbar(control.VerticalScroll.Maximum, control.VerticalScroll.Value, control.ClientSize.Height);
            //AdjustScrollbar(hMyScrollBar, fctb.HorizontalScroll.Maximum, fctb.HorizontalScroll.Value, fctb.ClientSize.Width);

            //this.AdjustScrollbar(vScrollBar, control.VerticalScroll.Maximum, control.VerticalScroll.Value, control.ClientSize.Height);
            ////AdjustScrollbar(hScrollBar, fctb.HorizontalScroll.Maximum, fctb.HorizontalScroll.Value, fctb.ClientSize.Width);
            ////订阅滚动事件
            //vScrollBar.Scroll -= VScrollBar_Scroll;
            //vScrollBar.Scroll += VScrollBar_Scroll;
            //
            this.Scroll -= VScrollBar_Scroll;
            this.Scroll += VScrollBar_Scroll;
        }

        private void VScrollBar_Scroll(object sender, ScrollEventArgs e) {

            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll &&
                this.m_ContainerControl != null) {
                //align by line height
                int newValue = e.NewValue;
                //if(alignByLines)
                //    newValue = (int)(Math.Ceiling(1d * newValue / CharHeight) * CharHeight);
                //
                this.m_ContainerControl.VerticalScroll.Value = Math.Max(this.m_ContainerControl.VerticalScroll.Minimum, Math.Min(this.m_ContainerControl.VerticalScroll.Maximum, newValue));

                //
                this.AdjustScrollbar(this.m_ContainerControl.VerticalScroll.Maximum, this.m_ContainerControl.VerticalScroll.Value, this.m_ContainerControl.ClientSize.Height);
            }
            //if(se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            //    HorizontalScroll.Value = Math.Max(HorizontalScroll.Minimum, Math.Min(HorizontalScroll.Maximum, se.NewValue));
        }


        private ScrollableControl m_ContainerControl;
        [System.ComponentModel.Browsable(true)]
        [Localizable(true)]
        [System.ComponentModel.Category("Appearance")]
        [System.ComponentModel.DefaultValue(null)]
        [System.ComponentModel.Description("要滚动的容器")]
        public ScrollableControl ScrollContainerControl {
            get {
                return m_ContainerControl;
            }
            set {
                m_ContainerControl = value;
                if (m_ContainerControl != null) {
                    if (this.Orientation == ScrollOrientation.VerticalScroll) {
                        BindSysVScrollBar(this.m_ContainerControl);
                    }
                    else {
                        BindSysHScrollBar(this.m_ContainerControl);
                    }

                }
            }
        }
    }
}
