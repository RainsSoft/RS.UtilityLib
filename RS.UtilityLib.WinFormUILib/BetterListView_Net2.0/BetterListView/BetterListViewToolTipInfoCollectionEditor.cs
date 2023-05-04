using System;
using System.ComponentModel.Design;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Collection editor for BetterListViewToolTipInfoCollection.
	/// </summary>
	public sealed class BetterListViewToolTipInfoCollectionEditor : CollectionEditor
	{
		/// <summary>
		///   Initialize a new BetterListViewToolTipInfoCollectionEditor instance.
		/// </summary>
		public BetterListViewToolTipInfoCollectionEditor()
			: base(typeof(BetterListViewToolTipInfoCollection)) {
		}

		/// <summary>
		///   Gets the data type that this collection contains.
		/// </summary>
		/// <returns>
		///   The data type of the items in the collection, or an <see cref="T:System.Object" /> if no Item property can be located on the collection.
		/// </returns>
		protected override Type CreateCollectionItemType() {
			return typeof(BetterListViewToolTipInfo);
		}

		/// <summary>
		///   Creates a new instance of the specified collection item type.
		/// </summary>
		/// <returns>
		///   A new instance of the specified object.
		/// </returns>
		/// <param name="itemType">The type of item to create. 
		/// </param>
		protected override object CreateInstance(Type itemType) {
			return new BetterListViewToolTipInfo(BetterListViewToolTipLocation.Custom, string.Empty);
		}
	}
}