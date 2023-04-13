//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2006 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Defines a list adapter.
// </description>
// <history>
//  $Modtime: 1/03/06 18:59 $
//  $Revision: 6 $
//  $Logfile: /Actions/ListAdapter.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  using System;
  using System.Collections;
  using System.Collections.Generic;

  /// <summary>
  /// Defines a list adapter.
  /// </summary>
  /// <typeparam name="T">The type of elements in the list.</typeparam>
  /// <typeparam name="P">Adapted type.</typeparam>
  public class ListAdapter<T, P>: IList<T>, IList
  {
    private IList<P> list;

    /// <summary>
    /// Create an instance of the UndoRedoList.
    /// </summary>
    /// <param name="list">List to wrap.</param>
    public ListAdapter(IList<P> list)
    {
      if (list == null)
        throw new ArgumentNullException("list");

      this.list = list;
    }

    /// <summary>
    /// Gets adapted list instance.
    /// </summary>
    public IList<P> List
    {
      get { return list; }
    }

    /// <summary>
    /// Adds item to the collection.
    /// </summary>
    /// <param name="item">Item to add.</param>
    public void Add(P item)
    {
      InsertItem(Count, item);
    }

    /// <summary>
    /// Inserts item into the collection.
    /// </summary>
    /// <param name="index">Position where to insert.</param>
    /// <param name="item">Item to insert.</param>
    public void Insert(int index, P item)
    {
      InsertItem(index, item);
    }

    /// <summary>
    /// Removes the first occurrence of a specific item from the list. 
    /// </summary>
    /// <param name="item">Item to remove.</param>
    /// <returns>
    /// true if item was successfully removed from the list and false otherwis.
    /// </returns>
    public bool Remove(P item)
    {
      int index = list.IndexOf(item);

      if (index == -1)
        return false;

      RemoveItem(index);

      return true;
    }

    /// <summary>
    /// Gets index of an item.
    /// </summary>
    /// <param name="item">Item to get index for.</param>
    /// <returns>Index of the item or -1 if item is not in the collection.</returns>
    public int IndexOf(P item)
    {
      return list.IndexOf(item);
    }

    /// <summary>
    /// Checks if item is in the collection.
    /// </summary>
    /// <param name="item">Item to check.</param>
    /// <returns>true if item is in the collection and false otherwise.</returns>
    public bool Contains(P item)
    {
      return list.Contains(item);
    }

    /// <summary>
    /// Gets indexed item.
    /// </summary>
    /// <param name="index">Index of the item.</param>
    /// <returns>Item value.</returns>
    public P Get(int index)
    {
      return list[index];
    }

    /// <summary>
    /// Sets indexed item.
    /// </summary>
    /// <param name="index">Index of the item.</param>
    /// <param name="value">Item value.</param>
    public void Set(int index, P value)
    {
      SetItem(index, value);
    }

    /// <summary>
    /// Inserts item into the collection.
    /// </summary>
    /// <param name="index">Index of the item.</param>
    /// <param name="item">Item to insert.</param>
    protected virtual void InsertItem(int index, P item)
    {
      list.Insert(index, item);
    }

    /// <summary>
    /// Removes item from the collection.
    /// </summary>
    /// <param name="index">Index of an item to remove.</param>
    protected virtual void RemoveItem(int index)
    {
      list.RemoveAt(index);
    }

    /// <summary>
    /// Sets item given a specified index.
    /// </summary>
    /// <param name="index">Index of item to set.</param>
    /// <param name="item">Item to set.</param>
    protected virtual void SetItem(int index, P item)
    {
      list[index] = item;
    }

    /// <summary>
    /// Clears collection.
    /// </summary>
    protected virtual void ClearItems()
    {
      list.Clear();
    }

    /// <summary>
    /// Converts instance of P into instance of T.
    /// </summary>
    /// <param name="p">Instance of P type.</param>
    /// <param name="index">Index of the item.</param>
    /// <returns>Instance of T type.</returns>
    protected virtual T Convert(P p, int index)
    {
      return (T)(object)p;
    }

    /// <summary>
    /// Converts instance of T into instance of P.
    /// </summary>
    /// <param name="t">Instance of T type.</param>
    /// <param name="index">Index of the item.</param>
    /// <returns>Instance of P type.</returns>
    protected virtual P Convert(T t)
    {
      return (P)(object)t;
    }

    // IList<T> members

    /// <summary>
    /// Gets index of an item.
    /// </summary>
    /// <param name="item">Item to get index for.</param>
    /// <returns>Index of the item or -1 if item is not in the collection.</returns>
    public int IndexOf(T item)
    {
      return IndexOf(Convert(item));
    }

    /// <summary>
    /// Inserts item into the collection.
    /// </summary>
    /// <param name="index">Position where to insert.</param>
    /// <param name="item">Item to insert.</param>
    public void Insert(int index, T item)
    {
      InsertItem(index, Convert(item));
    }

    /// <summary>
    /// Removes an item from the collection.
    /// </summary>
    /// <param name="index">Index of item to remove.</param>
    public void RemoveAt(int index)
    {
      RemoveItem(index);
    }

    /// <summary>
    /// Gets or sets indexed item.
    /// </summary>
    /// <param name="index">Index of the item.</param>
    /// <returns>Item value.</returns>
    public T this[int index]
    {
      get { return Convert(Get(index), index); }
      set { Set(index, Convert(value)); }
    }

    // ICollection<T> members

    /// <summary>
    /// Adds item to the collection.
    /// </summary>
    /// <param name="item">Item to add.</param>
    public void Add(T item)
    {
      Add(Convert(item));
    }

    /// <summary>
    /// Clears items.
    /// </summary>
    public void Clear()
    {
      ClearItems();
    }

    /// <summary>
    /// Checks if item is in the collection.
    /// </summary>
    /// <param name="item">Item to check.</param>
    /// <returns>true if item is in the collection and false otherwise.</returns>
    public bool Contains(T item)
    {
      return Contains(Convert(item));
    }

    /// <summary>
    /// Copies items to the array.
    /// </summary>
    /// <param name="array">Array to copy to.</param>
    /// <param name="arrayIndex">Start index.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      if ((arrayIndex < 0) || (arrayIndex >= array.Length))
        throw new ArgumentException("arrayIndex");

      for(int i = arrayIndex, end = Math.Min(arrayIndex + Count, array.Length); 
        i < end; 
        i++)
      {
        array[i] = this[i];
      }
    }

    /// <summary>
    /// Gets the number of elements contained in the list.
    /// </summary>
    public int Count
    {
      get { return list.Count; }
    }

    /// <summary>
    /// Gets a value indicating whether the list is read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get { return list.IsReadOnly; }
    }

    /// <summary>
    /// Removes the first occurrence of a specific item from the list. 
    /// </summary>
    /// <param name="item">Item to remove.</param>
    /// <returns>
    /// true if item was successfully removed from the list and false otherwis.
    /// </returns>
    public bool Remove(T item)
    {
      return list.Remove(Convert(item));
    }

    /// <summary>
    /// Gets list enumerator.
    /// </summary>
    /// <returns>Enumerator instance.</returns>
    public Enumerator<T> GetEnumerator()
    {
      return new Enumerator<T>(this);
    }

    // IEnumerable<T> members

    /// <summary>
    /// Gets list enumerator.
    /// </summary>
    /// <returns>Enumerator instance.</returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return new Enumerator<T>(this);
    }

    // IEnumerable members
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return new Enumerator<T>(this);
    }

    // IList members
    int IList.Add(object value)
    {
      Add((T)value);

      return Count - 1;
    }

    bool IList.Contains(object value)
    {
      return Contains((T)value);
    }

    int IList.IndexOf(object value)
    {
      return IndexOf((T)value);
    }

    void IList.Insert(int index, object value)
    {
      Insert(index, (T)value);
    }

    bool IList.IsFixedSize
    {
      get 
      {
        IList list = this.list as IList;

        return (list != null) && list.IsFixedSize; 
      }
    }

    void IList.Remove(object value)
    {
      Remove((T)value);
    }

    object IList.this[int index]
    {
      get { return this[index]; }
      set { this[index] = (T)value; }
    }

    // ICollection members

    void ICollection.CopyTo(Array array, int index)
    {
      T[] a = array as T[];

      if (a != null)
      {
        CopyTo(a, index);

        return;
      }

      ICollection collection = list as ICollection;

      if (collection != null)
      {
        collection.CopyTo(array, index);

        return;
      }

      throw new NotSupportedException();
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    object ICollection.SyncRoot
    {
      get
      {
        ICollection collection = this.list as ICollection;

        if (collection != null)
          return collection.SyncRoot;

        throw new NotSupportedException();
      }
    }
  }
}