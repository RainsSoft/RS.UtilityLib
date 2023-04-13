//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Type descriptor for a type that supports undo and redo operations.
// </description>
// <history>
//  $Modtime: 25/02/06 12:07 $
//  $Revision: 5 $
//  $Logfile: /Actions/UndoRedoTypeDescriptor.cs $
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
  public class UndoRedoTypeDescriptor: UndoRedoWrapper
  {
    private string name;
    private UndoRedoManager undoRedoManager;

    /// <summary>
    /// Creates an instance of the UndoRedoTypeDescriptor.
    /// </summary>
    /// <param name="instance">Instance wrapped with UndoRedoTypeDescriptor.</param>
    /// <param name="undoRedoManager">Undo redo manager.</param>
    public UndoRedoTypeDescriptor(object instance, UndoRedoManager undoRedoManager):
      this(instance, null, undoRedoManager)
    {
    }

    /// <summary>
    /// Creates an instance of the UndoRedoTypeDescriptor.
    /// </summary>
    /// <param name="instance">Instance wrapped with UndoRedoTypeDescriptor.</param>
    /// <param name="name">Component name.</param>
    /// <param name="undoRedoManager">Undo redo manager.</param>
    public UndoRedoTypeDescriptor(
      object instance,
      string name,
      UndoRedoManager undoRedoManager):
      base(instance)
    {
      if (undoRedoManager == null)
        throw new ArgumentNullException("undoRedoManager");

      this.name = name;
      this.undoRedoManager = undoRedoManager;
    }

    /// <summary>
    /// Gets component name.
    /// </summary>
    /// <returns>Component name.</returns>
    public override string GetComponentName()
    {
      return name;
    }

    /// <summary>
    /// Undo and redo manager.
    /// </summary>
    public override UndoRedoManager UndoRedoManager
    {
      get { return undoRedoManager; }
    }

    /// <summary>
    /// Occurs when property of the value has changed.
    /// </summary>
    public event EventHandler OnChanged;

    protected override void Modified(UndoRedoProperty property, object oldValue)
    {
      base.Modified(property, oldValue);

      EventHandler eventHandler = OnChanged;

      if (eventHandler != null)
        eventHandler(this, EventArgs.Empty);
    }
  }
}