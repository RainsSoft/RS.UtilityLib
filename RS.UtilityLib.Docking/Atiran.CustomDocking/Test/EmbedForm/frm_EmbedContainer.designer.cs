namespace Test.EmbedForm {
    partial class frm_EmbedContainer {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

     

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dockPanel1 = new Atiran.CustomDocking.Docking.DockPanel();
            this.vS2017LightTheme1 = new Atiran.CustomDocking.Docking.Theme.ThemeVS2017.VS2017LightTheme();
            this.m_MainMenu = new System.Windows.Forms.MenuStrip();
            this.ToolStripMenuItem_扩展工具 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_设备程序 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Web程序 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dockPanel1)).BeginInit();
            this.m_MainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // dockPanel1
            // 
            this.dockPanel1._Theme = this.vS2017LightTheme1;
            this.dockPanel1.BackColor = System.Drawing.SystemColors.Control;
      
            this.dockPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.dockPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dockPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel1.DockBackColor = System.Drawing.SystemColors.Control;
            this.dockPanel1.DocumentStyle = Atiran.CustomDocking.Docking.DocumentStyle.DockingWindow;
            this.dockPanel1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dockPanel1.Location = new System.Drawing.Point(0, 25);
            this.dockPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Padding = new System.Windows.Forms.Padding(6);
            this.dockPanel1.ShowAutoHideContentOnHover = false;
            this.dockPanel1.ShowDocumentIcon = true;
            this.dockPanel1.Size = new System.Drawing.Size(384, 336);
            this.dockPanel1.TabIndex = 1;
            this.dockPanel1.Theme = this.vS2017LightTheme1;
            // 
            // m_MainMenu
            // 
            this.m_MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_扩展工具,
            this.ToolStripMenuItem_设备程序,
            this.ToolStripMenuItem_Web程序});
            this.m_MainMenu.Location = new System.Drawing.Point(0, 0);
            this.m_MainMenu.Name = "m_MainMenu";
            this.m_MainMenu.Size = new System.Drawing.Size(384, 25);
            this.m_MainMenu.TabIndex = 2;
            this.m_MainMenu.Text = "menuStrip1";
            // 
            // ToolStripMenuItem_扩展工具
            // 
            this.ToolStripMenuItem_扩展工具.Name = "ToolStripMenuItem_扩展工具";
            this.ToolStripMenuItem_扩展工具.Size = new System.Drawing.Size(68, 21);
            this.ToolStripMenuItem_扩展工具.Text = "扩展工具";
            // 
            // ToolStripMenuItem_设备程序
            // 
            this.ToolStripMenuItem_设备程序.BackColor = System.Drawing.SystemColors.Control;
            this.ToolStripMenuItem_设备程序.Name = "ToolStripMenuItem_设备程序";
            this.ToolStripMenuItem_设备程序.Size = new System.Drawing.Size(68, 21);
            this.ToolStripMenuItem_设备程序.Text = "设备程序";
            // 
            // ToolStripMenuItem_Web程序
            // 
            this.ToolStripMenuItem_Web程序.Name = "ToolStripMenuItem_Web程序";
            this.ToolStripMenuItem_Web程序.Size = new System.Drawing.Size(71, 21);
            this.ToolStripMenuItem_Web程序.Text = "Web程序";
            // 
            // frm_EmbedContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.m_MainMenu);
            this.MainMenuStrip = this.m_MainMenu;
            this.MinimizeBox = false;
            this.Name = "frm_EmbedContainer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "交互工作控制台";
            this.Load += new System.EventHandler(this.frm_EmbedContainer_Load);
            this.Controls.SetChildIndex(this.m_MainMenu, 0);
            this.Controls.SetChildIndex(this.dockPanel1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.dockPanel1)).EndInit();
            this.m_MainMenu.ResumeLayout(false);
            this.m_MainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private Atiran.CustomDocking.Docking.DockPanel dockPanel1;
        private Atiran.CustomDocking.Docking.Theme.ThemeVS2017.VS2017LightTheme vS2017LightTheme1;
        private System.Windows.Forms.MenuStrip m_MainMenu;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_扩展工具;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_设备程序;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Web程序;
    }
}