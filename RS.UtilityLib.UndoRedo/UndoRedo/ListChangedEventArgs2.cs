//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Provides extended data for the ListChanged event.
// </description>
// <history>
//  $Modtime: 26/02/06 12:52 $
//  $Revision: 1 $
//  $Logfile: /Actions/ListChangedEventArgs2.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  using System.ComponentModel;

  /// <summary>
  /// Provides extended data for the ListChanged event. 
  /// </summary>
  public class ListChangedEventArgs2: ListChangedEventArgs
  {
    private object value;

    /// <summary>
    /// Initializes a new instance of the ListChangedEventArgs class 
    /// given the type of change and the index of the affected item. 
    /// </summary>
    /// <param name="listChangedType">
    /// A ListChangedType value indicating the type of change.
    /// </param>
    /// <param name="newIndex">
    /// The index of the item that was added, changed, or removed.
    /// </param>
    public ListChangedEventArgs2(ListChangedType listChangedType, int newIndex):
      base(listChangedType, newIndex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ListChangedEventArgs class 
    /// given the type of change and the PropertyDescriptor affected. 
    /// </summary>
    /// <param name="listChangedType">
    /// A ListChangedType value indicating the type of change.
    /// </param>
    /// <param name="propDesc">
    /// The PropertyDescriptor that was added, removed, or changed.
    /// </param>
    public ListChangedEventArgs2(
      ListChangedType listChangedType, 
      PropertyDescriptor propDesc):
      base(listChangedType, propDesc)
    { 
    }

    /// <summary>
    /// Initializes a new instance of the ListChangedEventArgs class 
    /// given the type of change and the old and new index of the item that was moved. 
    /// </summary>
    /// <param name="listChangedType">
    /// A ListChangedType value indicating the type of change.
    /// </param>
    /// <param name="newIndex">The new index of the item that was moved.</param>
    /// <param name="oldIndex">The old index of the item that was moved.</param>
    public ListChangedEventArgs2(
      ListChangedType listChangedType,
      int newIndex,
      int oldIndex):
      base(listChangedType, newIndex, oldIndex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ListChangedEventArgs class 
    /// given the type of change, the index of the affected item, and 
    /// a PropertyDescriptor describing the affected item. 
    /// </summary>
    /// <param name="listChangedType">
    /// A ListChangedType value indicating the type of change.
    /// </param>
    /// <param name="newIndex">The index of the item that was added or changed.</param>
    /// <param name="propDesc">The PropertyDescriptor describing the item.</param>
    public ListChangedEventArgs2(
      ListChangedType listChangedType, 
      int newIndex, 
      PropertyDescriptor propDesc):
      base(listChangedType, newIndex, propDesc)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ListChangedEventArgs class 
    /// given the type of change, the index of the affected item, and 
    /// a value of the affected item. 
    /// </summary>
    /// <param name="listChangedType">
    /// A ListChangedType value indicating the type of change.
    /// </param>
    /// <param name="newIndex">The index of the item that was added or changed.</param>
    /// <param name="value">The value of the item.</param>
    public ListChangedEventArgs2(
      ListChangedType listChangedType,
      int newIndex,
      object value):
      base(listChangedType, newIndex)
    {
      this.value = value;
    }

    /// <summary>
    /// Initializes a new instance of the ListChangedEventArgs class 
    /// given the type of change, the index of the affected item, 
    /// a PropertyDescriptor describing the affected item, and 
    /// a value of the affected item. 
    /// </summary>
    /// <param name="listChangedType">
    /// A ListChangedType value indicating the type of change.
    /// </param>
    /// <param name="newIndex">The index of the item that was added or changed.</param>
    /// <param name="propDesc">The PropertyDescriptor describing the item.</param>
    /// <param name="value">The value of the item.</param>
    public ListChangedEventArgs2(
      ListChangedType listChangedType,
      int newIndex,
      PropertyDescriptor propDesc,
      object value):
      base(listChangedType, newIndex, propDesc)
    {
      this.value = value;
    }

    /// <summary>
    /// Gets previous value of changed or value of removed item.
    /// </summary>
    public object Value
    {
      get { return value; }
    }
  }
}