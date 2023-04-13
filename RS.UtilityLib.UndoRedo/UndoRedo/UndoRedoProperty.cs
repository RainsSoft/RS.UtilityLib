//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Property descriptor for a type that supports undo and redo operations.
// </description>
// <history>
//  $Modtime: 8/05/06 19:57 $
//  $Revision: 10 $
//  $Logfile: /Actions/UndoRedoProperty.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  using System;
  using System.ComponentModel;
  using System.Reflection;

  using NesterovskyBros.Actions.Properties;

  /// <summary>
  /// Property descriptor for a type that supports undo and redo operations.
  /// </summary>
  public class UndoRedoProperty: PropertyDescriptor
  {
    private class MethodAction: IAction
    {
      private UndoRedoWrapper owner;
      private UndoRedoProperty property;
      private object value;
      private object state;

      public MethodAction(UndoRedoWrapper owner, UndoRedoProperty property, object value)
      {
        this.owner = owner;
        this.property = property;
        this.value = value;
      }

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

      public virtual bool Run(UndoRedoManager undoRedoManager, ActionType type)
      {
        ActionEventArgs eventArgs = new ActionEventArgs(
          undoRedoManager,
          type,
          owner,
          value);

        eventArgs.State = state;

        property.UndoRedoMethodInfo.Invoke(
          owner.Instance, new object[] { property, eventArgs });

        state = eventArgs.State;

        return eventArgs.Result;
      }
    }

    private PropertyDescriptor parent;
    private bool undoRedoMethodInfoInitialized;
    private MethodInfo undoRedoMethodInfo;

    /// <summary>
    /// Creates an instance of the UndoRedoPropertyDescriptor.
    /// </summary>
    /// <param name="parent">Parent property descriptor.</param>
    public UndoRedoProperty(PropertyDescriptor parent):
      base(CheckParent(parent))
    {
      this.parent = parent;
    }

    /// <summary>
    /// Gets primary property descriptor.
    /// </summary>
    public PropertyDescriptor Parent
    {
      get { return parent; }
    }

    /// <summary>
    /// Gets undo and redo setter.
    /// </summary>
    public MethodInfo UndoRedoMethodInfo
    {
      get 
      {
        if (undoRedoMethodInfoInitialized)
          return undoRedoMethodInfo;

        undoRedoMethodInfoInitialized = true;

        UndoRedoAttribute attribute =
          Attributes[typeof(UndoRedoAttribute)] as UndoRedoAttribute;

        if (attribute == null)
          return null;

        string setterName = attribute.UndoRedoMethodName;

        if (string.IsNullOrEmpty(setterName))
          setterName = "UndoRedo" + parent.Name;

        Type componentType = parent.ComponentType;

        if (componentType == null)
          throw new InvalidOperationException();

        MethodInfo methodInfo = componentType.GetMethod(
          setterName,
          new Type[] { typeof(object), typeof(ActionEventArgs) });

        if (methodInfo == null)
        {
          methodInfo = componentType.GetMethod(
            setterName,
            new Type[] { typeof(object), typeof(EventArgs) });

          if (methodInfo == null)
            throw new InvalidOperationException();
        }

        undoRedoMethodInfo = methodInfo;

        return undoRedoMethodInfo;
      }
    }

    /// <summary>
    /// Returns whether resetting an object changes its value. 
    /// </summary>
    /// <param name="component">The component to test for reset capability.</param>
    /// <returns>
    /// true if resetting the component changes its value and false otherwise.
    /// </returns>
    public override bool CanResetValue(object component)
    {
      return false;
    }

    /// <summary>
    /// Gets the type of the component this property is bound to.
    /// </summary>
    public override Type ComponentType
    {
      get { return parent.ComponentType; }
    }

    /// <summary>
    /// Gets the current value of the property on a component.
    /// </summary>
    /// <param name="component">
    /// The component with the property for which to retrieve the value.
    /// </param>
    /// <returns>The value of a property for a given component.</returns>
    public override object GetValue(object component)
    {
      UndoRedoWrapper owner = component as UndoRedoWrapper;

      return parent.GetValue(owner != null ? owner.Instance : component);
    }

    /// <summary>
    /// Gets a value indicating whether this property is read-only.
    /// </summary>
    public override bool IsReadOnly
    {
      get { return parent.IsReadOnly; }
    }

    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    public override Type PropertyType
    {
      get { return parent.PropertyType; }
    }

    /// <summary>
    /// Resets the value for this property of the component to the default value. 
    /// </summary>
    /// <param name="component">
    /// The component with the property value that is to be reset to the default value.
    /// </param>
    public override void ResetValue(object component) { }

    /// <summary>
    /// Sets the value of the component to a different value.
    /// </summary>
    /// <param name="component">
    /// The component with the property value that is to be set.
    /// </param>
    /// <param name="value">The new value.</param>
    public override void SetValue(object component, object value)
    {
      UndoRedoWrapper owner = component as UndoRedoWrapper;

      if (owner == null)
      {
        parent.SetValue(component, value);
      }
      else if (!owner.CanRun)
      { 
        throw new InvalidOperationException();
      }
      else
      {
        MethodInfo undoRedoSetter = UndoRedoMethodInfo;

        if (undoRedoSetter != null)
        {
          owner.UndoRedoManager.Run(new MethodAction(owner, this, value));
        }
        else
        {
          IAction action = owner.CreateAction(this, value);

          if (action != null)
            owner.UndoRedoManager.Run(action);
        }
      }
    }

    /// <summary>
    /// Determines a value indicating whether the value of this property needs 
    /// to be persisted. 
    /// </summary>
    /// <param name="component">
    /// The component with the property to be examined for persistence.
    /// </param>
    /// <returns>true if the property should be persisted and false otherwise.</returns>
    public override bool ShouldSerializeValue(object component)
    {
      UndoRedoWrapper owner = component as UndoRedoWrapper;

      return parent.ShouldSerializeValue(owner != null ? owner.Instance : component);
    }

    /// <summary>
    /// Indicates whether property descriptor suppors change events.
    /// </summary>
    public override bool SupportsChangeEvents
    {
      get { return parent.SupportsChangeEvents; }
    }

    /// <summary>
    /// Enables other objects to be notified when this property changes.
    /// </summary>
    /// <param name="component">The component to add the handler for.</param>
    /// <param name="handler">The delegate to add as a listener.</param>
    public override void AddValueChanged(object component, EventHandler handler)
    {
      UndoRedoWrapper owner = component as UndoRedoWrapper;

      parent.AddValueChanged(owner != null ? owner.Instance : component, handler);
    }

    /// <summary>
    /// Enables other objects to be notified when this property changes.
    /// </summary>
    /// <param name="component">The component to remove the handler for.</param>
    /// <param name="handler">The delegate to remove as a listener.</param>
    public override void RemoveValueChanged(object component, EventHandler handler)
    {
      UndoRedoWrapper owner = component as UndoRedoWrapper;

      parent.RemoveValueChanged(owner != null ? owner.Instance : component, handler);
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

      UndoRedoProperty that = obj as UndoRedoProperty;

      if (that == null)
        return false;

      return parent.Equals(that.parent);
    }

    /// <summary>
    /// Calculates hash code for the property descriptor.
    /// </summary>
    /// <returns>Hash code for the property descriptor</returns>
    public override int GetHashCode()
    {
      return parent.GetHashCode();
    }

    private static PropertyDescriptor CheckParent(PropertyDescriptor parent)
    {
      if (parent == null)
        throw new ArgumentNullException("parent");

      return parent;
    }
  }
}
