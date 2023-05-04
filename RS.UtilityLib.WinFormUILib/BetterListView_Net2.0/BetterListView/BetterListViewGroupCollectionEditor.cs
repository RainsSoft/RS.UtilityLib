using System;
using System.ComponentModel.Design;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Collection editor for BetterListViewGroupCollection.
	/// </summary>
	public sealed class BetterListViewGroupCollectionEditor : CollectionEditor
	{
		/// <summary>
		///   Initialize a new BetterListViewGroupCollectionEditor instance.
		/// </summary>
		public BetterListViewGroupCollectionEditor()
			: base(typeof(BetterListViewGroupCollection)) {
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
			if (editValue != null && editValue is BetterListViewGroupCollection betterListViewGroupCollection) {
				betterListViewGroupCollection.Clear();
				BetterListViewGroup[] array = new BetterListViewGroup[value.Length];
				Array.Copy(value, 0, array, 0, value.Length);
				object[] values = array;
				betterListViewGroupCollection.AddRange(values);
			}
			return editValue;
		}
	}
}