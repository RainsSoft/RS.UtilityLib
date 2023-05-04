namespace ComponentOwl.BetterListView
{

	/// <summary>
	///   Collection of items tied to Better ListView state.
	/// </summary>
	public abstract class BetterListViewCachedItemCollection : BetterListViewCachedCollection<BetterListViewItem>
	{
		/// <summary>
		///   Gets the <see cref="T:ComponentOwl.BetterListView.BetterListViewItem" /> with the specified key.
		/// </summary>
		public BetterListViewItem this[string key] => (BetterListViewItem)BetterListViewElementCollectionBase.GetElementByKey(base.CachedItems, key);

		/// <summary>
		///   Initializes a new instance of the <see cref="T:ComponentOwl.BetterListView.BetterListViewCachedItemCollection" /> class.
		/// </summary>
		/// <param name="listView">BetterListView that owns this collection.</param>
		protected BetterListViewCachedItemCollection(BetterListView listView)
			: base(listView) {
		}

		/// <summary>
		///   Check whether this collection contains item with the specified key.
		/// </summary>
		/// <param name="key">Search key.</param>
		/// <returns>This collection contains item with the specified key.</returns>
		public bool ContainsKey(string key) {
			return BetterListViewElementCollectionBase.GetElementByKey(base.CachedItems, key) != null;
		}

		/// <summary>
		///   Get index of item with the specified key.
		/// </summary>
		/// <param name="key">Search key.</param>
		/// <returns>Index of item with the specified key if found, -1 otherwise.</returns>
		public int IndexOfKey(string key) {
			return BetterListViewElementCollectionBase.GetElementByKey(base.CachedItems, key)?.Index ?? (-1);
		}
	}
}