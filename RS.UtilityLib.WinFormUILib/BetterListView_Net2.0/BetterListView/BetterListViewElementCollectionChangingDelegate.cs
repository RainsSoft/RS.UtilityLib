namespace ComponentOwl.BetterListView
{

    /// <summary>
    ///   Callback for start of a collection change.
    /// </summary>
    /// <param name="collection">collection that is changing</param>
    /// <returns>proceed collection change</returns>
    internal delegate bool BetterListViewElementCollectionChangingDelegate(BetterListViewElementCollectionBase collection);
}