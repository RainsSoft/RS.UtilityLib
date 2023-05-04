using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Embedded text editing control for BetterListView.
	/// </summary>
	[ToolboxItem(false)]
	public class BetterListViewTextBoxEmbeddedControl : TextBox, IBetterListViewEmbeddedControl, IDisposable
	{
		/// <summary>
		///   current (edited) label text
		/// </summary>
		public virtual string LabelText => this.Text;

		/// <summary>
		///   request accepting updated data in BetterListView
		/// </summary>
		public event EventHandler RequestAccept;

		/// <summary>
		///   request cancelling editing
		/// </summary>
		public event EventHandler RequestCancel;

		/// <summary>
		///   Initialize a new BetterListViewTextBoxEmbeddedControl instance.
		/// </summary>
		public BetterListViewTextBoxEmbeddedControl() {
			base.AcceptsReturn = true;
			base.BorderStyle = BorderStyle.FixedSingle;
			base.CausesValidation = false;
		}

		/// <summary>
		///   Gets a value indicating the state of the <see cref="P:System.Windows.Forms.TextBoxBase.ShortcutsEnabled" /> property.
		/// </summary>
		/// <param name="msg">A <see cref="T:System.Windows.Forms.Message" />, passed by reference that represents the window message to process.</param>
		/// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values that represents the shortcut key to process.</param>
		/// <returns>
		///   true if the shortcut key was processed by the control; otherwise, false.
		/// </returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			switch (keyData) {
				case Keys.Return:
					this.RequestAccept(this, EventArgs.Empty);
					return true;
				case Keys.Escape:
					this.RequestCancel(this, EventArgs.Empty);
					return true;
				default:
					return base.ProcessCmdKey(ref msg, keyData);
			}
		}

		/// <summary>
		///   set control size
		/// </summary>
		/// <param name="subItem">sub-item whose data are being edited</param>
		/// <param name="placement">placement of the embedded control within sub-item</param>
		public virtual void SetSize(BetterListViewSubItem subItem, BetterListViewEmbeddedControlPlacement placement) {
			BetterListViewItem item = subItem.Item;
			this.Multiline = false;
			this.Font = (item.UseItemStyleForSubItems ? item.Font : subItem.Font);
			base.Size = new Size(placement.BoundsCell.Width, base.Size.Height);
		}

		public virtual void GetData(BetterListViewSubItem subItem) {
			this.Text = subItem.Text;
		}

		public virtual void SetData(BetterListViewSubItem subItem) {
			subItem.Text = this.Text;
		}
	}
}