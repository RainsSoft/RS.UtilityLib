namespace Sample
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.Drawing;
  using System.Text;
  using System.Windows.Forms;

  using Sample.Properties;
  using NesterovskyBros.Actions;

  public partial class SampleForm: Form
  {
    // This is data itself.
    private Data data;

    // Undo and redo manager.
    private UndoRedoManager undoRedoManager;


    public SampleForm()
    {
      // Create data object.
      data = new Data();

      InitializeComponent();

      // Attach (name, value) mapping to comboBox list.
      comboBox.DataSource = SharedResources.ItemTypeItems;

      // Create UndoRedoManager and register "Changed" event.
      undoRedoManager = new UndoRedoManager();
      undoRedoManager.Changed += OnUndoRedoManagerChanged;

      // Create undo and redo wrapper around the data object.
      UndoRedoTypeDescriptor undoRedoObject = 
        new UndoRedoTypeDescriptor(data, Resources.Data_Name, undoRedoManager);

      // Create undo and redo wrapper of the data.Items collection.
      UndoRedoList<GridItem> undoRedoList = 
        new UndoRedoList<GridItem>(data.Items, Resources.Data_Name,  undoRedoManager);

      // Bind controls.
      propertyGrid.SelectedObject = undoRedoObject;
      dataBindingSource.DataSource = undoRedoObject;
      gridItemBindingSource.DataSource = undoRedoList;
    }

    // Reflect state change of the undo and redo manager.
    private void OnUndoRedoManagerChanged(object sender, EventArgs e)
    {
      bool canUndo = undoRedoManager.CanUndo;
      bool canRedo = undoRedoManager.CanRedo;

      string undoText = null;
      string redoText = null;

      if (canUndo)
        undoText = undoRedoManager[undoRedoManager.RedoPosition - 1];

      undoText = string.Format(Resources.UndoMask, undoText);

      if (canRedo)
        redoText = undoRedoManager[undoRedoManager.RedoPosition];

      redoText = string.Format(Resources.RedoMask, redoText);

      undoButton.Enabled = canUndo;
      undoButton.Text = undoText;

      redoButton.Enabled = canRedo;
      redoButton.Text = redoText;

      // Update property grid.
      propertyGrid.Refresh();
    }

    // Run undo.
    private void OnUndo(object sender, EventArgs e)
    {
      if (undoRedoManager.CanUndo)
        undoRedoManager.Undo();
    }

    // Run redo.
    private void OnRedo(object sender, EventArgs e)
    {
      if (undoRedoManager.CanRedo)
        undoRedoManager.Redo();
    }
  }
}