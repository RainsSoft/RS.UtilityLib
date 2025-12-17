using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon
{
    public partial class FormUIControlTest : Form
    {
        public FormUIControlTest() {
            InitializeComponent();
        
            this.myScrollPanel1.AddControl(this.pingClient1, new Point(4,400));
           
        }
        
        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
                      
        }
        void initToolTipEx() {
            string tipImageBackPath = "Skin\\Res\\roomuserinfo.bmp";//
            Image imgt = new Bitmap(tipImageBackPath);
            Debug.Assert(imgt != null, "不存在Skin\\Res\\tooltip_room.png");
            this.toolTipEx1.Size = imgt.Size;
            this.toolTipEx1.BackImage = imgt;
            this.toolTipEx1.TitleTxtOffset = new Point(50, 4);
            this.toolTipEx1.TipTxtOffset = new Point(100, 4);
            //
            //m_RoomTip.Opacity = 0.8d;
            //m_RoomTip.IsShowWidget = false;

            UI.ToolTipEx.ToolTipWidget user_Widgetphoto = new  UI.ToolTipEx.ToolTipWidget(16, 66, 64, 64);
            user_Widgetphoto.Name = "User_Tip_Photo";
            user_Widgetphoto.Image = new Bitmap(32, 32);
            this.toolTipEx1.AddWidget(user_Widgetphoto);

            UI.ToolTipEx.ToolTipWidget user_WidgetUrl = new  UI.ToolTipEx.ToolTipWidget(120, 152, 80, 16);
            user_WidgetUrl.Text = "用户空间";
            user_WidgetUrl.TextLinkUrl = "用户空间网页";
            user_WidgetUrl.TextColor = System.Drawing.Color.FromArgb(255, 0, 0, 255);
            this.toolTipEx1.AddWidget(user_WidgetUrl);

            UI.ToolTipEx.ToolTipWidget user_Widgetrongyu = new  UI.ToolTipEx.ToolTipWidget(200, 152, 80, 16);
            user_Widgetrongyu.Text = "用户荣誉";
            user_Widgetrongyu.TextLinkUrl = "用户荣誉网页";
            user_Widgetrongyu.TextColor = System.Drawing.Color.FromArgb(255, 0, 0, 255);
            this.toolTipEx1.AddWidget(user_Widgetrongyu);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            if (this.toolTipEx1.ActionMouseDown()) return;
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            if (this.toolTipEx1.ActionMouseUp()) return;
            base.OnMouseUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            if (this.toolTipEx1.ActionMouseMove()) return;
            base.OnMouseMove(e);
        }

        private void FormUIControlTest_Load(object sender, EventArgs e) {

        }

        private void button1_Click(object sender, EventArgs e) {
            for (int i = 0; i < 100; i++) {
                Button btn = new Button();
                btn.Size = new Size(100,48);
                btn.Text = "按钮："+i.ToString();
                btn.Location = new Point(10, 10 + i * 48);
                this.myScrollPanel1.AddControl(btn,false);
            }
            this.myScrollPanel1.ReCaculScrollPosAndSize();
        }
    }
}
