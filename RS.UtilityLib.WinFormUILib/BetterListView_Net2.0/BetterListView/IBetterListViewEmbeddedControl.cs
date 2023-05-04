using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Control that is embeddable in BetterListView as a custom editor control.
	/// </summary>
	public interface IBetterListViewEmbeddedControl : IDisposable
	{
		/// <summary>
		///   current (edited) label text
		/// </summary>
		string LabelText { get; }

		/// <summary>
		///   request accepting updated data in BetterListView
		/// </summary>
		event EventHandler RequestAccept;

		/// <summary>
		///   request cancelling editing
		/// </summary>
		event EventHandler RequestCancel;

		/// <summary>
		///   set control size
		/// </summary>
		/// <param name="subItem"> sub-item whose data are being edited </param>
		/// <param name="placement"> placement of the embedded control within sub-item </param>
		void SetSize(BetterListViewSubItem subItem, BetterListViewEmbeddedControlPlacement placement);

		void GetData(BetterListViewSubItem subItem);

		void SetData(BetterListViewSubItem subItem);
	}
}