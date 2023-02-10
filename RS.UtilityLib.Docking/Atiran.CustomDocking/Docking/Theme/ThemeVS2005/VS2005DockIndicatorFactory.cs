using static Atiran.CustomDocking.Docking.DockPanel.DockDragHandler;
using static Atiran.CustomDocking.Docking.DockPanelExtender;

namespace Atiran.CustomDocking.Docking.Theme.ThemeVS2005
{
    public class VS2005DockIndicatorFactory : IDockIndicatorFactory
    {
        public DockIndicator CreateDockIndicator(DockPanel.DockDragHandler dockDragHandler)
        {
            return new DockIndicator(dockDragHandler);
        }
    }
}
