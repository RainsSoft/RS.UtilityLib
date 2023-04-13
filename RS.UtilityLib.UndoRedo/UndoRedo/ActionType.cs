//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Defines an action types for the IAction interface.
// </description>
// <history>
//  $Modtime: 13/01/06 21:10 $
//  $Revision: 1 $
//  $Logfile: /Actions/ActionType.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  /// <summary>
  /// Defines an action types for the IAction interface.
  /// </summary>
  public enum ActionType
  {
    /// <summary>
    /// Defines a "Do" action.
    /// </summary>
    Do,
    /// <summary>
    /// Defines an "Undo" action.
    /// </summary>
    Undo,
    /// <summary>
    /// Defines a "Redo" action.
    /// </summary>
    Redo
  }
}
