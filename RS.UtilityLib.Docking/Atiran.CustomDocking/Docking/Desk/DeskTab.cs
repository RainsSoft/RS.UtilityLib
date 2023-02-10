using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using Atiran.CustomDocking.Docking;
using Atiran.CustomDocking.MassageBox;

namespace Atiran.CustomDocking.Docking.Desk
{
    public class DeskTab : DockContent
    {
        public DeskTab()
        {
            InitializeComponent();
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            DockAreas = DockAreas.Document;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miClose = new System.Windows.Forms.ToolStripMenuItem();
            this.miCloseAllButThis = new System.Windows.Forms.ToolStripMenuItem();
            this.miCloseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.miClose,
                this.miCloseAllButThis,
                this.miCloseAll
            });
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(161, 70);
            // 
            // miClose
            // 
            this.miClose.Name = "miClose";
            this.miClose.Size = new System.Drawing.Size(160, 22);
            this.miClose.Text = "بستن";
            this.miClose.Click += new System.EventHandler(this.miClose_Click);
            // 
            // miCloseAllButThis
            // 
            this.miCloseAllButThis.Name = "miCloseAllButThis";
            this.miCloseAllButThis.Size = new System.Drawing.Size(160, 22);
            this.miCloseAllButThis.Text = "بستن  ساير تب ها";
            this.miCloseAllButThis.Click += new System.EventHandler(this.miCloseAllButThis_Click);
            // 
            // miCloseAll
            // 
            this.miCloseAll.Name = "miCloseAll";
            this.miCloseAll.Size = new System.Drawing.Size(160, 22);
            this.miCloseAll.Text = "بستن همه";
            this.miCloseAll.Click += new System.EventHandler(this.miCloseAll_Click);
            // 
            // DeskTab
            // 
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(519, 280);
            this.DockAreas = Atiran.CustomDocking.Docking.DockAreas.Document;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.Name = "DeskTab";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.TabPageContextMenuStrip = this.contextMenuStrip1;
            //this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DeskTab_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public string TextName ;
        public int Kind = 0;
        private string m_fileName = string.Empty;
        private ContextMenuStrip contextMenuStrip1;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem miClose;
        private ToolStripMenuItem miCloseAllButThis;
        private ToolStripMenuItem miCloseAll;
        private bool isCLoseAll = false;
        private bool isCanselCLoseAll = false;
        private List<Form> deskTabs;

        public string FileName
        {
            get { return m_fileName; }
            set
            {
                if (value != string.Empty)
                {
                    Stream s = new FileStream(value, FileMode.Open);

                    FileInfo efInfo = new FileInfo(value);

                    string fext = efInfo.Extension.ToUpper();

                    s.Close();
                }

                m_fileName = value;
                this.ToolTipText = value;
            }
        }

        private bool m_resetText = true;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_resetText)
            {
                m_resetText = false;
                FileName = FileName;
            }
        }

        protected override string GetPersistString()
        {
            // Add extra information into the persist string for this document
            // so that it is available when deserialized.
            return GetType().ToString() + "," + FileName + "," + Text;
        }

        private void miClose_Click(object sender, EventArgs e)
        {
            if (Kind == 1)
            {
                if (ShowPersianMessageBox.ShowMessge("پيغام", "آيا تب " + Text + " بسته شود",
                        MessageBoxButtons.YesNo, false, false) == DialogResult.Yes)
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private void miCloseAllButThis_Click(object sender, EventArgs e)
        {
            deskTabs = ((Form)TopLevelControl).MdiChildren.ToList();
            foreach (DeskTab form in ((Form) TopLevelControl).MdiChildren)
            {
                if (form != this && !isCanselCLoseAll)
                    TryClose(form, deskTabs.Where(f => f != form && f!=this).ToArray());
            }

            isCLoseAll = false;
            isCanselCLoseAll = false;
        }

        private void miCloseAll_Click(object sender, EventArgs e)
        {
            deskTabs = ((Form) TopLevelControl).MdiChildren.ToList();
            foreach (DeskTab form in ((Form) TopLevelControl).MdiChildren)
            {
                if (!isCanselCLoseAll)
                    TryClose(form, deskTabs.Where(f => f != form).ToArray());
            }
            isCLoseAll = false;
            isCanselCLoseAll = false;
        }

        private void TryClose(Atiran.CustomDocking.Docking.Desk.DeskTab form, Form[] forms)
        {
            if (form.Kind == 1)
            {
                if (!isCLoseAll)
                {
                    string TextTabs = form.Text;
                    foreach (Form tab in forms)
                    {
                        TextTabs += "\n" + tab.Text;
                    }
                    var result = ShowPersianMessageBox.ShowMessge("آيا تب ها بسته شوند؟", TextTabs,
                        MessageBoxButtons.YesNo, false);
                    if (result == DialogResult.Yes)
                    {
                        form.Close();
                    }
                    else if (result == DialogResult.OK)
                    {
                        isCLoseAll = true;
                        form.Close();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        isCanselCLoseAll = true;
                    }
                }
                else
                {
                    form.Close();
                }
            }
            else
            {
                form.Close();
            }

            deskTabs.Remove(form);
        }
    }
}
