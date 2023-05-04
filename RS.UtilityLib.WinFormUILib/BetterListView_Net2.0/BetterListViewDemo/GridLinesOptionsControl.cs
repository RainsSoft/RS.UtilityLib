// -----------------------------------------------------------------------
// <copyright file="GridLinesOptionsControl.cs" company="ComponentOwl.com">
//     Copyright © 2010-2014 ComponentOwl.com. All rights reserved.
// </copyright>
// <author>Libor Tinka</author>
// -----------------------------------------------------------------------

namespace ComponentOwl.BetterListView.Samples.CSharp
{
    #region Usings

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    #endregion

    [ToolboxItem(false)]
    internal sealed partial class GridLinesOptionsControl : UserControl
    {
        #region Private Constants

        private const BetterListViewGridLines DefaultStyle = BetterListViewGridLines.Grid;

        private const bool DefaultCustomColor = false;

        #endregion

        #region Private Properties

        private BetterListViewGridLines CurrentStyle
        {
            get
            {
                return (BetterListViewGridLines)this.comboBoxStyle.SelectedIndex;
            }
            set
            {
                this.comboBoxStyle.SelectedIndex = (int)value;
            }
        }

        private bool CurrentCustomColor
        {
            get
            {
                return this.checkBoxCustomColor.Checked;
            }
            set
            {
                this.checkBoxCustomColor.Checked = value;
            }
        }

        #endregion

        #region Private Fields

        private readonly BetterListView listView;

        #endregion

        #region Public Constructors

        public GridLinesOptionsControl()
        {
            InitializeComponent();
        }

        public GridLinesOptionsControl(BetterListView listView)
        {
            this.listView = listView;

            InitializeComponent();

            // set defaults
            CurrentStyle = DefaultStyle;
            CurrentCustomColor = DefaultCustomColor;

            UpdateView();
        }

        #endregion

        #region Private Methods

        private void ComboBoxStyleOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            UpdateView();
        }

        private void CheckBoxCustomColorOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            this.listView.BeginUpdate();

            this.listView.GridLines = CurrentStyle;

            this.listView.ColorGridLines = (CurrentCustomColor
                                                ? Color.FromArgb(64, Color.Blue)
                                                : Color.Empty);

            this.listView.EndUpdate();
        }

        #endregion
    }
}