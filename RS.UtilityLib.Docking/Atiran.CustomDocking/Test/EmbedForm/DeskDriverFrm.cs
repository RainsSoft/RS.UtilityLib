using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Atiran.CustomDocking.Desk;
using Atiran.CustomDocking.Docking;
using System.Runtime.InteropServices;

namespace Test.EmbedForm {
    public class DeskDriverFrm : DeskDoc {
        public Panel panelContainer;

        public DeskDriverFrm() : base() {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;
            DockAreas = DockAreas.Document;//| DockAreas.Float;
            this.CloseButton = true;
            this.CloseButtonVisible = true;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
      
        }
      

        /// <summary>
        /// 关联进程ID
        /// </summary>
        public int ProcessID {
            get;
            set;
        }

        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hwnd, uint Msg, uint wParam, uint lParam);
        /// <summary>
        /// 当前窗口销毁时 销毁关联进程
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleDestroyed(EventArgs e) {
            //当前窗口销毁时 销毁关联进程    
            if(this.ProcessID != 0) {
                try {
                    var p = System.Diagnostics.Process.GetProcessById(this.ProcessID);
                    IntPtr appWinPH = p.MainWindowHandle;
                    // Post a colse message
                    uint WM_CLOSE = 16;//0x10
                    PostMessage(appWinPH, WM_CLOSE, 0, 0);
                    // Delay for it to get the message
                    System.Threading.Thread.Sleep(50);
                    // Clear internal handle                   
                }
                catch(Exception ee) {
                    Console.WriteLine(ee.ToString());
                }
            }
            //            
            try {
                if(this.ProcessID != 0) {
                    var p = System.Diagnostics.Process.GetProcessById(this.ProcessID);
                    p.Kill();
                }
            }
            catch {

            }     
            base.OnHandleDestroyed(e);
        }

        private void InitializeComponent() {
            this.panelContainer = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelContainer
            // 
            this.panelContainer.Location = new System.Drawing.Point(2, 2);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(600, 600);
            this.panelContainer.TabIndex = 0;
            // 
            // DeskDriverFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(484, 461);
            this.Controls.Add(this.panelContainer);
            this.DockAreas = Atiran.CustomDocking.Docking.DockAreas.Document;
            this.Name = "DeskDriverFrm";
            this.ResumeLayout(false);

        }
    }
}
