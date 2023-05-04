using System.ComponentModel.Design;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Collection editor for BetterListViewSubItemCollection.
	/// </summary>
	public sealed class BetterListViewSubItemCollectionEditor : CollectionEditor
	{
		/// <summary>
		///   Initialize a new BetterListViewSubItemCollectionEditor instance.
		/// </summary>
		public BetterListViewSubItemCollectionEditor()
			: base(typeof(BetterListViewSubItemCollection)) {
		}

		/// <summary>
		///   Indicates whether original members of the collection can be removed.
		/// </summary>
		/// <returns>
		///   true if it is permissible to remove this value from the collection; otherwise, false. The default implementation always returns true.
		/// </returns>
		/// <param name="value">The value to remove. 
		/// </param>
		protected override bool CanRemoveInstance(object value) {
			if (value != null && value is BetterListViewSubItem betterListViewSubItem) {
				return betterListViewSubItem.Index != 0;
			}
			return true;
		}

		/// <summary>
		///   Sets the specified array as the items of the collection.
		/// </summary>
		/// <returns>
		///   The newly created collection object or, otherwise, the collection indicated by the <paramref name="editValue" /> parameter.
		/// </returns>
		/// <param name="editValue">The collection to edit. 
		/// </param>
		/// <param name="value">An array of objects to set as the collection items. 
		/// </param>
		protected override object SetItems(object editValue, object[] value) {
			if (editValue != null && editValue is BetterListViewSubItemCollection betterListViewSubItemCollection) {
				betterListViewSubItemCollection.Clear();
				BetterListViewSubItem betterListViewSubItem = (BetterListViewSubItem)value[0];
				if (betterListViewSubItemCollection[0] != betterListViewSubItem) {
					betterListViewSubItem.OwnerCollection = null;
					betterListViewSubItemCollection[0] = betterListViewSubItem;
				}
				if (value.Length > 1) {
					for (int i = 1; i < value.Length; i++) {
						BetterListViewSubItem betterListViewSubItem2 = (BetterListViewSubItem)value[i];
						betterListViewSubItem2.OwnerCollection = null;
						betterListViewSubItemCollection.Add(betterListViewSubItem2);
					}
				}
			}
			return editValue;
		}
	}
}