namespace Sample
{
  using System;
  using System.Collections.Generic;
  using System.Text;

  public enum ItemType
  {
    A,
    B,
    C,
    D
  }

  public class GridItem
  {
    private string name;
    private double value;

    public string Name
    {
      get { return name; }
      set { name = value; }
    }

    public double Value
    {
      get { return this.value; }
      set { this.value = value; }
    }
  }

  public class Data
  {
    private bool enabled;
    private ItemType type;
    private string text;
    private int number;
    private List<GridItem> items;

    public Data()
    {
      enabled = true;
      items = new List<GridItem>();
    }

    public bool Enabled
    {
      get { return enabled; }
      set { enabled = value; }
    }

    public ItemType Type
    {
      get { return type; }
      set { type = value; }
    }
    public string Text
    {
      get { return text; }
      set { text = value; }
    }

    public int Number
    {
      get { return number; }
      set { number = value; }
    }

    public IList<GridItem> Items
    {
      get { return items; }
    }
  }
}
