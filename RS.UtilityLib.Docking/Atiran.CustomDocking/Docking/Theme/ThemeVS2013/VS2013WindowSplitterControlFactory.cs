﻿
namespace Atiran.CustomDocking.Docking.Theme.ThemeVS2013
{
    internal class VS2013WindowSplitterControlFactory : DockPanelExtender.IWindowSplitterControlFactory
    {
        public SplitterBase CreateSplitterControl(ISplitterHost host)
        {
            return new VS2013WindowSplitterControl(host);
        }
    }
}
