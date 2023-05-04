using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Embedded date/time editing control.
	/// </summary>
	[ToolboxItem(false)]
	public class BetterListViewDateTimePickerEmbeddedControl : DateTimePicker, IBetterListViewEmbeddedControlExtended, IBetterListViewEmbeddedControl, IDisposable
	{
		private bool calendarVisible;

		/// <summary>
		///   current (edited) label text
		/// </summary>
		public virtual string LabelText => base.Value.ToShortDateString();

		/// <summary>
		///   request accepting updated data in BetterListView
		/// </summary>
		public event EventHandler RequestAccept;

		/// <summary>
		///   request cancelling editing
		/// </summary>
		public event EventHandler RequestCancel;

		/// <summary>
		///   Initialize a new BetterListViewDateTimePickerEmbeddedControl instance.
		/// </summary>
		public BetterListViewDateTimePickerEmbeddedControl() {
			base.CausesValidation = false;
			base.Format = DateTimePickerFormat.Short;
		}

		/// <summary>
		///   Raises the <see cref="E:System.Windows.Forms.DateTimePicker.CloseUp" /> event.
		/// </summary>
		/// <param name="eventargs">An <see cref="T:System.EventArgs" /> that contains the event data. 
		/// </param>
		protected override void OnCloseUp(EventArgs eventargs) {
			this.calendarVisible = false;
			base.OnCloseUp(eventargs);
		}

		/// <summary>
		///   Raises the <see cref="E:System.Windows.Forms.DateTimePicker.DropDown" /> event.
		/// </summary>
		/// <param name="eventargs">An <see cref="T:System.EventArgs" /> that contains the event data. 
		/// </param>
		protected override void OnDropDown(EventArgs eventargs) {
			this.calendarVisible = true;
			base.OnDropDown(eventargs);
		}

		/// <summary>
		///   Raises the <see cref="E:System.Windows.Forms.Control.KeyDown" /> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data. 
		/// </param>
		protected override void OnKeyDown(KeyEventArgs e) {
			if (e.KeyCode == Keys.Return && this.RequestAccept != null) {
				this.RequestAccept(this, EventArgs.Empty);
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.Escape && this.RequestCancel != null) {
				this.RequestCancel(this, EventArgs.Empty);
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else {
				base.OnKeyDown(e);
			}
		}

		/// <summary>
		///   Decide on ending label editing by the control.
		/// </summary>
		/// <returns>true, if proceed to end label editing; false, otherwise</returns>
		public virtual bool RequestEndEdit() {
			return !this.calendarVisible;
		}

		/// <summary>
		///   set control size
		/// </summary>
		/// <param name="subItem">sub-item whose data are being edited</param>
		/// <param name="placement">placement of the embedded control within sub-item</param>
		public virtual void SetSize(BetterListViewSubItem subItem, BetterListViewEmbeddedControlPlacement placement) {
			BetterListViewItem item = subItem.Item;
			this.Font = (item.UseItemStyleForSubItems ? item.Font : subItem.Font);
			base.ClientSize = new Size(placement.BoundsCell.Width, (int)Math.Ceiling(this.Font.GetHeight()));
		}

		public virtual void GetData(BetterListViewSubItem subItem) {
			base.Value = DateTime.Parse(subItem.Text);
		}

		public virtual void SetData(BetterListViewSubItem subItem) {
			subItem.Text = this.LabelText;
		}
	}
}