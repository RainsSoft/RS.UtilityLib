
namespace Atiran.CustomDocking.Docking.Theme.ThemeVS2013
{
    public class VS2013DockPaneStripFactory : DockPanelExtender.IDockPaneStripFactory
    {
        public DockPaneStripBase CreateDockPaneStrip(DockPane pane)
        {
            return new VS2013DockPaneStrip(pane);
        }
    }
}
