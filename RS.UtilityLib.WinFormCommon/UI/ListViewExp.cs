using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
 
    /// <summary>
    /// 进度条/链接 [ 第一列不支持进度条/链接 ]
    /// </summary>
    public class ListViewExp : ListViewEx
    {
        private Color _rowBackColor1 = Color.White;
        private Color _rowBackColor2 = Color.FromArgb(254, 216, 249);
        private Color _selectedColor = Color.FromArgb(166, 222, 255);
        private Color _headColor = Color.FromArgb(166, 222, 255);

        public ListViewExp()
            : base() {
            base.OwnerDraw = true;
            //base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        [DefaultValue(typeof(Color), "White")]
        public Color RowBackColor1 {
            get {
                return _rowBackColor1;
            }
            set {
                _rowBackColor1 = value;
                base.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "254, 216, 249")]
        public Color RowBackColor2 {
            get {
                return _rowBackColor2;
            }
            set {
                _rowBackColor2 = value;
                base.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "166, 222, 255")]
        public Color SelectedColor {
            get {
                return _selectedColor;
            }
            set {
                _selectedColor = value;
                base.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "166, 222, 255")]
        public Color HeadColor {
            get {
                return _headColor;
            }
            set {
                _headColor = value;
                base.Invalidate();
            }
        }

        protected override void OnDrawColumnHeader(
            DrawListViewColumnHeaderEventArgs e) {
            base.OnDrawColumnHeader(e);

            Graphics g = e.Graphics;
            Rectangle bounds = e.Bounds;

            //Color baseColor = _headColor;
            //Color borderColor = _headColor;
            //Color innerBorderColor = Color.FromArgb(200, 255, 255);

            //RenderBackgroundInternal(
            //    g,
            //    bounds,
            //    baseColor,
            //    borderColor,
            //    innerBorderColor,
            //    0.35f,
            //    true,
            //    LinearGradientMode.Vertical);

            TextFormatFlags flags = GetFormatFlags(e.Header.TextAlign);
            Rectangle textRect = new Rectangle(
                       bounds.X + 3,
                       bounds.Y,
                       bounds.Width - 6,
                       bounds.Height);
            ;

            //if (e.Header.ImageList != null) {
            //    Image image = e.Header.ImageIndex == -1 ?
            //        null : e.Header.ImageList.Images[e.Header.ImageIndex];
            //    if (image != null) {
            //        Rectangle imageRect = new Rectangle(
            //            bounds.X + 3,
            //            bounds.Y + 2,
            //            bounds.Height - 4,
            //            bounds.Height - 4);
            //        g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            //        g.DrawImage(image, imageRect);

            //        textRect.X = imageRect.Right + 3;
            //        textRect.Width -= imageRect.Width;
            //    }
            //}
            //
            e.DrawBackground();
            System.Windows.Forms.GroupBoxRenderer.DrawGroupBox(g, bounds, System.Windows.Forms.VisualStyles.GroupBoxState.Normal);
            //System.Windows.Forms.ButtonRenderer.DrawButton(g, bounds, System.Windows.Forms.VisualStyles.PushButtonState.Default);
            //
            TextRenderer.DrawText(
                   g,
                   e.Header.Text,
                   e.Font,
                   textRect,
                   e.ForeColor,
                   flags);
        }

        protected override void OnPaint(PaintEventArgs e) {
            //e.Graphics.Clear(this.BackColor);//清除显示类容
            //System.Windows.Forms.GroupBoxRenderer.DrawGroupBox(e.Graphics, e.ClipRectangle, System.Windows.Forms.VisualStyles.GroupBoxState.Normal);
            base.OnPaint(e);
        }
        protected override void OnDrawItem(DrawListViewItemEventArgs e) {
            //int x=  Cursor.Current.HotSpot.X;
            //int y = Cursor.Current.HotSpot.Y;
            //Point p= this.PointToClient(new Point(x, y));
            // if (e.Bounds.Contains(p)) {
            //     int i = 0;
            // }

            base.OnDrawItem(e);
            if (View != View.Details) {
                e.DrawDefault = true;
            }
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e) {
            base.OnDrawSubItem(e);
            if (View != View.Details) {
                return;
            }
            if (e.ItemIndex == -1) {
                return;
            }
            Rectangle eBounds = e.Bounds;           
            bool eItemSelected = e.Item.Selected;
            Graphics eGraphics = e.Graphics;
         
            //if ((itemState & ListViewItemStates.Selected)
            //    == ListViewItemStates.Selected) {
            if (eItemSelected) {
                //e.DrawFocusRectangle(e.Bounds);
                //e.DrawBackground();
                //e.Graphics.FillRectangle(SystemBrushes.WindowFrame, e.Bounds);
                //bounds.Height--;
                Color baseColor = _selectedColor;
                Color borderColor = _selectedColor;
                Color innerBorderColor = Color.FromArgb(200, 255, 255);

                RenderBackgroundInternal(
                    eGraphics,
                    eBounds,
                    baseColor,
                    borderColor,
                    innerBorderColor,
                    0.35f,
                    true,
                    LinearGradientMode.Vertical);
                //bounds.Height++;
            }
            else {
                Color backColor = e.ItemIndex % 2 == 0 ?
                _rowBackColor1 : _rowBackColor2;

                using (SolidBrush brush = new SolidBrush(backColor)) {
                    eGraphics.FillRectangle(brush, eBounds);
                }
            }
            //           
            TextFormatFlags flags = GetFormatFlags(e.Header.TextAlign);
            //
            if (e.ColumnIndex == 0) {
                //g.FillRectangle(SystemBrushes.Info, bounds);
                if (e.Item.ImageList == null) {
                    e.DrawText(flags);
                    return;
                }
                Image image = e.Item.ImageIndex == -1 ?
                    null : e.Item.ImageList.Images[e.Item.ImageIndex];
                if (image == null) {
                    e.DrawText(flags);
                    return;
                }
                Rectangle imageRect = new Rectangle(
                    eBounds.X + 4,
                    eBounds.Y + 2,
                    eBounds.Height - 4,
                    eBounds.Height - 4);
                eGraphics.DrawImage(image, imageRect);

                Rectangle textRect = new Rectangle(
                    imageRect.Right + 3,
                    eBounds.Y,
                    eBounds.Width - imageRect.Right - 3,
                    eBounds.Height);
                TextRenderer.DrawText(
                    eGraphics,
                    e.Item.Text,
                    e.Item.Font,
                    textRect,
                    e.Item.ForeColor,
                    flags);
                return;
            }
            //
            ListViewItemStates itemState = e.ItemState;
            string eSubItemText = e.SubItem.Text;
            HorizontalAlignment textAlign = e.Header.TextAlign;
            //绘制进度条
            if (e.Header.Text == mProgressColumnText) {
                if (CheckIsFloat(eSubItemText)) {
                    float tmpf = float.Parse(eSubItemText);
                    if (tmpf >= 1.0f) {
                        tmpf = tmpf / 100.0f;
                    }
                    DrawProgress(
                        new Rectangle(eBounds.X + 2, eBounds.Y + 2, eBounds.Width - 4, eBounds.Height - 6),
                        tmpf, eGraphics, eItemSelected);
                }
                else {
                    //文本非数字
                    Rectangle rect = eBounds;
                    DrawString(rect, eSubItemText, textAlign, eGraphics, eItemSelected);
                }
            }
            else if (e.Header.Text == mLinkColumnText) {
                //绘制可链接文本
                //e.DrawText(flags);
                Rectangle rect = eBounds;
                //Size size = rect.Size;
                //SizeF sf= e.Graphics.MeasureString(string.IsNullOrEmpty(e.Item.Text)?"aa":e.Item.Text,this.Font);
                //float w = sf.Width ;
                //g.DrawLine(System.Drawing.Pens.Blue, rect.X + 2, rect.Y + size.Height * 3 / 4, rect.X + w-2, rect.Y + size.Height * 3 / 4);
                bool over = rect.Contains(mMouseX, mMouseY);
                DrawLinkString(rect, eSubItemText, textAlign, eGraphics, eItemSelected, true);
            }
            else {
                bool isimage = false;
                System.Drawing.Image btnImg = null;
                if (mBtnImgs != null && mBtnImgs.ContainsKey(e.Header.Text)) {
                    isimage = true;
                    btnImg = mBtnImgs[e.Header.Text];
                }
                Rectangle tmpRect = eBounds;
                if (isimage && btnImg != null) {
                    bool over = tmpRect.Contains(mMouseX, mMouseY);
                    DrawButtonImg(tmpRect, btnImg, textAlign, eGraphics,
                    eItemSelected, over);
                }
                else {
                    e.DrawText(flags);
                }
            }
        }

        protected TextFormatFlags GetFormatFlags(HorizontalAlignment align) {
            TextFormatFlags flags =
                    TextFormatFlags.EndEllipsis |
                    TextFormatFlags.VerticalCenter;

            switch (align) {
                case HorizontalAlignment.Center:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
                case HorizontalAlignment.Right:
                    flags |= TextFormatFlags.Right;
                    break;
                case HorizontalAlignment.Left:
                    flags |= TextFormatFlags.Left;
                    break;
            }

            return flags;
        }

        internal void RenderBackgroundInternal(
          Graphics g,
          Rectangle rect,
          Color baseColor,
          Color borderColor,
          Color innerBorderColor,
          float basePosition,
          bool drawBorder,
          LinearGradientMode mode) {
            if (drawBorder) {
                rect.Width--;
                rect.Height--;
            }
            using (LinearGradientBrush brush = new LinearGradientBrush(
               rect, Color.Transparent, Color.Transparent, mode)) {
                Color[] colors = new Color[4];
                colors[0] = GetColor(baseColor, 0, 35, 24, 9);
                colors[1] = GetColor(baseColor, 0, 13, 8, 3);
                colors[2] = baseColor;
                colors[3] = GetColor(baseColor, 0, 68, 69, 54);

                ColorBlend blend = new ColorBlend();
                blend.Positions = new float[] { 0.0f, basePosition, basePosition + 0.05f, 1.0f };
                blend.Colors = colors;
                brush.InterpolationColors = blend;
                g.FillRectangle(brush, rect);
            }
            if (baseColor.A > 80) {
                Rectangle rectTop = rect;
                if (mode == LinearGradientMode.Vertical) {
                    rectTop.Height = (int)(rectTop.Height * basePosition);
                }
                else {
                    rectTop.Width = (int)(rect.Width * basePosition);
                }

                using (SolidBrush brushAlpha =
                    new SolidBrush(Color.FromArgb(80, 255, 255, 255))) {
                    g.FillRectangle(brushAlpha, rectTop);
                }
            }

            if (drawBorder) {
                using (Pen pen = new Pen(borderColor)) {
                    g.DrawRectangle(pen, rect);
                }

                rect.Inflate(-1, -1);
                using (Pen pen = new Pen(innerBorderColor)) {
                    g.DrawRectangle(pen, rect);
                }
            }
        }

        private Color GetColor(Color colorBase, int a, int r, int g, int b) {
            int a0 = colorBase.A;
            int r0 = colorBase.R;
            int g0 = colorBase.G;
            int b0 = colorBase.B;

            if (a + a0 > 255) {
                a = 255;
            }
            else {
                a = a + a0;
            }
            if (r + r0 > 255) {
                r = 255;
            }
            else {
                r = r + r0;
            }
            if (g + g0 > 255) {
                g = 255;
            }
            else {
                g = g + g0;
            }
            if (b + b0 > 255) {
                b = 255;
            }
            else {
                b = b + b0;
            }

            return Color.FromArgb(a, r, g, b);
        }
        #region 组件设计器生成的代码
        //const int  WM_NCPAINT = 0x0085;
        //const int WM_WINDOWPOSCHANGED = 0x0047;
        //protected override void WndProc(ref Message m) {
        //    switch (m.Msg) {
        //        case (int)WM_NCPAINT:
        //            //WmNcPaint(ref m);
        //            break;
        //        case (int)WM_WINDOWPOSCHANGED:
        //            base.WndProc(ref m);
        //            //IntPtr result = m.Result;
        //            //WmNcPaint(ref m);
        //            //m.Result = result;
        //            break;
        //        default:
        //            base.WndProc(ref m);
        //            break;
        //    }
        //}
        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e) {
            //base.OnColumnWidthChanging(e);
            //return;
            //          
            if (e.NewWidth < 48) {
                e.NewWidth = 48;
                e.Cancel = true;
                //return;
            }
            //else {
            //    e.Cancel = false;
            //}
            //e.NewWidth = this.Columns[e.ColumnIndex].Width;
            base.OnColumnWidthChanging(e);
        }
       
        //protected override void OnColumnReordered(ColumnReorderedEventArgs e) {
        //    e.Cancel = true;
        //}

        #endregion

        #region 进度条
        private String mProgressColumnText = "progress";//String.Empty;

        /// <summary>
        /// 需要设置为进度条的列标头文字,根据这些文字判断所在列是否为进度条显示列
        /// </summary>
        internal void SetProgressColumnText(string ProgressColumn) {
            mProgressColumnText = ProgressColumn;
        }
        private Color mProgressColor = Color.Red;
        public Color ProgressColor {
            get {
                return this.mProgressColor;
            }
            set {
                this.mProgressColor = value;
            }
        }
        private Color mProgressTextColor = Color.Black;
        public Color ProgressTextColor {
            get {
                return mProgressTextColor;
            }
            set {
                mProgressTextColor = value;
            }
        }

        const string numberstring = "0123456789.";
        private bool CheckIsFloat(String s) {
            foreach (char c in s) {
                if (numberstring.IndexOf(c) > -1) {
                    continue;
                }
                else
                    return false;
            }
            return true;
        }
        #endregion
        int mMouseX, mMouseY;
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            mMouseX = e.X;
            mMouseY = e.Y;
            //return;
            //ListViewItem item=this.GetItemAt(mMouseX, mMouseY);
            ListViewHitTestInfo hinfo = this.HitTest(e.X, e.Y);
            if (hinfo.SubItem != null) {
                //this.Cursor = Cursors.Hand;
                //Rectangle rec = hinfo.Item.SubItems[0].Bounds;
                //Rectangle reca=hinfo.Item.Bounds;
                //rec = new Rectangle(rec.X + rec.Width, rec.Y, reca.Width - rec.Width, rec.Height);
                this.Invalidate(hinfo.Item.Bounds);
            }
            else {
                //this.Cursor = Cursors.Arrow;
            }
        }


        Dictionary<string, System.Drawing.Image> mBtnImgs;
        /// <summary>
        /// 列名称 背景图
        /// </summary>
        /// <param name="btnImgs"></param>
        internal void SetButtonImgs(Dictionary<string, System.Drawing.Image> btnImgs) {
            mBtnImgs = btnImgs;

        }

        string mLinkColumnText="link";
        internal void SetLinkColumnText(string LinkColumnColumn) {
            mLinkColumnText = LinkColumnColumn;
        }
        private void DrawButtonImg(Rectangle rect, System.Drawing.Image s, HorizontalAlignment aglin, Graphics g, bool isSelect, bool mouseover) {
            if (rect.Height < 2 || rect.Width < 2) {
                return;
            }
            //StringFormat sf = new StringFormat();
            //switch (aglin) {
            //    case HorizontalAlignment.Center:
            //        sf.Alignment = StringAlignment.Center;
            //        break;
            //    case HorizontalAlignment.Left:
            //        sf.Alignment = StringAlignment.Near;
            //        break;
            //    case HorizontalAlignment.Right:
            //        sf.Alignment = StringAlignment.Far;
            //        break;
            //}
            //sf.LineAlignment = StringAlignment.Center;
            //sf.Trimming = StringTrimming.EllipsisCharacter;
            int mImgWidth = s.Width / 3 > rect.Width ? rect.Width : s.Width / 3;
            int mImgHeight = s.Height > rect.Height ? rect.Height : s.Height;
            if (!isSelect) {
                //g.DrawString(s, this.Font, Brushes.Black, rect, sf);
                if (mouseover) {
                    this.Cursor = Cursors.Hand;
                    Rectangle imgRec = new Rectangle(s.Width * 1 / 3, 0, s.Width / 3, s.Height);
                    //g.DrawImage(s, rect.X, rect.Y, imgRec, GraphicsUnit.Pixel);
                    g.DrawImage(s, new Rectangle(rect.X + 4, rect.Y + 4, mImgWidth, mImgHeight), imgRec, GraphicsUnit.Pixel);
                }
                else {
                    Rectangle imgRec = new Rectangle(s.Width * 0 / 3, 0, s.Width / 3, s.Height);
                    g.DrawImage(s, new Rectangle(rect.X + 4, rect.Y + 4, mImgWidth, mImgHeight), imgRec, GraphicsUnit.Pixel);
                    //g.DrawImage(s, rect.X, rect.Y, imgRec, GraphicsUnit.Pixel);
                }
            }
            else {
                //g.FillRectangle(Brushes.RoyalBlue, rect);
                //g.DrawString(s, this.Font, Brushes.White, rect, sf);
                if (mouseover) {
                    this.Cursor = Cursors.Hand;
                    Rectangle imgRec = new Rectangle(s.Width * 1 / 3, 0, s.Width / 3, s.Height);
                    g.DrawImage(s, new Rectangle(rect.X + 4, rect.Y + 4, mImgWidth, mImgHeight), imgRec, GraphicsUnit.Pixel);
                    //g.DrawImage(s, rect.X, rect.Y, imgRec, GraphicsUnit.Pixel);

                }
                else {

                    Rectangle imgRec = new Rectangle(s.Width * 2 / 3, 0, s.Width / 3, s.Height);
                    g.DrawImage(s, new Rectangle(rect.X + 4, rect.Y + 4, mImgWidth, mImgHeight), imgRec, GraphicsUnit.Pixel);
                    //g.DrawImage(s, rect.X, rect.Y, imgRec, GraphicsUnit.Pixel);
                }
            }

        }
        //System.Drawing.Color mLinkColor = System.Drawing.Color.Blue;
        //System.Drawing.Color mLinkColorMouseOver= System.Drawing.Color.Red;

        private void DrawLinkString(Rectangle rect, string s, HorizontalAlignment aglin, Graphics g, bool isSelect, bool mouseover) {
            StringFormat sf = new StringFormat();
            switch (aglin) {
                case HorizontalAlignment.Center:
                    sf.Alignment = StringAlignment.Center;
                    break;
                case HorizontalAlignment.Left:
                    sf.Alignment = StringAlignment.Near;
                    break;
                case HorizontalAlignment.Right:
                    sf.Alignment = StringAlignment.Far;
                    break;
            }
            sf.LineAlignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            if (!isSelect) {
                g.DrawString(s, this.Font, Brushes.Blue, rect, sf);
                if (mouseover) {
                    //this.Cursor = Cursors.Hand;
                    SizeF size = g.MeasureString(s, this.Font);
                    float offset = 0; 
                    if (aglin == HorizontalAlignment.Center) {
                        offset=System.Math.Max(0f,(rect.Width-size.Width)*0.5f);
                    }
                    else if (aglin == HorizontalAlignment.Right) { 
                        offset= System.Math.Max(0f, (rect.Width - size.Width));
                    }
                    g.DrawLine(System.Drawing.Pens.Blue, rect.X+offset, rect.Y + size.Height, rect.X + offset+size.Width, rect.Y + size.Height);                  
                    
                }
                else {
                    //this.Cursor = Cursors.Arrow;
                }
            }
            else {
                //g.FillRectangle(Brushes.RoyalBlue, rect);
                g.DrawString(s, this.Font, Brushes.White, rect, sf);
                if (mouseover) {
                    //this.Cursor = Cursors.Hand;
                    SizeF size = g.MeasureString(s, this.Font);
                    g.DrawLine(System.Drawing.Pens.White, rect.X, rect.Y + size.Height, rect.Right, rect.Y + size.Height);
                   
                }
                else {
                    //this.Cursor = Cursors.Arrow;
                }
            }

        }
        //绘制非进度条列的subitem
        private void DrawString(Rectangle rect, String s, HorizontalAlignment aglin, Graphics g, bool isSelect) {
            StringFormat sf = new StringFormat();
            switch (aglin) {
                case HorizontalAlignment.Center:
                    sf.Alignment = StringAlignment.Center;
                    break;
                case HorizontalAlignment.Left:
                    sf.Alignment = StringAlignment.Near;
                    break;
                case HorizontalAlignment.Right:
                    sf.Alignment = StringAlignment.Far;
                    break;
            }
            sf.LineAlignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            if (!isSelect)
                g.DrawString(s, this.Font, Brushes.Black, rect, sf);
            else {
                g.FillRectangle(Brushes.RoyalBlue, rect);
                g.DrawString(s, this.Font, Brushes.White, rect, sf);
            }
        }

        ///绘制进度条列的subitem
        private void DrawProgress(Rectangle rect, float percent, Graphics g, bool isSelect) {
            if (rect.Height > 2 && rect.Width > 2) {
                if (rect.Top > 0 && rect.Top < this.Height &&
                 (rect.Left > this.Left && rect.Left < this.Width)) {
                    //绘制进度
                    int width = (int)(rect.Width * percent);
                    Rectangle newRect = new Rectangle(rect.Left + 1, rect.Top + 1, width - 2, rect.Height - 2);
                    using (Brush tmpb = new SolidBrush(this.mProgressColor)) {
                        g.FillRectangle(tmpb, newRect);
                    }
                    if (!isSelect)
                        g.DrawRectangle(Pens.Yellow, rect);
                    else {
                        newRect = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2);
                        g.DrawRectangle(Pens.RoyalBlue, newRect);
                    }
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    newRect = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2);
                    using (Brush b = new SolidBrush(mProgressTextColor)) {
                        g.DrawString(percent.ToString("p1"), this.Font, b, newRect, sf);
                    }
                }
            }
            else
                return;
        }



        //private void ListViewEx_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
        //    e.Graphics.Clear(this.BackColor);//清除显示类容

        //    int progressIndex = -1;
        //    //检索被设置为进度条的列
        //    if (progressIndex == -1 && !string.IsNullOrEmpty(mProgressColumnText)) {
        //        for (int i = 0; i < this.Columns.Count; i++) {
        //            if (this.Columns[i].Text == mProgressColumnText) {
        //                progressIndex = i; break;
        //            }
        //        }
        //    }
        //    int linIndex = -1;
        //    if (linIndex == -1 && !string.IsNullOrEmpty(mLinkColumnText)) {
        //        for (int i = 0; i < this.Columns.Count; i++) {
        //            if (this.Columns[i].Text == mLinkColumnText) {
        //                linIndex = i; break;
        //            }
        //        }
        //    }
        //    //依次绘制每一行
        //    for (int i = 0; i < this.Items.Count; i++) {
        //        Rectangle rect = this.GetItemRect(i);//获取当前要绘制行的Rect
        //        if (rect.Top < 0 || rect.Top >= this.ClientRectangle.Height)
        //            continue; //不在显示范围内,跳过

        //        if (rect != Rectangle.Empty) {
        //            int totalwidth = 0; //列宽记数,实际上就是当前要绘制列的X坐标
        //            for (int j = 0; j < this.Columns.Count; j++) {
        //                int currwidth = this.Columns[j].Width;//获取当前列宽度
        //                Rectangle tmpRect = new Rectangle(totalwidth, rect.Top, currwidth, rect.Height);//生成当前subitem的外接矩形
        //                if (j == linIndex) {
        //                    //非进度条列
        //                    bool over = tmpRect.Contains(mMouseX, mMouseY);
        //                    DrawLinkString(tmpRect, this.Items[i].SubItems[j].Text, this.Columns[j].TextAlign, e.Graphics,
        //                    this.Items[i].Selected, over);
        //                }
        //                else if (j == progressIndex) {
        //                    //进度条列
        //                    //判断当前subitem文本是否可以转为浮点数
        //                    if (CheckIsFloat(this.Items[i].SubItems[j].Text)) {
        //                        float tmpf = float.Parse(this.Items[i].SubItems[j].Text);
        //                        if (tmpf >= 1.0f) {
        //                            tmpf = tmpf / 100.0f;
        //                        }
        //                        DrawProgress(tmpRect, tmpf, e.Graphics,
        //                         this.Items[i].Selected);
        //                    }
        //                }
        //                else {
        //                    bool isimage = false;
        //                    System.Drawing.Image btnImg = null;
        //                    if (mBtnImgs != null && mBtnImgs.ContainsKey(this.Columns[j].Text)) {
        //                        isimage = true;
        //                        btnImg = mBtnImgs[this.Columns[j].Text];
        //                    }
        //                    if (isimage && btnImg != null) {
        //                        bool over = tmpRect.Contains(mMouseX, mMouseY);
        //                        DrawButtonImg(tmpRect, btnImg, this.Columns[j].TextAlign, e.Graphics,
        //                        this.Items[i].Selected, over);
        //                    }
        //                    else {
        //                        DrawString(tmpRect, this.Items[i].SubItems[j].Text, this.Columns[j].TextAlign, e.Graphics,
        //                        this.Items[i].Selected);
        //                    }
        //                }
        //                totalwidth += this.Columns[j].Width;

        //            }

        //        }

        //    }

        //}
    }
}
