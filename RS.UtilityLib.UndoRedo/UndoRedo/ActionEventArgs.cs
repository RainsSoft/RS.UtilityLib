//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Undo and redo action event arguments.
// </description>
// <history>
//  $Modtime: 8/05/06 19:15 $
//  $Revision: 2 $
//  $Logfile: /Actions/ActionEventArgs.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  using System;

  /// <summary>
  /// Undo and redo action event arguments.
  /// </summary>
  public class ActionEventArgs: EventArgs
  {
    private UndoRedoManager undoRedoManager;
    private ActionType type;
    private UndoRedoWrapper instance;
    private object value;
    private bool result;
    private object state;
    
    /// <summary>
    /// Creates an instance of the ActionEventArgs.
    /// </summary>
    /// <param name="undoRedoManager">
    /// An instance of undo and redo manager that runs an action.
    /// </param>
    /// <param name="type">Action type.</param>
    /// <param name="instance">Instance, which is a subject of the action.</param>
    /// <param name="value">Optional action value.</param>
    public ActionEventArgs(
      UndoRedoManager undoRedoManager,
      ActionType type,
      UndoRedoWrapper instance,
      object value)
    {
      this.undoRedoManager = undoRedoManager;
      this.type = type;
      this.instance = instance;
      this.value = value;
      this.result = true;
    }

    /// <summary>
    /// Gets instance of undo and redo manager that runs an action.
    /// </summary>
    public UndoRedoManager UndoRedoManager
    {
      get { return undoRedoManager; }
    }

    /// <summary>
    /// Action type.
    /// </summary>
    public ActionType Type
    {
      get { return type; }
    }

    /// <summary>
    /// Gets optional undo and redo instance wrapper.
    /// </summary>
    public UndoRedoWrapper Instance
    {
      get { return instance; }
    }

    /// <summary>
    /// Action value.
    /// </summary>
    public object Value
    {
      get { return this.value; }
    }

    /// <summary>
    /// Gets or sets result value.
    /// </summary>
    public bool Result
    {
      get { return result; }
      set { result = value; }
    }

    /// <summary>
    /// Gets or sets action state.
    /// </summary>
    public object State
    {
      get { return state; }
      set { state = value; }
    }
  }
}
