using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Silver.UI
{
    public partial class TestToolBox : Form
    {
        public TestToolBox() {
            InitializeComponent();
  
            this.Controls.Add(this.m_ToolBox);
            this.Size = new Size(220, 400);
            m_ToolBox.Dock = DockStyle.Fill;
            m_ToolBox.BackgroundImage = RS.UtilityLib.WinFormCommon.Properties.Resources.listback3;
            //m_ToolBox.TabSpacing
            m_ToolBox.SetSmallImageList(RS.UtilityLib.WinFormCommon.Properties.Resources.nofind, new Size(48, 48), Color.Transparent);
            m_ToolBox.SmallItemSize = new Size(48, 48);
            //m_ToolBox.SmallImageList.Images[1].Save("t1.png");
            m_ToolBox.SetLargeImageList(RS.UtilityLib.WinFormCommon.Properties.Resources.nofind, new Size(64, 64), Color.Transparent);
            m_ToolBox.ItemSpacing = 8;
        }
        ToolBox m_ToolBox = new ToolBox();
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            ToolBoxTab tbtable = new ToolBoxTab("最近", 0);
            tbtable.AllowDrag = false;
            //tbtable.CaptionChecked = true;
            //tbtable.ForceCaptionToolTip = true;
            tbtable.ShowOnlyOneItemPerRow = false;
            tbtable.View = ViewMode.LargeIcons;
            this.m_ToolBox.AddTab(tbtable);
            // 
            ToolBoxTab tbtable2 = new ToolBoxTab("更早", 0);
            tbtable2.AllowDrag = false;
            //tbtable2.CaptionChecked = true;
            //tbtable2.ForceCaptionToolTip = true;
            tbtable2.ShowOnlyOneItemPerRow = false;
            tbtable2.View = ViewMode.SmallIcons;
            tbtable2.IsShowHoverItemLarge = true;
            this.m_ToolBox.AddTab(tbtable2);
            //
            for (int i = 0; i < 100; i++) {
                ToolBoxItem item = new ToolBoxItem(i.ToString(), 1, false);
                item.LargeImageIndex = 1;
                tbtable.AddItem(item);
                tbtable2.AddItem(i.ToString(), 1, 1, false, null);
            }
        }
    }
}
