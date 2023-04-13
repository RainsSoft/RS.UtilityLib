//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Action that can be canceled.
// </description>
// <history>
//  $Modtime: 3/03/06 1:28 $
//  $Revision: 1 $
//  $Logfile: /Actions/ISecondaryAction.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  /// <summary>
  /// Action that can be canceled.
  /// </summary>
  public interface ISecondaryAction
  {
    /// <summary>
    /// Indicates whether action can be canceled.
    /// </summary>
    bool CanCancel { get; }
  }
}
