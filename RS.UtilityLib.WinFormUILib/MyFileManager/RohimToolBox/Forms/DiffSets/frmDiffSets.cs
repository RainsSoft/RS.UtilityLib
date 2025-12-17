using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RohimToolBox.Forms.DiffSets
{
    public partial class frmDiffSets : Form
    {
        public frmDiffSets()
        {
            InitializeComponent();


            // show line numbers
            //this.txtLeft.Margins[0].Width = 35;
            //this.txtRight.Margins[0].Width = 35;
        }

        private void btnDiff_Click(object sender, EventArgs e)
        {
            string[] lines1 = this.txtLeft.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            string[] lines2 = this.txtRight.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            string[] lines1ExceptLines2 = lines1.Except(lines2).ToArray();
            string[] lines2ExceptLines1 = lines2.Except(lines1).ToArray();

            this.txtLeft.Text = string.Join(Environment.NewLine, lines1ExceptLines2);
            this.txtRight.Text = string.Join(Environment.NewLine, lines2ExceptLines1);
        }
    }
}
