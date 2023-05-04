using System.Collections.Generic;
using ComponentOwl.BetterListView.Collections;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Immutable set of BetterListViewGroup instances ensuring reference equality comparison.
	/// </summary>
	public sealed class BetterListViewReadOnlyGroupSet : ReadOnlySet<BetterListViewGroup>
	{
		/// <summary>
		///   Initialize a new BetterListViewReadOnlyGroupSet instance.
		/// </summary>
		public BetterListViewReadOnlyGroupSet() {
		}

		/// <summary>
		///   Initialize a new BetterListViewReadOnlyGroupSet instance.
		/// </summary>
		/// <param name="groups">enumerable of groups to fill this collection with</param>
		public BetterListViewReadOnlyGroupSet(IEnumerable<BetterListViewGroup> groups)
			: base(groups) {
		}
	}
}