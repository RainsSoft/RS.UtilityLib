//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Defines an action interface.
// </description>
// <history>
//  $Modtime: 16/01/06 11:47 $
//  $Revision: 2 $
//  $Logfile: /Actions/IAction.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  /// <summary>
  /// Defines an action interface.
  /// </summary>
  public interface IAction
  {
    /// <summary>
    /// Gets action name.
    /// </summary>
    string Name { get; }

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
    bool Run(UndoRedoManager undoRedoManager, ActionType type);
  }
}
