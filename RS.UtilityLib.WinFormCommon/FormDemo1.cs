using RS.UtilityLib.WinFormCommon.IME;
using RS.UtilityLib.WinFormCommon.RibbonUI;
using RS.UtilityLib.WinFormCommon.UI.MyScrollBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon
{
    public partial class FormDemo1 : Form
    {
        public FormDemo1() {
            InitializeComponent();

        }
        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            var frm1 = new Form1();
            frm1.Show();
            UINotifier.UIFollowFormHelper uf = new UINotifier.UIFollowFormHelper(this, frm1,10,0, UINotifier.UIFollowMode.FixedRight);
           
        }
    

        private void Form1_Load(object sender, EventArgs e) {
            this.lV_ModelList.Hint = new RibbonHintWindowMemo();
            this.lV_ModelList.Hint.ShowTipEvent += Hint_ShowTipEvent;
            this.lV_ModelList.BeginUpdate();
            for (int i = 0; i < 32; i++) {
                RibbonListViewItem li = new RibbonListViewItem();
                li.Text = "test_" + i.ToString();
                li.SmallImage = new Bitmap(RS.UtilityLib.WinFormCommon.Properties.Resources.nofind, new Size(48, 48)); //防止ListView在清除Items的时候把image给释放了.
                //li.Enabled = modelinfo.Enable;
                //li.Tag = modelinfo;
                this.lV_ModelList.AddItem(li);
            }
            this.lV_ModelList.EndUpdate();
        }

        private void Hint_ShowTipEvent() {
            if (this.lV_ModelList.HoverItem == null) {
                return;
            }
            var mi = this.lV_ModelList.HoverItem;
            if (mi != null) {
                //目前只显示名称和说明,其中,说明在Hint内部做了额外的处理
                (this.lV_ModelList.Hint as RibbonHintWindowMemo).SetHint(mi);
            }

        }
        private void button1_Click(object sender, EventArgs e) {
            ShineControlHelper.Show(this.degreePiePicture1, 3);
        }

        private void button2_Click(object sender, EventArgs e) {
            IMEForm imeForm = new IMEForm();
            imeForm.ShowWithMask(this, this.ckb_mask.Checked);
            //
            CustomScrollBarsSample cs = new CustomScrollBarsSample();           
            cs.ShowDialogWithMask(this,this.ckb_mask.Checked); 
            //
            this.ShowMsgDialogWithMask((maskOwner) => {
                var ret = MessageBox.Show(maskOwner,"测试showdialogwithmask");
                return ret;
            },this.ckb_mask.Checked);
        }
        private void button3_Click(object sender, EventArgs e) {
            UI.FormShadow fs = new UI.FormShadow();
            fs.Show();
        }

        private void button4_Click(object sender, EventArgs e) {
            //this.ShowControlMaskByContextMenu("正在加载...", null, 0, 0, this.Width, this.Height);
            //MessageBox.Show("test");
            //button3_Click(null,null);            
           
        }
    }
}
