namespace ComponentOwl.BetterListView
{

    /// <summary>
    ///   Represents the method handling BetterListView.RequestEmbeddedControl event.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="eventArgs">Event data.</param>
    /// <returns>Custom embedded editing control instance.</returns>
    public delegate IBetterListViewEmbeddedControl BetterListViewRequestEmbeddedControlEventHandler(object sender, BetterListViewRequestEmbeddedControlEventArgs eventArgs);
}