using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
    public partial class ctl_ListBoxExItem : UserControl {
        public static Icon IRobotQ_Icon;
        public object ItemObj {
            get;
            set;
        }
        public ctl_ListBoxExItem() {
            InitializeComponent();
            this.DoubleBuffered = true;

            this.button1.Click += new EventHandler(button1_Click);
        }
        public event EventHandler OnDelButtonClick;
        public event EventHandler OnItemClick;
        void button1_Click(object sender, EventArgs e) {
            if (OnDelButtonClick != null) {
                OnDelButtonClick(this, null);
            }

        }
        public override bool Focused {
            get {
                return base.Focused || this.button1.Focused;
            }
        }
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (m_MouseState == MouseState.Hover) {
                this.BackColor = Color.FromArgb(107, 178, 43);
            }
            else {
                this.BackColor = Color.White;
            }
            if (ItemObj != null) {
                using (Font font = new Font("宋体", 9, FontStyle.Regular)) {
                    string s = this.ItemObj.ToString();
                    var sizef = e.Graphics.MeasureString(s, font);
                    if (IRobotQ_Icon != null) {
                        e.Graphics.DrawIcon(IRobotQ_Icon, new Rectangle(1, 1, this.Height - 2, this.Height - 2));

                    }
                    else {
                        e.Graphics.DrawString(s, font, SystemBrushes.ControlText, 0, (this.Height - sizef.Height) * 0.5f);

                    }
                }
            }
        }
        public bool HasDestroyed {
            get;
            private set;
        }
        protected override void OnHandleDestroyed(EventArgs e) {
            HasDestroyed = true;           
            base.OnHandleDestroyed(e);
        }
        //protected override void OnMouseHover(EventArgs e) {
        //    base.OnMouseHover(e);
        //    m_MouseState = MouseState.Hover;
        //    this.Refresh();
        //}
        //protected override void OnMouseLeave(EventArgs e) {
        //    base.OnMouseLeave(e);
        //    m_MouseState = MouseState.None;
        //    this.Refresh();
        //}
        public MouseState m_MouseState;
        public enum MouseState {
            None = 0,
            Hover = 1,

        }
    }
}
