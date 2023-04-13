//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Defines a base resource binding item.
// </description>
// <history>
//  $Modtime: 6/12/05 12:38 $
//  $Revision: 2 $
//  $Logfile: /ResourceBindings/ResourceBindingItem.cs $
// </history>
//------------------------------------------------------------------------------
namespace Sample
{
  /// <summary>
  /// Defines a base resource binding item.
  /// </summary>
  /// <typeparam name="T">Defines type to bind to a resource string.</typeparam>
  public class ResourceBindingItem<T>
  {
    private string name;
    private T value;

    /// <summary>
    /// Creates a resource binding item.
    /// </summary>
    /// <param name="name">determines the item display name.</param>
    /// <param name="value">determines the item value.</param>
    public ResourceBindingItem(string name, T value)
    {
      this.name = name;
      this.value = value;
    }

    /// <summary>
    /// Gets the item name.
    /// </summary>
    public string Name 
    { 
      get { return name; } 
    }

    /// <summary>
    /// Gets the item value.
    /// </summary>
    public T Value 
    { 
      get { return value; } 
    }
  }
}
