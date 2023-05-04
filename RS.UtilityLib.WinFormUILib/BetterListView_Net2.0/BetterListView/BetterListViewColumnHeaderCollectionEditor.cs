using System;
using System.Collections;
using System.ComponentModel.Design;

namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Collection editor for BetterListViewColumnHeaderCollection.
	/// </summary>
	public sealed class BetterListViewColumnHeaderCollectionEditor : CollectionEditor
	{
		/// <summary>
		///   Initialize a new BetterListViewColumnHeaderCollectionEditor instance.
		/// </summary>
		public BetterListViewColumnHeaderCollectionEditor()
			: base(typeof(BetterListViewColumnHeaderCollection)) {
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
			if (editValue != null && editValue is BetterListViewColumnHeaderCollection betterListViewColumnHeaderCollection) {
				betterListViewColumnHeaderCollection.Clear();
				BetterListViewColumnHeader[] array = new BetterListViewColumnHeader[value.Length];
				Array.Copy(value, 0, array, 0, value.Length);
				betterListViewColumnHeaderCollection.AddRange((IEnumerable)array);
			}
			return editValue;
		}
	}
}