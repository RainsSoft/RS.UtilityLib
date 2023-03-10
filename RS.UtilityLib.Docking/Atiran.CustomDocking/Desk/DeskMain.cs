using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Atiran.CustomDocking.Docking;


namespace Atiran.CustomDocking.Desk
{
    /// <summary>
    /// DockPanel容器主窗口,封装了一些实用的方法
    /// </summary>
    public partial class DeskMain : Form
    {
        public DeskMain() {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;
            vsToolStripExtender1.DefaultRenderer = _toolStripProfessionalRenderer;
        }
        #region  override
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(
          int hwnd,
          int hWndInsertAfter,
          int x,
          int y,
          int cx,
          int cy,
         int wFlags
  );
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd); //WINAPI 设置当前活动窗体的句柄
        public static void SetTop(IntPtr formHandle, bool top) {
            int hwnd = formHandle.ToInt32();
            if (top) {
                SetWindowPos(hwnd, -1, 0, 0, 0, 0, 0x001 | 0x002 | 0x040);
            }
            else {
                SetWindowPos(hwnd, -2, 0, 0, 0, 0, 0x001 | 0x002 | 0x040);
            }
        }
        protected override void OnVisibleChanged(EventArgs e) {
            base.OnVisibleChanged(e);
            CustomOnVisibleChanged(e);
        }
        protected virtual void CustomOnVisibleChanged(EventArgs e) {
            if (Visible) {
                //SetTop(this.Handle, true);
                ////this.Focus();
                //this.Activate();
                //SetTop(this.Handle, false);
                //SetForegroundWindow(this.Handle);
            }
        }
        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            CustomOnShown(e);
        }
        protected virtual void CustomOnShown(EventArgs e) {
            //SetTop(this.Handle, true);
            ////CaptionArea.Select();
            //this.KeyPreview = true;
            //SetTop(this.Handle, false);
            //this.Refresh();
        }
        #endregion
        /// <summary>
        /// 继承窗口需要手动设置该值
        /// </summary>
        public virtual DockPanel dockPanel {
            get;
        }
        /// <summary>
        /// 继承窗口需要手动设置该值,继承DeskDoc
        /// </summary>
        public virtual Type DeskDocFrm {
            get {
                return typeof(DeskDoc);
            }
        }
        /// <summary>
        /// 状态栏渲染器
        /// </summary>
        protected readonly ToolStripRenderer _toolStripProfessionalRenderer = new ToolStripProfessionalRenderer();
        /// <summary>
        /// 根据dock窗口标题查找
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IDockContent FindDocument(string text) {
            if (dockPanel == null)
                return null;
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi) {
                foreach (Form form in MdiChildren)
                    if (form.Text == text)
                        return form as IDockContent;

                return null;
            }
            else {
                foreach (IDockContent content in dockPanel.Documents)
                    if (content.DockHandler.TabText == text)
                        return content;

                return FindDeskDocInSelfCreated(text);
            }
        }
        /// <summary>
        /// 通过ShowNewDocument(out DeskDoc dummyDoc, string titleName = "")创建的所有窗口中查找
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public DeskDoc FindDeskDocInSelfCreated(string text) {
            for (int i = 0; i < m_AllDeskDocSelfCreated.Count; i++) {
                if (m_AllDeskDocSelfCreated[i].Text == text) {
                    return m_AllDeskDocSelfCreated[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 调用 ShowNewDocument(out DeskDoc dummyDoc, string titleName = "")创建的所有窗口
        /// </summary>
        protected List<DeskDoc> m_AllDeskDocSelfCreated = new List<DeskDoc>();
        /// <summary>
        /// 显示新doc
        /// </summary>
        public void ShowNewDocument(out DeskDoc dummyDoc, string titleName = "") {
            if (dockPanel == null) {
                dummyDoc = null;
                return;
            }
            dummyDoc = string.IsNullOrEmpty(titleName) ? CreateNewDocument() : CreateNewDocument(titleName);
            //
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi) {
                dummyDoc.MdiParent = this;
                dummyDoc.Show();
            }
            else
                dummyDoc.Show(dockPanel);
            //
            //try {
            //    dummyDoc.FileName = titleName;
            //}
            //catch(Exception exception) {
            //    dummyDoc.Close();
            //    System.Windows.Forms.MessageBox.Show(exception.Message);
            //}
        }
        /// <summary>
        /// 创建一个新的Document类型dock窗口,并保证窗口标题名称不重复
        /// </summary>
        /// <returns></returns>
        protected DeskDoc CreateNewDocument() {
            DeskDoc dummyDoc = null;
            if (this.DeskDocFrm != null) {
                dummyDoc = (DeskDoc)System.Activator.CreateInstance(this.DeskDocFrm);
            }
            else {
                dummyDoc = new DeskDoc();
            }
            int count = 1;
            string text = $"Document{count}";
            while (FindDocument(text) != null) {
                count++;
                text = $"Document{count}";
            }
            dummyDoc.Text = text;
            m_AllDeskDocSelfCreated.Add(dummyDoc);
            dummyDoc.FormClosed += DummyDoc_FormClosed;
            return dummyDoc;
        }
        /// <summary>
        /// 创建一个新的Document类型dock窗口,指定窗口标题名称
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected DeskDoc CreateNewDocument(string text) {
            DeskDoc dummyDoc = null;
            if (this.DeskDocFrm != null) {
                dummyDoc = (DeskDoc)System.Activator.CreateInstance(this.DeskDocFrm);
            }
            else {
                dummyDoc = new DeskDoc();
            }
            dummyDoc.Text = text;
            m_AllDeskDocSelfCreated.Add(dummyDoc);
            dummyDoc.FormClosed += DummyDoc_FormClosed;
            return dummyDoc;
        }

        private void DummyDoc_FormClosed(object sender, FormClosedEventArgs e) {
            DeskDoc doc = sender as DeskDoc;
            for (int i = m_AllDeskDocSelfCreated.Count - 1; i >= 0; i--) {
                if (m_AllDeskDocSelfCreated[i] == doc) {
                    m_AllDeskDocSelfCreated.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 关闭指定Document类型的所有dock窗口
        /// </summary>
        public virtual void CloseAllDocuments() {
            if (dockPanel == null)
                return;
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi) {
                foreach (Form form in MdiChildren)
                    form.Close();
            }
            else {
                foreach (IDockContent document in dockPanel.DocumentsToArray()) {
                    // IMPORANT: dispose all panes.
                    document.DockHandler.DockPanel = null;
                    document.DockHandler.Close();
                }
            }
        }
        /// <summary>
        /// 关闭所有类型的dock窗口
        /// </summary>
        public virtual void CloseAllContents() {
            if (dockPanel == null)
                return;
            // we don't want to create another instance of tool window, set DockPanel to null
            //m_solutionExplorer.DockPanel = null;
            //m_propertyWindow.DockPanel = null;
            //m_toolbox.DockPanel = null;
            //m_outputWindow.DockPanel = null;
            //m_taskList.DockPanel = null;

            // Close all other document windows
            CloseAllDocuments();

            // IMPORTANT: dispose all float windows.
            foreach (var window in dockPanel.FloatWindows.ToList())
                window.Dispose();

            System.Diagnostics.Debug.Assert(dockPanel.Panes.Count == 0);
            System.Diagnostics.Debug.Assert(dockPanel.Contents.Count == 0);
            System.Diagnostics.Debug.Assert(dockPanel.FloatWindows.Count == 0);
        }

        /*
        private void SetSchema(object sender, System.EventArgs e) {
            // Persist settings when rebuilding UI
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.temp.config");

            dockPanel.SaveAsXml(configFile);
            CloseAllContents();

            if(sender == this.menuItemSchemaVS2005) {
                this.dockPanel.Theme = this.vS2005Theme1;
                this.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2005, vS2005Theme1);
            }
            else if(sender == this.menuItemSchemaVS2003) {
                this.dockPanel.Theme = this.vS2003Theme1;
                this.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2003, vS2003Theme1);
            }
            else if(sender == this.menuItemSchemaVS2012Light) {
                this.dockPanel.Theme = this.vS2012LightTheme1;
                this.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2012, vS2012LightTheme1);
            }
            else if(sender == this.menuItemSchemaVS2012Blue) {
                this.dockPanel.Theme = this.vS2012BlueTheme1;
                this.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2012, vS2012BlueTheme1);
            }
            else if(sender == this.menuItemSchemaVS2012Dark) {
                this.dockPanel.Theme = this.vS2012DarkTheme1;
                this.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2012, vS2012DarkTheme1);
            }
            else if(sender == this.menuItemSchemaVS2013Blue) {
                this.dockPanel.Theme = this.vS2013BlueTheme1;
                this.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2013, vS2013BlueTheme1);
            }
            else if(sender == this.menuItemSchemaVS2013Light) {
                this.dockPanel.Theme = this.vS2013LightTheme1;
                this.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2013, vS2013LightTheme1);
            }
            else if(sender == this.menuItemSchemaVS2013Dark) {
                this.dockPanel.Theme = this.vS2013DarkTheme1;
                this.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2013, vS2013DarkTheme1);
            }
            else if(sender == this.menuItemSchemaVS2015Blue) {
                this.dockPanel.Theme = this.vS2015BlueTheme1;
                this.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2015, vS2015BlueTheme1);
            }
            else if(sender == this.menuItemSchemaVS2015Light) {
                this.dockPanel.Theme = this.vS2015LightTheme1;
                this.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2015, vS2015LightTheme1);
            }
            else if(sender == this.menuItemSchemaVS2015Dark) {
                this.dockPanel.Theme = this.vS2015DarkTheme1;
                this.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2015, vS2015DarkTheme1);
            }

            menuItemSchemaVS2005.Checked = (sender == menuItemSchemaVS2005);
            menuItemSchemaVS2003.Checked = (sender == menuItemSchemaVS2003);
            menuItemSchemaVS2012Light.Checked = (sender == menuItemSchemaVS2012Light);
            menuItemSchemaVS2012Blue.Checked = (sender == menuItemSchemaVS2012Blue);
            menuItemSchemaVS2012Dark.Checked = (sender == menuItemSchemaVS2012Dark);
            menuItemSchemaVS2013Light.Checked = (sender == menuItemSchemaVS2013Light);
            menuItemSchemaVS2013Blue.Checked = (sender == menuItemSchemaVS2013Blue);
            menuItemSchemaVS2013Dark.Checked = (sender == menuItemSchemaVS2013Dark);
            menuItemSchemaVS2015Light.Checked = (sender == menuItemSchemaVS2015Light);
            menuItemSchemaVS2015Blue.Checked = (sender == menuItemSchemaVS2015Blue);
            menuItemSchemaVS2015Dark.Checked = (sender == menuItemSchemaVS2015Dark);
            if(dockPanel.Theme.ColorPalette != null) {
                statusBar.BackColor = dockPanel.Theme.ColorPalette.MainWindowStatusBarDefault.Background;
            }

            if(File.Exists(configFile))
                dockPanel.LoadFromXml(configFile, m_deserializeDockContent);
        }

        private void EnableVSRenderer(VisualStudioToolStripExtender.VsVersion version, ThemeBase theme) {
            vsToolStripExtender1.SetStyle(mainMenu, version, theme);
            vsToolStripExtender1.SetStyle(toolBar, version, theme);
            vsToolStripExtender1.SetStyle(statusBar, version, theme);
        }

        */
        /// <summary>
        /// 指定当前窗口的渲染主题风格
        /// </summary>
        /// <param name="vsVersion"></param>
        /// <param name="theme"></param>
        protected virtual void SetTheme(VisualStudioToolStripExtender.VsVersion vsVersion, ThemeBase theme) {
            if (dockPanel == null)
                return;
            this.dockPanel.Theme = theme;
            this.EnableVSRenderer(vsVersion, theme);
            if (dockPanel.Theme.ColorPalette != null) {
                //
                //var c = this.Controls.Count;
                //for(int i = 0; i < c; i++) {
                //    var ctl = this.Controls[i];
                //    if(ctl is System.Windows.Forms.StatusStrip) {
                //        ctl.BackColor = dockPanel.Theme.ColorPalette.MainWindowStatusBarDefault.Background;
                //    }
                //}
            }
        }
        /// <summary>
        /// 根据theme修改ToolStrip的渲染Style
        /// </summary>
        /// <param name="version"></param>
        /// <param name="theme"></param>
        protected virtual void EnableVSRenderer(VisualStudioToolStripExtender.VsVersion version, ThemeBase theme) {
            var c = this.Controls.Count;
            for (int i = 0; i < c; i++) {
                var ctl = this.Controls[i];
                if (ctl is System.Windows.Forms.ToolStrip) {
                    vsToolStripExtender1.SetStyle(ctl as System.Windows.Forms.ToolStrip, version, theme);
                }
            }
        }


        /// <summary>
        /// 设置dockStyle
        /// </summary>
        /// <param name="newDocumentStyle"></param>
        protected virtual void SetDocumentStyle(DocumentStyle newDocumentStyle) {
            if (dockPanel == null)
                return;
            DocumentStyle oldStyle = dockPanel.DocumentStyle;
            DocumentStyle newStyle = newDocumentStyle;
            //
            if (oldStyle == newStyle)
                return;

            if (oldStyle == DocumentStyle.SystemMdi || newStyle == DocumentStyle.SystemMdi)
                CloseAllDocuments();

            dockPanel.DocumentStyle = newStyle;
            //menuItemDockingMdi.Checked = (newStyle == DocumentStyle.DockingMdi);
            //menuItemDockingWindow.Checked = (newStyle == DocumentStyle.DockingWindow);
            //menuItemDockingSdi.Checked = (newStyle == DocumentStyle.DockingSdi);
            //menuItemSystemMdi.Checked = (newStyle == DocumentStyle.SystemMdi);
            //menuItemLayoutByCode.Enabled = (newStyle != DocumentStyle.SystemMdi);
            //menuItemLayoutByXml.Enabled = (newStyle != DocumentStyle.SystemMdi);
            //toolBarButtonLayoutByCode.Enabled = (newStyle != DocumentStyle.SystemMdi);
            //toolBarButtonLayoutByXml.Enabled = (newStyle != DocumentStyle.SystemMdi);
        }
        /// <summary>
        /// 除选择项外关闭所有
        /// </summary>
        public virtual void CloseAllButThisOne() {
            if (dockPanel == null)
                return;
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi) {
                Form activeMdi = ActiveMdiChild;
                foreach (Form form in MdiChildren) {
                    if (form != activeMdi) {
                        form.Close();
                    }
                }
            }
            else {
                foreach (IDockContent document in dockPanel.DocumentsToArray()) {
                    if (!document.DockHandler.IsActivated) {
                        document.DockHandler.DockPanel = null;
                        //
                        document.DockHandler.Close();
                    }
                }
            }
        }
        public virtual void CloseDocument(string text) {
            if (dockPanel == null)
                return;
            var document = FindDocument(text);
            if (document != null) {
                if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi) {
                    (document as Form).Close();
                }
                else {
                    document.DockHandler.DockPanel = null;
                    document.DockHandler.Close();
                }
            }
        }
        /// <summary>
        /// 创建布局 dock窗口
        /// </summary>
        protected virtual void CreateStandardControls() {
            //m_solutionExplorer = new DummySolutionExplorer();
            //m_propertyWindow = new DummyPropertyWindow();
            //m_toolbox = new DummyToolbox();
            //m_outputWindow = new DummyOutputWindow();
            //m_taskList = new DummyTaskList();
        }
        /// <summary>
        /// 重新构建布局dock窗口
        /// </summary>
        public virtual void ReBuildDockLayout() {
            if (dockPanel == null)
                return;
            dockPanel.SuspendLayout(true);

            CloseAllContents();

            CreateStandardControls();

            //m_solutionExplorer.Show(dockPanel, DockState.DockRight);
            //m_propertyWindow.Show(m_solutionExplorer.Pane, m_solutionExplorer);
            //m_toolbox.Show(dockPanel, new Rectangle(98, 133, 200, 383));
            //m_outputWindow.Show(m_solutionExplorer.Pane, DockAlignment.Bottom, 0.35);
            //m_taskList.Show(m_toolbox.Pane, DockAlignment.Left, 0.4);

            //DummyDoc doc1 = CreateNewDocument("Document1");
            //DummyDoc doc2 = CreateNewDocument("Document2");
            //DummyDoc doc3 = CreateNewDocument("Document3");
            //DummyDoc doc4 = CreateNewDocument("Document4");
            //doc1.Show(dockPanel, DockState.Document);
            //doc2.Show(doc1.Pane, null);
            //doc3.Show(doc1.Pane, DockAlignment.Bottom, 0.5);
            //doc4.Show(doc3.Pane, DockAlignment.Right, 0.5);

            dockPanel.ResumeLayout(true, true);
        }
    }
}
