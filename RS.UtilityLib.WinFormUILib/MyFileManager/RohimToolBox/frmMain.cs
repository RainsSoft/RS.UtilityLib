using RohimToolBox.Forms.CheckDuplicates;
using RohimToolBox.Forms.DiffSets;
using RohimToolBox.Forms.TWSBatHelper;
using System.Windows.Forms;

namespace RohimToolBox
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            this.LoadForm(new frmFileExplorer());
        }

        private void btnDiffSets_Click(object sender, System.EventArgs e)
        {
            this.LoadForm(new frmDiffSets());
        }

        private void btnCheckDuplicates_Click(object sender, System.EventArgs e)
        {
            this.LoadForm(new frmCheckDuplicates());
        }

        private void btnTWSBatHelper_Click(object sender, System.EventArgs e)
        {
            this.LoadForm(new frmFileExplorer());
        }

        private void LoadForm(Form form)
        {

            form.TopLevel = false;
            form.Dock = DockStyle.Fill;

            this.pnlMain.Controls.Clear();
            this.pnlMain.Controls.Add(form);
            this.pnlMain.Tag = form;

            form.Show();
        }

        private void button1_Click(object sender, System.EventArgs e) {
            //Form1 f1 = new Form1();
            //f1.Show();
           
        }
    }
}
