using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HML;
namespace WindowsFormsApp1
{
    public partial class FormMaskTest : Form
    {
        public FormMaskTest() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            Masking.Show(this);
        }
    }
}
