//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Action factory for the undo and redo property change.
// </description>
// <history>
//  $Modtime: 8/05/06 19:02 $
//  $Revision: 2 $
//  $Logfile: /Actions/UndoRedoAttribute.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  using System;

  /// <summary>
  /// Undo and redo attribute.
  /// </summary>
  public class UndoRedoAttribute: Attribute
  {
    private string undoRedoMethodName;

    /// <summary>
    /// Creates an instance of the UndoRedoAttribute.
    /// </summary>
    public UndoRedoAttribute() { }

    /// <summary>
    /// Creates an instance of the UndoRedoAttribute.
    /// </summary>
    /// <param name="undoRedoMethodName">Optional name of the undo and redo method.</param>
    public UndoRedoAttribute(string undoRedoMethodName) 
    {
      this.undoRedoMethodName = undoRedoMethodName;
    }

    /// <summary>
    /// Gets undo and redo method name.
    /// </summary>
    public string UndoRedoMethodName
    {
      get { return undoRedoMethodName; }
    }
  }
}
