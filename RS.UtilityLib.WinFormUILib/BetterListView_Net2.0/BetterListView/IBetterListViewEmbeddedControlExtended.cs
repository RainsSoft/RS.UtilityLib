using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Control that is embeddable in BetterListView as a custom editor control.
	///   Provides additional features.
	/// </summary>
	public interface IBetterListViewEmbeddedControlExtended : IBetterListViewEmbeddedControl, IDisposable
	{
		/// <summary>
		///   Decide on ending label editing by the control.
		/// </summary>
		/// <returns>true, if proceed to end label editing; false, otherwise</returns>
		bool RequestEndEdit();
	}
}