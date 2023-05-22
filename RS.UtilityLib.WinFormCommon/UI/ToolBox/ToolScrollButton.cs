//----------------------------------------------------------------------------
// File    : ToolObject.cs
// Date    : 9/09/2011
// Author  :  
// Email   : andyhebear@163.com
// 
// Updates : 增加对图片的支持
//           See ToolBox.cs
//
// Legal   : See ToolBox.cs
//----------------------------------------------------------------------------

using System;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Silver.UI {
    /// <summary>
    /// 支持绘制图片
    /// </summary>
    [Serializable]
    public class ToolScrollButton : ToolObject {
        #region Private Attributes

        private bool _mouseDown;
        private bool _mouseHover;
        private bool _enabled;
        private ScrollDirection _direction;

        #endregion //Private Attributes

        #region Properties

        [Browsable(false), XmlIgnore]
        public bool MouseDown {
            get { return _mouseDown; }
            set { _mouseDown = value; }
        }

        [Browsable(false), XmlIgnore]
        public bool MouseHover {
            get { return _mouseHover; }
            set { _mouseHover = value; }
        }

        [Category("General")]
        public bool Enabled {
            get { return _enabled; }
            set {
                _enabled = value;
            }
        }

        [Category("General")]
        public ScrollDirection ScrollDirection {
            get { return _direction; }
            set { _direction = value; }
        }
        /// <summary>
        /// 箭头多态图
        /// </summary>
        private Image _arrowBtnImg = null;
        /// <summary>
        /// 图片的销毁在外面做 需要注意销毁后设置为null
        /// 实在箭头图片 多态图 12张
        /// 上一排6张为上下多态图
        /// 下一排6张为左右多态图  
        /// 取图区为每一派除6得到
        /// </summary>
        /// <param name="img"></param>
        internal Image ArrowBtnImg {
            get { return _arrowBtnImg; }
            set {
                _arrowBtnImg = value;
                Center_OverSideRelRect = new Rectangle(0, 0, _arrowBtnImg.Width / 6, _arrowBtnImg.Height / 6);
            }
        }
        #endregion //Properties

        #region Construction
        public ToolScrollButton(ScrollDirection direction, int width, int height) {
            _rectangle = new Rectangle(0, 0, width, height);
            _direction = direction;
            _toolTip = (ScrollDirection.Up == direction) ? "Scroll Up" : "Scroll Down";
            if (direction == ScrollDirection.Center)
                _toolTip = "Scroll Center";
        }
        #endregion //Construction
        /// <summary>
        /// 相对的两头
        /// </summary>
        private Rectangle Center_OverSideRelRect;
        /// <summary>
        /// 最小尺寸大于或等于Center_OverSideRelRect的3倍
        /// </summary>
        internal Rectangle Center_MinRect;

        /// <summary>
        /// 外面绘制中心的背景图
        /// this.ScrollDirection == ScrollDirection.Center情况下有效
        /// </summary>
        /// <param name="g"></param>
        /// <param name="centerRectBack"></param>
        internal void DrawCenterBackRect(Graphics g,Rectangle centerRectBack) {
            if (this.ScrollDirection == ScrollDirection.Center) { 
            
            }
        }
        private void DrawCenterBtn(Graphics g, Image arrowBtnImg, ScrollDirection _direction, Rectangle rect) { 
        }
        #region Public Methods

        public void Paint(Graphics g, Rectangle clipRect, bool ctrlEnabled) {
            Rectangle rect = Rectangle.Empty;
            Pen pen = null;
            int length = 0;
            Point p1;
            Point p2;

            if (_rectangle.IntersectsWith(clipRect)) {
                pen = (!_enabled || !ctrlEnabled) ? SystemPens.GrayText : Pens.Black;
                rect = _rectangle;

                if (this.ArrowBtnImg != null) {
                    DrawArrow(g,ArrowBtnImg,_direction, rect);
                    ToolBox.DrawBorders(g, rect, _mouseDown);
//#if DEBUG
//                    g.DrawRectangle(System.Drawing.Pens.Red, rect);
//#endif
                    return;
                }
                ToolBox.DrawBorders(g, rect, _mouseDown);
//#if DEBUG
//                g.DrawRectangle(System.Drawing.Pens.Red, rect);
//#endif
                rect.Inflate(-rect.Width / 3, -rect.Height / 3);

                if (0 != rect.Width % 2) {
                    rect.Width--;
                }

                if (0 != rect.Height % 2) {
                    rect.Height--;
                }

                rect.Width = Math.Max(8, rect.Width);
                rect.Height = Math.Max(8, rect.Height);

                rect.X -= rect.Width / 4;
                //rect.Y -= rect.Height/4;

                if (_mouseDown) {
                    rect.Offset(1, 1);
                }

                //g.DrawRectangle(Pens.Red,rect);

                //rect.X = _rectangle.X + (_rectangle.Width - rect.Width)/2;
                length = (int)rect.Width;
                p1 = rect.Location;
                p2 = rect.Location;

                if (ScrollDirection.Down == _direction) {
                    p2.X = rect.Right;

                    while (0 <= length) {
                        g.DrawLine(pen, p1, p2);
                        p1.X++; p2.X--;
                        p1.Y++; p2.Y++;
                        length -= 2;
                    }

                    p1.X = rect.Left + rect.Width / 2;
                    p1.Y = rect.Top;
                    p2.X = p1.X;
                    p2.Y = rect.Top + rect.Height / 2;

                    g.DrawLine(pen, p1, p2);
                    //                    
                }
                else if (ScrollDirection.Up == _direction) {
                    p1.X = rect.Left + rect.Width / 2;
                    p1.Y = rect.Top;
                    p2.X = p1.X;
                    p2.Y = p1.Y;

                    while (0 <= length) {
                        g.DrawLine(pen, p1, p2);
                        p1.X--; p2.X++;
                        p1.Y++; p2.Y++;
                        length -= 2;
                    }

                    p1.X = rect.Left + rect.Width / 2;
                    p1.Y = rect.Bottom - rect.Height / 2;
                    p2.X = p1.X;
                    p2.Y = rect.Top;

                    g.DrawLine(pen, p1, p2);
                    //

                }

            }
        }

        private void DrawArrow(Graphics g, Image arrowBtnImg, ScrollDirection _direction, Rectangle rect) {
            Rectangle arrowrec = new Rectangle(0, 0, _arrowBtnImg.Width, _arrowBtnImg.Height);
            //
            int offsetx = 0;
            int offsety = 0;
            if (_direction == ScrollDirection.Up) {
                offsetx = 0;
                if (_mouseDown) {
                    offsety = _arrowBtnImg.Width / 6 * 2;
                }
                else if (_mouseHover) {
                    offsety = _arrowBtnImg.Width / 6 * 1;
                }
                else {
                    offsety = 0;
                }
            }
            else if(_direction == ScrollDirection.Down) {
                offsetx = _arrowBtnImg.Width /6;
                if (_mouseDown) {
                    offsety = _arrowBtnImg.Width / 6 * 2;
                }
                else if (_mouseHover) {
                    offsety = _arrowBtnImg.Width / 6 * 1;
                }
                else {
                    offsety = 0;
                }
            } 
            else if(_direction == ScrollDirection.Left) { 
                
            } 
            else if(_direction == ScrollDirection.Left) { 
                
            }
            g.DrawImage(arrowBtnImg, rect, new Rectangle(offsetx, offsety, arrowrec.Width / 6, arrowrec.Height / 6), GraphicsUnit.Pixel);

        }

      

        public bool HitTest(int x, int y) {
            return _rectangle.Contains(x, y);
        }

        #endregion //Public Methods
    }
}

//----------------------------------------------------------------------------