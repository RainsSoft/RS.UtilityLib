namespace ComponentOwl.BetterListView.Samples.CSharp
{
    partial class GridLinesOptionsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxStyle = new System.Windows.Forms.ComboBox();
            this.checkBoxCustomColor = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Style:";
            // 
            // comboBoxStyle
            // 
            this.comboBoxStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStyle.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxStyle.FormattingEnabled = true;
            this.comboBoxStyle.Items.AddRange(new object[] {
            "None",
            "Horizontal",
            "Vertical",
            "Grid"});
            this.comboBoxStyle.Location = new System.Drawing.Point(0, 24);
            this.comboBoxStyle.Name = "comboBoxStyle";
            this.comboBoxStyle.Size = new System.Drawing.Size(120, 21);
            this.comboBoxStyle.TabIndex = 1;
            this.comboBoxStyle.SelectedIndexChanged += new System.EventHandler(this.ComboBoxStyleOnSelectedIndexChanged);
            // 
            // checkBoxCustomColor
            // 
            this.checkBoxCustomColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxCustomColor.Location = new System.Drawing.Point(0, 64);
            this.checkBoxCustomColor.Name = "checkBoxCustomColor";
            this.checkBoxCustomColor.Size = new System.Drawing.Size(120, 24);
            this.checkBoxCustomColor.TabIndex = 2;
            this.checkBoxCustomColor.Text = "Custom color";
            this.checkBoxCustomColor.UseVisualStyleBackColor = true;
            this.checkBoxCustomColor.CheckedChanged += new System.EventHandler(this.CheckBoxCustomColorOnCheckedChanged);
            // 
            // GridLinesOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.checkBoxCustomColor);
            this.Controls.Add(this.comboBoxStyle);
            this.Controls.Add(this.label1);
            this.Name = "GridLinesOptionsControl";
            this.Size = new System.Drawing.Size(120, 88);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxStyle;
        private System.Windows.Forms.CheckBox checkBoxCustomColor;
    }
}
