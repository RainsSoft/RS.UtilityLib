//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Undo and redo action that uses delegates for the undo and redo operation.
// </description>
// <history>
//  $Modtime: 8/05/06 17:56 $
//  $Revision: 4 $
//  $Logfile: /Actions/DelegateAction.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Undo and redo action that uses delegates for the undo and redo operation.
  /// </summary>
  public class DelegateAction: IAction, IMergeableAction, ISecondaryAction
  {
    [Flags]
    enum Flags
    { 
      CanMerge = 1,
      CanCancel = 2
    }

    private object value;
    private EventHandler<ActionEventArgs> actionHandler;
    private string name;
    private Flags flags;

    /// <summary>
    /// Creates an instance of the delegate action.
    /// Created action is mergeable and cancelable.
    /// </summary>
    /// <param name="actionHandler">
    /// Action handler that gets called to process action.
    /// </param>
    public DelegateAction(EventHandler<ActionEventArgs> actionHandler):
      this(actionHandler, null, null, true, true)
    {
    }

    /// <summary>
    /// Creates an instance of the delegate action.
    /// Created action is mergeable and secondary.
    /// </summary>
    /// <param name="actionHandler">
    /// Action handler that gets called to process action.
    /// </param>
    /// <param name="value">Action parameter.</param>
    public DelegateAction(EventHandler<ActionEventArgs> actionHandler, object value):
      this(actionHandler, value, null, true, true)
    {
    }

    /// <summary>
    /// Creates an instance of the delegate action.
    /// Created action is mergeable and cancelable.
    /// </summary>
    /// <param name="actionHandler">
    /// Action handler that gets called to process action.
    /// </param>
    /// <param name="name">Action name.</param>
    public DelegateAction(EventHandler<ActionEventArgs> actionHandler, string name):
      this(actionHandler, null, name, true, true)
    {
    }

    /// <summary>
    /// Creates an instance of the delegate action.
    /// Created action is mergeable and cancelable.
    /// </summary>
    /// <param name="actionHandler">
    /// Action handler that gets called to process action.
    /// </param>
    /// <param name="value">Action parameter.</param>
    /// <param name="name">Action name.</param>
    public DelegateAction(
      EventHandler<ActionEventArgs> actionHandler, 
      object value, 
      string name):
      this(actionHandler, value, name, true, true)
    {
    }

    /// <summary>
    /// Creates an instance of the delegate action.
    /// </summary>
    /// <param name="actionHandler">
    /// Action handler that gets called to process action.
    /// </param>
    /// <param name="value">Action parameter.</param>
    /// <param name="name">Action name.</param>
    /// <param name="canMerge">Indicates whether action is mergeable.</param>
    /// <param name="canCancel">Indicates if action is secondary.</param>
    public DelegateAction(
      EventHandler<ActionEventArgs> actionHandler,
      object value,
      string name,
      bool canMerge,
      bool canCancel)
    {
      if (actionHandler == null)
        throw new ArgumentNullException("actionHandler");

      this.actionHandler = actionHandler;
      this.value = value;
      this.name = name;

      if (canMerge)
        this.flags |= Flags.CanMerge;

      if (canCancel)
        this.flags |= Flags.CanCancel;
    }

    // IAction members
    string IAction.Name
    {
      get { return name; }
    }

    bool IAction.Run(UndoRedoManager undoRedoManager, ActionType type)
    {
      ActionEventArgs eventArgs = new ActionEventArgs(undoRedoManager, type, null, value);

      actionHandler(undoRedoManager, eventArgs);

      return eventArgs.Result;
    }

    // IMergeableAction members

    IAction IMergeableAction.Merge(
      UndoRedoManager undoRedoManager,
      IList<IAction> actions)
    {
      if (((flags & Flags.CanMerge) == 0) || (actions.Count == 0))
        return this;

      DelegateAction that = actions[0] as DelegateAction;

      if (that == null)
        return this;

      if (!EqualityComparer<object>.Default.Equals(actionHandler, that.actionHandler))
        return this;

      if (!EqualityComparer<object>.Default.Equals(value, that.value))
        return this;

      actions.RemoveAt(0);

      return this;
    }

    // ISecondaryAction members

    bool ISecondaryAction.CanCancel
    {
      get { return (flags & Flags.CanCancel) != 0; }
    }
  }
}
