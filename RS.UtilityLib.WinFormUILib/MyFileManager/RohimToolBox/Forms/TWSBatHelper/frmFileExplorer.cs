using RohimToolBox.CustControls;
using System;
using System.Threading;
using System.Windows.Forms;

namespace RohimToolBox.Forms.TWSBatHelper {
	public partial class frmFileExplorer : Form {
		public frmFileExplorer() {
			InitializeComponent();

			// add frmTWSBatHelperPage to first tab
			frmFileExplorerPage page1 = new frmFileExplorerPage();
			page1.TopLevel = false;
			page1.Dock = DockStyle.Fill;
			page1.Show();
			this.tabPage1.Controls.Add(page1);
		}

		private void btnHelp_Click(object sender, EventArgs e) {
			string text =
				$"Select All Folders & Files\tCtrl + A{Environment.NewLine}" +
				$"Copy Files\t\tCtrl + C{Environment.NewLine}" +
				$"Copy Files Paths\t\tCtrl + Shift + C{Environment.NewLine}";

			Form owner = this.ParentForm ?? this;
			using (new CenterWinDialog(owner)) {
				MessageBox.Show(text, "Shortcut Keys");
			}
		}
	}
}
