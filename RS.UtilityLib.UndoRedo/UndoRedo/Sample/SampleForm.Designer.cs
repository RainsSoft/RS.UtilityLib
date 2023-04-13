namespace Sample
{
  partial class SampleForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.ToolStrip toolStrip;
      System.Windows.Forms.CheckBox enableGroupCheckBox;
      System.Windows.Forms.NumericUpDown numericUpDown;
      System.Windows.Forms.TextBox textBox;
      System.Windows.Forms.GroupBox groupBox;
      System.Windows.Forms.Label editBoxLabel;
      System.Windows.Forms.Label comboBoxLabel;
      System.Windows.Forms.Label dataGridViewLabel;
      System.Windows.Forms.SplitContainer mainContainer;
      this.undoButton = new System.Windows.Forms.ToolStripButton();
      this.redoButton = new System.Windows.Forms.ToolStripButton();
      this.upDownLabel = new System.Windows.Forms.Label();
      this.comboBox = new System.Windows.Forms.ComboBox();
      this.propertyGrid = new System.Windows.Forms.PropertyGrid();
      this.dataGridView = new System.Windows.Forms.DataGridView();
      this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.valueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.gridItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.dataBindingSource = new System.Windows.Forms.BindingSource(this.components);
      toolStrip = new System.Windows.Forms.ToolStrip();
      enableGroupCheckBox = new System.Windows.Forms.CheckBox();
      numericUpDown = new System.Windows.Forms.NumericUpDown();
      textBox = new System.Windows.Forms.TextBox();
      groupBox = new System.Windows.Forms.GroupBox();
      editBoxLabel = new System.Windows.Forms.Label();
      comboBoxLabel = new System.Windows.Forms.Label();
      dataGridViewLabel = new System.Windows.Forms.Label();
      mainContainer = new System.Windows.Forms.SplitContainer();
      toolStrip.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(numericUpDown)).BeginInit();
      groupBox.SuspendLayout();
      mainContainer.Panel1.SuspendLayout();
      mainContainer.Panel2.SuspendLayout();
      mainContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridItemBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // toolStrip
      // 
      toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoButton,
            this.redoButton});
      toolStrip.Location = new System.Drawing.Point(0, 0);
      toolStrip.Name = "toolStrip";
      toolStrip.Size = new System.Drawing.Size(654, 25);
      toolStrip.TabIndex = 0;
      // 
      // undoButton
      // 
      this.undoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.undoButton.Enabled = false;
      this.undoButton.Image = global::Sample.Properties.Resources.UndoImage;
      this.undoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.undoButton.Name = "undoButton";
      this.undoButton.Size = new System.Drawing.Size(23, 22);
      this.undoButton.Click += new System.EventHandler(this.OnUndo);
      // 
      // redoButton
      // 
      this.redoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.redoButton.Enabled = false;
      this.redoButton.Image = global::Sample.Properties.Resources.RedoImage;
      this.redoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.redoButton.Name = "redoButton";
      this.redoButton.Size = new System.Drawing.Size(23, 22);
      this.redoButton.Click += new System.EventHandler(this.OnRedo);
      // 
      // enableGroupCheckBox
      // 
      enableGroupCheckBox.AutoSize = true;
      enableGroupCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dataBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      enableGroupCheckBox.Location = new System.Drawing.Point(8, 8);
      enableGroupCheckBox.Name = "enableGroupCheckBox";
      enableGroupCheckBox.Size = new System.Drawing.Size(145, 21);
      enableGroupCheckBox.TabIndex = 1;
      enableGroupCheckBox.Text = "Enable Group Box";
      enableGroupCheckBox.UseVisualStyleBackColor = true;
      // 
      // numericUpDown
      // 
      numericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dataBindingSource, "Number", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      numericUpDown.Location = new System.Drawing.Point(107, 74);
      numericUpDown.Name = "numericUpDown";
      numericUpDown.Size = new System.Drawing.Size(120, 22);
      numericUpDown.TabIndex = 3;
      // 
      // textBox
      // 
      textBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataBindingSource, "Text", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      textBox.Location = new System.Drawing.Point(107, 48);
      textBox.Name = "textBox";
      textBox.Size = new System.Drawing.Size(120, 22);
      textBox.TabIndex = 4;
      // 
      // groupBox
      // 
      groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      groupBox.Controls.Add(this.upDownLabel);
      groupBox.Controls.Add(editBoxLabel);
      groupBox.Controls.Add(comboBoxLabel);
      groupBox.Controls.Add(this.comboBox);
      groupBox.Controls.Add(textBox);
      groupBox.Controls.Add(numericUpDown);
      groupBox.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.dataBindingSource, "Enabled", true, System.Windows.Forms.DataSourceUpdateMode.Never));
      groupBox.Location = new System.Drawing.Point(8, 35);
      groupBox.Name = "groupBox";
      groupBox.Size = new System.Drawing.Size(417, 107);
      groupBox.TabIndex = 5;
      groupBox.TabStop = false;
      groupBox.Text = "Group Box:";
      // 
      // upDownLabel
      // 
      this.upDownLabel.AutoSize = true;
      this.upDownLabel.Location = new System.Drawing.Point(7, 76);
      this.upDownLabel.Name = "upDownLabel";
      this.upDownLabel.Size = new System.Drawing.Size(96, 17);
      this.upDownLabel.TabIndex = 7;
      this.upDownLabel.Text = "Up Down Box:";
      // 
      // editBoxLabel
      // 
      editBoxLabel.AutoSize = true;
      editBoxLabel.Location = new System.Drawing.Point(7, 51);
      editBoxLabel.Name = "editBoxLabel";
      editBoxLabel.Size = new System.Drawing.Size(63, 17);
      editBoxLabel.TabIndex = 7;
      editBoxLabel.Text = "Edit Box:";
      // 
      // comboBoxLabel
      // 
      comboBoxLabel.AutoSize = true;
      comboBoxLabel.Location = new System.Drawing.Point(6, 24);
      comboBoxLabel.Name = "comboBoxLabel";
      comboBoxLabel.Size = new System.Drawing.Size(83, 17);
      comboBoxLabel.TabIndex = 7;
      comboBoxLabel.Text = "Combo Box:";
      // 
      // comboBox
      // 
      this.comboBox.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.dataBindingSource, "Type", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.comboBox.DisplayMember = "Name";
      this.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBox.FormattingEnabled = true;
      this.comboBox.Location = new System.Drawing.Point(106, 21);
      this.comboBox.Name = "comboBox";
      this.comboBox.Size = new System.Drawing.Size(121, 24);
      this.comboBox.TabIndex = 2;
      this.comboBox.ValueMember = "Value";
      // 
      // dataGridViewLabel
      // 
      dataGridViewLabel.AutoSize = true;
      dataGridViewLabel.Location = new System.Drawing.Point(8, 147);
      dataGridViewLabel.Name = "dataGridViewLabel";
      dataGridViewLabel.Size = new System.Drawing.Size(106, 17);
      dataGridViewLabel.TabIndex = 7;
      dataGridViewLabel.Text = "Data Grid View:";
      // 
      // mainContainer
      // 
      mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      mainContainer.Location = new System.Drawing.Point(0, 25);
      mainContainer.Name = "mainContainer";
      // 
      // mainContainer.Panel1
      // 
      mainContainer.Panel1.Controls.Add(this.propertyGrid);
      // 
      // mainContainer.Panel2
      // 
      mainContainer.Panel2.Controls.Add(this.dataGridView);
      mainContainer.Panel2.Controls.Add(dataGridViewLabel);
      mainContainer.Panel2.Controls.Add(enableGroupCheckBox);
      mainContainer.Panel2.Controls.Add(groupBox);
      mainContainer.Panel2.Padding = new System.Windows.Forms.Padding(5);
      mainContainer.Size = new System.Drawing.Size(654, 356);
      mainContainer.SplitterDistance = 217;
      mainContainer.TabIndex = 9;
      // 
      // propertyGrid
      // 
      this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.propertyGrid.Location = new System.Drawing.Point(0, 0);
      this.propertyGrid.Name = "propertyGrid";
      this.propertyGrid.Size = new System.Drawing.Size(217, 356);
      this.propertyGrid.TabIndex = 9;
      // 
      // dataGridView
      // 
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.AutoGenerateColumns = false;
      this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
      this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.valueDataGridViewTextBoxColumn});
      this.dataGridView.DataSource = this.gridItemBindingSource;
      this.dataGridView.Location = new System.Drawing.Point(8, 165);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.RowTemplate.Height = 24;
      this.dataGridView.Size = new System.Drawing.Size(417, 183);
      this.dataGridView.TabIndex = 6;
      // 
      // nameDataGridViewTextBoxColumn
      // 
      this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
      this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
      this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
      // 
      // valueDataGridViewTextBoxColumn
      // 
      this.valueDataGridViewTextBoxColumn.DataPropertyName = "Value";
      this.valueDataGridViewTextBoxColumn.HeaderText = "Value";
      this.valueDataGridViewTextBoxColumn.Name = "valueDataGridViewTextBoxColumn";
      // 
      // gridItemBindingSource
      // 
      this.gridItemBindingSource.DataSource = typeof(Sample.GridItem);
      // 
      // dataBindingSource
      // 
      this.dataBindingSource.DataSource = typeof(Sample.Data);
      // 
      // SampleForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(654, 381);
      this.Controls.Add(mainContainer);
      this.Controls.Add(toolStrip);
      this.Name = "SampleForm";
      this.Text = "Sample";
      toolStrip.ResumeLayout(false);
      toolStrip.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(numericUpDown)).EndInit();
      groupBox.ResumeLayout(false);
      groupBox.PerformLayout();
      mainContainer.Panel1.ResumeLayout(false);
      mainContainer.Panel2.ResumeLayout(false);
      mainContainer.Panel2.PerformLayout();
      mainContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridItemBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView;
    private System.Windows.Forms.ToolStripButton undoButton;
    private System.Windows.Forms.ToolStripButton redoButton;
    private System.Windows.Forms.Label upDownLabel;
    private System.Windows.Forms.PropertyGrid propertyGrid;
    private System.Windows.Forms.ComboBox comboBox;
    private System.Windows.Forms.BindingSource dataBindingSource;
    private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn valueDataGridViewTextBoxColumn;
    private System.Windows.Forms.BindingSource gridItemBindingSource;
  }
}

