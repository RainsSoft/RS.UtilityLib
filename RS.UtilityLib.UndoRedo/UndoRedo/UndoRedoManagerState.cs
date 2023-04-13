//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Defines state of the undo redo manager.
// </description>
// <history>
//  $Modtime: 14/01/06 14:32 $
//  $Revision: 1 $
//  $Logfile: /Actions/UndoRedoManagerState.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  /// <summary>
  /// Defines state of the undo redo manager.
  /// </summary>
  public enum UndoRedoManagerState
  {
    /// <summary>
    /// Idle state.
    /// </summary>
    Idle,
    /// <summary>
    /// Do action is being processed.
    /// </summary>
    ProcessDo,
    /// <summary>
    /// Undo action is being processed.
    /// </summary>
    ProcessUndo,
    /// <summary>
    /// Redo action is being processed.
    /// </summary>
    ProcessRedo
  }
}
