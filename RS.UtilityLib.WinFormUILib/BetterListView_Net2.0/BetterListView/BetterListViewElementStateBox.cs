using System;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Implements element state logic.
	/// </summary>
	internal sealed class BetterListViewElementStateBox
	{
		private BetterListViewElementState state;

		/// <summary>
		///   Current state.
		/// </summary>
		public BetterListViewElementState State => this.state;

		/// <summary>
		///   Change current state to a new state with the specified state transition.
		/// </summary>
		/// <param name="stateChange">State transition.</param>
		public void ChangeState(BetterListViewElementStateChange stateChange) {
			switch (stateChange) {
				case BetterListViewElementStateChange.ResetElement:
					this.state = BetterListViewElementState.Inactive;
					return;
				case BetterListViewElementStateChange.ResetMeasurement:
					switch (this.state) {
						case BetterListViewElementState.Inactive:
							return;
						case BetterListViewElementState.Active:
							return;
						case BetterListViewElementState.InactiveCoarse:
							this.state = BetterListViewElementState.Inactive;
							return;
						case BetterListViewElementState.InactiveFine:
							this.state = BetterListViewElementState.Inactive;
							return;
						case BetterListViewElementState.ActiveCoarse:
							this.state = BetterListViewElementState.Active;
							return;
						case BetterListViewElementState.ActiveFine:
							this.state = BetterListViewElementState.Active;
							return;
						case BetterListViewElementState.ActiveVisible:
							this.state = BetterListViewElementState.Active;
							return;
					}
					break;
				case BetterListViewElementStateChange.Activate:
					switch (this.state) {
						case BetterListViewElementState.Active:
						case BetterListViewElementState.ActiveCoarse:
						case BetterListViewElementState.ActiveFine:
						case BetterListViewElementState.ActiveVisible:
							return;
						case BetterListViewElementState.Inactive:
							this.state = BetterListViewElementState.Active;
							return;
						case BetterListViewElementState.InactiveCoarse:
							this.state = BetterListViewElementState.ActiveCoarse;
							return;
						case BetterListViewElementState.InactiveFine:
							this.state = BetterListViewElementState.ActiveFine;
							return;
					}
					break;
				case BetterListViewElementStateChange.Deactivate:
					switch (this.state) {
						case BetterListViewElementState.Inactive:
						case BetterListViewElementState.InactiveCoarse:
						case BetterListViewElementState.InactiveFine:
							return;
						case BetterListViewElementState.Active:
							this.state = BetterListViewElementState.Inactive;
							return;
						case BetterListViewElementState.ActiveCoarse:
							this.state = BetterListViewElementState.InactiveCoarse;
							return;
						case BetterListViewElementState.ActiveFine:
							this.state = BetterListViewElementState.InactiveFine;
							return;
						case BetterListViewElementState.ActiveVisible:
							this.state = BetterListViewElementState.InactiveFine;
							return;
					}
					break;
				case BetterListViewElementStateChange.MeasureCoarse:
					switch (this.state) {
						case BetterListViewElementState.ActiveCoarse:
							return;
						case BetterListViewElementState.Active:
							this.state = BetterListViewElementState.ActiveCoarse;
							return;
					}
					break;
				case BetterListViewElementStateChange.MeasureFine:
					switch (this.state) {
						case BetterListViewElementState.ActiveFine:
							return;
						case BetterListViewElementState.Active:
							this.state = BetterListViewElementState.ActiveFine;
							return;
						case BetterListViewElementState.ActiveCoarse:
							this.state = BetterListViewElementState.ActiveFine;
							return;
					}
					break;
				case BetterListViewElementStateChange.MakeVisible:
					switch (this.state) {
						case BetterListViewElementState.ActiveVisible:
							return;
						case BetterListViewElementState.ActiveFine:
							this.state = BetterListViewElementState.ActiveVisible;
							return;
					}
					break;
				case BetterListViewElementStateChange.MakeInvisible:
					switch (this.state) {
						case BetterListViewElementState.Active:
						case BetterListViewElementState.ActiveCoarse:
						case BetterListViewElementState.ActiveFine:
							return;
						case BetterListViewElementState.ActiveVisible:
							this.state = BetterListViewElementState.ActiveFine;
							return;
					}
					break;
				default:
					throw new ApplicationException($"Unknown state change: '{stateChange}'.");
			}
			throw new InvalidOperationException($"Cannot use '{stateChange}' when element state is '{this.state}'.");
		}
	}
}