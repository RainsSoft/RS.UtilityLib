using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormKeyBoard
{
    public partial class FormTestKeyBoard : Form
    {
        public FormTestKeyBoard() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            if (!keyBoardFormShow) {
                Point p = this.txtPwd.PointToScreen(new Point(0,this.txtPwd.Height));
                var keyBoardFrom = new KeyBoardForm( this.txtPwd,p.X, p.Y);
                keyBoardFrom.Show(this);
                keyBoardFrom.FormClosed += new FormClosedEventHandler(keyBoardFrom_FormClosed);
            }

            keyBoardFormShow = !keyBoardFormShow;
        }
        private bool keyBoardFormShow = false;

        void keyBoardFrom_FormClosed(object sender, FormClosedEventArgs e) {
            keyBoardFormShow = false;
        }
    }
}
