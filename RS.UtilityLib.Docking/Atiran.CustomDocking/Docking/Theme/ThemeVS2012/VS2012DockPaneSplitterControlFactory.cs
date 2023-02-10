using Atiran.CustomDocking.Docking;

namespace Atiran.CustomDocking.Docking.Theme.ThemeVS2012
{
    internal class VS2012DockPaneSplitterControlFactory : DockPanelExtender.IDockPaneSplitterControlFactory
    {
        public DockPane.SplitterControlBase CreateSplitterControl(DockPane pane)
        {
            return new VS2012SplitterControl(pane);
        }
    }
}
