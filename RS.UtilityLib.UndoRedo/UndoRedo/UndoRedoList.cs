//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   A collection that supports undo and redo.
// </description>
// <history>
//  $Modtime: 29/05/06 15:36 $
//  $Revision: 11 $
//  $Logfile: /Actions/UndoRedoList.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  using System;
  using System.ComponentModel;
  using System.Collections;
  using System.Collections.Generic;

  using NesterovskyBros.Actions.Properties;

  /// <summary>
  /// A collection that supports undo and redo.
  /// </summary>
  public class UndoRedoList<T>: 
    ListAdapter<UndoRedoWrapper, T>,
    IBindingList,
    ICancelAddNew,
    IRaiseItemChangedEvents,
    ITypedList
  {
    class UndoRedoItem: UndoRedoWrapper
    {
      private UndoRedoList<T> owner;
      private int index;

      public UndoRedoItem(UndoRedoList<T> owner, int index, T instance):
        base(instance)
      {
        this.owner = owner;
        this.index = index;
      }

      public UndoRedoList<T> Owner
      {
        get { return owner; }
      }

      public new T Instance
      {
        get { return (T)base.Instance; }
      }

      public int Index
      {
        get { return index; }
      }

      public override UndoRedoManager UndoRedoManager
      {
	      get { return owner.UndoRedoManager; }
      }

      public override string GetComponentName()
      {
 	       return owner.ComponentName;
      }

      protected override void Modified(UndoRedoProperty property, object oldValue)
      {
        owner.OnListChanged(
          new ListChangedEventArgs2(
            ListChangedType.ItemChanged, 
            index, 
            property, 
            oldValue));
      }

      public override int GetHashCode()
      {
        return Instance.GetHashCode();
      }

      public override bool Equals(object other)
      {
        return Equals(other as UndoRedoItem);
      }

      public bool Equals(UndoRedoItem other)
      {
        if ((object)other == null)
          return false;

        if ((object)this == (object)other)
          return true;

        return (owner == other.owner) && Instance.Equals(other.Instance);
      }
    }

    enum Operation
    { 
      Insert,
      Remove,
      Set
    }

    class Action: IAction, IMergeableAction
    {
      private UndoRedoList<T> owner;
      private Operation operation;
      private int index;
      private T item;

      public Action(
        UndoRedoList<T> owner, 
        Operation operation, 
        int index,
        T item)
      {
        this.owner = owner;
        this.operation = operation;
        this.index = index;
        this.item = item;
      }

      public string Name
      {
        get 
        {
          string name = owner.ComponentName;

          if (string.IsNullOrEmpty(name))
          {
            switch(operation)
            { 
              case Operation.Insert:
                return Resources.Action_InsertItem;
              case Operation.Remove:
                return Resources.Action_RemoveItem;
              case Operation.Set:
                return Resources.Action_SetItem;
            }
          }
          else
          {
            switch (operation)
            {
              case Operation.Insert:
                return string.Format(Resources.Action_InsertItem2, name);
              case Operation.Remove:
                return string.Format(Resources.Action_RemoveItem2, name);
              case Operation.Set:
                return string.Format(Resources.Action_SetItem2, name);
            }
          }

          return null;
        }
      }

      public bool Run(UndoRedoManager undoRedoManager, ActionType type)
      {
        switch(operation)
        { 
          case Operation.Insert:
          {
            int count = owner.Count;
            int index = this.index;

            if (type != ActionType.Undo)
            {
              if ((index < 0) || (index > count))
                return false;

              owner.List.Insert(index, item);
            
              owner.OnListChanged(
                new ListChangedEventArgs2(ListChangedType.ItemAdded, index));
            }
            else
            {
              if ((index < 0) || (index >= count))
                return false;

              owner.List.RemoveAt(index);

              owner.OnListChanged(
                new ListChangedEventArgs2(
                  ListChangedType.ItemDeleted, 
                  index, 
                  item));
            }

            break;
          }
          case Operation.Remove:
          {
            type = type != ActionType.Undo ? ActionType.Undo : ActionType.Do;

            goto case Operation.Insert;
          }
          case Operation.Set:
          {
            if ((index < 0) || (index >= owner.Count))
              return false;
              
            T prevItem = owner.List[index];

            if (EqualityComparer<T>.Default.Equals(prevItem, item))
              return false;

            owner.List[index] = item;
            item = prevItem;

            owner.OnListChanged(
              new ListChangedEventArgs2(ListChangedType.ItemChanged, index, item));

            break;
          }
        }

        return true;
      }

      public IAction Merge(UndoRedoManager undoRedoManager, IList<IAction> actions)
      {
        if (actions.Count == 0)
          return this;

        Action action = actions[0] as Action;

        if ((action == null) || (action.index != index) || (action.owner != owner))
          return this;

        switch(operation)
        {
          case Operation.Insert:
            if (action.operation != Operation.Remove)
              return this;

            break;
          case Operation.Remove:
            if (action.operation != Operation.Insert)
              return this;

            break;
          default:
            return this;
        }

        if (!EqualityComparer<T>.Default.Equals(action.item, item))
          return this;

        actions.RemoveAt(0);

        return null;
      }
    }

    private string componentName;
    private UndoRedoManager undoRedoManager;
    private PropertyDescriptorCollection properties;

    /// <summary>
    /// Create an instance of the UndoRedoList.
    /// </summary>
    /// <param name="list">List to wrap.</param>
    /// <param name="undoRedoManager">Undo and redo manager.</param>
    public UndoRedoList(IList<T> list, UndoRedoManager undoRedoManager):
      this(list, null, undoRedoManager) 
    {
    }

    /// <summary>
    /// Create an instance of the UndoRedoList.
    /// </summary>
    /// <param name="list">List to wrap.</param>
    /// <param name="componentName">Element name.</param>
    /// <param name="undoRedoManager">Undo and redo manager.</param>
    public UndoRedoList(
      IList<T> list, 
      string componentName, 
      UndoRedoManager undoRedoManager):
      base(list)
    {
      if (undoRedoManager == null)
        throw new ArgumentNullException("undoRedoManager");

      this.componentName = componentName;
      this.undoRedoManager = undoRedoManager;
    }

    /// <summary>
    /// Gets or sets element name.
    /// </summary>
    public string ComponentName
    {
      get { return componentName; }
    }

    /// <summary>
    /// Gets or sets undo and redo manager.
    /// </summary>
    public UndoRedoManager UndoRedoManager
    {
      get { return undoRedoManager; }
    }

    /// <summary>
    /// Event handler to create new instance of wrapped element of type T.
    /// </summary>
    public event AddingNewEventHandler AddingNew;

    /// <summary>
    /// Event that occurs when element of the collection is changed.
    /// </summary>
    public event ListChangedEventHandler ListChanged;

    /// <summary>
    /// Raises ListChanged event to indicate that wrapped list has changed.
    /// </summary>
    public void Reset()
    {
      OnListChanged(new ListChangedEventArgs2(ListChangedType.Reset, -1));
    }

    /// <summary>
    /// Creates type descriptor for the T instance.
    /// </summary>
    /// <param name="t">Element of the wrapped collection.</param>
    public UndoRedoWrapper GetUndoRedoWrapper(T t)
    {
      if (t == null)
        return null;

      return new UndoRedoItem(this, -1, t);
    }

    /// <summary>
    /// Creates type descriptor for the T instance.
    /// </summary>
    public UndoRedoWrapper CreateItem()
    {
      return GetUndoRedoWrapper(OnCreateNew());
    }

    /// <summary>
    /// Creates new instance of the T object.
    /// </summary>
    /// <returns>New T instance.</returns>
    protected virtual T OnCreateNew()
    {
      AddingNewEventHandler eventHandler = AddingNew;

      if (eventHandler == null)
      {
        if (default(T) == null)
          return Activator.CreateInstance<T>();
        else
          return default(T);
      }

      AddingNewEventArgs eventArgs = new AddingNewEventArgs();

      eventHandler(this, eventArgs);

      if (eventArgs.NewObject == null)
        return Activator.CreateInstance<T>();

      UndoRedoWrapper wrapper = eventArgs.NewObject as UndoRedoWrapper;

      if (wrapper != null)
        return (T)wrapper.Instance;

      return (T)eventArgs.NewObject;
    }

    /// <summary>
    /// Fires list change event.
    /// </summary>
    /// <param name="eventArgs">Event argument.</param>
    public virtual void OnListChanged(ListChangedEventArgs eventArgs)
    {
      ListChangedEventHandler eventHandler = ListChanged;

      if (eventHandler != null)
        eventHandler(this, eventArgs);
    }

    /// <summary>
    /// Converts instance of T into instance of UndoRedoWrapper.
    /// </summary>
    /// <param name="p">Instance of T type.</param>
    /// <param name="index">Index of the item.</param>
    /// <returns>Instance of UndoRedoWrapper type.</returns>
    protected override UndoRedoWrapper Convert(T p, int index)
    {
      if (p == null)
        return null;

      return new UndoRedoItem(this, index, p);
    }

    /// <summary>
    /// Converts instance of UndoRedoWrapper into instance of T.
    /// </summary>
    /// <param name="t">Instance of UndoRedoWrapper type.</param>
    /// <returns>Instance of T type.</returns>
    protected override T Convert(UndoRedoWrapper t)
    {
      if (t == null)
        return default(T);

      return ((UndoRedoItem)t).Instance;
    }

    /// <summary>
    /// Inserts item into the collection.
    /// </summary>
    /// <param name="index">Index of the item.</param>
    /// <param name="item">Item to insert.</param>
    protected override void InsertItem(int index, T item)
    {
      Run(Operation.Insert, index, item);
    }

    /// <summary>
    /// Removes item from the collection.
    /// </summary>
    /// <param name="index">Index of an item to remove.</param>
    protected override void RemoveItem(int index)
    {
      Run(Operation.Remove, index, Get(index));
    }

    /// <summary>
    /// Sets item given a specified index.
    /// </summary>
    /// <param name="index">Index of item to set.</param>
    /// <param name="item">Item to set.</param>
    protected override void SetItem(int index, T item)
    {
      Run(Operation.Set, index, item);
    }

    /// <summary>
    /// Clears collection.
    /// </summary>
    protected override void ClearItems()
    {
      if (Count == 0)
        return;

      using(undoRedoManager.CreateScope(Resources.Action_ClearItems))
      {
        for (int i = Count; i-- > 0; )
        {
          RemoveAt(i);
        }
      }
    }

    // IBindingList Members

    void IBindingList.AddIndex(PropertyDescriptor property) { }

    object IBindingList.AddNew()
    {
      UndoRedoWrapper item = CreateItem();
      
      this.Add(item);

      return item;
    }

    bool IBindingList.AllowEdit
    {
      get { return !IsReadOnly; }
    }

    bool IBindingList.AllowNew
    {
      get { return !IsReadOnly && !(this as IList).IsFixedSize; }
    }

    bool IBindingList.AllowRemove
    {
      get { return !IsReadOnly && !(this as IList).IsFixedSize; }
    }

    void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
    {
      throw new NotSupportedException();
    }

    int IBindingList.Find(PropertyDescriptor property, object key)
    {
      throw new NotSupportedException();
    }

    bool IBindingList.IsSorted
    {
      get { return false; }
    }

    void IBindingList.RemoveIndex(PropertyDescriptor property) { }

    void IBindingList.RemoveSort()
    {
      throw new NotSupportedException();
    }

    ListSortDirection IBindingList.SortDirection
    {
      get { return ListSortDirection.Ascending; }
    }

    PropertyDescriptor IBindingList.SortProperty
    {
      get { return null; }
    }

    bool IBindingList.SupportsChangeNotification
    {
      get { return true; }
    }

    bool IBindingList.SupportsSearching
    {
      get { return false; }
    }

    bool IBindingList.SupportsSorting
    {
      get { return false; }
    }

    // ICancelAddNew Members
    void ICancelAddNew.CancelNew(int itemIndex)
    {
      RemoveItem(itemIndex);
    }

    void ICancelAddNew.EndNew(int itemIndex)
    {
      // Nothing to do.
    }

    // IRaiseItemChangedEvents Members

    bool IRaiseItemChangedEvents.RaisesItemChangedEvents
    {
      get { return true; }
    }

    // ITypedList Members

    PropertyDescriptorCollection ITypedList.GetItemProperties(
      PropertyDescriptor[] listAccessors)
    {
      if (properties == null)
        properties = UndoRedoItem.GetProperties(typeof(T));

      return properties;
    }

    string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
    {
      return null;
    }

    private void Run(Operation operation, int index, T item)
    {
      Action action = new Action(this, operation, index, item);

      if (undoRedoManager.CanRun)
        undoRedoManager.Run(action);
      else
        action.Run(null, ActionType.Do);
    }
  }
}
