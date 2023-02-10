using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atiran.CustomDocking.Docking;
using Atiran.CustomDocking.Docking.Desk;
using Atiran.CustomDocking.Docking.Theme.ThemeVS2012;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int ali = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            var control = new UserControl();
            control.Dock = DockStyle.Fill;
            DeskTab sh = new DeskTab();
            sh.Text = "Alireza" + ali++;
            sh.Controls.Add(control);
            if (dockPanel1.DocumentStyle == DocumentStyle.SystemMdi)
            {
                sh.MdiParent = this;
                sh.Show();
            }
            else
                sh.Show(dockPanel1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dockPanel1.ActiveDocumentPane?.TabStripControl.StripMenuShowDropDown();
        }
    }
}
