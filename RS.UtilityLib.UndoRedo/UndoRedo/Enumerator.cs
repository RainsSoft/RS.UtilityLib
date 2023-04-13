//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2006 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Enumerator implementation for a generic list.
// </description>
// <history>
//  $Modtime: 28/02/06 7:11 $
//  $Revision: 1 $
//  $Logfile: /Actions/Enumerator.cs $
// </history>
//------------------------------------------------------------------------------

namespace NesterovskyBros.Actions
{
  using System;
  using System.Collections;
  using System.Collections.Generic;

  /// <summary>
  /// Enumerator implementation for a generic list.
  /// </summary>
  public struct Enumerator<T>: IEnumerator<T>
  {
    IList<T> list;
    int index;

    /// <summary>
    /// Creates an instance of the Enumerator.
    /// </summary>
    /// <param name="list">List to get enumerator for.</param>
    public Enumerator(IList<T> list)
    {
      this.list = list;
      this.index = -1;
    }

    /// <summary>
    /// Gets current value.
    /// </summary>
    public T Current
    {
      get { return list[index]; }
    }

    /// <summary>
    /// Disposes enumerator instance.
    /// </summary>
    public void Dispose() { }

    /// <summary>
    /// Gets current value.
    /// </summary>
    object System.Collections.IEnumerator.Current
    {
      get { return list[index]; }
    }

    /// <summary>
    /// Move to a next element in the list.
    /// </summary>
    /// <returns>true if there is next element, and false otherwise.</returns>
    public bool MoveNext()
    {
      if (index + 1 < list.Count)
      {
        index++;

        return true;
      }

      return false;
    }

    /// <summary>
    /// Resets enumerator.
    /// </summary>
    public void Reset()
    {
      index = -1;
    }
  }
}