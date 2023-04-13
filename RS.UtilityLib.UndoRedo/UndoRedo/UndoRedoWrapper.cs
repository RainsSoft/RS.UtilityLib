//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Type descriptor for a type that supports undo and redo operations.
// </description>
// <history>
//  $Modtime: 29/05/06 16:02 $
//  $Revision: 7 $
//  $Logfile: /Actions/UndoRedoWrapper.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  using System;
  using System.ComponentModel;
  using System.Collections.Generic;

  using NesterovskyBros.Actions.Properties;

  /// <summary>
  /// Type descriptor for a type that supports undo and redo operations.
  /// </summary>
  /// <remarks>
  /// Note that properties of type DBNull are not supported.
  /// </remarks>
  public class UndoRedoWrapper: ICustomTypeDescriptor
  {
    /// <summary>
    /// Undo and redo property change action.
    /// </summary>
    protected class Action: IAction, IMergeableAction
    {
      private UndoRedoWrapper owner;
      private UndoRedoProperty property;
      private object value;

      /// <summary>
      /// Creates an instance of the Action.
      /// </summary>
      /// <param name="owner">Owner type.</param>
      /// <param name="property">Property to change.</param>
      /// <param name="value">Value of the property.</param>
      public Action(
        UndoRedoWrapper owner, 
        UndoRedoProperty property, 
        object value)
      {
        if (owner == null)
          throw new ArgumentNullException("owner");

        if (property == null)
          throw new ArgumentNullException("property");

        this.owner = owner;
        this.property = property;
        this.value = value;
      }

      /// <summary>
      /// Gets owner type.
      /// </summary>
      public UndoRedoWrapper Owner
      {
        get { return owner; }
      }

      /// <summary>
      /// Gets property to change.
      /// </summary>
      public UndoRedoProperty Property
      {
        get { return property; }
      }

      /// <summary>
      /// Gets or sets property value.
      /// </summary>
      public object Value
      {
        get { return this.value; }
        set { this.value = value; }
      }

      /// <summary>
      /// Gets action name.
      /// </summary>
      public virtual string Name
      {
        get
        {
          string componentName = owner.GetComponentName();
          string propertyName = property.DisplayName;

          if (string.IsNullOrEmpty(componentName))
          {
            return string.Format(Resources.Action_PropertyChange, propertyName);
          }
          else
          {
            return string.Format(
              Resources.Action_PropertyChange2,
              propertyName,
              componentName);
          }
        }
      }

      /// <summary>
      /// Runs action.
      /// </summary>
      /// <param name="undoRedoManager">
      /// Optional undo and redo manager that runs action.
      /// </param>
      /// <param name="type">Type of the action to run.</param>
      /// <returns>
      /// true if operation can be put in undo redo list, and false otherwise.
      /// </returns>
      public virtual bool Run(UndoRedoManager undoRedoManager, ActionType type)
      {
        // If value is DBNull we return false, 
        // as data binding mechanisms use DBNull when data binding is being torn.
        if (value == DBNull.Value)
          return false;

        object oldValue = property.Parent.GetValue(owner.Instance);

        property.Parent.SetValue(owner.Instance, value);

        object newValue = property.Parent.GetValue(owner.Instance);

        if (EqualityComparer<object>.Default.Equals(newValue, oldValue))
          return false;

        value = oldValue;
        owner.Modified(property, oldValue);

        return true;
      }

      /// <summary>
      /// Merges action, if possible, with a list of actions.
      /// Merged actions have to be removed from actions list.
      /// </summary>
      /// <param name="undoRedoManager">
      /// Optional undo and redo manager that runs action.
      /// </param>
      /// <param name="actions">Actions to merge with this action.</param>
      /// <returns>Instance of the merged action.</returns>
      public virtual IAction Merge(
        UndoRedoManager undoRedoManager, 
        IList<IAction> actions)
      {
        if (actions.Count == 0)
          return this;

        Action action = actions[0] as Action;

        if ((action == null) || !action.property.Equals(property))
          return this;

        if (!owner.Equals(action.owner))
          return this;

        actions.RemoveAt(0);

        return this;
      }
    }

    private static object sync;
    private static Dictionary<Type, PropertyDescriptorCollection> propertyCache;

    private object instance;
    private PropertyDescriptorCollection properties;

    static UndoRedoWrapper()
    {
      sync = new object();
      propertyCache = new Dictionary<Type, PropertyDescriptorCollection>();
    }

    /// <summary>
    /// Creates an instance of the UndoRedoTypeDescriptor.
    /// </summary>
    /// <param name="instance">Instance wrapped with UndoRedoTypeDescriptor.</param>
    public UndoRedoWrapper(object instance)
    {
      if (instance == null)
        throw new ArgumentNullException("instance");

      this.instance = instance;
    }

    /// <summary>
    /// Gets or sets instance wrapped with UndoRedoTypeDescriptor.
    /// </summary>
    public object Instance
    {
      get { return instance; }
    }

    /// <summary>
    /// Undo and redo manager.
    /// </summary>
    public virtual UndoRedoManager UndoRedoManager
    {
      get { return null; }
    }

    /// <summary>
    /// Create undo and redo property change action.
    /// </summary>
    /// <param name="property">Property to change.</param>
    /// <param name="value">Property value.</param>
    /// <returns>Instance of an action.</returns>
    public virtual IAction CreateAction(
      UndoRedoProperty property, 
      object value)
    {
      return new Action(this, property, value);
    }


    /// <summary>
    /// Occurs when propery has changed.
    /// </summary>
    /// <param name="property">Property that has changed.</param>
    /// <param name="oldValue">An old value of the property.</param>
    protected virtual void Modified(UndoRedoProperty property, object oldValue) { }

    /// <summary>
    /// Indicates whether property change action can be run into 
    /// the scope of the UndoRedoManager.
    /// </summary>
    public bool CanRun
    {
      get { return UndoRedoManager.CanRun; }
    }

    /// <summary>
    /// Gets or sets property of the instance.
    /// Action is recorded in undo and redo manager.
    /// </summary>
    /// <param name="name">Property name.</param>
    /// <returns>Property value.</returns>
    public object this[string name]
    {
      get
      {
        if (name == null)
          throw new ArgumentNullException("name");

        PropertyDescriptor property = GetProperties()[name];

        return this[property];
      }
      set
      {
        if (name == null)
          throw new ArgumentNullException("name");

        PropertyDescriptor property = GetProperties()[name];

        this[property] = value;
      }
    }

    /// <summary>
    /// Gets or sets property of the instance.
    /// Action is recorded in undo and redo manager.
    /// </summary>
    /// <param name="property">Property to get or set.</param>
    /// <returns>Property value.</returns>
    public object this[PropertyDescriptor property]
    {
      get
      {
        if (property == null)
          throw new InvalidOperationException("property");

        UndoRedoProperty undoRedoProperty = property as UndoRedoProperty;

        if (undoRedoProperty != null)
          return undoRedoProperty.GetValue(this);
        else
          return property.GetValue(Instance);
      }
      set
      {
        if (property == null)
          throw new InvalidOperationException("property");

        UndoRedoProperty undoRedoProperty = property as UndoRedoProperty;

        if (undoRedoProperty != null)
          undoRedoProperty.SetValue(this, value);
        else
          property.SetValue(Instance, value);
      }
    }

    /// <summary>
    /// Gets attrubutes list.
    /// </summary>
    /// <returns>Attributes list.</returns>
    public virtual AttributeCollection GetAttributes()
    {
      return TypeDescriptor.GetAttributes(Instance);
    }

    /// <summary>
    /// Gets component name.
    /// </summary>
    /// <returns>Component name.</returns>
    public virtual string GetComponentName()
    {
      return TypeDescriptor.GetComponentName(Instance);
    }

    /// <summary>
    /// Gets instance class name.
    /// </summary>
    /// <returns>Instance class name.</returns>
    public virtual string GetClassName()
    {
      return TypeDescriptor.GetClassName(Instance);
    }

    /// <summary>
    /// Gets TypeConverter of the class.
    /// </summary>
    /// <returns>TypeConverter instance.</returns>
    public virtual TypeConverter GetConverter()
    {
      return TypeDescriptor.GetConverter(Instance);
    }

    /// <summary>
    /// Gets default event of the instance.
    /// </summary>
    /// <returns>Default event.</returns>
    public virtual EventDescriptor GetDefaultEvent()
    {
      return TypeDescriptor.GetDefaultEvent(Instance);
    }

    /// <summary>
    /// Gets default property of the instance.
    /// </summary>
    /// <returns>Default property.</returns>
    public virtual PropertyDescriptor GetDefaultProperty()
    {
      return TypeDescriptor.GetDefaultProperty(Instance);
    }

    /// <summary>
    /// Gets instance editor.
    /// </summary>
    /// <param name="editorBaseType">Base type of the editor.</param>
    /// <returns>Editor.</returns>
    public virtual object GetEditor(Type editorBaseType)
    {
      return TypeDescriptor.GetEditor(Instance, editorBaseType);
    }

    /// <summary>
    /// Gets instance events collection.
    /// </summary>
    /// <param name="attributes">Filter attributes.</param>
    /// <returns>Events collection.</returns>
    public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
    {
      return TypeDescriptor.GetEvents(Instance, attributes);
    }

    /// <summary>
    /// Gets instance events collection.
    /// </summary>
    /// <returns>Events collection.</returns>
    public virtual EventDescriptorCollection GetEvents()
    {
      return TypeDescriptor.GetEvents(Instance);
    }

    /// <summary>
    /// Gets property owner.
    /// </summary>
    /// <param name="pd">Property descriptor.</param>
    /// <returns>Property owner instance.</returns>
    public virtual object GetPropertyOwner(PropertyDescriptor pd)
    {
      UndoRedoProperty property = pd as UndoRedoProperty;

      if (property != null)
        return Instance;

      return null;
    }

    /// <summary>
    /// Gets type descriptor properties.
    /// </summary>
    /// <returns>Property descriptor collection.</returns>
    public virtual PropertyDescriptorCollection GetProperties()
    {
      if (properties == null)
        properties = GetProperties(Instance);

      return properties;
    }

    /// <summary>
    /// Gets type descriptor properties.
    /// </summary>
    /// <param name="attributes">Filter attributes.</param>
    /// <returns>Property descriptor collection.</returns>
    public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
      if (properties == null)
        properties = GetProperties(Instance);

      return properties;
    }

    /// <summary>
    /// Gets type descriptor properties.
    /// </summary>
    /// <param name="type">Type for which to get properties.</param>
    /// <returns>Property descriptor collection.</returns>
    public static PropertyDescriptorCollection GetProperties(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      PropertyDescriptorCollection properties;

      lock(sync)
      {
        if (propertyCache.TryGetValue(type, out properties))
          return properties;
      }

      properties = CreateProperties(TypeDescriptor.GetProperties(type));

      lock(sync)
      {
        propertyCache[type] = properties;
      }

      return properties;
    }

    /// <summary>
    /// Tests if two instances are equal.
    /// </summary>
    /// <param name="obj">Instance to compare with.</param>
    /// <returns>true if instances are equal, and false otherwise.</returns>
    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;

      UndoRedoWrapper that = obj as UndoRedoWrapper;

      if (that == null)
        return false;

      return Instance.Equals(that.Instance);
    }

    /// <summary>
    /// Calculates hash code for the property descriptor.
    /// </summary>
    /// <returns>Hash code for the property descriptor</returns>
    public override int GetHashCode()
    {
      return Instance.GetHashCode();
    }

    private PropertyDescriptorCollection GetProperties(object instance)
    {
      if (instance is ICustomTypeDescriptor)
        return CreateProperties(TypeDescriptor.GetProperties(instance));
      else
        return GetProperties(instance.GetType());
    }

    private static PropertyDescriptorCollection CreateProperties(
      PropertyDescriptorCollection collection)
    {
      int count = collection.Count;
      PropertyDescriptor[] items = new PropertyDescriptor[count];

      for (int i = 0; i < count; i++)
      {
        items[i] = new UndoRedoProperty(collection[i]);
      }

      return new PropertyDescriptorCollection(items, true);
    }
  }
}
