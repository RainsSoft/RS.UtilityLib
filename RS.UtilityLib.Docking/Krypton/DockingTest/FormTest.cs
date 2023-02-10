using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DockingTest
{
    public partial class FormTest : Form
    {
        public FormTest() {
            InitializeComponent();
        }

        private void kryptonButton1_Click(object sender, EventArgs e) {
            //https://github.com/ComponentFactory/Krypton/tree/master/Source/Krypton%20Docking%20Examples
            //PaletteDesigner
            var mf = new PaletteDesigner.MainForm();
            mf.Show();
        }

        private void kryptonCheckButton1_Click(object sender, EventArgs e) {
            //https://github.com/ComponentFactory/Krypton
            //DockingCustomized
            var cf = new DockingCustomized.Form1();
            cf.Show();
        }
    }
}
