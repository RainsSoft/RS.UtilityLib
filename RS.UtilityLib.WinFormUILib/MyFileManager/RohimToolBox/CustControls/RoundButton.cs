using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RohimToolBox.CustControls {
  public class RoundButton : Button {

	public RoundButton() {
	  this.FlatStyle = FlatStyle.Flat;
	  this.FlatAppearance.BorderSize = 0;
	  this.Cursor = Cursors.Hand;
	}

	protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
	  GraphicsPath grPath = new GraphicsPath();
	  grPath.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
	  this.Region = new System.Drawing.Region(grPath);
	  base.OnPaint(e);
	}
  }
}
