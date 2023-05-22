using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
    public class ComboBoxEx : ComboBox
    {
        private ImageList imgs = new ImageList();
        private bool _DisableMouseWheel = false;

        // constructor
        public ComboBoxEx() {
            // set draw mode to owner draw
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        // ImageList property
        public ImageList ImageList {
            get {
                return imgs;
            }
            set {
                imgs = value;
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            //base.OnPaint(e);
        }

        // customized drawing process
        protected override void OnDrawItem(DrawItemEventArgs e) {
            // draw background & focus rect
            e.DrawBackground();
            e.DrawFocusRectangle();

            // check if it is an item from the Items collection
            if (e.Index < 0)

                // not an item, draw the text (indented)
                e.Graphics.DrawString(this.Text, e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + imgs.ImageSize.Width, e.Bounds.Top + (e.Bounds.Height - e.Font.Height) / 2);

            else {

                // check if item is an ComboBoxExItem
                if (this.Items[e.Index].GetType() == typeof(ComboBoxExItem)) {
                    // get item to draw
                    ComboBoxExItem item = (ComboBoxExItem)this.Items[e.Index];

                    // get forecolor & font
                    Color forecolor = (item.ForeColor != Color.FromKnownColor(KnownColor.Transparent)) ? item.ForeColor : e.ForeColor;
                    Font font = item.Mark ? new Font(e.Font, FontStyle.Bold) : e.Font;

                    // -1: no image
                    if (item.ImageIndex != -1) {
                        // draw image, then draw text next to it
                        this.ImageList.Draw(e.Graphics, e.Bounds.Left + item.Retract, e.Bounds.Top, item.ImageIndex);
                        e.Graphics.DrawString(item.Text, font, new SolidBrush(forecolor), e.Bounds.Left + imgs.ImageSize.Width + item.Retract, e.Bounds.Top + (e.Bounds.Height - e.Font.Height) / 2);
                    }
                    else
                        // draw text (indented)
                        e.Graphics.DrawString(item.Text, font, new SolidBrush(forecolor), e.Bounds.Left + imgs.ImageSize.Width + item.Retract, e.Bounds.Top + (e.Bounds.Height - e.Font.Height) / 2);

                }
                else

                    // it is not an ComboBoxExItem, draw it
                    e.Graphics.DrawString(this.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left /*+ imgs.ImageSize.Width*/, e.Bounds.Top + (e.Bounds.Height - e.Font.Height) / 2);

            }

            base.OnDrawItem(e);
        }

        [Browsable(true), Description("禁用滚轮"), Category("行为")]
        public bool DisableMouseWheel {
            get {
                return _DisableMouseWheel;
            }
            set {
                _DisableMouseWheel = value;
            }
        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case 0x020A:
                    if (_DisableMouseWheel)
                        return;
                    else
                        break;
                default:
                    break;
            }
            base.WndProc(ref m);
        }
    }

    public class ComboBoxExItem : object
    {
        // forecolor: transparent = inherit
        private Color forecolor = Color.FromKnownColor(KnownColor.Transparent);
        private bool mark = false;
        private int imageindex = -1;
        private object tag = null;
        private string text = null;
        private int _Retract = 0;
        private string _stext;

        // constructors
        public ComboBoxExItem() {
        }

        public ComboBoxExItem(string Text) {
            text = Text;
            _stext = Text;
        }

        public ComboBoxExItem(int retract, string Text) {
            _Retract = retract;
            text = Text;
            _stext = Text;
        }

        public ComboBoxExItem(int retract, string Text, int ImageIndex) {
            _Retract = retract;
            text = Text;
            _stext = Text;
            imageindex = ImageIndex;
        }

        public ComboBoxExItem(int retract, string Text, int ImageIndex, object Tag) {
            _Retract = retract;
            text = Text;
            _stext = Text;
            imageindex = ImageIndex;
            tag = Tag;
        }

        public ComboBoxExItem(int retract, string Text, string stext, int ImageIndex, object Tag) {
            _Retract = retract;
            text = Text;
            _stext = stext;
            imageindex = ImageIndex;
            tag = Tag;
        }

        public ComboBoxExItem(int retract, string Text, int ImageIndex, bool Mark) {
            _Retract = retract;
            text = Text;
            _stext = Text;
            imageindex = ImageIndex;
            mark = Mark;
        }

        public ComboBoxExItem(int retract, string Text, int ImageIndex, bool Mark, Color ForeColor) {
            _Retract = retract;
            text = Text;
            _stext = Text;
            imageindex = ImageIndex;
            mark = Mark;
            forecolor = ForeColor;
        }

        public ComboBoxExItem(int retract, string Text, int ImageIndex, bool Mark, Color ForeColor, object Tag) {
            _Retract = retract;
            text = Text;
            _stext = Text;
            imageindex = ImageIndex;
            mark = Mark;
            forecolor = ForeColor;
            tag = Tag;
        }

        // forecolor
        public Color ForeColor {
            get {
                return forecolor;
            }
            set {
                forecolor = value;
            }
        }

        // image index
        public int ImageIndex {
            get {
                return imageindex;
            }
            set {
                imageindex = value;
            }
        }

        // mark (bold)
        public bool Mark {
            get {
                return mark;
            }
            set {
                mark = value;
            }
        }

        // tag
        public object Tag {
            get {
                return tag;
            }
            set {
                tag = value;
            }
        }

        // item text
        public string Text {
            get {
                return text;
            }
            set {
                text = value;
            }
        }

        public int Retract {
            get {
                return _Retract;
            }
            set {
                _Retract = value;
            }
        }

        public string sText {
            get {
                return _stext;
            }
            set {
                _stext = value;
            }
        }


        // ToString() should return item text
        public override string ToString() {
            return _stext;
        }
    }
}
