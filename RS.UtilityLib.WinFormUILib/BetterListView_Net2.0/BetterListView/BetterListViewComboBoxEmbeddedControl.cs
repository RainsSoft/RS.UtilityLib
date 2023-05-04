using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Embedded combo box editing control for BetterListView.
	/// </summary>
	[ToolboxItem(false)]
	public class BetterListViewComboBoxEmbeddedControl : ComboBox, IBetterListViewEmbeddedControl, IDisposable
	{
		private const bool DefaultDropDrownImmediate = true;

		private bool dropDownImmediate = true;

		/// <summary>
		///   drop down the combo box immediately when got focus
		/// </summary>
		public bool DropDownImmediate {
			get {
				return this.dropDownImmediate;
			}
			set {
				this.dropDownImmediate = value;
			}
		}

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
		///   Initialize a new BetterListViewComboBoxEmbeddedControl instance.
		/// </summary>
		public BetterListViewComboBoxEmbeddedControl() {
			base.CausesValidation = false;
		}

		/// <summary>
		///   Raises the <see cref="E:System.Windows.Forms.Control.GotFocus" /> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. 
		/// </param>
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			if (this.DropDownImmediate) {
				base.DroppedDown = true;
			}
		}

		/// <summary>
		///   Raises the <see cref="E:System.Windows.Forms.Control.KeyDown" /> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data. 
		/// </param>
		protected override void OnKeyDown(KeyEventArgs e) {
			if (e.KeyCode == Keys.Return && this.RequestAccept != null) {
				this.RequestAccept(this, EventArgs.Empty);
			}
			else if (e.KeyCode == Keys.Escape && this.RequestCancel != null) {
				this.RequestCancel(this, EventArgs.Empty);
			}
			else {
				base.OnKeyDown(e);
			}
		}

		/// <summary>
		///   set control size
		/// </summary>
		/// <param name="subItem">sub-item whose data are being edited</param>
		/// <param name="placement">placement of the embedded control within sub-item</param>
		public virtual void SetSize(BetterListViewSubItem subItem, BetterListViewEmbeddedControlPlacement placement) {
			BetterListViewItem item = subItem.Item;
			this.Font = (item.UseItemStyleForSubItems ? item.Font : subItem.Font);
			base.Width = placement.Bounds.Width;
		}

		public virtual void GetData(BetterListViewSubItem subItem) {
			this.Text = subItem.Text;
		}

		public virtual void SetData(BetterListViewSubItem subItem) {
			subItem.Text = this.Text;
		}
	}
}