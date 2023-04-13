//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005 MultiConn Technologies. All rights reserved.
// </copyright>
// <description>
//   Provides design time access to a shared sources.
// </description>
// <history>
//  $Modtime: 24/05/06 16:20 $
//  $Revision: 39 $
//  $Logfile: /GainSmarts/GainSmarts/Components/SharedResources.cs $
// </history>
//------------------------------------------------------------------------------

namespace Sample
{
  using System.Collections.Generic;
  using Sample.Properties;

  using ItemTypeItem = ResourceBindingItem<Sample.ItemType>;

  public class SharedResources
  {
    private static ResourceBindingItem<T> BindItem<T>(string name, T value)
    {
      return new ResourceBindingItem<T>(name, value);
    }

    public static readonly ItemTypeItem[] ItemTypeItems = 
		{
      BindItem(Resources.ItemType_A, ItemType.A),
      BindItem(Resources.ItemType_B, ItemType.B),
      BindItem(Resources.ItemType_C, ItemType.C),
      BindItem(Resources.ItemType_D, ItemType.D)
		};
  }
}