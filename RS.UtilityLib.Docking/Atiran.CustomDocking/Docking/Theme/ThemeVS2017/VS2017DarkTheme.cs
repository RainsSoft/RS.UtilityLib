namespace Atiran.CustomDocking.Docking.Theme.ThemeVS2017
{

    /// <summary>
    /// Visual Studio 2012 Dark theme.
    /// </summary>
    public class VS2017DarkTheme : VS2017ThemeBase
    {
        public VS2017DarkTheme()
            :base(Decompress(Resources.vs2017Dark_vstheme), null, null)
        {
        }
    }
}