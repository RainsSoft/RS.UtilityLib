//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Undo and redo service.
// </description>
// <history>
//  $Modtime: 17/01/06 12:08 $
//  $Revision: 2 $
//  $Logfile: /Actions/IUndoRedoService.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  using System;

  /// <summary>
  /// Undo and redo service.
  /// </summary>
  public interface IUndoRedoService
  {
    /// <summary>
    /// Event that occurs when state of the undo and redo manager is changed.
    /// </summary>
    event EventHandler Changed;

    /// <summary>
    /// Gets or sets an instance of the UndoRedoManager.
    /// </summary>
    UndoRedoManager UndoRedoManager { get; set; }
  }
}
