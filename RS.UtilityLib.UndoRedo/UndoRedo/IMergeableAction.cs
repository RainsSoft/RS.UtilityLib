//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Defines a mergeable action interface.
// </description>
// <history>
//  $Modtime: 8/03/06 15:24 $
//  $Revision: 1 $
//  $Logfile: /Actions/IMergeableAction.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  using System.Collections.Generic;

  /// <summary>
  /// Defines a mergeable action interface.
  /// </summary>
  public interface IMergeableAction
  {
    /// <summary>
    /// Merges action, if possible, with a list of actions.
    /// Merged actions have to be removed from actions list.
    /// </summary>
    /// <param name="undoRedoManager">
    /// Optional undo and redo manager that runs action.
    /// </param>
    /// <param name="actions">Actions to merge with this action.</param>
    /// <returns>Instance of the merged action.</returns>
    IAction Merge(UndoRedoManager undoRedoManager, IList<IAction> actions);
  }
}
