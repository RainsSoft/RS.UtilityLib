namespace ComponentOwl.BetterListView
{

    /// <summary>
    ///   Callback for element collection change.
    /// </summary>
    /// <param name="collection">collection that has changed</param>
    /// <param name="changeInfo">information about changes made to the collection</param>
    internal delegate void BetterListViewElementCollectionChangedDelegate(BetterListViewElementCollectionBase collection, BetterListViewElementCollectionChangeInfo changeInfo);
}