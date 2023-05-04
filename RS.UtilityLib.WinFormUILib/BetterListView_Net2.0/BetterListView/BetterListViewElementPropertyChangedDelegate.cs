using System;

namespace ComponentOwl.BetterListView
{

    /// <summary>
    ///   Callback for collection item property change.
    /// </summary>
    /// <param name="collection">collection containing the element</param>
    /// <param name="elementPropertyType">element property type</param>
    /// <param name="element">element whose property has changed</param>
    /// <param name="oldValue">property value before the property was set</param>
    /// <param name="eventArgs">event data (in case of property change event should be fired)</param>
    internal delegate void BetterListViewElementPropertyChangedDelegate(BetterListViewElementCollectionBase collection, BetterListViewElementPropertyType elementPropertyType, BetterListViewElementBase element, object oldValue, EventArgs eventArgs);
}